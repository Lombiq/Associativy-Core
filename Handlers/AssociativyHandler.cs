using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Autofac;
using Associativy.Services;

namespace Associativy.Handlers
{
    [OrchardFeature("Associativy")]
    public class AssociativyHandler : ContentHandler
    {
        private readonly IComponentContext _componentContext;

        public AssociativyHandler(IComponentContext componentContext)
        {
            _componentContext = componentContext;
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(AssociativyServices<,,,>)).As(typeof(IAssociativyServices<,,,>));
            builder.RegisterGeneric(typeof(ConnectionManager<,,,>)).As(typeof(IConnectionManager<,,,>));
            builder.RegisterGeneric(typeof(Mind<,,,>)).As(typeof(IMind<,,,>));
            builder.RegisterGeneric(typeof(NodeManager<,,>)).As(typeof(INodeManager<,,>));
            builder.Update(_componentContext.ComponentRegistry);
        }
    }
}