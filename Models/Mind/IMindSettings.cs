using Orchard;
using Orchard.ContentManagement;
using System;

namespace Associativy.Models.Mind
{
    public enum MindAlgorithms
    {
        Simple,
        Sophisticated
    }

    public interface IMindSettings
    {
        MindAlgorithms Algorithm { get; set; }
        bool UseCache { get; set; }
        int ZoomLevel { get; set; }

        /// <summary>
        /// Upper bound of the graph zoom levels (lower bound is always zero).
        /// </summary>
        int MaxZoomLevel { get; set; }

        int MaxDistance { get; set; }

        /// <summary>
        /// Use this to customize the query which is run against content items, e. g. to specify the version to use or to eager-load records to enhance performance
        /// </summary>
        Func<IContentQuery<ContentItem>, IContentQuery<ContentItem>> QueryModifier { get; set; }
    }
}
