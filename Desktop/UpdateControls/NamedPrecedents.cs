using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace UpdateControls
{
	public class NamedDependent : Dependent
	{
		public NamedDependent(Action update) : this(null, update) { }
		public NamedDependent(string name, Action update) : base(update) { _name = name; }

		protected string _name;
		public string Name
		{
			get {
				if (_name == null)
					_name = ComputeName();
				return _name;
			}
		}

		public override string VisualizerName(bool withValue)
		{
			return VisNameWithOptionalHash(Name, withValue);
		}
		public static string GetClassAndMethodName(Delegate d)
		{
			return MemoizedTypeName.GenericName(d.Method.DeclaringType) + "." + d.Method.Name;
		}
		protected virtual string ComputeName()
		{
			return string.Intern(GetClassAndMethodName(_update) + "()");
		}
	}

	public class NamedIndependent : Independent
	{
		public NamedIndependent() : base() { }
		public NamedIndependent(string name) : base() { _name = name; }
		public NamedIndependent(Type valueType) : this(valueType.NameWithGenericParams()) { }
		public NamedIndependent(Type containerType, string name) :
			this(string.Intern(string.Format("{0}.{1}", containerType.NameWithGenericParams(), name))) { }

		public override void OnGet()
		{
			if (DebugMode && _name == null)
				_name = ComputeNameFromStackTrace();
			base.OnGet();
		}

		private string ComputeNameFromStackTrace()
		{
			StackTrace stackTrace = new StackTrace(false);
			MethodBase method = null;
			Assembly assembly = null;
			int i;
				
			// Find the calling assembly
			for (i = 1; i < stackTrace.FrameCount; i++) {
				method = stackTrace.GetFrame(i).GetMethod();
				assembly = method.DeclaringType.Assembly;
				if (assembly != typeof(NamedIndependent).Assembly)
					break;
			}

			// Find a method in the calling assembly that looks like a property 
			// getter; if nothing is found, use the name of the first method
			// in the calling assembly.
			MethodBase candidate = method;
			string methodName = method.Name, candidateName = methodName;
			do {
				if (candidateName.StartsWith("get_")) {
					methodName = candidateName.Substring(4);
					method = candidate;
					break;
				}
				candidate = stackTrace.GetFrame(i).GetMethod();
				if (candidate.DeclaringType.Assembly != assembly)
					break; // quit searching
				candidateName = candidate.Name;
			} while(++i < stackTrace.FrameCount);

			return string.Intern(MemoizedTypeName.GenericName(method.DeclaringType) + "." + methodName);
		}

		protected string _name;
		public string Name
		{
			get { return _name ?? "NamedIndependent"; }
			set { _name = value; }
		}

		public override string VisualizerName(bool withValue)
		{
			return VisNameWithOptionalHash("[I] " + Name, withValue);
		}
	}

	public class NamedIndependent<T> : NamedIndependent
	{
		protected internal T _value;

		public NamedIndependent() : this((string)null) { }
		public NamedIndependent(T value) : this((string)null, value) { }
		public NamedIndependent(string name) : base(name) { }
		public NamedIndependent(string name, T value) : base(name) { _value = value; }
		public NamedIndependent(Type containerType, string name) : base(containerType, name) { }
		public NamedIndependent(Type containerType, string name, T value) : base(containerType, name) { _value = value; }

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
		public static implicit operator T(NamedIndependent<T> independent)
		{
			return independent.Value;
		}

		public override string VisualizerName(bool withValue)
		{
			string s = "[I] " + NamedDependent<T>.VisualizerName(Name);
			if (withValue)
				s += " = " + (_value == null ? "null" : _value.ToString());
			return s;
		}
	}

	public class NamedDependent<T> : NamedDependent
	{
		protected internal T _value;
		protected Func<T> _computeValue;

		public NamedDependent(Func<T> compute) : base((string)null, null)
		{
			base._update = Update; _computeValue = compute;
		}
		public NamedDependent(string name, Func<T> compute) : base(name, null)
		{
			base._update = Update; _computeValue = compute;
		}

		protected void Update()
		{
			_value = _computeValue();
			// TODO: don't propagate updates when _value did not change.
			//    T oldValue = _value;
			//    _value = _computeValue();
			//    return _value == null ? oldValue != null : !_value.Equals(oldValue);
		}

		public T Value
		{
			get { base.OnGet(); return _value; }
		}
		public static implicit operator T(NamedDependent<T> dependent)
		{
			return dependent.Value;
		}

		public override string VisualizerName(bool withValue)
		{
			string s = VisualizerName(_name ?? "NamedDependent");
			if (withValue)
				s += " = " + (_value == null ? "null" : _value.ToString());
			return s;
		}
		internal static string VisualizerName(string name)
		{
			string typeName = MemoizedTypeName<T>.GenericName();
			if (!string.IsNullOrEmpty(name))
				return string.Format("{0}: {1}", name, typeName);
			else
				return typeName;
		}
	}
}
