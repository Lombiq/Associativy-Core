using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

// Namespace is not .Notions because Orchard wouldn't pick up records: http://orchard.codeplex.com/workitem/18142
namespace Associativy.Models
{
    [OrchardFeature("Associativy.Notions")]
    public class NotionPart : NodePart<NotionPartRecord>
    {
    }
}