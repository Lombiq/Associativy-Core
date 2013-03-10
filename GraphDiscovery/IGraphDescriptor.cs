﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Associativy.Services;
using Orchard.Localization;

namespace Associativy.GraphDiscovery
{
    public interface IGraphDescriptor
    {
        string Name { get; }
        LocalizedString DisplayName { get; }
        IEnumerable<string> ContentTypes { get; }
        IGraphServices Services { get; }
    }


    public static class GraphDescriptorExtensions
    {
        public static IGraphContext MaximalContext(this IGraphDescriptor descriptor)
        {
            return new GraphContext { Name = descriptor.Name, ContentTypes = descriptor.ContentTypes };
        }
    }
}
