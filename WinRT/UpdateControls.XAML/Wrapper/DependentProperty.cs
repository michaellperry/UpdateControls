using System.Reflection;
using UpdateControls.Fields;
using System.Linq;
using System;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace UpdateControls.XAML.Wrapper
{
    class DependentProperty
    {
        private static readonly Type[] Primitives = new Type[]
        {
			typeof(object),
            typeof(string),
            typeof(ICommand)
        };

        private static readonly Type[] Bindables = new Type[]
        {
            typeof(DependencyObject),
            typeof(INotifyPropertyChanged),
            typeof(INotifyCollectionChanged)
        };

        private static readonly object[] EmptyIndexer = new object[0];

        private readonly IDependentObject _wrapper;
        private readonly object _wrappedObject;
        private readonly PropertyInfo _propertyInfo;
        private readonly bool _isPrimitive;
        private readonly bool _isCollection;

        private object _value;
        private List<object> _sourceCollection;
        private ObservableCollection<object> _collection;
        private Dependent _depValue;
        private bool _initialized = false;
        
        public DependentProperty(IDependentObject wrapper, object wrappedObject, PropertyInfo propertyInfo)
        {
            _wrapper = wrapper;
            _wrappedObject = wrappedObject;
            _propertyInfo = propertyInfo;

            if (IsPrimitive(_propertyInfo.PropertyType))
            {
                _isCollection = false;
                _isPrimitive = true;
            }
            else if (IsCollectionType(_propertyInfo.PropertyType))
            {
                _isCollection = true;
                _isPrimitive = IsPrimitive(GetItemType(_propertyInfo.PropertyType));
                _collection = new ObservableCollection<object>();
                _value = _collection;
            }
            else
            {
                _isCollection = false;
                _isPrimitive = false;
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
            var affectedSet = AffectedSet.Begin();

            try
            {
                if (!_isCollection)
                    _propertyInfo.SetValue(_wrappedObject, UnwrapValue(value), EmptyIndexer);
            }
            finally
            {
                if (affectedSet != null)
                {
                    foreach (var dependentProperty in affectedSet.End())
                        dependentProperty.UpdateNow();
                }
            }
        }

        private void UpdateValue()
        {
            if (_isCollection)
            {
                _sourceCollection = ((IEnumerable)_propertyInfo.GetValue(_wrappedObject, EmptyIndexer)).OfType<object>().ToList();
            }
            else
            {
                _value = WrapValue(_propertyInfo.GetValue(_wrappedObject, EmptyIndexer));
            }
        }

        private void ValueInvalidated()
        {
            if (!AffectedSet.CaptureDependent(this))
            {
                Window.Current.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Low,
                    new DispatchedHandler(delegate
                    {
                        UpdateNow();
                    }));
            }
        }

        private void UpdateNow()
        {
            if (_isCollection)
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
            else if (_isPrimitive)
                return value;
            else
                return ForView.Wrap(value);
        }

        private object UnwrapValue(object value)
        {
            if (value == null)
                return null;
            else if (_isPrimitive)
                return value;
            else
                return ((IDependentObject)value).GetWrappedObject();
        }

        private static bool IsCollectionType(Type propertyType)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo());
        }

        private static Type GetItemType(Type collectionType)
        {
            if (collectionType.GetTypeInfo().GenericTypeArguments.Length == 1)
                return collectionType.GetTypeInfo().GenericTypeArguments[0];
            else
                return typeof(object);
        }

        private static bool IsPrimitive(Type type)
        {
            return
                type.GetTypeInfo().IsValueType ||
                type.GetTypeInfo().IsPrimitive ||
                (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) ||
                Primitives.Contains(type) ||
                // Don't wrap objects that are already bindable
                Bindables.Any(b => b.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()));
            ;
        }
    }
}
