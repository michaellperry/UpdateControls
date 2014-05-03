using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;

namespace UpdateControls.XAML.Wrapper
{
    public class CustomMetadataProvider : IXamlMetadataProvider
    {
        private static Dictionary<Type, IXamlType> _xamlTypeFromCLR = new Dictionary<Type, IXamlType>();

        public IXamlType GetXamlType(string fullName)
        {
            return null;
        }

        public IXamlType GetXamlType(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(ObjectInstance<>))
                {
                    Type wrappedType = type.GenericTypeArguments[0];
                    return GetDependentType(wrappedType);
                }
            }
            return null;
        }

        public static IXamlType GetDependentType(Type wrappedType)
        {
            lock (_xamlTypeFromCLR)
            {
                IXamlType xamlType;
                if (!_xamlTypeFromCLR.TryGetValue(wrappedType, out xamlType))
                {
                    xamlType = new CustomTypeProvider(wrappedType);
                    _xamlTypeFromCLR.Add(wrappedType, xamlType);
                }
                return xamlType;
            }
        }

        public XmlnsDefinition[] GetXmlnsDefinitions()
        {
            return null;
        }
    }
}
