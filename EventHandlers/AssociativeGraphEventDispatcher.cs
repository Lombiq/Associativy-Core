using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

namespace Associativy.EventHandlers
{
    [OrchardFeature("Associativy")]
    public class AssociativeGraphEventDispatcher : IAssociativeGraphEventDispatcher
    {
        public event EventHandler ChangedEvent;

        public void Changed()
        {
            if (ChangedEvent != null)
            {
                ChangedEvent(this, null);
            }
        }
    }
}