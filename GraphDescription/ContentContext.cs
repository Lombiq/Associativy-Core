using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.GraphDescription
{
    [OrchardFeature("Associativy")]
    public class ContentContext : IContentContext
    {
        private readonly string _contentType;
        public string ContentType { get; }

        public ContentContext(string contentType)
        {
            _contentType = contentType;
        }
    }
}