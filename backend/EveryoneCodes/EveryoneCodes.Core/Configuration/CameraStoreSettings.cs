namespace EveryoneCodes.Core.Configuration
{
    public class CameraStoreSettings
    {
        public const string SectionName = "CameraStore";

        public string ResourcePath { get; set; } = "Data.cameras-defb.csv";
        public bool EnableCaching { get; set; } = true;
        public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(30);
    }
}
