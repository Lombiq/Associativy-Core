using Orchard;

namespace Associativy.GraphDiscovery
{
    public interface IGraphProvider : IDependency
    {
        void Describe(DescribeContext describeContext);
    }
}
