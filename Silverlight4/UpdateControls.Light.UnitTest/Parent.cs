using System.Collections.Generic;
using UpdateControls.Collections;

namespace UpdateControls.Light.UnitTest
{
    public class Parent
    {
        private IndependentList<Child> _children = new IndependentList<Child>();

        public void AddChild(Child child)
        {
            _children.Add(child);
        }

        public void RemoveChild(Child child)
        {
            _children.Remove(child);
        }

        public IEnumerable<Child> Children
        {
            get { return _children; }
        }
    }
}
