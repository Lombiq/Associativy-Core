using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;

// Namespace is not .Notions because Orchard wouldn't pick up records: http://stackoverflow.com/questions/4995785/orchard-project-module-getting-error-no-persister-for-somepartrecord
namespace Associativy.Models
{
    [OrchardFeature("Associativy.Notions")]
    public class NotionPart : NodePart<NotionPartRecord>
    {
    }
}