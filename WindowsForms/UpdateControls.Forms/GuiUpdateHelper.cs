using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateControls.Forms
{
	/// <summary>
	/// Helps implement WinForms Update Controls and other user-facing properties
	/// by automatically updating a list of dependents (which usually correspond 
	/// to actual properties of a control) when an Application.Idle event arrives.
	/// </summary>
	/// <remarks>
	/// See the documentation of each constructor for more information.
	/// <para/>
	/// Remember to dispose this object when the control (or other object) that 
	/// uses it is disposed. Otherwise, if this object is still subscribed to 
	/// Application.Idle, it will continue calling the update methods and there 
	/// will be a memory leak involving anything reachable from those methods.
	/// </remarks>
	public class GuiUpdateHelper : IDisposable
	{
		/// <summary>Initializes GuiUpdateHelper.</summary>
		/// <param name="updaters">A list of methods that perform updates.</param>
		/// <remarks>This constructor immediately subscribes to Application.Idle,
		/// and creates a series of dependents (one for each updater method) whose
		/// OnGet() method is called during the Idle event.</remarks>
		public GuiUpdateHelper(params Action[] updaters) : this(true, updaters) { }
		
		/// <summary>Initializes GuiUpdateHelper.</summary>
		/// <param name="updaters">A list of dependents whose OnGet() method needs to be called during every Application.Idle event.</param>
		/// <remarks>This constructor immediately subscribes to Application.Idle.</remarks>
		public GuiUpdateHelper(params Dependent[] dependents) : this(true, dependents) { }

		/// <summary>Initializes a GuiUpdateHelper and associates it with a Windows 
		/// Forms control.</summary>
		/// <param name="control">A control.</param>
		/// <param name="updaters">A list of methods that perform updates.</param>
		/// <remarks>The constructors that take a "control" parameter do not handle 
		/// the Application.Idle event immediately. Instead, they wait until the 
		/// control fires the HandleCreated event. This ensures that GuiUpdateHelper
		/// signs up for the correct "Idle" event in multithreaded applications, in
		/// case the control is not constructed in the GUI thread. Also, these 
		/// constructors avoid calling OnGet when the control does not physically 
		/// exist yet.</remarks>
		public GuiUpdateHelper(Control control, params Action[] updaters)
			: this(false, updaters)
		{
			InitEvents(control);
		}

		/// <summary>Initializes a GuiUpdateHelper and associates it with a Windows 
		/// Forms control.</summary>
		/// <param name="control">A control.</param>
		/// <param name="updaters">A list of dependents whose OnGet() method needs to be called during every Application.Idle event.</param>
		/// <remarks>The constructors that take a "control" parameter do not handle 
		/// the Application.Idle event immediately. Instead, they wait until the 
		/// control fires the HandleCreated event. This ensures that GuiUpdateHelper
		/// signs up for the correct "Idle" event in multithreaded applications, in
		/// case the control is not constructed in the GUI thread. Also, these 
		/// constructors avoid calling OnGet when the control does not physically 
		/// exist yet.</remarks>
		public GuiUpdateHelper(Control control, params Dependent[] dependents)
			: this(false, dependents)
		{
			InitEvents(control);
		}

		private GuiUpdateHelper(bool startNow, params Dependent[] dependents)
		{
			_dependents = dependents;
			if (startNow)
				StartOnCurrentThread();
		}

		private GuiUpdateHelper(bool startNow, params Action[] updaters)
		{
			_dependents = new Dependent[updaters.Length];
			for (int i = 0; i < updaters.Length; i++)
				_dependents[i] = Dependent.New(updaters[i]);
			if (startNow)
				StartOnCurrentThread();
		}

		private void InitEvents(Control control)
		{
			control.HandleCreated += (s, e) => StartOnCurrentThread();
			control.HandleDestroyed += (s, e) => Stop();
		}

		Dependent[] _dependents;
		bool _started;

		/// <summary>Finds the dependent associated with the specified updater 
		/// method and calls its <see cref="Dependent.OnGet"/> method.</summary>
		public void OnGet(Action updater)
		{
			for (int i = 0; i < _dependents.Length; i++)
				if (_dependents[i].UpdateMethod == updater)
					_dependents[i].OnGet();
		}

		/// <summary>This method is called after updating all dependents,
		/// regardless of whether anything changed.</summary>
		/// <remarks>The boolean is true if any of the dependents were
		/// out-of-date.</remarks>
		public event Action<bool> OnUpdate;

		/// <summary>Forces any out-of-date dependents to update, and 
		/// fires the OnUpdate event.</summary>
		virtual public void UpdateNow()
		{
			bool updated = false;
			for (int i = 0; i < _dependents.Length; i++)
			{
				if (!_dependents[i].IsUpToDate)
				{
					updated = true;
					_dependents[i].OnGet();
				}
			}
			if (OnUpdate != null)
				OnUpdate(updated);
		}

		/// <summary>Returns true if the Application.Idle event is being handled.</summary>
		public bool IsStarted
		{
			get { return _started; }
		}
		/// <summary>Subscribes to Application.Idle if a subscription wasn't done already.</summary>
		protected internal bool StartOnCurrentThread()
		{
			if (!_started)
			{
				_started = true;
				Application.Idle += new EventHandler(Application_Idle);
				return true;
			}
			return false;
		}
		/// <summary>Removes the subscription to Application.Idle.</summary>
		protected internal void Stop()
		{
			_started = false;
			Application.Idle -= new EventHandler(Application_Idle);
		}

		void Application_Idle(object sender, EventArgs e)
		{
			UpdateNow();
		}

		/// <summary>Unsubscribes from Application.Idle and disposes all Dependents.</summary>
		public virtual void Dispose()
		{
			Stop();
			for (int i = 0; i < _dependents.Length; i++)
				_dependents[i].Dispose();
		}
	}
}
