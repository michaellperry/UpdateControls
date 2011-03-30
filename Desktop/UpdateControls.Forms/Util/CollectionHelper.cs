using System;
using System.Collections;
using System.Collections.Generic;

namespace UpdateControls.Forms.Util
{
    public static class CollectionHelper
    {
        public static void RecycleCollection<T>(IList collection, IEnumerable<T> source)
        {
            // Recycle the collection of items.
            ArrayList newItems = new ArrayList(collection.Count);
            using (var recycleBin = new RecycleBin<T>())
            {
                foreach (T item in collection)
                    recycleBin.AddObject(item);

                // Extract each item from the recycle bin.
                foreach (T item in source)
                {
                    newItems.Add(recycleBin.Extract(item));
                }
            }

            collection.Clear();
            foreach (object item in newItems)
                collection.Add(item);
        }
    }
}
