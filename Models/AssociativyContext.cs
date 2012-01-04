using Orchard.Localization;

namespace Associativy.Models
{
    /// <summary>
    /// Describes the context in which Associativy services are run, i.e. it stores information about the purpose and database
    /// of associations
    /// </summary>
    public abstract class AssociativyContext : IAssociativyContext
    {
        protected LocalizedString _graphName;
        public LocalizedString GraphName
        {
            get { return _graphName; }
        }

        protected string _technicalGraphName;
        public string TechnicalGraphName
        {
            get { return _technicalGraphName; }
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