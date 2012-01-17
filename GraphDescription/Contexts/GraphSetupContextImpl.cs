using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.Services;

namespace Associativy.GraphDescription.Contexts
{
    [OrchardFeature("Associativy")]
    public class GraphSetupContextImpl : GraphSetupContext
    {
        private readonly IList<string> _contentTypes;
        public override IList<string> ContentTypes
        {
            get { return _contentTypes; }
        }

        public GraphSetupContextImpl()
        {
            _contentTypes = new List<string>();
        }
    }
}