using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.Models;
using Orchard.Environment.Extensions;

namespace Associativy.Services.Notions
{
    [OrchardFeature("Associativy.Notions")]
    public class AssociativyNotionsServices : AssociativyServices<NotionPart, NotionPartRecord, NotionParams, NotionToNotionConnectorRecord>, IAssociativyNotionsServices
    {
        public AssociativyNotionsServices(
            IConnectionManager<NotionPart, NotionPartRecord, NotionParams, NotionToNotionConnectorRecord> connectionManager,
            IMind<NotionPart, NotionPartRecord, NotionParams, NotionToNotionConnectorRecord> mind,
            INodeManager<NotionPart, NotionPartRecord, NotionParams> nodeManager)
            : base(connectionManager, mind, nodeManager)
        {
        }
    }
}