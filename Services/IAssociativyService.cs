using Associativy.Models;

namespace Associativy.Services
{
    public interface IAssociativyService
    {
        /// <summary>
        /// The AssociativyGraphDescriptor the services use
        /// </summary>
        IGraphDescriptor GraphDescriptor { get; set; }
    }
}
