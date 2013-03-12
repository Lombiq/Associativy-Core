
namespace Associativy.Models.Services
{
    public class MindSettings : IMindSettings
    {
        public string Algorithm { get; set; }
        public bool UseCache { get; set; }
        public int MaxDistance { get; set; }

        private static MindSettings _default = new MindSettings();
        public static MindSettings Default { get { return _default; } }


        public MindSettings()
        {
            Algorithm = "sophisticated";
            UseCache = false;
            MaxDistance = 3;
        }
    }
}