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
			return GetClassAndMethodName(_update) + "()";
		}
	}

	public class NamedIndependent : Independent
	{
		public NamedIndependent() : base() { }
		public NamedIndependent(string name) : base() { _name = name; }
		public NamedIndependent(Type valueType) : this(valueType.NameWithGenericParams()) { }
		public NamedIndependent(Type containerType, string name) :
			this(string.Format("{0}.{1}", containerType.NameWithGenericParams(), name)) { }

		public override void OnGet()
		{
            // TODO: Figure out _name
			base.OnGet();
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

	[Obsolete]
	public class NamedIndependent<T> : UpdateControls.Fields.Independent<T>
	{
		public NamedIndependent() : base() { }
		public NamedIndependent(T value) : base(value) { }
		public NamedIndependent(string name, T value) : base(name, value) { }
		public NamedIndependent(Type containerType, string name) : base(containerType, name) { }
		public NamedIndependent(Type containerType, string name, T value) : base(containerType, name, value) { }
	}

	[Obsolete]
	public class NamedDependent<T> : UpdateControls.Fields.Dependent<T>
	{
		public NamedDependent(Func<T> compute) : base(compute) { }
		public NamedDependent(string name, Func<T> compute) : base(name, compute) { }
	}
}
