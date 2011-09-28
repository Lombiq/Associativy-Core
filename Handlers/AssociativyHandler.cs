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
            builder.RegisterGeneric(typeof(AssociativyService<,,>)).As(typeof(IAssociativyService<,,>));
            builder.Update(_componentContext.ComponentRegistry);
        }
    }
}