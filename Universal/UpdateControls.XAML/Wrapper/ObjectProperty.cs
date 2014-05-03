using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
    class ObjectProperty : IUpdatable
    {
        private static readonly object[] EmptyIndexer = new object[0];

        private readonly IObjectInstance _wrapper;
        private readonly object _wrappedObject;
        private readonly PropertyInfo _propertyInfo;
        private readonly CustomMemberProvider _provider;

        private object _value;
        private List<object> _sourceCollection;
        private ObservableCollection<object> _collection;
        private Dependent _depValue;
        private bool _initialized = false;
        
        public ObjectProperty(IObjectInstance wrapper, object wrappedObject, PropertyInfo propertyInfo, CustomMemberProvider provider)
        {
            _wrapper = wrapper;
            _wrappedObject = wrappedObject;
            _propertyInfo = propertyInfo;
            _provider = provider;

            if (_provider.IsCollection)
            {
                _collection = new ObservableCollection<object>();
                _value = _collection;
            }

            _depValue = new Dependent(UpdateValue);
            _depValue.Invalidated += ValueInvalidated;
            UpdateNow();
        }

        public object GetValue()
        {
            return _value;
        }

        public void SetValue(object value)
        {
            var affectedSet = UpdateScheduler.Begin();

            try
            {
                if (!_provider.IsCollection && _propertyInfo.CanWrite)
                    _propertyInfo.SetValue(_wrappedObject, UnwrapValue(value), EmptyIndexer);
            }
            finally
            {
                if (affectedSet != null)
                {
                    foreach (var updatable in affectedSet.End())
                        updatable.UpdateNow();
                }
            }
        }

        private void UpdateValue()
        {
            if (_provider.IsCollection)
            {
                IEnumerable propertyValue = _propertyInfo.GetValue(_wrappedObject, EmptyIndexer) as IEnumerable;
                _sourceCollection = propertyValue == null ? null :
                    propertyValue.OfType<object>().ToList();
            }
            else
            {
                object value = WrapValue(_propertyInfo.GetValue(_wrappedObject, EmptyIndexer));
                if (!Object.Equals(value, _value))
                    _value = value;
            }
        }

        private void ValueInvalidated()
        {
            UpdateScheduler.ScheduleUpdate(this);
        }

        public void UpdateNow()
        {
            if (_provider.IsCollection)
            {
                _depValue.OnGet();

                // Create a list of new items.
                List<CollectionItem> items = new List<CollectionItem>();

                // Dump all previous items into a recycle bin.
                using (RecycleBin<CollectionItem> bin = new RecycleBin<CollectionItem>())
                {
                    foreach (object oldItem in _collection)
                        bin.AddObject(new CollectionItem(_collection, oldItem, true));
                    // Add new objects to the list.
                    if (_sourceCollection != null)
                        foreach (object obj in _sourceCollection)
                            items.Add(bin.Extract(new CollectionItem(_collection, WrapValue(obj), false)));
                    _sourceCollection = null;
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
            else
            {
                object oldValue = _value;
                _depValue.OnGet();
                object newValue = _value;

                if (_initialized)
                {
                    if ((!Object.Equals(newValue, oldValue)))
                    {
                        _wrapper.FirePropertyChanged(_propertyInfo.Name);
                    }
                }
                else
                {
                    _initialized = true;
                }
            }
        }

        private object WrapValue(object value)
        {
            if (value == null)
                return null;
            else if (_provider.IsPrimitive)
                return value;
            else
                return ForView.Wrap(value);
        }

        private object UnwrapValue(object value)
        {
            if (value == null)
                return null;
            else if (_provider.IsPrimitive)
                return value;
            else
                return ((IObjectInstance)value).WrappedObject;
        }
    }
}
