using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    /// <summary>
    /// Abstract implementation of INode as a record for further use
    /// </summary>
    [OrchardFeature("Associativy")]
    public abstract class NodePartRecord : ContentPartRecord, INode
    {
        public virtual string Label { get; set; }
    }
}