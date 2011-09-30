﻿using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Associativy.Models;
using System.Collections.Generic;
using Orchard.Data;

namespace Associativy.Services
{
    public interface INodeManager<TNodePart, TNodePartRecord, TNodeParams> : IDependency // Maybe ISingletonDependency?
        where TNodePart : ContentPart<TNodePartRecord>, INode
        where TNodePartRecord : ContentPartRecord, INode
        where TNodeParams : INodeParams<TNodePart>, new()
    {
        TNodeParams NodeParamsFactory();
        IList<string> GetSimilarTerms(string snippet, int maxCount = 10);
        
        #region Node CRUD
        IContentQuery<TNodePart, TNodePartRecord> ContentQuery { get; }
        TNodePart Create(TNodeParams nodeParams);
        TNodePart Get(int id);
        TNodePart Update(TNodeParams nodeParams);
        TNodePart Update(TNodePart node);

        /// <summary>
        /// Soft deletes the node and leaves connections intact (that means the whole partial graph can be reconstructed)
        /// </summary>
        /// <param name="id">Id of the node</param>
        void Remove(int id);
        #endregion
    }
}
