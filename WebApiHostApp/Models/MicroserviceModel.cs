namespace CoreXF.WebApiHostApp.Models
{
    public class MicroserviceModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public MsStatus Status { get; set; }

        public string Version { get; set; }

        public string Url { get; set; }

        public string Authors { get; set; }

        public string Location { get; set; }

        public enum MsStatus
        {
            Ready, Starting, Running, Stopped
        }
    }
}