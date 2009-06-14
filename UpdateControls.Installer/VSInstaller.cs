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
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using Microsoft.Win32;

namespace UpdateControls.Installer
{
    [RunInstaller(true)]
    public partial class VSInstaller : System.Configuration.Install.Installer
    {
        public VSInstaller()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            string targetDir = Context.Parameters["TargetDir"];
            stateSaver.Add("TargetDir", targetDir);
            Directory.CreateDirectory(targetDir);
            string formsPath = Path.Combine(targetDir, "UpdateControls.Forms.dll");
            string themesPath = Path.Combine(targetDir, "UpdateControls.Themes.dll");
            using (TextWriter logWriter = new StreamWriter(formsPath + ".install.log", true))
            {
                try
                {
                    // Install the add-in.
                    AddInInstallerUtility addInUtility = new AddInInstallerUtility(logWriter);
                    addInUtility.InstallAddIn(stateSaver, targetDir, "UpdateControls.VSAddIn");

                    // Add the update controls to the toolbox.
                    if (!File.Exists(formsPath))
                        throw new Exception(string.Format("Target file not found: {0}", formsPath));

                    logWriter.WriteLine("{0}: Installing control at {1}.", DateTime.Now, formsPath);

                    ToolboxInstallerUtility utility = new ToolboxInstallerUtility();
                    utility.Init(logWriter);

                    // Delete the tab if it is already present.
                    utility.UninstallControl("Update Controls");
                    utility.UninstallControl("Update Controls Themes");

                    // Install the controls.
                    utility.InstallControl(formsPath, "Update Controls");
                    utility.InstallControl(themesPath, "Update Controls Themes");

                    utility.Cleanup();
                }
                catch (Exception x)
                {
                    logWriter.Write(x.ToString());
                }
            }

            base.Install(stateSaver);
        }

        public override void Rollback(System.Collections.IDictionary savedState)
        {
            string targetDir = savedState.Contains("TargetDir") ?
                (string)savedState["TargetDir"] :
                @"C:\Program Files\Mallard Software Designs\Update Controls";
            string formsPath = Path.Combine(targetDir, "UpdateControls.Forms.dll");
            using (TextWriter logWriter = new StreamWriter(formsPath + ".install.log", true))
            {
                try
                {
                    logWriter.WriteLine("{0}: Rolling back controls at {1}.", DateTime.Now, formsPath);
                    if (!savedState.Contains("TargetDir"))
                        logWriter.WriteLine("{0}: Target dir was not found!", DateTime.Now);
                    ToolboxInstallerUtility utility = new ToolboxInstallerUtility();
                    utility.Init(logWriter);

                    // Delete the tabs if present.
                    utility.UninstallControl("Update Controls");
                    utility.UninstallControl("Update Controls Themes");

                    // Roll back the add-in.
                    AddInInstallerUtility addInUtility = new AddInInstallerUtility(logWriter);
                    addInUtility.RemoveAddIn(savedState);
                }
                catch (Exception x)
                {
                    logWriter.Write(x.ToString());
                }
            }

            base.Rollback(savedState);
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            string targetDir = savedState.Contains("TargetDir") ?
                (string)savedState["TargetDir"] :
                @"C:\Program Files\Mallard Software Designs\Update Controls";
            string formsPath = Path.Combine(targetDir, "UpdateControls.Forms.dll");
            using (TextWriter logWriter = new StreamWriter(formsPath + ".install.log", true))
            {
                try
                {
                    logWriter.WriteLine("{0}: Uninstalling controls at {1}.", DateTime.Now, formsPath);
                    if (!savedState.Contains("TargetDir"))
                        logWriter.WriteLine("{0}: Target dir was not found!", DateTime.Now);
                    ToolboxInstallerUtility utility = new ToolboxInstallerUtility();
                    utility.Init(logWriter);

                    // Delete the tabs if present.
                    utility.UninstallControl("Update Controls");
                    utility.UninstallControl("Update Controls Themes");

                    utility.Cleanup();

                    // Uninstall the add-in.
                    AddInInstallerUtility addInUtility = new AddInInstallerUtility(logWriter);
                    addInUtility.RemoveAddIn(savedState);
                }
                catch (Exception x)
                {
                    logWriter.Write(x.ToString());
                }
            }

            base.Uninstall(savedState);
        }
    }
}