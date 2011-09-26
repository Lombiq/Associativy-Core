using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;

namespace Associativy.Models
{
    interface INode : IContent
    {
        //public int Id { get; }
        string Label { get; }
    }
}
