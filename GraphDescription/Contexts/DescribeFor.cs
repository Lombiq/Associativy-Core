using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.Services;
using Orchard.Localization;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public abstract class DescribeFor
    {
        public abstract string GraphName { get; }
        public abstract LocalizedString DisplayName { get; }
        public abstract string[] ContentTypes { get; }
        public abstract IConnectionManager ConnectionManager { get; }

        public abstract void Set(LocalizedString DisplayName, string[] contentTypes, IConnectionManager connectionManager);
    }
}