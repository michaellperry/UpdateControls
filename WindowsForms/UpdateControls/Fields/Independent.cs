/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2011 Michael L Perry
 * MIT License
 * 
 * This class based on a contribution by David Piepgrass.
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/
using System;
using System.ComponentModel;

namespace UpdateControls.Fields
{
    public class Independent<T> : NamedIndependent
    {
		protected internal T _value;

		public Independent() { }
		public Independent(T value) : this((string)null, value) { }
		
		// Oops, this constructor causes ambiguity in case of Independent<string>. 
		// In that case, C# compilers will reinterpret existing code that previously 
		// used the Independent(value) constructor to call Independent(name) instead.
		//public Independent(string name) : base(name) { }
		
		public Independent(string name, T value) : base(name) { _value = value; }
		public Independent(Type containerType, string name) : base(containerType, name) { }
		public Independent(Type containerType, string name, T value) : base(containerType, name) { _value = value; }

		public T Value
		{
			get { base.OnGet(); return _value; }
			set {
				if (_value == null ? value != null : !_value.Equals(value))
				{
					base.OnSet();
					_value = value;
				}
			}
		}
		public static implicit operator T(Independent<T> independent)
		{
			return independent.Value;
		}

		public override string VisualizerName(bool withValue)
		{
			string s = "[I] " + Dependent<T>.VisualizerName(Name);
			if (withValue)
				s += " = " + (_value == null ? "null" : _value.ToString());
			return s;
		}

		[Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
		public Independent IndependentSentry
		{
			get { return this; }
		}
    }
}