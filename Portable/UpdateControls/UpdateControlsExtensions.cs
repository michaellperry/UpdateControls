/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
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
		{
			RecycleBin<T> bin = new RecycleBin<T>(collection);

			if (collection != null)
				collection.Clear();

			return bin;
		}
    }
}
