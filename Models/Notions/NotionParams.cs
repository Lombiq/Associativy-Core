using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.Models
{
    public class NotionParams : NodeParams<NotionPart>
    {
        public override string ContentTypeName
        {
            get { return "Notion"; }
        }
    }
}