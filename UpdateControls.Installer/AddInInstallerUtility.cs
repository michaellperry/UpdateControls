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
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;

namespace UpdateControls.Installer
{
    class AddInInstallerUtility
    {
        /// <summary>
        /// Namespace used in the .addin configuration file.
        /// </summary>         
        private const string ExtNameSpace = "http://schemas.microsoft.com/AutomationExtensibility";

        private TextWriter _logWriter;

        public AddInInstallerUtility(TextWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public void InstallAddIn(IDictionary savedState, string assemblyPath, string assemblyName, ToolboxInstallerUtility.VisualStudioVersionID visualStudioVersion)
        {
            // Setup .addin path and assembly path
            string edition = visualStudioVersion == ToolboxInstallerUtility.VisualStudioVersionID.VS2005 ? "2005" : "2008";
            string addinTargetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Visual Studio " + edition + @"\Addins" );
            string addinControlFileName = assemblyName + "." + edition + ".Addin";
            string addinAssemblyFileName = assemblyName + ".dll";
            _logWriter.WriteLine("{0}: Installing add-in target={1}, control file={2}, assembly file={3}.",
                DateTime.Now,
                addinTargetPath,
                addinControlFileName,
                addinAssemblyFileName);

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(addinTargetPath);
                if (!dirInfo.Exists)
                {
                    _logWriter.WriteLine("{0}: Creating directory {1}.",
                        DateTime.Now,
                        addinTargetPath);
                    dirInfo.Create();
                }

                string sourceFile = Path.Combine(assemblyPath, addinControlFileName);
                _logWriter.WriteLine("{0}: Loading control file {1}.",
                    DateTime.Now,
                    sourceFile);
                XmlDocument doc = new XmlDocument();
                doc.Load(sourceFile);
                XmlNamespaceManager xnm = new XmlNamespaceManager(doc.NameTable);
                xnm.AddNamespace("def", ExtNameSpace);

                // Update Addin/Assembly node
                string assemblyFullPath = Path.Combine(assemblyPath, addinAssemblyFileName);
                XmlNode node = doc.SelectSingleNode("/def:Extensibility/def:Addin/def:Assembly", xnm);
                if (node != null)
                {
                    _logWriter.WriteLine("{0}: Setting assembly path to {1}.",
                        DateTime.Now,
                        assemblyFullPath);
                    node.InnerText = assemblyFullPath;
                }
                else
                {
                    _logWriter.WriteLine("{0}: Extensibility/Addin/Assembly node not found!", DateTime.Now);
                }

                // Update ToolsOptionsPage/Assembly node
                node = doc.SelectSingleNode("/def:Extensibility/def:ToolsOptionsPage/def:Category/def:SubCategory/def:Assembly", xnm);
                if (node != null)
                {
                    _logWriter.WriteLine("{0}: Setting assembly path to {1}.",
                        DateTime.Now,
                        assemblyFullPath);
                    node.InnerText = assemblyFullPath;
                }
                else
                {
                    _logWriter.WriteLine("{0}: Extensibility/ToolsOptionsPage/Category/SubCategory/Assembly node not found!", DateTime.Now);
                }

                _logWriter.WriteLine("{0}: Saving control file {1}.",
                    DateTime.Now,
                    sourceFile);
                doc.Save(sourceFile);

                string targetFile = Path.Combine(addinTargetPath, addinControlFileName);
                _logWriter.WriteLine("{0}: Copying control file from {1} to {2}.",
                    DateTime.Now,
                    sourceFile,
                    targetFile);
                File.Copy(sourceFile, targetFile, true);

                // Save AddinPath to be used in Uninstall or Rollback
                savedState.Add("AddinPath", targetFile);
            }
            catch (Exception ex)
            {
                _logWriter.WriteLine("{0}: Failed to install add-in: {1}", DateTime.Now, ex.ToString());
            }
        }

        public void RemoveAddIn(IDictionary savedState)
        {
            try
            {
                string fileName = (string)savedState["AddinPath"];
                _logWriter.WriteLine("{0}: Deleting add-in file {1}.", DateTime.Now, fileName);
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    _logWriter.WriteLine("{0}: Success.", DateTime.Now);
                }
                else
                {
                    _logWriter.WriteLine("{0}: Add-in file does not exist.", DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                _logWriter.WriteLine("{0}: Failed to delete add-in file: {1}", DateTime.Now, ex.ToString());
            }
        }
    }
}
