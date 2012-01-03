using Associativy.Models;

namespace Associativy.Services
{
    public abstract class AssociativyService<TAssociativyContext>
        where TAssociativyContext : IAssociativyContext
    {
        protected readonly TAssociativyContext _associativyContext;

        public AssociativyService(TAssociativyContext associativyContext)
        {
            _associativyContext = associativyContext;
        }
    }
}