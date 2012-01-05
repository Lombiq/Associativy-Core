﻿using System.Linq;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions;
using Orchard.Mvc;
using Orchard.Themes;

namespace Associativy.FrontendEngines.Controllers
{
    [OrchardFeature("Associativy")]
    [Themed]
    // Some way to set the IAssociativyContext maybe?
    public abstract class FrontendEngineDispatcherController : Controller, IFrontendEngineController
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
            if (_componentContext.IsRegistered<IDiscoverableFrontendEngineController>())
            {
                var frontendEngineRegistration = (from registration in _componentContext.ComponentRegistry.Registrations
                                                  where
                                                     registration.Activator.LimitType.FullName.Contains(FrontendEngineName)
                                                     && registration.Services.Where(service => service.Description.StartsWith("Associativy.FrontendEngines.Controllers.IDiscoverableFrontendEngineController")).Count() == 1
                                                  select registration).First();

                var frontendEngineController = (IDiscoverableFrontendEngineController)_componentContext.ResolveComponent(frontendEngineRegistration, Enumerable.Empty<Parameter>());
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