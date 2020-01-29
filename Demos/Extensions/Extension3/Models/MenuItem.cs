namespace Extension3.Models
{
    public class MenuItem
    {
        public string Routing { set; private get; }

        public string DisplayText { get; set; }

        public string Area
        {
            get
            {
                var parts = this.Routing.Split('/');
                return parts.Length == 3 ? parts[0] : null;
            }
        }

        public string Controller
        {
            get
            {
                var parts = this.Routing.Split('/');
                return parts.Length == 3 ? parts[1] : parts[0];
            }
        }

        public string Action
        {
            get
            {
                var parts = this.Routing.Split('/');
                return parts.Length == 3 ? parts[2] : parts[1];
            }
        }
    }
}