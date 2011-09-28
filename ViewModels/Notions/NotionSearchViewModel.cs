using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Orchard.Environment.Extensions;

namespace Associativy.ViewModels.Notions
{
    [OrchardFeature("Associativy.Notions")]
    public class NotionSearchViewModel
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