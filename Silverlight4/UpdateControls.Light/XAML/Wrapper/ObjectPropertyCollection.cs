/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UpdateControls.XAML.Wrapper
{
	internal abstract class ObjectPropertyCollection : ObjectProperty
	{
		private ObservableCollection<object> _collection = new ObservableCollection<object>();
        private Dependent _depCollection;
        private List<IObjectInstance> _children = new List<IObjectInstance>();

        public ObjectPropertyCollection(IObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
            if (ClassProperty.CanRead)
            {
                // Bind to the observable collection.
                ClassProperty.SetUserOutput(ObjectInstance, _collection);

                // When the collection is out of date, update it from the wrapped object.
                _depCollection = new Dependent(OnUpdateCollection);
            }
        }

        private void OnUpdateCollection()
        {
            // Get the source collection from the wrapped object.
            _children.Clear();
            IEnumerable source = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject) as IEnumerable;
            List<object> sourceCollection = source.OfType<object>().Select(TranslateOutgoingValue).ToList();

			ObjectInstance.Defer(new Action(delegate
			{
				// Create a list of new items.
				List<CollectionItem> items = new List<CollectionItem>();

				// Dump all previous items into a recycle bin.
				using (RecycleBin<CollectionItem> bin = new RecycleBin<CollectionItem>())
				{
					foreach (object oldItem in _collection)
						bin.AddObject(new CollectionItem(_collection, oldItem, true));
					// Add new objects to the list.
					if (sourceCollection != null)
						foreach (object obj in sourceCollection)
							items.Add(bin.Extract(new CollectionItem(_collection, obj, false)));
					// All deleted items are removed from the collection at this point.
				}
				// Ensure that all items are added to the list.
				int index = 0;
				foreach (CollectionItem item in items)
				{
					item.EnsureInCollection(index);
					++index;
				}
			}));
        }

        public override void OnUserInput(object value)
		{
			throw new NotSupportedException("Update Controls does not support two-way binding of collection properties.");
		}

        public override void UpdateNodes()
        {
            _depCollection.OnGet();
            foreach (IObjectInstance child in _children)
                child.UpdateNodes();
        }

        protected void AddChild(IObjectInstance child)
        {
            _children.Add(child);
        }

        public abstract object TranslateOutgoingValue(object value);
    }
}
