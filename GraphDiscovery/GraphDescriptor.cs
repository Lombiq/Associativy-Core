using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using System.Data;
using Orchard.Localization;
using Associativy.Services;

namespace Associativy.GraphDiscovery
{
    /// <summary>
    /// This is a lightweight appraoch to the Freezable class from WPF
    /// </summary>
    public interface IFreezable
    {
        bool IsFrozen { get; }
        void Freeze();
    }

    public class Freezable : IFreezable
    {
        public bool IsFrozen { get; private set; }

        public void Freeze()
        {
            IsFrozen = true;
        }

        protected void ThrowIfFrozen()
        {
            if (IsFrozen) throw new ReadOnlyException("A frozen IFreezable object can't be modified");
        }
    }

    [OrchardFeature("Associativy")]
    public abstract class GraphDescriptor : Freezable
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
                _graphName = value;
                ThrowIfFrozen();
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
                _displayGraphName = value;
                ThrowIfFrozen();
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
                _contentTypes = value;
                ThrowIfFrozen();
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
                _connectionManager = value;
                ThrowIfFrozen();
            }
        }
    }
}