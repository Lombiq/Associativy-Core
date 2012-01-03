using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Associativy.Models
{
    /// <summary>
    /// Describes the context in which Associativy services are run, i.e. it stores information about the purpose and database
    /// of associations
    /// </summary>
    public abstract class AssociativyContext : IAssociativyContext
    {
        protected LocalizedString _name;
        public LocalizedString Name
        {
            get { return _name; }
        }

        protected string _technicalName;
        public string TechnicalName
        {
            get { return _technicalName; }
        }

        protected string[] _contentTypeNames;
        public string[] ContentTypeNames
        {
            get { return _contentTypeNames; }
        }

        public Localizer T { get; set; }

        public AssociativyContext()
        {
            T = NullLocalizer.Instance;
        }
    }
}