using Associativy.Models;

namespace Associativy.Services
{
    public abstract class AssociativyService : IAssociativyService
    {
        // Although reference read/write is atomic, this approach might cause headaches when concurrent threads wanted to use
        // the same service with different contexts. Revise if necessary.
        private IAssociativyContext _associativyContext;
        public virtual IAssociativyContext Context
        {
            get { return _associativyContext; }
            set { _associativyContext = value; }
        }
        
        public AssociativyService(IAssociativyContext associativyContext)
        {
            _associativyContext = associativyContext;
        }
    }
}