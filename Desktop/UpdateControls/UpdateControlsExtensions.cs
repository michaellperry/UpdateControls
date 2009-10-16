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
using System.Collections.Generic;

namespace UpdateControls
{
	public static class UpdateControlsExtensions
	{
		/// <summary>
		/// Moves all objects into a new recycle bin, from which they can be extracted.
		/// </summary>
		/// <param name="objects">A collection of objects to add to the bin.</param>
		/// <remarks>
		/// After the objects are added to the bin, the collection
		/// is cleared. Then it can be repopulated by extraction from
		/// the bin.
		/// </remarks>
		public static RecycleBin<T> Recycle<T>(this ICollection<T> collection)
			where T : IDisposable
		{
			RecycleBin<T> bin = new RecycleBin<T>();

			// Take all objects into the recycle bin.
			if (collection != null)
			{
				foreach (T recyclableObject in collection)
					bin.AddObject(recyclableObject);

				// Empty the collection.
				collection.Clear();
			}

			return bin;
		}
    }
}
