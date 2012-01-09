using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement;

namespace Associativy.FrontendEngines.ViewModels
{
    enum NodeZone
	{
        Header,
        Url,
        Meta,
        Content,
        Footer
	}

    [OrchardFeature("Associativy")]
    public class NodeViewModel : INodeViewModel
    {
        protected Dictionary<string, string> _zones = new Dictionary<string, string>();

        public IContent ContentItem { get; set; }

        public virtual void AddToZone(string zone, string content)
        {
            if (!_zones.ContainsKey(zone)) _zones[zone] = "";
            _zones[zone] += content;
        }

        public virtual void SetZone(string zone, string content)
        {
            ClearZone(zone);
            AddToZone(zone, content);
        }

        public virtual void ClearZone(string zone)
        {
            if (!_zones.ContainsKey(zone)) throw new ApplicationException("There is no zone with name \"" + zone + "\" in this NodeViewModel.");
            _zones[zone] = "";
        }

        public virtual string GetZoneContent(string zone)
        {
            if (!_zones.ContainsKey(zone)) throw new ApplicationException("There is no zone with name \"" + zone + "\" in this NodeViewModel.");
            return _zones[zone];
        }
    }
}