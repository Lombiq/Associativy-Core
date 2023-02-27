using Associativy.GraphDiscovery;
using Orchard.ContentManagement.Handlers;
using System.Linq;
using System.Xml.Linq;

namespace Associativy.Handlers
{
    public class GraphMetadataContentHandler : ContentHandler
    {
        public const string ElementName = "GraphMetadata";

        private readonly IGraphManager _graphManager;

        public GraphMetadataContentHandler(IGraphManager graphManager)
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
            element.SetAttributeValue("NodeId", context.ContentItemRecord.Id);
            foreach (var id in neighbourIds) element.Add(new XElement("ConnectionId", id));
        }
    }
}