using Associativy.Models;
using Orchard.Environment.Extensions;

// If circular dependencies should happen, use property injection:
// http://www.szmyd.com.pl/blog/di-property-injection-in-orchard
// http://code.google.com/p/autofac/wiki/CircularDependencies

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices: AssociativyService, IAssociativyServices
    {
        private object _contextLocker = new object();
        public override IAssociativyContext Context
        {
            set
            {
                lock (_contextLocker) // This is to ensure that used services also have the same context
                {
                    _nodeManager.Context = value;
                    _mind.Context = value;
                    base.Context = value;
                }
            }
        }

        public IConnectionManager ConnectionManager
        {
            get { return Context.ConnectionManager; }
        }

        protected readonly IMind _mind;
        public IMind Mind
        {
            get { return _mind; }
        }

        protected readonly INodeManager _nodeManager;
        public INodeManager NodeManager
        {
            get { return _nodeManager; }
        }

        public AssociativyServices(
            IAssociativyContext associativyContext,
            IMind mind,
            INodeManager nodeManager)
            : base(associativyContext)
        {
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}