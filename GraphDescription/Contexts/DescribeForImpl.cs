using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Associativy.Services;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public class DescribeForImpl : DescribeFor
    {
        private readonly string _graphName;
        public override string GraphName
        {
            get { throw new NotImplementedException(); }
        }

        private LocalizedString _displayName;
        public override LocalizedString DisplayName
        {
            get { return _displayName; }
        }

        private string[] _contentTypes;
        public override string[] ContentTypes
        {
            get { return _contentTypes; }
        }

        private IConnectionManager _connectionManager;
        public override IConnectionManager ConnectionManager
        {
            get { return _connectionManager; }
        }

        public DescribeForImpl(string graphName)
        {
            _graphName = graphName;
        }

        public override void Set(LocalizedString displayName, string[] contentTypes, IConnectionManager connectionManager)
        {
            _displayName = displayName;
            _contentTypes = contentTypes;
            _connectionManager = connectionManager;
        }
    }
}