using Associativy.GraphDiscovery;
using Associativy.Queryable;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    public class AssociativyServices : IAssociativyServices
    {
        protected readonly IGraphManager _graphManager;
        public IGraphManager GraphManager { get { return _graphManager; } }

        protected readonly IGraphEditor _graphEditor;
        public IGraphEditor GraphEditor { get { return _graphEditor; } }

        protected readonly IQueryableGraphFactory _queryableGraphFactory;
        public IQueryableGraphFactory QueryableGraphFactory { get { return _queryableGraphFactory; } }


        public AssociativyServices(
            IGraphManager graphManager,
            IGraphEditor graphEditor,
            IQueryableGraphFactory queryableGraphFactory)
        {
            _graphManager = graphManager;
            _graphEditor = graphEditor;
            _queryableGraphFactory = queryableGraphFactory;
        }
    }
}