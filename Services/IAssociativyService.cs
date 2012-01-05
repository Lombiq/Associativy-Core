using System;
using Associativy.Models;

namespace Associativy.Services
{
    public interface IAssociativyService
    {
        /// <summary>
        /// The AssociativyContext the services use
        /// </summary>
        IAssociativyContext Context { get; set; }
    }
}
