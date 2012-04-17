using Orchard;
using Orchard.ContentManagement;
using System;

namespace Associativy.Models.Mind
{
    public enum MindAlgorithm
    {
        Simple,
        Sophisticated
    }

    public delegate void QueryModifer(IContentQuery<ContentItem> query);

    public interface IMindSettings
    {
        MindAlgorithm Algorithm { get; set; }
        bool UseCache { get; set; }
        int ZoomLevel { get; set; }

        /// <summary>
        /// Maximal number of zoom levels.
        /// </summary>
        int ZoomLevelCount { get; set; }

        int MaxDistance { get; set; }

        /// <summary>
        /// Use this to customize the query which is run against content items, e. g. to specify the version to use or to eager-load records to enhance performance
        /// </summary>
        QueryModifer ModifyQuery { get; set; }
    }
}
