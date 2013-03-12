
namespace Associativy.Models.Services
{
    public interface IPathFinderSettings
    {
        int MaxDistance { get; set; }
        bool UseCache { get; set; }
    }
}
