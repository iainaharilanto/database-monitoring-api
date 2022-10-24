namespace Monitor.WebApi.Models
{
    using Monitor.WebApi.Entities;
    public class ProjectRole
    {
        public Project Project { get; set; }
        public Role Role { get; set; }
    }
}
