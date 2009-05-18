/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections.ObjectModel;

namespace UpdateControls.XAML.Wrapper
{
    internal class CollectionItemObject : CollectionItem
    {
        private ObjectInstance _objectInstance;

        public CollectionItemObject(ObservableCollection<object> collection, ObjectInstance objectInstance, bool inCollection)
            : base(collection, inCollection)
        {
            _objectInstance = objectInstance;
        }

        protected override object Item
        {
            get { return _objectInstance; }
        }

        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (!(obj is CollectionItem))
                return false;
            CollectionItemObject that = (CollectionItemObject)obj;
            return Object.Equals(
                this._objectInstance.WrappedObject,
                that._objectInstance.WrappedObject);
        }
    }
}
