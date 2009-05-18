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
    internal class CollectionItemNative : CollectionItem
    {
        private object _item;

        public CollectionItemNative(ObservableCollection<object> collection, object item, bool inCollection)
            : base(collection, inCollection)
        {
            _item = item;
        }

        protected override object Item
        {
            get { return _item; }
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
            CollectionItemNative that = (CollectionItemNative)obj;
            return Object.Equals(
                this.Item,
                that.Item);
        }
    }
}
