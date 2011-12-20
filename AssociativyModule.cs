using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac.Core;
using Autofac;
using Associativy.Services;

namespace Associativy
{
    public class AssociativyModule : IModule
    {
        public void Configure(IComponentRegistry componentRegistry)
        {
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(AssociativyServices<,,>)).As(typeof(IAssociativyServices<,,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ConnectionManager<>)).As(typeof(IConnectionManager<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Mind<,,>)).As(typeof(IMind<,,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(NodeManager<,>)).As(typeof(INodeManager<,>)).InstancePerLifetimeScope();

            builder.Update(componentRegistry);
        }
    }
}