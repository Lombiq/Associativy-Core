using System;
using Associativy.Models;

namespace Associativy.EventHandlers
{
    /// <summary>
    /// Used to dispatch event raised through IAssociativeGraphEventHandler to .NET-style events.
    /// 
    /// This can be crucial to avoid circular dependencies: if class A depends on class B, and class B calls event handling
    /// methods through IAssociativeGraphEventHandler instances then there is no proper way for class A to hook into B's events.
    /// See Associativy.Services.Mind how IGraphEventDispatcher can be used to circumvent the problem.
    /// </summary>
    public interface IAssociativeGraphEventDispatcher : IAssociativeGraphEventHandler
    {
        event EventHandler ChangedEvent;
        void Changed(IAssociativyContext associativyContext);
    }
}
