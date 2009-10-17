
namespace UpdateControls
{
    public partial class Dependent
    {
        private static Dependent _currentUpdateSlot = null;

        internal static Dependent GetCurrentUpdate()
        {
            return _currentUpdateSlot;
        }

        private static void SetCurrentUpdate(Dependent dependent)
        {
            _currentUpdateSlot = dependent;
        }
    }
}