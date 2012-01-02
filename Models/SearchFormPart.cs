using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Associativy.Models
{
    [OrchardFeature("Associativy")]
    public class SearchFormPart : ContentPart
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