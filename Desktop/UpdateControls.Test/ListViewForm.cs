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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UpdateControls;

namespace UpdateControls.Test
{
    public partial class ListViewForm : Form
    {
        private Document _document = new Document();

        public ListViewForm()
        {
            InitializeComponent();
        }

        private System.Collections.IEnumerable personListView_GetItems()
        {
            return _document.People;
        }

        private System.Collections.IEnumerable personListView_GetGroups()
        {
            return _document.Companies;
        }

        private object personListView_GetItemGroup(object tag)
        {
            Person person = (Person)tag;
            return person.Employer;
        }

        private string personListView_GetGroupName(object tag)
        {
            return string.Format("Name - {0}", ((Company)tag).Name);
        }

        private string personListView_GetGroupHeader(object tag)
        {
            return tag.ToString();
            //return string.Format("Header - {0}", ((Company)tag).Name);
        }

        private HorizontalAlignment personListView_GetGroupAlignment(object tag)
        {
            return HorizontalAlignment.Right;
        }

        private string personListView_GetItemText(object tag)
        {
            Person person = (Person)tag;
            return person.Name;
        }
    }
}