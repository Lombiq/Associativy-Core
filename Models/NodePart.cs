using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Associativy.Models
{
    /// <summary>
    /// Abstract implementation of INode as a part for further use
    /// </summary>
    [OrchardFeature("Associativy")]
    public abstract class NodePart<TRecord> :  ContentPart<TRecord>, INode where TRecord : NodePartRecord
    {
        [Required]
        public string Label
        {
            get { return Record.Label; }
            set { Record.Label = value; }
        }

        // LazyField neighbours?
    }
}