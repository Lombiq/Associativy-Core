﻿using Associativy.FrontendEngines;
using Associativy.FrontendEngines.Engines.Dracula;
using Associativy.FrontendEngines.ViewModels;
using Associativy.Services;
using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions;
using Associativy.FrontendEngines.Engines.JIT;
using Associativy.FrontendEngines.Engines.JIT.ViewModels;

namespace Associativy
{
    [OrchardFeature("Associativy")]
    public class AssociativyModule : IModule
    {
        public void Configure(IComponentRegistry componentRegistry)
        {
            var builder = new ContainerBuilder();

            builder.RegisterGeneric(typeof(AssociativyServices<,,>)).As(typeof(IAssociativyServices<,,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ConnectionManager<>)).As(typeof(IConnectionManager<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Mind<,,>)).As(typeof(IMind<,,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(NodeManager<,>)).As(typeof(INodeManager<,>)).InstancePerLifetimeScope();

            // Frontend engines common
            builder.RegisterGeneric(typeof(FrontendEngineDriverLocator<>)).As(typeof(IFrontendEngineDriverLocator<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GraphNodeViewModel<>)).As(typeof(IGraphNodeViewModel<>));

            // Frontend engine drivers
            builder.RegisterGeneric(typeof(DraculaDriver<>)).As(typeof(IFrontendEngineDriver<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(JITDriver<>)).As(typeof(IFrontendEngineDriver<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(JITGraphNodeViewModel<>)).As(typeof(IJITGraphNodeViewModel<>));

            builder.Update(componentRegistry);
        }
    }
}