using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services
{
    [OrchardFeature("Associativy")]
    public class AssociativyContextLocator : IAssociativyContextLocator
    {
        private readonly IEnumerable<IAssociativyContext> _registeredContexts;

        public AssociativyContextLocator(IEnumerable<IAssociativyContext> registeredContexts)
        {
            _registeredContexts = registeredContexts;
        }

        public IAssociativyContext GetContext(string technicalGraphName)
        {
            var context = (from c in _registeredContexts
                           where c.TechnicalGraphName == technicalGraphName
                           select c).FirstOrDefault();

            if (context == null) throw new ApplicationException("Associativy context \"" + technicalGraphName + "\" not found");

            return context;
        }
    }
}