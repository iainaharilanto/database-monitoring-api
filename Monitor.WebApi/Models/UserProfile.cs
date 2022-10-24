namespace Monitor.WebApi.Models
{
    using Monitor.WebApi.Entities;
    using System.Collections.Generic;

    public class UserProfile
    {
        public User User { get; set; }
        public IList<ProjectRole> Projects { get; set; }
        public ProjectRole ActiveProject { get; set; }
    }
}
