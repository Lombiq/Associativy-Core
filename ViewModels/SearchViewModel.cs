using Orchard.Environment.Extensions;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace Associativy.ViewModels
{
    [OrchardFeature("Associativy")]
    public class SearchViewModel
    {
        [Required]
        public string Terms { get; set; }

        public string[] TermsArray
        {
            get
            {
                string[] termsArray = new string[0];
                if (Terms != null)
                {
                    termsArray = Terms.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    termsArray = (from p in termsArray select p.Trim()).ToArray();
                }
                return termsArray;
            }
        }
    }
}