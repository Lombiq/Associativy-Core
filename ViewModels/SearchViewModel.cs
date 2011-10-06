using Orchard.Environment.Extensions;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace Associativy.ViewModels
{
    [OrchardFeature("Associativy")]
    public class SearchViewModel
    {
        private string terms;

        [Required]
        public string Terms
        {
            get
            {
                return terms;
            }

            set
            {
                if (value != null)
                {
                    termsArray = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    termsArray = (from p in termsArray select p.Trim()).ToArray();
                }
                terms = value;
            }
        }


        private string[] termsArray;
        public string[] TermsArray
        {
            get
            {
                return termsArray;
            }
        }
    }
}