using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UpdateControls.Collections;
using System.Collections;
using System.Threading;
using UpdateControls.Fields;

namespace UpdateControls.Test
{
    public partial class ListBoxForm : Form
    {
        private IndependentList<Person> _people = new IndependentList<Person>();
        private Independent<Person> _selectedPerson = new Independent<Person>();
        private Random _random = new Random();

        public ListBoxForm()
        {
            InitializeComponent();

            QueueNext();
        }

        private IEnumerable personListBox_GetItems()
        {
            lock (this)
            {
                return new List<Person>(_people);
            }
        }

        private string personListBox_GetItemText(object tag)
        {
            return ((Person)tag).Name;
        }

        private void QueueNext()
        {
            ThreadPool.QueueUserWorkItem(o => Background());
        }

        private void Background()
        {
            Thread.Sleep(500);
            lock (this)
            {
                _people.Add(new Person() { Name = _random.Next().ToString() });
            }
            QueueNext();
        }

        private object personListBox_GetSelectedItem()
        {
            return _selectedPerson.Value;
        }

        private void personListBox_SetSelectedItem(object value)
        {
            _selectedPerson.Value = (Person)value;
        }
    }
}
