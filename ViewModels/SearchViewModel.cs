﻿using Orchard.Environment.Extensions;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Web.Routing;

namespace Associativy.ViewModels
{
    [OrchardFeature("Associativy")]
    public class SearchViewModel : ISearchViewModel
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
                    termsArray = (from p in termsArray where p.Trim() != "" select p.Trim()).ToArray();
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

        public RouteValueDictionary PostRouteValueDictionary { get; set; }
        public string FetchUrl { get; set; }
    }
}