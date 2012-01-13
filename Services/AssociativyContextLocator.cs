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

            return context;
        }

        public IAssociativyContext[] GetContextsForContentType(string contentType)
        {
            // Might be worth storing contexts in a dictionary indexed by content types so it doesn't have to be recalculated.
            // But with a reasonable number of contexts it takes slightly less than nothing to run...
            var contexts = (from c in _registeredContexts
                            where c.ContentTypes.Contains(contentType)
                            select c).ToArray();

            return contexts;
        }

        public IDictionary<string, IList<IAssociativyContext>> GetContextsByRegisteredContentTypes()
        {
            var associativyContexts = new Dictionary<string, IList<IAssociativyContext>>();

            foreach (var context in _registeredContexts)
            {
                foreach (var contentType in context.ContentTypes)
                {
                    if (!associativyContexts.ContainsKey(contentType)) associativyContexts[contentType] = new List<IAssociativyContext>();
                    associativyContexts[contentType].Add(context);
                }
            }

            return associativyContexts;
        }
    }
}