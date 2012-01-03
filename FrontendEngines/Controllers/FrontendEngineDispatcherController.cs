using System.Linq;
using System.Web.Mvc;
using Associativy.Models;
using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions;
using Orchard.Mvc;
using Orchard.Themes;

namespace Associativy.FrontendEngines.Controllers
{
    [OrchardFeature("Associativy")]
    [Themed]
    public class FrontendEngineDispatcherController<TNodeToNodeConnectorRecord, TAssociativyContext> : Controller, IFrontendEngineController
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
        where TAssociativyContext : IAssociativyContext
    {
        private readonly IComponentContext _componentContext;

        protected virtual string FrontendEngineName
        {
            get { return "Dracula"; }
        }

        public FrontendEngineDispatcherController(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            if (_componentContext.IsRegistered<IDiscoverableFrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>>())
            {
                var frontendEngineRegistration = (from registration in _componentContext.ComponentRegistry.Registrations
                                                  where
                                                     registration.Activator.LimitType.FullName.Contains(FrontendEngineName)
                                                     && registration.Services.Where(service => service.Description.StartsWith("Associativy.FrontendEngines.Controllers.IDiscoverableFrontendEngineController")).Count() == 1
                                                  select registration).First();

                var frontendEngineController = (IDiscoverableFrontendEngineController<TNodeToNodeConnectorRecord, TAssociativyContext>)frontendEngineRegistration.Activator.ActivateInstance(_componentContext, Enumerable.Empty<Parameter>());
                frontendEngineController.Execute(ControllerContext.RequestContext);
            }

            // This way theming gets applied, but the result should be somehow captured.
            ActionInvoker.InvokeAction(this.ControllerContext, "Dispatch");
        }

        public ViewResult Dispatch()
        {
            return new ShapeResult(this, null);
        }
    }
}