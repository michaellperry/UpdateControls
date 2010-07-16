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
	/// <summary>
	/// A collection that recycles objects during a dependent update.
	/// </summary>
	/// <remarks>
	/// This class is for advanced use of the library. There is little need for
	/// recycle bins in typical applications.
	/// <para/>
	/// Construct a recycle bin within a <see cref="Dependent"/>'s update
	/// function when that Dependent controls a collection. Fill the
	/// recycle bin with the contents of the dependent collection,
	/// then reconstruct the collection by extracting objects from the
	/// recycle bin. This prevents the unnecessary destruction and
	/// recreation of objects in the dependent collection.
	/// <para/>
	/// The recycle bin extracts objects based on a prototype. If
	/// the recycle bin contains an object matching the prototype
	/// according to <see cref="Object.GetHashCode"/> and
	/// <see cref="Object.Equals(object)"/>, then that matching object
	/// is extracted. If not, the prototype itself is used. It is
	/// imperitive that you properly implement GetHashCode and
	/// Equals in your recycled classes.
	/// <para/>
	/// Your implementation of GetHashCode and Equals must only consider
	/// fields that do not change. If a field can be changed, or is
	/// itself dependent, then it must not be used either as part of the
	/// hash code, or to determine equality. The best practice is to
	/// implement GetHashCode and Equals in terms of fields that are
	/// initialized by the constructor, and are thereafter immutable.
	/// <para/>
	/// The advantage of RecycleBin is not found in any time or memory savings.
	/// In fact, using RecycleBin in most cases adds a small amount of overhead.
	/// However, the advantage comes from preserving the dynamic and
	/// dependent state of the recycled objects. If your depenent collection
	/// contains only immutable objects (such as strings), there is no
	/// advantage to using a RecycleBin.
	/// </remarks>
	/// <example>
	/// The following code adds location to each device in a network
	/// (the classes Device and Network are not shown). LocatedDevice
	/// has a location that can be changed. LocatedNetwork constructs
	/// a LocatedDevice for each Device in a Network. It uses a recycle
	/// bin so that the locations of the devices are not lost between
	/// updates.
	/// <code language="C#">
	/// // A class that adds location to a device.
	/// public class LocatedDevice : DynamicObject
	/// {
	/// 	private Device _device;
	/// 	private Point _location;
	/// 
	/// 	public LocatedDevice( Device device )
	/// 	{
	/// 		// Capture the reference to a device.
	/// 		_device = device;
	/// 	}
	/// 
	/// 	// Allow the location to change.
	/// 	public Point Location
	/// 	{
	/// 		get
	/// 		{
	/// 			OnGet( "_location" );
	/// 			return _location;
	/// 		}
	/// 		set
	/// 		{
	/// 			OnSet( "_location" );
	/// 			_location = value;
	/// 		}
	/// 	}
	/// 
	/// 	// The hash code depends only upon the
	/// 	// referenced device, not the location.
	/// 	public override int GetHashCode()
	/// 	{
	/// 		return _device.GetHashCode();
	/// 	}
	/// 
	/// 	// The objects are equal if they refer
	/// 	// to the same device. Location doesn't
	/// 	// count, because it can change.
	/// 	public override bool Equals(object obj)
	/// 	{
	/// 		if ( obj == null )
	/// 			return false;
	/// 		if ( this.GetType() != obj.GetType() )
	/// 			return false;
	/// 		LocatedDevice that = (LocatedDevice)obj;
	/// 		return this._device.Equals( that._device );
	/// 	}
	/// 
	/// }
	/// 
	/// // A class that adds location to all devices in a network.
	/// public class LocatedNetwork
	/// {
	/// 	private Network _network;
	/// 	private ArrayList _locatedDevices = new ArrayList();
	/// 	private Dependent _depLocatedDevices;
	/// 
	/// 	public LocatedNetwork( Network network )
	/// 	{
	/// 		// Capture a reference to a network.
	/// 		_network = network;
	/// 		// Create a dependent sentry to control the collection.
	/// 		_depLocatedDevices = new Dependent(
	/// 			UpdateLocatedDevices );
	/// 	}
	/// 
	/// 	// The collection is dependent, and therefore read-only.
	/// 	public ICollection LocatedDevices
	/// 	{
	/// 		get
	/// 		{
	/// 			_depLocatedDevices.OnGet();
	/// 			return _locatedDevices;
	/// 		}
	/// 	}
	/// 
	/// 	private void UpdateLocatedDevices()
	/// 	{
	/// 		// Create and fill a recycle bin.
	/// 		RecycleBin bin = new RecycleBin( _locatedDevices );
	/// 		_locatedDevices.Clear();
	/// 
	/// 		// Create a located device for every device in the network.
	/// 		foreach ( Device device in _network.Devices )
	/// 		{
	/// 			_locatedDevices.Add(
	/// 				bin.Extract( new LocatedDevice(device) ) );
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	public class RecycleBin<T> : IDisposable
        where T : IDisposable
	{
		private List<T> _recyclableObjects = new List<T>();

		/// <summary>
		/// Creates an empty recycle bin.
		/// </summary>
		/// <remarks>
		/// The recycle bin should be filled with objects from a dependent
		/// collection, and the collection should be emptied. Then it can be
		/// repopulaed by extraction from the bin.
		/// </remarks>
		public RecycleBin()
		{
		}

		/// <summary>
		/// Add an object to the recycle bin.
		/// </summary>
		/// <param name="recyclableObject">The object to put in the recycle bin.</param>
		public void AddObject( T recyclableObject )
		{
			_recyclableObjects.Add( recyclableObject );
		}

		/// <summary>
		/// If a matching object is in the recycle bin, remove and return it.
		/// Otherwise, return the prototype.
		/// </summary>
		/// <param name="prototype">An object equal to the one to be extracted.</param>
		/// <returns>The matching object that was added to the recycle bin, or
		/// the prototype if no such object is found.</returns>
		public T Extract( T prototype )
		{
			// See if a matching object is already in the recycle bin.
			int index = _recyclableObjects.IndexOf(prototype);
			if ( index == -1 )
			{
				// No, so use the prototype.
				return prototype;
			}
			else
			{
				// Yes, so extract it.
                prototype.Dispose();
				T match = _recyclableObjects[index];
				_recyclableObjects.RemoveAt( index );
				return match;
			}
		}

		/// <summary>
		/// Disposes all objects remaining in the recycle bin.
		/// </summary>
		/// <remarks>
		/// Call this method at the end of the update function. Any objects
		/// that have not been recycled will be disposed, thus removing any
		/// dependencies they may have. This allows cached objects to be
		/// unloaded and garbage collected.
		/// </remarks>
		public void Dispose()
		{
			foreach ( T obj in _recyclableObjects )
			{
				obj.Dispose();
			}
		}
	}
}
