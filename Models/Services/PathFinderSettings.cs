
namespace Associativy.Models.Services
{
    public class PathFinderSettings : IPathFinderSettings
    {
        public int MaxDistance { get; set; }

        private static PathFinderSettings _default = new PathFinderSettings();
        public static PathFinderSettings Default { get { return _default; } }


        public PathFinderSettings()
        {
            MaxDistance = 3;
        }
    }
}