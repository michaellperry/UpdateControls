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

namespace UpdateControls.Test
{
    public partial class ListBoxForm : Form
    {
        private IndependentList<Person> _people = new IndependentList<Person>();
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
    }
}
