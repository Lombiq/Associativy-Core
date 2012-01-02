using Associativy.FrontendEngines;
using Associativy.FrontendEngines.Engines.Dracula;
using Associativy.Services;
using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.Engines.Graphviz;
using Associativy.FrontendEngines.Controllers;
using Orchard.ContentManagement;
using Associativy.Models;
using Orchard.ContentManagement.Records;
using Associativy.FrontendEngines.Engines.Graphviz.Services;

namespace Associativy
{
    [OrchardFeature("Associativy")]
    public class AssociativyModule : IModule
    {
        public void Configure(IComponentRegistry componentRegistry)
        {
            // Ideally all these registrations should not exist, but currently generics are not properly auto-registered
            // by Orchard.
            var builder = new ContainerBuilder();


            builder.RegisterGeneric(typeof(AssociativyServices<,>)).As(typeof(IAssociativyServices<,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ConnectionManager<,>)).As(typeof(IConnectionManager<,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Mind<,>)).As(typeof(IMind<,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(NodeManager<>)).As(typeof(INodeManager<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(PathFinder<,>)).As(typeof(IPathFinder<,>)).InstancePerLifetimeScope();


            // Frontend engines

            // Dracula
            builder.RegisterGeneric(typeof(Associativy.FrontendEngines.Engines.Dracula.Controllers.FrontendEngineController<,>)).As(typeof(IDiscoverableFrontendEngineController<,>)).InstancePerLifetimeScope();

            //// Graphviz
            builder.RegisterGeneric(typeof(GraphImageService<>)).As(typeof(IGraphImageService<>)).InstancePerLifetimeScope();

            //// JIT


            builder.Update(componentRegistry);
        }
    }
}