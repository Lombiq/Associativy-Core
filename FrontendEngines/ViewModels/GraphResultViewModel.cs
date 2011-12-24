using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Associativy.FrontendEngines.ViewModels
{
    public class GraphResultViewModel : IGraphResultViewModel
    {
        public dynamic SearchForm { get; set; }
        public dynamic Graph { get; set; }
    }
}