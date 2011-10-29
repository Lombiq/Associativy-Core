using Associativy.Services;
using Autofac;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyHandler : ContentHandler
    {
        private readonly IComponentContext _componentContext;

        public AssociativyHandler(IComponentContext componentContext)
        {
            _componentContext = componentContext;

            // If the debugger is not running, these registrations get destroyed if a code file is modifed, causing YSOD.
            // Therefore after a change: F5
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(AssociativyServices<,,>)).As(typeof(IAssociativyServices<,,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ConnectionManager<,,>)).As(typeof(IConnectionManager<,,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Mind<,,>)).As(typeof(IMind<,,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(NodeManager<,>)).As(typeof(INodeManager<,>)).InstancePerLifetimeScope();

            builder.Update(_componentContext.ComponentRegistry);
        }
    }
}