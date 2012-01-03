using Orchard.Localization;

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

        protected int _maxZoomLevel = 10;
        public int MaxZoomLevel
        {
            get { return _maxZoomLevel; }
        }

        public Localizer T { get; set; }

        public AssociativyContext()
        {
            T = NullLocalizer.Instance;
        }
    }
}