
namespace UpdateControls.ManualTest
{
    public class ViewModel
    {
        private Model _model;

        public ViewModel(Model model)
        {
            _model = model;
        }

        public int Number1
        {
            get { return _model.FirstNumber; }
        }

        public int Number2
        {
            get { return _model.FirstNumber; }
        }

        public int Number3
        {
            get { return _model.SecondNumber; }
        }

        public int Number4
        {
            get { return _model.SecondNumber; }
        }

        public int Number5
        {
            get { return _model.FirstNumber + _model.SecondNumber; }
        }

        public int Number6
        {
            get { return _model.FirstNumber + _model.SecondNumber; }
        }

        public int Number7
        {
            get { return _model.FirstNumber + _model.SecondNumber; }
        }

        public int Number8
        {
            get { return _model.FirstNumber + _model.SecondNumber; }
        }
    }
}
