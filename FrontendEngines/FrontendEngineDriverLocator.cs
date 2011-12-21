using System;
using System.Collections.Generic;
using System.Linq;
using Associativy.Models;
using Orchard.Environment.Extensions;

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