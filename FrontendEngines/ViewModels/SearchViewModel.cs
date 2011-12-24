using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Routing;
using Orchard.Environment.Extensions;

namespace Associativy.FrontendEngines.ViewModels
{
    [OrchardFeature("Associativy")]
    public class SearchViewModel : ISearchViewModel
    {
        [Required]
        public string Terms
        {
            get
            {
                if (TermsArray == null) return "";
                return String.Join(", ", TermsArray);
            }

            set
            {
                if (value != null)
                {
                    TermsArray = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    TermsArray = (from p in TermsArray where p.Trim() != "" select p.Trim()).ToArray();
                }
            }
        }

        public string[] TermsArray { get; private set; }
    }
}