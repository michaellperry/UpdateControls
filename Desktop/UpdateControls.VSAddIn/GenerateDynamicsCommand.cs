/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using EnvDTE;

namespace UpdateControls.VSAddIn
{
    class GenerateDynamicsCommand
    {
        public static void GenerateIndependentProperties(DTE2 application)
        {
            ProjectItem projItem = application.ActiveDocument.ProjectItem;
            TextSelection selection = (TextSelection)application.ActiveDocument.Selection;
            string languageStr = application.ActiveDocument.Language;
            Language language = languageStr == "CSharp" ? Language.CS :
                languageStr == "Basic" ? Language.VB :
                Language.VB;

            // Parse the selection to find all fields.
            string[] lines =
                language == Language.CS ?
                    selection.Text.Split(new char[] { ';' }) :
                    selection.Text.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.None);
            StringBuilder dynamics = new StringBuilder();
            StringBuilder properties = new StringBuilder();
            bool matchedLines = false;

            FieldParser parser = new FieldParser(language);
            FieldProducer producer = new FieldProducer(language);
            foreach (string line in lines)
            {
                parser.Line = line;
                if (parser.IsCollection)
                {
                    // Extract the collection, type, and name.
                    string propertyName = GetPropertyName(parser.Name);
                    string singular = Singular(propertyName);
                    string valueName = ValueName(singular);
                    if (!string.IsNullOrEmpty(propertyName))
                    {
                        producer.AppendCollection(dynamics, properties, parser.Type, parser.Name, propertyName, singular, valueName);
                        matchedLines = true;
                    }
                }
                else if (parser.IsAtom)
                {
                    // Extract the type and name.
                    string propertyName = GetPropertyName(parser.Name);
                    if (!string.IsNullOrEmpty(propertyName))
                    {
                        producer.AppendAtom(dynamics, properties, propertyName, parser.Type, parser.Name);
                        matchedLines = true;
                    }
                }
            }

            if (matchedLines)
            {
                int start = selection.BottomLine;

                // Search for the beginning of the region.
                selection.MoveToLineAndOffset(1, 1, false);
                if (FindText(application, FieldProducer.BeginTagGeneratedRegion))
                {
                    // Scan for the end of the block of dynamic sentries.
                    selection.MoveToLineAndOffset(selection.TopLine + 1, 1, false);
                    selection.LineDown(true, 1);
                    while (!selection.IsEmpty && producer.IsIndependentSentry(selection.Text))
                    {
                        selection.MoveToLineAndOffset(selection.BottomLine, 1, false);
                        selection.LineDown(true, 1);
                    }

                    // Insert the dynamic sentry at the end.
                    selection.MoveToLineAndOffset(selection.TopLine, 1, false);
                    selection.Insert(
                        dynamics.ToString(),
                        (int)vsInsertFlags.vsInsertFlagsContainNewText);
                    application.ExecuteCommand("Edit.FormatSelection", "");
                    selection.MoveToLineAndOffset(selection.BottomLine, 1, false);

                    if (FindText(application, FieldProducer.EndTagGeneratedRegion))
                    {
                        selection.MoveToLineAndOffset(selection.TopLine, 1, false);
                    }
                    selection.Insert(
                        properties.ToString(),
                        (int)vsInsertFlags.vsInsertFlagsContainNewText);
                    application.ExecuteCommand("Edit.FormatSelection", "");
                }
                else
                {
                    // Insert the dynamics.
                    selection.MoveToLineAndOffset(start, 1, false);
                    selection.Insert(
                        producer.BeginRegion +
                            dynamics.ToString(),
                        (int)vsInsertFlags.vsInsertFlagsContainNewText);
                    application.ExecuteCommand("Edit.FormatSelection", "");

                    // Insert the properties.
                    selection.MoveToLineAndOffset(selection.BottomLine, 1, false);
                    selection.Insert(
                        properties.ToString() +
                            producer.EndRegion,
                        (int)vsInsertFlags.vsInsertFlagsContainNewText);
                    application.ExecuteCommand("Edit.FormatSelection", "");
                }

                if (producer.RequiresUsing)
                {
                    // Insert the using statement.
                    selection.MoveToLineAndOffset(1, 1, false);
                    selection.LineDown(true, 1);
                    UsingParser usingParser = new UsingParser();
                    int insertPoint = 1;
                    while (!selection.IsEmpty)
                    {
                        usingParser.Line = selection.Text;
                        if (usingParser.IsUsingUpdateControls)
                            break;
                        else if (usingParser.IsUsing)
                            insertPoint = selection.BottomLine;
                        else if (usingParser.IsNamespace)
                        {
                            selection.MoveToLineAndOffset(insertPoint, 1, false);
                            selection.Insert("using UpdateControls;\r\n", (int)vsInsertFlags.vsInsertFlagsContainNewText);
                            break;
                        }
                        selection.MoveToLineAndOffset(selection.BottomLine, 1, false);
                        selection.LineDown(true, 1);
                    }
                }

                selection.MoveToLineAndOffset(start, 1, false);
            }
        }

        private static bool FindText(DTE2 application, string searchText)
        {
            application.Find.FindWhat = searchText;
            application.Find.Target = vsFindTarget.vsFindTargetCurrentDocument;
            application.Find.MatchCase = true;
            application.Find.MatchWholeWord = false;
            application.Find.Backwards = false;
            application.Find.MatchInHiddenText = true;
            application.Find.PatternSyntax = vsFindPatternSyntax.vsFindPatternSyntaxLiteral;
            application.Find.Action = vsFindAction.vsFindActionFind;
            return application.Find.Execute() == vsFindResult.vsFindResultFound;
        }

        private static string GetPropertyName(string name)
        {
            string propertyName = null;
            if (!string.IsNullOrEmpty(name))
            {
                int firstChar = 0;
                while (firstChar < name.Length && name[firstChar] == '_')
                    ++firstChar;
                int firstLower = firstChar;
                while (firstLower < name.Length && !char.IsLower(name[firstLower]))
                    ++firstLower;
                if (firstLower < name.Length)
                {
                    propertyName =
                        name.Substring(firstChar, firstLower - firstChar) +
                        char.ToUpper(name[firstLower]) +
                        name.Substring(firstLower + 1);
                }
            }
            return propertyName;
        }

        private static Dictionary<string, string> BuildMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("Children", "Child");
            map.Add("People", "Person");
            map.Add("Processes", "Process");
            return map;
        }

        private static Dictionary<string, string> PluralToSingular = BuildMap();

        private static string Singular(string p)
        {
            // First check for special cases.
            if (PluralToSingular.ContainsKey(p))
                return PluralToSingular[p];
            // Drop the trailing "s" if it exists.
            else if (char.ToLower(p[p.Length - 1]) == 's')
                return p.Substring(0, p.Length - 1);
            else
                return p;
        }

        private static string ValueName(string singular)
        {
            // Convert the first upper-case character to lower case.
            StringBuilder valueName = new StringBuilder(singular);
            for (int index = 0; index < valueName.Length; ++index)
            {
                if (char.IsUpper(valueName[index]))
                {
                    valueName[index] = char.ToLower(valueName[index]);
                    break;
                }
            }
            return valueName.ToString();
        }
    }
}
