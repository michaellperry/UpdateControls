/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
	internal abstract class ObjectPropertyCollection : ObjectProperty
	{
		private ObservableCollection<object> _collection = new ObservableCollection<object>();
        private Dependent _depCollection;
        private Action _delay;

        public ObjectPropertyCollection(IObjectInstance objectInstance, ClassMember classProperty)
			: base(objectInstance, classProperty)
		{
            if (ClassProperty.CanRead)
            {
                // When the collection is out of date, update it from the wrapped object.
                _depCollection = new Dependent(OnUpdateCollection);

                // When the property becomes out of date, trigger an update.
                _depCollection.Invalidated += TriggerUpdate;
            }
        }

        private void OnUpdateCollection()
        {
            // Get the source collection from the wrapped object.
            IEnumerable source = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject) as IEnumerable;
            if (source == null)
            {
                _collection = null;
                return;
            }

            List<object> sourceCollection = source.OfType<object>().ToList();

            // Delay the update to the observable collection so that we don't record dependencies on
            // properties used in the items template. XAML will invoke the item template synchronously
            // as we add items to the observable collection, thus causing other view model property
            // getters to fire.
            _delay = delegate
            {
				if (_collection == null)
				{
					_collection = new ObservableCollection<object>();
					ObjectInstance.FirePropertyChanged(ClassProperty.Name);
				}

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
                            items.Add(bin.Extract(new CollectionItem(_collection, TranslateOutgoingValue(obj), false)));
                    // All deleted items are removed from the collection at this point.
                }
                // Ensure that all items are added to the list.
                int index = 0;
                foreach (CollectionItem item in items)
                {
                    item.EnsureInCollection(index);
                    ++index;
                }
            };
        }

        private void TriggerUpdate()
        {
            ObjectInstance.Dispatcher.BeginInvoke(new Action(delegate
            {
                UpdateNow();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        public override void OnUserInput(object value)
		{
			// Use reflection to convert the collection to the ViewModel type
			// (which must be compatible with List<T>, e.g. IEnumerable<T> or IList)
			if (_translateIncomingList == null)
			{
				Type propType = ClassProperty.UnderlyingType;
				Type elemType = (propType.GetInterfaces().Concat(new Type[] { propType })
					.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ?? typeof(IEnumerable<object>))
					.GetGenericArguments()[0];
				MethodInfo mi = GetType().GetMethod("TranslateIncomingList").MakeGenericMethod(new Type[] { elemType });
				_translateIncomingList = (Func<IEnumerable, IEnumerable>)Delegate.CreateDelegate(typeof(Func<IEnumerable, IEnumerable>), this, mi);
			}
			value = _translateIncomingList((IEnumerable)value);
			ClassProperty.SetObjectValue(ObjectInstance.WrappedObject, value);

			// If the UI object switches to a new collection, we can expect it to
			// cancel its subscription to _collection.CollectionChanged and subscribe
			// to the new collection instead. So let's adopt the collection as our
			// own, if it happens to be ObservableCollection<object>.
			_collection = value as ObservableCollection<object>;
		}

        public override object Value
        {
            get
            {
                UpdateNow();
                return _collection;
            }
        }

        public abstract object TranslateOutgoingValue(object value);
		public abstract object TranslateIncomingValue(object value);

		Func<IEnumerable, IEnumerable> _translateIncomingList;
		public IEnumerable TranslateIncomingList<T>(IEnumerable list)
		{
			var translated = new List<T>();
			foreach (object elem in list)
				translated.Add((T)TranslateIncomingValue(elem));
			return translated;
		}

        private void UpdateNow()
        {
            _depCollection.OnGet();
            if (_delay != null)
            {
                // Update the observable collection outside of the update method
                // so we don't take a dependency on item template properties.
                _delay();
                _delay = null;
            }
        }
    }
}
