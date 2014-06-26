using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace UpdateControls.XAML.Wrapper
{
    public abstract class ClassMember : PropertyDescriptor
    {
        private Type _objectInstanceType;
        private string _propertyName;
        private Type _valueType;
        private Func<IObjectInstance, ObjectProperty> _makeObjectProperty;

        private static readonly Type[] Primitives = new Type[]
        {
            typeof(string),
            typeof(Uri),
            typeof(Cursor)
        };

        private static readonly Type[] Bindables = new Type[]
        {
            typeof(DispatcherObject),
            typeof(INotifyPropertyChanged),
            typeof(INotifyCollectionChanged),
            typeof(ICommand),
            typeof(CommandBindingCollection),
            typeof(InputBindingCollection),
            typeof(InputScope),
            typeof(XmlLanguage)
        };

        public abstract Type UnderlyingType
        {
            get;
        }

        public abstract bool CanRead
        {
            get;
        }

        public override Type ComponentType
        {
            get { return _objectInstanceType; }
        }

        public override Type PropertyType
        {
            get { return _valueType; }
        }


        public abstract object GetObjectValue(object wrappedObject);
        public abstract void SetObjectValue(object wrappedObject, object value);

        protected ClassMember(string propertyName, Type propertyType, Type objectInstanceType)
            : base(propertyName, null)
        {
            _objectInstanceType = objectInstanceType;
            _propertyName = propertyName;

            // Determine which type of object property to create.
            Type valueType;
            if (IsPrimitive(propertyType))
            {
                _makeObjectProperty = objectInstance =>
                    new ObjectPropertyAtomNative(objectInstance, this);
                valueType = propertyType;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                // Figure out what it's an IEnumerable of.
                Type itemType;
                if (propertyType.GetGenericArguments().Length == 1)
                    itemType = propertyType.GetGenericArguments()[0];
                else
                    itemType = typeof(object);
                if (IsPrimitive(itemType))
                    _makeObjectProperty = objectInstance =>
                        new ObjectPropertyCollectionNative(objectInstance, this);
                else
                {
                    _makeObjectProperty = objectInstance =>
                        new ObjectPropertyCollectionObject(objectInstance, this);
                }
                valueType = typeof(IEnumerable);
            }
            else
            {
                _makeObjectProperty = objectInstance =>
                    new ObjectPropertyAtomObject(objectInstance, this);
                valueType = typeof(IObjectInstance);
            }

            _valueType = valueType;
        }

        public ObjectProperty MakeObjectProperty(IObjectInstance objectInstance)
        {
            return _makeObjectProperty(objectInstance);
        }

        public override object GetValue(object component)
        {
            return GetObjectProperty(component).Value;
        }

        public override void SetValue(object component, object value)
        {
            GetObjectProperty(component).OnUserInput(value);
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", _objectInstanceType.Name, _propertyName);
        }

        private ObjectProperty GetObjectProperty(object component)
        {
            // Find the object property.
            IObjectInstance objectInstance = ((IObjectInstance)component);
            ObjectProperty objectProperty = objectInstance.LookupProperty(this);
            return objectProperty;
        }

        private static bool IsPrimitive(Type type)
        {
            return
                type.IsValueType ||
                type.IsPrimitive ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) ||
                Primitives.Contains(type) ||
                // Don't wrap objects that are already bindable
                Bindables.Any(b => b.IsAssignableFrom(type));
        }
    }
}
