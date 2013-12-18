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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;

namespace UpdateControls.XAML.Wrapper
{
    public class ObjectInstance<TWrappedObjectType> : DependencyObject, IObjectInstance, IEditableObject
    {
        // Wrap the class and all of its property definitions.
		private static ClassInstance _classInstance = new ClassInstance(typeof(TWrappedObjectType));

        // Wrap the object instance.
        private object _wrappedObject;

        private Tree _tree;

		// Wrap all properties.
		private List<ObjectProperty> _properties;

        private Dependent _depNodes;

        private bool _disposed = false;

        public ObjectInstance(TWrappedObjectType wrappedObject, Tree tree)
		{
			_wrappedObject = wrappedObject;
            _tree = tree;

            // Create a wrapper around each property.
            _properties = _classInstance.ClassProperties.Select(p => ObjectProperty.From(this, p)).ToList();

            _depNodes = new Dependent(delegate
            {
                foreach (ObjectProperty property in _properties)
                {
                    property.UpdateNodes();
                }
            });
		}

        public object WrappedObject
        {
            get { return _wrappedObject; }
        }

        public Tree Tree
        {
            get { return _tree; }
        }

        public void Defer(Dependent dependent)
        {
            if (UnitTestDispatcher.On)
                UnitTestDispatcher.Defer(delegate { dependent.OnGet(); });
            else
                Dispatcher.BeginInvoke(delegate { dependent.OnGet(); });
        }

        public ObjectProperty LookupProperty(ClassProperty classProperty)
        {
            if (_properties == null)
                return null;
            else
    			return _properties.Single(p => p.ClassProperty == classProperty);
		}

		public ObjectProperty GetPropertyByName(string name)
		{
			return _properties.FirstOrDefault(property => property.ClassProperty.Name == name);
		}

        public void UpdateNodes()
        {
            _depNodes.OnGet();
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        public void Dispose()
        {
            foreach (ObjectProperty property in _properties)
            {
                property.Dispose();
            }
            _depNodes.Dispose();
            _disposed = true;
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
    }
}
