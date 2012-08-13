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
        public IXamlType GetXamlType(string fullName)
        {
            return null;
        }

        public IXamlType GetXamlType(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(DependentObject<>))
                {
                    return new CustomTypeProvider(type.GenericTypeArguments[0]);
                }
            }
            return null;
        }

        public XmlnsDefinition[] GetXmlnsDefinitions()
        {
            return null;
        }
    }
}
