
namespace Associativy.Models.Services
{
    public interface IMindSettings
    {
        /// <summary>
        /// The algorithm to use to find associations
        /// </summary>
        string Algorithm { get; set; }

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
                MaxDistance = settings.MaxDistance,
            };
        }
    }
}
