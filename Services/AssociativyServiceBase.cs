using Associativy.Models;

namespace Associativy.Services
{
    public abstract class AssociativyServiceBase : IAssociativyService
    {
        // Although reference read/write is atomic, this approach might cause headaches when concurrent threads wanted to use
        // the same service with different graphDescriptors. Revise if necessary.
        private IAssociativyGraphDescriptor _associativyGraphDescriptor;
        public virtual IAssociativyGraphDescriptor GraphDescriptor
        {
            get { return _associativyGraphDescriptor; }
            set { _associativyGraphDescriptor = value; }
        }

        public AssociativyServiceBase(IAssociativyGraphDescriptor associativyGraphDescriptor)
        {
            _associativyGraphDescriptor = associativyGraphDescriptor;
        }
    }
}