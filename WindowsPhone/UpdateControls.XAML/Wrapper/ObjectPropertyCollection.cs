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
	internal class ObjectPropertyCollection : ObjectProperty
	{
        private bool _hasChildObjects = false;
		private ObservableCollection<object> _collection = new ObservableCollection<object>();
        private Dependent _depCollection;
        private List<IObjectInstance> _children = new List<IObjectInstance>();

        public ObjectPropertyCollection(IObjectInstance objectInstance, ClassProperty classProperty, bool hasChildObjects)
			: base(objectInstance, classProperty)
		{
            _hasChildObjects = hasChildObjects;

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
            IEnumerable values = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject) as IEnumerable;
            List<object> newCollection;
            if (_hasChildObjects)
            {
                List<IObjectInstance> oldChildren = new List<IObjectInstance>(_children);
                Dictionary<object, IObjectInstance> oldValues = oldChildren.ToDictionary(wrapper => wrapper.WrappedObject);

                // Get the source collection from the wrapped object.
                _children.Clear();
                newCollection = new List<object>();
                foreach (object value in values)
                {
                    IObjectInstance wrapper;
                    if (oldValues.TryGetValue(value, out wrapper))
                    {
                        _children.Add(wrapper);
                        newCollection.Add(wrapper);
                    }
                    else
                    {
                        if (WrapObject(value, out wrapper))
                            _children.Add(wrapper);
                        newCollection.Add(wrapper);
                    }
                }

                foreach (IObjectInstance child in oldChildren.Except(_children))
                {
                    ObjectInstance.Tree.RemoveKey(child.WrappedObject);
                    child.Dispose();
                }
            }
            else
            {
                newCollection = values.OfType<object>().ToList();
            }

			AssignToObservableCollection(newCollection);
		}

		private void AssignToObservableCollection(List<object> newCollection)
		{
			// Create a list of new items.
			List<CollectionItem> items = new List<CollectionItem>();

			// Dump all previous items into a recycle bin.
			using (RecycleBin<CollectionItem> bin = new RecycleBin<CollectionItem>())
			{
				foreach (object oldItem in _collection)
					bin.AddObject(new CollectionItem(_collection, oldItem, true));
				// Add new objects to the list.
				if (newCollection != null)
					foreach (object obj in newCollection)
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

        public override void Dispose()
        {
            foreach (IObjectInstance child in _children)
            {
				ObjectInstance.Tree.RemoveKey(child.WrappedObject);
				child.Dispose();
            }
            using (NotificationGate.BeginOutbound())
            {
                ObjectInstance.ClearValue(ClassProperty.DependencyProperty);
            }
            _depCollection.Dispose();
        }

        protected void AddChild(IObjectInstance child)
        {
            _children.Add(child);
        }
    }
}
