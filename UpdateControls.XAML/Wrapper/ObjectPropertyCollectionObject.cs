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
    internal class ObjectPropertyCollectionObject : ObjectPropertyCollection
	{
        public ObjectPropertyCollectionObject(ObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
		}

        public override CollectionItem MakeCollectionItem(ObservableCollection<object> collection, object value, bool inCollection)
        {
            // If it's already in the collection, it's already wrapped.
            if (inCollection)
                return new CollectionItemObject(
                    collection,
                    (ObjectInstance)value,
                    inCollection);
            else
                return new CollectionItemObject(
                    collection,
                    value == null ? null : new ObjectInstance(value),
                    inCollection);
        }
    }
}
