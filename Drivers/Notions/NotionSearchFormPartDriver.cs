using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Drivers;
using Associativy.Models;
using Associativy.ViewModels.Notions;

namespace Associativy.Drivers.Notions
{
    public class NotionSearchFormPartDriver : ContentPartDriver<NotionSearchFormPart>
    {

        protected override DriverResult Display(NotionSearchFormPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_Notions_SearchForm",
                                () => shapeHelper.Parts_Notions_SearchForm(
                                                                ViewModel: new NotionSearchViewModel()));
        }
    }
}