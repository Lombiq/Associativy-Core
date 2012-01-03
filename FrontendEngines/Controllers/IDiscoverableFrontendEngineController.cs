using Associativy.Models;

namespace Associativy.FrontendEngines.Controllers
{
    /// <summary>
    /// Marker interface for frontend engine controllers that are auto-discovered and dispatched to from FrontendEngineDispatcherController
    /// </summary>
    public interface IDiscoverableFrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext> : IFrontendEngineController//, IDependency
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
    }
}
