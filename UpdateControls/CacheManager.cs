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

using System;
using System.Collections;

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

		private ArrayList _fresh = new ArrayList();
		private ArrayList _stale = new ArrayList();

		private CacheManager()
		{
		}

		public void Age()
		{
			ArrayList toUnload = null;
			lock ( this )
			{
				// Age the fresh caches.
				toUnload = _stale;
				_stale = _fresh;
				_fresh = new ArrayList();
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
