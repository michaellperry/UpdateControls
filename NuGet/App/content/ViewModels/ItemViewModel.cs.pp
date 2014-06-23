using System;
using System.Collections.Generic;
using System.Linq;
using $rootnamespace$.Models;

namespace $rootnamespace$.ViewModels
{
    public class ItemViewModel
    {
        private readonly Item _item;

        public ItemViewModel(Item Item)
        {
            _item = Item;
        }

        public string Name
        {
            get { return _item.Name; }
            set { _item.Name = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            ItemViewModel that = obj as ItemViewModel;
            if (that == null)
                return false;
            return Object.Equals(this._item, that._item);
        }

        public override int GetHashCode()
        {
            return _item.GetHashCode();
        }
    }
}
