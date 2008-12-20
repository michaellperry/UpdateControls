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

		private List<Cache> _fresh = new List<Cache>();
		private List<Cache> _stale = new List<Cache>();

		private CacheManager()
		{
		}

		public void Age()
		{
			List<Cache> toUnload = null;
			lock ( this )
			{
				// Age the fresh caches.
				toUnload = _stale;
				_stale = _fresh;
				_fresh = new List<Cache>();
			}
			// Unload the stale caches.
			foreach ( Cache cache in toUnload )
				cache.Unload();
		}

		public void Retire( Cache cache )
		{
			lock ( this )
			{
				// Add the cache to the "fresh" MRU.
				_fresh.Add( cache );
			}
		}

		public void Revive( Cache cache )
		{
			lock ( this )
			{
				// Remove the cache from the MRUs.
				_fresh.Remove( cache );
				_stale.Remove( cache );
			}
		}
	}
}
