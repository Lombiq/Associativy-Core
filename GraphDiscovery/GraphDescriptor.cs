using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using System.Data;
using Orchard.Localization;
using Associativy.Services;
using Piedone.HelpfulLibraries.Utilities;

namespace Associativy.GraphDiscovery
{
    [OrchardFeature("Associativy")]
    public abstract class GraphDescriptor : FreezableBase
    {
        private string _graphName;

        /// <summary>
        /// Name of the graph
        /// </summary>
        public string GraphName
        {
            get { return _graphName; }
            set
            {
                ThrowIfFrozen();
                _graphName = value;
            }
        }


        private LocalizedString _displayGraphName;

        /// <summary>
        /// Human-readable name of the graph
        /// </summary>
        public LocalizedString DisplayGraphName
        {
            get { return _displayGraphName; }
            set
            {
                ThrowIfFrozen();
                _displayGraphName = value;
            }
        }


        private IEnumerable<string> _contentTypes;

        /// <summary>
        /// The types of the content items stored by the graph
        /// </summary>
        public IEnumerable<string> ContentTypes
        {
            get { return _contentTypes; }
            set
            {
                ThrowIfFrozen();
                _contentTypes = value;
            }
        }


        private IConnectionManager _connectionManager;

        /// <summary>
        /// The IConnectionManager instance used to discover connections in the graph
        /// </summary>
        // This should maybe be done lazily
        public IConnectionManager ConnectionManager
        {
            get { return _connectionManager; }
            set
            {
                ThrowIfFrozen();
                _connectionManager = value;
            }
        }
    }

    [OrchardFeature("Associativy")]
    public static class GraphDescriptorExtensions
    {
        /// <summary>
        /// Creates the maximal context the descriptor supports
        /// </summary>
        public static IGraphContext ProduceMaximalContext(this GraphDescriptor descriptor)
        {
            return new GraphContext { GraphName = descriptor.GraphName, ContentTypes = descriptor.ContentTypes };
        }
    }
}