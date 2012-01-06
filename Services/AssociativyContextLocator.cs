using System;
using System.Collections.Generic;
using System.Linq;
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

        public IAssociativyContext[] GetContextsForContentType(string contentType)
        {
            // Might be worth storing contexts in a dictionary indexed by content types so it doesn't have to be recalculated.
            // But with a reasonable number of contexts it takes slightly less than nothing to run...
            var contexts = (from c in _registeredContexts
                            where c.ContentTypes.Contains(contentType)
                            select c).ToArray();

            if (contexts.Length == 0) throw new ApplicationException("There are no Associativy contexts for the  \"" + contentType + "\" content type.");

            return contexts;
        }
    }
}