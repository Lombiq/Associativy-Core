using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Associativy.Models;

namespace Associativy.FrontendEngines
{
    [OrchardFeature("Associativy")]
    public class FrontendEngineDriverLocator<TNode> : IFrontendEngineDriverLocator<TNode>
        where TNode : INode
    {
        private readonly IEnumerable<IFrontendEngineDriver<TNode>> _frontendEngineDrivers;

        public FrontendEngineDriverLocator(IEnumerable<IFrontendEngineDriver<TNode>> frontendEngineDrivers)
        {
            _frontendEngineDrivers = frontendEngineDrivers;
        }

        public IFrontendEngineDriver<TNode> GetDriver(string frontendEngineName)
        {
            var driver = (from d in _frontendEngineDrivers
                          where d.GetType().Name.Contains(frontendEngineName + "Driver")
                          select d).FirstOrDefault();

            if (driver == null) throw new ApplicationException("Associativy front end driver \"" + frontendEngineName + "\" not found");

            return driver;
        }
    }
}