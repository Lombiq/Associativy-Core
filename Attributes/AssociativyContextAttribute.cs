using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;

namespace Associativy.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AssociativyContextAttribute : Attribute
    {
        private readonly string _technicalName;
        public string TechnicalName
        {
            get { return _technicalName; }
        }

        public LocalizedString Name { get; set; }

        public AssociativyContextAttribute(string technicalName)
        {
            _technicalName = technicalName;
        }
    }
}