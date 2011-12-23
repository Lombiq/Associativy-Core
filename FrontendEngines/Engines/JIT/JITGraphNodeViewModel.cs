using System.Collections.Generic;
using Associativy.FrontendEngines.ViewModels;
using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.Engines.JIT.ViewModels
{
    /// <summary>
    /// View model for JIT graphs. Naming follows JIT namings!
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    [OrchardFeature("Associativy")]
    public class JITGraphNodeViewModel<TNode> : GraphNodeViewModel<TNode>
        where TNode : INode
    {
        public int id
        {
            get { return Id; }
            set { Id = value; }
        }

        public string name
        {
            get { return Label; }
            set { Label = value; }
        }

        public IList<INode> adjacencies
        {
            get { return Neighbours; }
            set { Neighbours = value; }
        }

        public IDictionary<string, string> data { get; set; }
    }
}