using Associativy.GraphDiscovery;
using Orchard.ContentManagement.Handlers;
using System.Linq;
using System.Xml.Linq;

namespace Associativy.Handlers
{
    public class GraphMetaDataContentHandler : ContentHandler
    {
        public const string ElementName = "GraphMetaData";

        private readonly IGraphManager _graphManager;

        public GraphMetaDataContentHandler(IGraphManager graphManager)
        {
            _graphManager = graphManager;
        }

        protected override void Exporting(ExportContentContext context)
        {
            var neighbourIds = _graphManager
                .FindGraph(new GraphContext())
                .Services
                .ConnectionManager
                .GetNeighbourIds(context.ContentItemRecord.Id, skip: 0, count: int.MaxValue)
                .ToList();

            if (!neighbourIds.Any()) return;

            var element = context.Element(ElementName);
            element.SetAttributeValue("RecordId", context.ContentItemRecord.Id);
            foreach (var id in neighbourIds) element.Add(new XElement("ConnectionId", id));
        }
    }
}