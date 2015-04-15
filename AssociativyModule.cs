using Associativy.Queryable;
using Associativy.Services;
using Autofac;
using Autofac.Core;

namespace Associativy
{
    public class AssociativyModule : IModule
    {
        public void Configure(IComponentRegistry componentRegistry)
        {
            // Ideally all these registrations should not exist, but currently generics are not properly auto-registered
            // by Orchard, see: https://github.com/OrchardCMS/Orchard/issues/1968
            var builder = new ContainerBuilder();

            builder.RegisterGeneric(typeof(SqlConnectionManager<>)).As(typeof(ISqlConnectionManager<>));
            builder.RegisterGeneric(typeof(GraphServicesFactory<,,,>)).As(typeof(IGraphServicesFactory<,,,>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(QueryableGraph<>)).As(typeof(IQueryableGraph<>));

            builder.Update(componentRegistry);
        }
    }
}