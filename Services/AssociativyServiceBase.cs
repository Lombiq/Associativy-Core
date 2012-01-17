using Associativy.Models;

namespace Associativy.Services
{
    public abstract class AssociativyServiceBase : IAssociativyService
    {
        // Although reference read/write is atomic, this approach might cause headaches when concurrent threads wanted to use
        // the same service with different graphDescriptors. Revise if necessary.
        private IGraphDescriptor _graphDescriptor;
        public virtual IGraphDescriptor GraphDescriptor
        {
            get { return _graphDescriptor; }
            set { _graphDescriptor = value; }
        }

        protected AssociativyServiceBase(IGraphDescriptor graphDescriptor)
        {
            _graphDescriptor = graphDescriptor;
        }
    }
}