using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace UpdateControls.XAML
{
    internal class CollectionItem<T> : IDisposable
    {
        private ObservableCollection<T> _collection;
        private T _item;
        private bool _inCollection;

        public CollectionItem(ObservableCollection<T> collection, T item, bool inCollection)
        {
            _collection = collection;
            _item = item;
            _inCollection = inCollection;
        }

        public void Dispose()
        {
            if (_inCollection)
                _collection.Remove(_item);
        }

        public void EnsureInCollection(int index)
        {
            if (!_inCollection)
            {
                // Insert the item into the correct position.
                _collection.Insert(index, _item);
            }
            else if (!object.Equals(_collection[index], _item))
            {
                // Remove the item from the old position.
                _collection.Remove(_item);

                // Insert the item in the correct position.
                _collection.Insert(index, _item);
            }
        }

        public override int GetHashCode()
        {
            return _item.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (!(obj is CollectionItem<T>))
                return false;
            CollectionItem<T> that = (CollectionItem<T>)obj;
            return Object.Equals(
                this._item,
                that._item);
        }
    }

    internal abstract class DependentPropertyBase
    {
        public abstract object Value { get; }
    }

    internal class DependentAtom<T> : DependentPropertyBase, IUpdatable
    {
        private Dependent _depValue;
        private T _value;
        private Action _firePropertyChanged;
        
        public DependentAtom(Action firePropertyChanged, Func<T> getMethod)
        {
            _firePropertyChanged = firePropertyChanged;
            _depValue = new Dependent(() => _value = getMethod());
            _depValue.Invalidated += () => UpdateScheduler.ScheduleUpdate(this);
        }

        public override object Value
        {
            get { _depValue.OnGet(); return _value; }
        }

        public void UpdateNow()
        {
            _firePropertyChanged();
        }
    }

    internal class DependentCollection<T> : DependentPropertyBase, IUpdatable
    {
        private Func<IEnumerable<T>> _getMethod;
        private Dependent _depCollection;
        private ObservableCollection<T> _collection = new ObservableCollection<T>();

        public DependentCollection(Func<IEnumerable<T>> getMethod)
        {
            _getMethod = getMethod;
            _depCollection = new Dependent(OnUpdateCollection);
            _depCollection.Invalidated += () => UpdateScheduler.ScheduleUpdate(this);
            _depCollection.Touch();
        }

        private void OnUpdateCollection()
        {
            // Get the source collection from the wrapped object.
            IEnumerable<T> sourceCollection = _getMethod();

            // Create a list of new items.
            List<CollectionItem<T>> items = new List<CollectionItem<T>>();

            // Dump all previous items into a recycle bin.
            using (RecycleBin<CollectionItem<T>> bin = new RecycleBin<CollectionItem<T>>())
            {
                foreach (T oldItem in _collection)
                    bin.AddObject(new CollectionItem<T>(_collection, oldItem, true));
                // Add new objects to the list.
                if (sourceCollection != null)
                    foreach (T obj in sourceCollection)
                        items.Add(bin.Extract(new CollectionItem<T>(_collection, obj, false)));
                // All deleted items are removed from the collection at this point.
            }
            // Ensure that all items are added to the list.
            int index = 0;
            foreach (CollectionItem<T> item in items)
            {
                item.EnsureInCollection(index);
                ++index;
            }
        }

        public override object Value
        {
            get { return _collection; }
        }

        public void UpdateNow()
        {
            _depCollection.OnGet();
        }
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IDictionary<string, DependentPropertyBase> _dependentPropertyByName = new Dictionary<string, DependentPropertyBase>();

        protected T Get<T>(Func<T> getMethod)
        {
            ForView.Initialize();
            string caller = new StackFrame(1).GetMethod().Name;
            if (!caller.StartsWith("get_"))
                throw new ArgumentException("Only call Get from a property getter.");
            return Get<T>(caller.Substring(4), getMethod);
        }

        protected T Get<T>(string propertyName, Func<T> getMethod)
        {
            ForView.Initialize();
            DependentPropertyBase property;
            if (!_dependentPropertyByName.TryGetValue(propertyName, out property))
            {
                property = new DependentAtom<T>(() => FirePropertyChanged(propertyName), getMethod);
                _dependentPropertyByName.Add(propertyName, property);
            }
            return (T)property.Value;
        }

        protected IEnumerable<T> GetCollection<T>(Func<IEnumerable<T>> getMethod)
        {
            ForView.Initialize();
            string caller = new StackFrame(1).GetMethod().Name;
            if (!caller.StartsWith("get_"))
                throw new ArgumentException("Only call Get from a property getter.");
            return GetCollection<T>(caller.Substring(4), getMethod);
        }

        protected IEnumerable<T> GetCollection<T>(string propertyName, Func<IEnumerable<T>> getMethod)
        {
            ForView.Initialize();
            DependentPropertyBase property;
            if (!_dependentPropertyByName.TryGetValue(propertyName, out property))
            {
                property = new DependentCollection<T>(getMethod);
                _dependentPropertyByName.Add(propertyName, property);
            }
            return (IEnumerable<T>)property.Value;
        }

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
