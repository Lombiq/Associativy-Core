using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.Services;
using Orchard.ContentManagement;
using Associativy.GraphDiscovery;

namespace Associativy.Tests.Stubs
{
    public class StubNodeManager : INodeManager
    {
        public IContentQuery<ContentItem> GetContentQuery(IGraphContext graphContext)
        {
            throw new NotImplementedException();
        }

        public IContentQuery<ContentItem> GetManyContentQuery(IGraphContext graphContext, IEnumerable<int> ids)
        {
            return new StubContentQuery();
        }

        public IEnumerable<IContent> GetSimilarNodes(IGraphContext graphContext, string labelSnippet, int maxCount = 10, QueryHints queryHints = null)
        {
            throw new NotImplementedException();
        }

        public IContent Get(IGraphContext graphContext, string label, QueryHints queryHints = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContent> GetMany(IGraphContext graphContext, IEnumerable<string> labels, QueryHints queryHints = null)
        {
            throw new NotImplementedException();
        }

        private class StubContentQuery : IContentQuery<ContentItem>
        {
            public IContentQuery<ContentItem> ForType(params string[] contentTypes)
            {
                throw new NotImplementedException();
            }

            public IContentQuery<ContentItem> ForVersion(VersionOptions options)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ContentItem> List()
            {
                return Enumerable.Empty<ContentItem>();
            }

            public IEnumerable<ContentItem> Slice(int skip, int count)
            {
                throw new NotImplementedException();
            }

            public int Count()
            {
                throw new NotImplementedException();
            }

            public IContentQuery<ContentItem, TRecord> Join<TRecord>() where TRecord : Orchard.ContentManagement.Records.ContentPartRecord
            {
                throw new NotImplementedException();
            }

            public IContentQuery<ContentItem, TRecord> Where<TRecord>(System.Linq.Expressions.Expression<Func<TRecord, bool>> predicate) where TRecord : Orchard.ContentManagement.Records.ContentPartRecord
            {
                throw new NotImplementedException();
            }

            public IContentQuery<ContentItem, TRecord> OrderBy<TRecord, TKey>(System.Linq.Expressions.Expression<Func<TRecord, TKey>> keySelector) where TRecord : Orchard.ContentManagement.Records.ContentPartRecord
            {
                throw new NotImplementedException();
            }

            public IContentQuery<ContentItem, TRecord> OrderByDescending<TRecord, TKey>(System.Linq.Expressions.Expression<Func<TRecord, TKey>> keySelector) where TRecord : Orchard.ContentManagement.Records.ContentPartRecord
            {
                throw new NotImplementedException();
            }

            public IContentManager ContentManager
            {
                get { throw new NotImplementedException(); }
            }

            public IContentQuery<TPart> ForPart<TPart>() where TPart : IContent
            {
                throw new NotImplementedException();
            }
        }
    }
}
