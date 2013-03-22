﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Indexing;

namespace Associativy.Services
{
    public interface INodeIndexingService : IDependency
    {
        bool IsIndexingSetupForGraph(string graphName);
        void SetupIndexingForGraph(string graphName);
        ISearchBuilder GetSearchBuilder(string graphName);
    }

    public static class NodeIndexingServiceExtensions
	{
        public static IEnumerable<ISearchHit> Search(this INodeIndexingService indexingService, string graphName, string labelQuery)
        {
            return indexingService.Search(graphName, searchBuilder => searchBuilder.Parse("nodeLabel", labelQuery + "*", false));
        }

        public static IEnumerable<ISearchHit> SearchExact(this INodeIndexingService indexingService, string graphName, string label)
        {
            return indexingService.Search(graphName, searchBuilder => searchBuilder.Parse("nodeLabel", label, true).ExactMatch());
        }


        private static IEnumerable<ISearchHit> Search(this INodeIndexingService indexingService, string graphName, Func<ISearchBuilder, ISearchBuilder> setup)
        {
            var searchBuilder = indexingService.GetSearchBuilder(graphName);

            if (searchBuilder == null) throw new InvalidOperationException("No search builder is present for the graph " + graphName + ". This most possibly means that indexing is not set up for the graph. Thus nodes can't be fetched by label.");

            return setup(searchBuilder).Search().OrderByDescending(hit => hit.Score);
        }
	}
}
