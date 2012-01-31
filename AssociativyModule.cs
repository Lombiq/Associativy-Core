using Associativy.Services;
using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions;

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

            builder.RegisterGeneric(typeof(DatabaseConnectionManager<>)).As(typeof(IDatabaseConnectionManager<>)).InstancePerLifetimeScope();

            builder.Update(componentRegistry);
        }
    }
}