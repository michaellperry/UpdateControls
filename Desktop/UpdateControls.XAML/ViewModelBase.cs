using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections;
using UpdateControls.XAML.Wrapper;

namespace UpdateControls.XAML
{
    internal abstract class DependentPropertyBase
    {
        public abstract object Value { get; }
    }

    internal class DependentAtom<T> : DependentPropertyBase
    {
        private Dependent _depValue;
        private T _value;

        public DependentAtom(Action firePropertyChanged, Func<T> getMethod)
        {
            _depValue = new Dependent(() => _value = getMethod());
            _depValue.Invalidated += firePropertyChanged;
        }

        public override object Value
        {
            get { _depValue.OnGet(); return _value; }
        }
    }

    internal partial class DependentCollection : DependentPropertyBase
    {
        private Func<IEnumerable> _getMethod;
        private Dependent _depCollection;
        private ObservableCollection<object> _collection = new ObservableCollection<object>();

        public DependentCollection(Func<IEnumerable> getMethod)
        {
            _getMethod = getMethod;
            _depCollection = new Dependent(OnUpdateCollection);
            _depCollection.Invalidated += () => TriggerUpdate();
            _depCollection.Touch();
        }

        private void OnUpdateCollection()
        {
            // Get the source collection from the wrapped object.
            IEnumerable sourceCollection = _getMethod();

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
        }

        public override object Value
        {
            get { return _collection; }
        }

        partial void TriggerUpdate();
    }

    public partial class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IDictionary<string, DependentPropertyBase> _dependentPropertyByName = new Dictionary<string, DependentPropertyBase>();

        protected T Get<T>(Func<T> getMethod)
        {
            string caller = new StackFrame(1).GetMethod().Name;
            if (!caller.StartsWith("get_"))
                throw new ArgumentException("Only call Get from a property getter.");
            return Get<T>(caller.Substring(4), getMethod);
        }

        protected T Get<T>(string propertyName, Func<T> getMethod)
        {
            DependentPropertyBase property;
            if (!_dependentPropertyByName.TryGetValue(propertyName, out property))
            {
                // Determine whether the property is an atom or a collection.
                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                {
                    // It's a collection.
                    property = new DependentCollection(() => (IEnumerable)getMethod());
                }
                else
                {
                    // It's an atom.
                    property = new DependentAtom<T>(() => FirePropertyChanged(propertyName), getMethod);
                }
                _dependentPropertyByName.Add(propertyName, property);
            }
            return (T)property.Value;
        }

        partial void FirePropertyChanged(string propertyName);
    }
}
