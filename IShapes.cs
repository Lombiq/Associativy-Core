using System;
using Orchard;
using Orchard.ContentManagement;

namespace Associativy
{
    public interface IShapes : IDependency
    {
        dynamic SearchBoxShape(IContent searchForm);
        dynamic SearchResultShape(dynamic searchBoxShape, dynamic graphShape);
        dynamic AssociationsNotFoundShape(IContent searchForm);
    }
}
