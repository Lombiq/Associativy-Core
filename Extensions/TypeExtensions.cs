using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.Attributes;

namespace Associativy.Extensions
{
    public static class TypeExtensions
    {
        public static string GetAssociativyContextName(this Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(AssociativyContextAttribute), false);

            if (attributes.Length == 1) return ((AssociativyContextAttribute)attributes[0]).TechnicalName;

            var strippedName = type.Name.Replace("ConnectorRecord", "");
            strippedName = strippedName.Replace("Record", "");

            return type.Module.Name + strippedName;
        }
    }
}