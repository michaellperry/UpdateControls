using UpdateControls.Fields;

namespace UpdateControls.ManualTest
{
    public class Model
    {
        private Independent<int> _firstNumber = new Independent<int>();
        private Independent<int> _secondNumber = new Independent<int>();

        private object _firstLock = new object();
        private object _secondLock = new object();

        public int FirstNumber
        {
            get
            {
                lock (_firstLock)
                {
                    return _firstNumber;
                }
            }
            set
            {
                lock (_firstLock)
                {
                    _firstNumber.Value = value;
                }
            }
        }

        public int SecondNumber
        {
            get
            {
                lock (_secondLock)
                {
                    return _secondNumber;
                }
            }
            set
            {
                lock (_secondLock)
                {
                    _secondNumber.Value = value;
                }
            }
        }
    }
}
