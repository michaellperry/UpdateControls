using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateControls.XAML;
using $rootnamespace$.Models;

namespace $rootnamespace$.ViewModels
{
    public class ViewModelLocator
    {
        private MainViewModel _main;

        public ViewModelLocator()
        {
            Document document = LoadDocument();
			Selection selection = new Selection();
            _main = new MainViewModel(document, selection);
        }

        public object Main
        {
            get { return ForView.Wrap(_main); }
        }

		private Document LoadDocument()
		{
			// TODO: Load your document here.
			return new Document();
		}
    }
}
