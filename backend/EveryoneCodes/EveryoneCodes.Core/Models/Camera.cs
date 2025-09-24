namespace EveryoneCodes.Core.Models
{
    public class Camera
    {
        public int Number { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Latitude { get; set; } = "";
        public string Longitude { get; set; } = "";
    }

    public class CameraCsvRow
    {
        public string Camera { get; set; } = "";
        public string Latitude { get; set; } = "";
        public string Longitude { get; set; } = "";
    }
}
