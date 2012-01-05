using Associativy.Models;
using Orchard;

namespace Associativy.FrontendEngines.Controllers
{
    /// <summary>
    /// Marker interface for frontend engine controllers that are auto-discovered and dispatched to from FrontendEngineDispatcherController
    /// </summary>
    public interface IDiscoverableFrontendEngineController : IFrontendEngineController, IDependency//, IAssociativyService?
    {
    }
}
