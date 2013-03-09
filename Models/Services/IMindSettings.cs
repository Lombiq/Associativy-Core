﻿using Orchard.ContentManagement;

namespace Associativy.Models.Services
{
    public interface IMindSettings
    {
        /// <summary>
        /// The algorithm to use to fing associations
        /// </summary>
        string Algorithm { get; set; }

        /// <summary>
        /// Use cache or regenerate the result
        /// </summary>
        bool UseCache { get; set; }

        /// <summary>
        /// Maximal distance between two nodes that can be considered as related
        /// </summary>
        int MaxDistance { get; set; }
    }

    public static class MindSettingsExtensions
    {
        public static IMindSettings MakeShallowCopy(this IMindSettings settings)
        {
            return new MindSettings
            {
                Algorithm = settings.Algorithm,
                UseCache = settings.UseCache,
                MaxDistance = settings.MaxDistance,
            };
        }
    }
}
