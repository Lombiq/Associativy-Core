using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Drivers;
using Associativy.ViewModels;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Associativy.Models;

namespace Associativy.Drivers
{
    [OrchardFeature("Associativy")]
    public class SearchFormPartDriver : ContentPartDriver<SearchFormPart>
    {
        protected override DriverResult Display(SearchFormPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_SearchForm",
                                () => shapeHelper.Parts_SearchForm(
                                                                ViewModel: new SearchViewModel()));
        }
    }
}