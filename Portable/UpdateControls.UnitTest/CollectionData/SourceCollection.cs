using System.Collections.Generic;

namespace UpdateControls.UnitTest.CollectionData
{
    public class SourceCollection
    {
        private List<int> _numbers = new List<int>();
        private Independent _indNumbers = new Independent();

        public void Insert(int number)
        {
            _indNumbers.OnSet();
            _numbers.Add(number);
        }

        public IEnumerable<int> Numbers
        {
            get { _indNumbers.OnGet(); return _numbers; }
        }
    }
}
