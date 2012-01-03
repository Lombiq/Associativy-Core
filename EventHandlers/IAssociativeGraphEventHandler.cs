using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Events;
using Associativy.Models;

namespace Associativy.EventHandlers
{
    /// <summary>
    /// Interface for graph event handlers.
    /// 
    /// When an event occurs, all types implementing this interface will get their respective method called.
    /// </summary>
    public interface IAssociativeGraphEventHandler : IEventHandler
    {
        /// <summary>
        /// Gets called when the associative graph has changed.
        /// </summary>
        void Changed(IAssociativyContext context);
    }
}
