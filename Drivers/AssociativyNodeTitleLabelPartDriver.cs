using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Associativy.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace Associativy.Drivers
{
    public class AssociativyNodeTitleLabelPartDriver : ContentPartDriver<AssociativyNodeTitleLabelPart>
    {
        protected override DriverResult Display(AssociativyNodeTitleLabelPart part, string displayType, dynamic shapeHelper)
        {
            return Combined(
                ContentShape("Parts_Title",
                    () => shapeHelper.Parts_Title(Title: part.Title)),
                ContentShape("Parts_Title_Summary",
                    () => shapeHelper.Parts_Title_Summary(Title: part.Title)),
                ContentShape("Parts_Title_SummaryAdmin",
                    () => shapeHelper.Parts_Title_SummaryAdmin(Title: part.Title))
                );
        }
    }
}