﻿using System.Collections.Generic;
using Associativy.Models;
using Associativy.Models.Mind;
using Orchard;
using Associativy.GraphDiscovery;

namespace Associativy.Services
{
    /// <summary>
    /// Deals with node-to-node path calculations
    /// </summary>
    public interface IPathFinder : IDependency
    {
        /// <summary>
        /// Calculates all paths between two nodes, depending on the settings.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="targetNode"></param>
        /// <param name="maxDistance"></param>
        /// <param name="useCache"></param>
        /// <returns>A list of succeeded paths, where a path is a list of the ids of the nodes on the path.</returns>
        IEnumerable<IEnumerable<int>> FindPaths(IGraphContext graphContext, int startNodeId, int targetNodeId, int maxDistance = 3, bool useCache = false);
    }
}
