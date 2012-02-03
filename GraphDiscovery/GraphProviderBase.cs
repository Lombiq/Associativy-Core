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
        public string GraphName { get; protected set; }
        public LocalizedString DisplayGraphName { get; protected set; }
        public IEnumerable<string> ContentTypes { get; protected set; }

        private readonly IResolve<TConnectionManager> _connectionManagerResolver;
        public virtual IConnectionManager ConnectionManager
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
    }
}