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
using System.Text;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using System.IO;

namespace UpdateControls.Installer
{
    // Definition of the IMessageFilter interface which we need to implement and 
    // register with the CoRegisterMessageFilter API.
    [ComImport(), Guid("00000016-0000-0000-C000-000000000046"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    interface IOleMessageFilter // Renamed to avoid confusion w/ System.Windows.Forms.IMessageFilter
    {
        [PreserveSig]
        int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);
        [PreserveSig]
        int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);
        [PreserveSig]
        int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
    }

    class ToolboxInstallerUtility : IOleMessageFilter
    {
        private TextWriter _logWriter;
        private DTE _dte2008;
        private bool _has2008;
        private Solution2 _solution;
        private Project _proj;
        private EnvDTE.Window _window;
        private EnvDTE.ToolBox _toolbox;
        private EnvDTE.ToolBoxTab _myTab;

        [DllImport("ole32.dll")]
        private static extern int CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);

        public void Init(TextWriter logWriter)
        {
            _logWriter = logWriter;
            Register();

            // Create an instance of the VS IDE.
            _has2008 = (_dte2008 = MakeDTE("VisualStudio.DTE.9.0")) != null;
            if (_has2008)
                _logWriter.WriteLine("{0}: Created DTE for Visual Studio 2008.", DateTime.Now);
            if (!_has2008)
                _logWriter.WriteLine("{0}: Failed to create DTE.", DateTime.Now);
        }

        public bool Has2008
        {
            get { return _has2008; }
        }

        private DTE MakeDTE(string progId)
        {
            try
            {
                Type type = System.Type.GetTypeFromProgID(progId);
                if (type == null)
                    return null;
                DTE dte = null;
                Retry(() => { dte = (DTE)System.Activator.CreateInstance(type, true); });
                return dte;
            }
            catch (Exception x)
            {
                _logWriter.WriteLine("{0}: {1}", DateTime.Now, x.ToString());
                return null;
            }
        }

        public void Cleanup()
        {
            CloseDTE(_dte2008);

            Revoke();

            // to ensure the dte object is actually released, and the devenv.exe process terminates.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void CloseDTE(DTE dte)
        {
            if (dte != null)
            {
                Retry(() => dte.Quit());
                Marshal.ReleaseComObject(dte);
            }
        }

        private delegate void TryMe();

        private void Retry(TryMe tryMe)
        {
            int count = 3;
            while (count > 0)
            {
                try
                {
                    tryMe();
                }
                catch (COMException x)
                {
                    _logWriter.WriteLine("{0}: {1}", DateTime.Now, x.ToString());
                    if ((uint)x.ErrorCode == 0x8001010A)
                    {
                        System.Threading.Thread.Sleep(5000);
                        --count;
                        continue;
                    }
                    throw;
                }
                catch (Exception x)
                {
                    _logWriter.WriteLine("{0}: {1}", DateTime.Now, x.ToString());
                    throw;
                }
                break;
            }
        }
        
        public void InstallControl(string ctrlPath, string tabName)
        {
            InstallControl(ctrlPath, tabName, _dte2008);
        }

        private void InstallControl(string ctrlPath, string tabName, DTE dte)
        {
            if (dte != null)
            {
                // create a temporary winform project;
                string tmpFile = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
                string tmpDir = string.Format("{0}{1}", Path.GetTempPath(), tmpFile);
                Retry(delegate()
                {
                    _solution = dte.Solution as Solution2;
                });
                string templatePath = _solution.GetProjectTemplate("WindowsApplication.zip", "CSharp");
                Retry(delegate()
                {
                    _proj = _solution.AddFromTemplate(templatePath, tmpDir, "dummyproj", false);
                });
                _logWriter.WriteLine("{0}: Created temporary project at {1}.", DateTime.Now, tmpDir);

                // add the control to the toolbox.
                Retry(delegate()
                {
                    _window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindToolbox);
                });
                Retry(delegate()
                {
                    _toolbox = (EnvDTE.ToolBox)_window.Object;
                });
                Retry(delegate()
                {
                    _myTab = _toolbox.ToolBoxTabs.Add(tabName);
                });
                _logWriter.WriteLine("{0}: Added tab.", DateTime.Now, tmpDir);
                Retry(delegate()
                {
                    _myTab.Activate();
                });
                _logWriter.WriteLine("{0}: Activated tab.", DateTime.Now, tmpDir);
                Retry(delegate()
                {
                    _myTab.ToolBoxItems.Add("MyUserControl", ctrlPath, vsToolBoxItemFormat.vsToolBoxItemFormatDotNETComponent);
                });
                _logWriter.WriteLine("{0}: Added controls to tab.", DateTime.Now, tmpDir);
                Retry(delegate()
                {
                    dte.Solution.Close(false);
                });
                _logWriter.WriteLine("{0}: Closed solution.", DateTime.Now, tmpDir);

                _logWriter.WriteLine("{0}: Succeeded.", DateTime.Now);
            }
        }

        public void UninstallControl(string tabName)
        {
            UninstallControl(tabName, _dte2008);
        }

        private void UninstallControl(string tabName, DTE dte)
        {
            if (dte != null)
            {
                Retry(delegate()
                {
                    _window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindToolbox);
                });
                Retry(delegate()
                {
                    _toolbox = (EnvDTE.ToolBox)_window.Object;
                });
                try
                {
                    Retry(delegate()
                    {
                        _myTab = _toolbox.ToolBoxTabs.Item(tabName);
                    });
                    Retry(delegate()
                    {
                        _myTab.Activate();
                    });
                    _logWriter.WriteLine("{0}: Activated tab.", DateTime.Now);
                    Retry(delegate()
                    {
                        _myTab.Delete();
                    });
                    _logWriter.WriteLine("{0}: Deleted tab.", DateTime.Now);
                }
                catch (ArgumentException)
                {
                    // The tab wasn't there. Ignore the error.
                    _logWriter.WriteLine("{0}: Tab not found.", DateTime.Now);
                }

                _logWriter.WriteLine("{0}: Succeeded.", DateTime.Now);
            }
        }

        private void Register()
        {
            IOleMessageFilter oldFilter;
            CoRegisterMessageFilter(this, out oldFilter);
        }

        private void Revoke()
        {
            IOleMessageFilter oldFilter;
            CoRegisterMessageFilter(null, out oldFilter);
        }

        #region IOleMessageFilter Members

        public int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
        {
            return 0; //SERVERCALL_ISHANDLED
        }

        public int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
        {
            if (dwRejectType == 2) // SERVERCALL_RETRYLATER
                return 200; // wait 2 seconds and try again
            return -1; // cancel call
        }

        public int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
        {
            return 2; //PENDINGMSG_WAITDEFPROCESS
        }

        #endregion
    }
}
