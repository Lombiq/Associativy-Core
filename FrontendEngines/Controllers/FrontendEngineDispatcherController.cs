using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Associativy.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using System.Web.Routing;
using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions;
using Orchard;
using Associativy.FrontendEngines.Engines.Dracula;

namespace Associativy.FrontendEngines.Controllers
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineDispatcherController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord> : Controller, IFrontendEngineController<TAssocociativyServices, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TAssocociativyServices : IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeToNodeConnectorRecord : INodeToNodeConnectorRecord, new()
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

        protected override void Execute(RequestContext requestContext)
        {
            //var controllerServiceType = (from registration in _componentContext.ComponentRegistry.Registrations
            //                      where registration.Activator.LimitType.FullName.Contains(FrontendEngineName) && registration.Activator.LimitType.FullName.Contains("FrontendEngineController")
            //                      select ((TypedService)registration.Services.First()).ServiceType).FirstOrDefault();

            //var controllerRegistration = (from registration in _componentContext.ComponentRegistry.Registrations
            //                              where
            //                                typeof(IFrontendEngineController).IsAssignableFrom(registration.Activator.LimitType)
            //                                && registration.Activator.LimitType.FullName.Contains(FrontendEngineName)
            //                              select registration).FirstOrDefault();

            //var d = _componentContext.Resolve<IDiscoverableFrontendEngineController<IAssociativyServices<TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>, TNodePart, TNodePartRecord, TNodeToNodeConnectorRecord>>();


            //var z = Type.GetType("Associativy.FrontendEngines.Engines." + FrontendEngineName + ".Controllers.IFrontendEngineController, Associativy");
            //var frontendEngineController = (IController)_componentContext.Resolve(
            //    Type.GetType("Associativy.FrontendEngines.Engines." + FrontendEngineName + ".Controllers.IFrontendEngineController")
            //    .MakeGenericType(
            //        typeof(TAssocociativyServices), 
            //        typeof(TNodePart), 
            //        typeof(TNodePartRecord), 
            //        typeof(TNodeToNodeConnectorRecord)));

            //frontendEngineController.Execute(requestContext);
            //base.Execute(requestContext);
        }
    }
}