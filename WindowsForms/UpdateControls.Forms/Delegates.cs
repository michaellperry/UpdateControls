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
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

namespace UpdateControls.Forms
{
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate string GetStringDelegate();
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void SetStringDelegate( string value );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate bool GetBoolDelegate();
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void SetBoolDelegate( bool value );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate CheckState GetCheckStateDelegate();
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void SetCheckStateDelegate( CheckState value );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate DateTime GetDateTimeDelegate();
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void SetDateTimeDelegate( DateTime value );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate DateTime[] GetDateTimeArrayDelegate();
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate IEnumerable GetCollectionDelegate();
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate object GetObjectDelegate();
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void SetObjectDelegate( object value );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
    public delegate object GetObjectObjectDelegate(object tag);
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
    public delegate HorizontalAlignment GetObjectHorizontalAlignmentDelegate(object tag);
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate string GetObjectStringDelegate( object tag );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void SetObjectStringDelegate( object tag, string value );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate bool GetObjectBoolDelegate( object tag );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void SetObjectBoolDelegate( object tag, bool value );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate CheckState GetObjectCheckStateDelegate( object tag );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void SetObjectCheckStateDelegate( object tag, CheckState value );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
    public delegate ColumnDefinitions GetColumnDefinitionsDelegate();
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate object GetObjectColumnValueDelegate( object tag, int column );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void SetObjectColumnValueDelegate( object tag, int column, object value );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate int GetObjectIntDelegate( object tag );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate IEnumerable GetObjectCollectionDelegate(  object tag );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate Form CreateFormDelegate( object tag );
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void ActionDelegate();
	/// <summary>
	/// A delegate type used by dependent controls.
	/// </summary>
	public delegate void ObjectActionDelegate( object tag );
    /// <summary>
    /// A delegate type used by dependent controls.
    /// </summary>
    public delegate T GetColumnValueDelegate<OBJ,T>(OBJ obj);
    /// <summary>
    /// A delegate type used by dependent controls.
    /// </summary>
    public delegate void SetColumnValueDelegate<OBJ,T>(OBJ obj, T value);
    /// <summary>
    /// A delegate type used by dependent controls.
    /// </summary>
    public delegate IEnumerable<OBJ> GetItemsDelegate<OBJ>();
    /// <summary>
    /// A delegate type used by dependent controls.
    /// </summary>
    public delegate OBJ ItemAddedDelegate<OBJ>();
    /// <summary>
    /// A delegate type used by dependent controls.
    /// </summary>
    public delegate void ItemDeletedDelegate<OBJ>(OBJ obj);
}
