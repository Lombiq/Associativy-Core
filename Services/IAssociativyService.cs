using Associativy.Models;

namespace Associativy.Services
{
    public interface IAssociativyService
    {
        /// <summary>
        /// The AssociativyGraphDescriptor the services use
        /// </summary>
        IAssociativyGraphDescriptor GraphDescriptor { get; set; }
    }
}
