using Associativy.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.HelpfulLibraries.DependencyInjection;
using Associativy.GraphDiscovery;
using System.Collections.Generic;
using Associativy.Models;
using System.Diagnostics;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public abstract class GraphProviderBase<TConnectionManager> : IGraphProvider
        where TConnectionManager : IConnectionManager
    {
        private readonly IResolve<TConnectionManager> _connectionManagerResolver;
        protected IConnectionManager ConnectionManager
        {
            get
            {
                return _connectionManagerResolver.Value;
            }
        }

        public Localizer T { get; set; }

        protected GraphProviderBase(IResolve<TConnectionManager> connectionManagerResolver)
        {
            _connectionManagerResolver = connectionManagerResolver;

            T = NullLocalizer.Instance;
        }

        public abstract void Describe(DescribeContext describeContext);
    }
}