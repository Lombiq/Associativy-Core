using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement;
using System.ComponentModel.DataAnnotations;

namespace Associativy
{
    public class NodePartRecord : ContentPartRecord
    {
        public virtual string Label { get; set; }
    }

    //public abstract class NodePart<T> : ContentPart<T> where T : NodePartRecord
    public class NodePart : ContentPart<NodePartRecord>
    {
        private readonly IContentManager _contentManager;

        public NodePart(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        [Required]
        public string Label
        {
            get { return Record.Label; }
            set { Record.Label = value; }
        }


        private IList<NodePart> _neighbours;
        public IList<NodePart> Neighbours
        {
            get
            {
                if (_neighbours == null)
                {
                    //_contentManager.Query<NodePart, NodePartRecord>().Where(node => node.
                }
                return _neighbours;
            }
            private set { _neighbours = value; }
        }
    }
}