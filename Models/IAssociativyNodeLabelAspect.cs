using Orchard.ContentManagement;

namespace Associativy.Models
{
    public interface IAssociativyNodeLabelAspect : IContent
    {
        string Label { get; set; }
    }
}
