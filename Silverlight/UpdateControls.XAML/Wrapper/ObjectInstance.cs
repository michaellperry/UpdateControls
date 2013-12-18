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
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Threading;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
    public class ObjectInstance<TWrappedObjectType> : ICustomTypeProvider, IObjectInstance, INotifyPropertyChanged, IDataErrorInfo, IEditableObject
    {
        // Wrap the class and all of its property definitions.
		private static ClassInstance _classInstance = new ClassInstance(typeof(TWrappedObjectType), typeof(ObjectInstance<TWrappedObjectType>));

        // Wrap the object instance.
        private object _wrappedObject;

        // The dispatcher for the view that I'm attached to.
        private Dispatcher _dispatcher;

		// Wrap all properties.
        private Dictionary<ClassProperty, ObjectProperty> _properties = new Dictionary<ClassProperty, ObjectProperty>();

		public ObjectInstance(object wrappedObject, Dispatcher dispatcher)
		{
			_wrappedObject = wrappedObject;
            _dispatcher = dispatcher;
		}

        public ClassInstance ClassInstance
        {
            get { return _classInstance; }
        }

        public object WrappedObject
        {
            get { return _wrappedObject; }
        }

        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        public ObjectProperty LookupProperty(ClassProperty classProperty)
        {
            ObjectProperty objectProperty;
            if (!_properties.TryGetValue(classProperty, out objectProperty))
            {
                objectProperty = ObjectProperty.From(this, classProperty);
                _properties.Add(classProperty, objectProperty);
            }
			return objectProperty;
		}

        public override string ToString()
        {
            return String.Format("ForView.Wrap({0})", _wrappedObject);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            ObjectInstance<TWrappedObjectType> that = (ObjectInstance<TWrappedObjectType>)obj;
            return Object.Equals(this._wrappedObject, that._wrappedObject);
        }

        public override int GetHashCode()
        {
            return _wrappedObject.GetHashCode();
        }

        public void FirePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

		public string Error
		{
			get
			{
				IDataErrorInfo wrappedObject = _wrappedObject as IDataErrorInfo;
				return wrappedObject != null ? wrappedObject.Error : null;
			}
		}

		public string this[string columnName]
		{
			get
			{
				IDataErrorInfo wrappedObject = _wrappedObject as IDataErrorInfo;
				return wrappedObject != null ? wrappedObject[columnName] : null;
			}
		}

        #region IEditableObject Members

        public void BeginEdit()
        {
            IEditableObject wrappedObject = _wrappedObject as IEditableObject;
            if (wrappedObject != null)
                wrappedObject.BeginEdit();
        }

        public void CancelEdit()
        {
            IEditableObject wrappedObject = _wrappedObject as IEditableObject;
            if (wrappedObject != null)
                wrappedObject.CancelEdit();
        }

        public void EndEdit()
        {
            IEditableObject wrappedObject = _wrappedObject as IEditableObject;
            if (wrappedObject != null)
                wrappedObject.EndEdit();
        }

        #endregion

        public Type GetCustomType()
        {
            return new ClassInstance(typeof(TWrappedObjectType), typeof(ObjectInstance<TWrappedObjectType>));
        }
    }
}
