using Associativy.Models;
using Orchard.Environment.Extensions;

// If circular dependencies should happen, use property injection:
// http://www.szmyd.com.pl/blog/di-property-injection-in-orchard
// http://code.google.com/p/autofac/wiki/CircularDependencies

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyServices: AssociativyServiceBase, IAssociativyServices
    {
        private object _graphDescriptorLocker = new object();
        public override IAssociativyGraphDescriptor GraphDescriptor
        {
            set
            {
                lock (_graphDescriptorLocker) // This is to ensure that used services also have the same graphDescriptor
                {
                    _nodeManager.GraphDescriptor = value;
                    _mind.GraphDescriptor = value;
                    base.GraphDescriptor = value;
                }
            }
        }

        public IConnectionManager ConnectionManager
        {
            get { return GraphDescriptor.ConnectionManager; }
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
            IAssociativyGraphDescriptor associativyGraphDescriptor,
            IMind mind,
            INodeManager nodeManager)
            : base(associativyGraphDescriptor)
        {
            _nodeManager = nodeManager;
            _mind = mind;
        }
    }
}