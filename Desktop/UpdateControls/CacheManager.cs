/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System.Collections.Generic;
using System;
using System.Linq;

namespace UpdateControls
{
	public interface Cache
	{
		void Unload();
	}

	/// <summary>
	/// Summary description for CacheManager.
	/// </summary>
	public class CacheManager
	{
		private static CacheManager _instance = new CacheManager();

		public static CacheManager Instance
		{
			get
			{
				return _instance;
			}
		}

		private List<WeakReference> _fresh = new List<WeakReference>();
		private List<WeakReference> _stale = new List<WeakReference>();

		private CacheManager()
		{
		}

		public void Age()
		{
			List<WeakReference> toUnload = null;
			lock ( this )
			{
				// Age the fresh caches.
				toUnload = _stale;
				_stale = _fresh;
				_fresh = new List<WeakReference>();
			}
			// Unload the stale caches.
			foreach (WeakReference reference in toUnload)
			{
				Cache cache = (Cache)reference.Target;
				if (cache != null)
					cache.Unload();
			}
		}

		public void Retire( Cache cache )
		{
			lock ( this )
			{
				// Add the cache to the "fresh" MRU.
				_fresh.Add( new WeakReference(cache) );
			}
		}

        public void Revive(Cache cache)
        {
            lock (this)
            {
                // Remove the cache from the MRUs.
                RemoveFromList(cache, _fresh);
                RemoveFromList(cache, _stale);
            }
        }

        private static void RemoveFromList(Cache cache, List<WeakReference> list)
        {
            for (int i = 0; i < list.Count; )
            {
                WeakReference r = list[i];
                if (r.Target == cache || !r.IsAlive)
                    list.RemoveAt(i);
                else
                    ++i;
            }
        }
    }
}
