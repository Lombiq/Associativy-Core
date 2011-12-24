using System.Collections.Generic;
using Associativy.FrontendEngines.ViewModels;
using Associativy.Models;
using Orchard.Environment.Extensions;
using System.Runtime.Serialization;
using System.Linq;

namespace Associativy.FrontendEngines.Engines.JIT.ViewModels
{
    /// <summary>
    /// View model for JIT graphs. Naming follows JIT namings!
    /// </summary>
    /// <typeparam name="TNode">Node type</typeparam>
    [OrchardFeature("Associativy")]
    [DataContract(Name = "JITGraphNodeViewModel")]
    public class JITGraphNodeViewModel<TNode> : IJITGraphNodeViewModel<TNode>
        where TNode : INode
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public IList<INode> Neighbours { get; set; }
        public TNode Node { get; set; }

        [DataMember]
        public string id
        {
            get { return Id.ToString(); }
            set { } // Unused, empty set just for serialization (since the object is never deserialized, this doesn't matter)
        }

        [DataMember]
        public string name
        {
            get { return Label; }
            set { } // Unused, empty set just for serialization (since the object is never deserialized, this doesn't matter)
        }

        [DataMember]
        public string[] adjacencies
        {
            get
            {
                return (from neighbour in Neighbours
                        select neighbour.Id.ToString()).ToArray();
            }
            set { } // Unused, empty set just for serialization (since the object is never deserialized, this doesn't matter)
        }

        public IDictionary<string, string> data { get; set; }

        public JITGraphNodeViewModel()
        {
            Neighbours = new List<INode>();
        }

        public virtual void MapFromNode(TNode node)
        {
            Id = node.Id;
            Label = node.Label;
        }
    }
}