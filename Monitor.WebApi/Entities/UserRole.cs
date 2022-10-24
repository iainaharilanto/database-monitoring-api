namespace Monitor.WebApi.Entities
{
    using System;

    public class UserRole
    {
        public Guid IdUser { get; set; }
        public Guid IdProject { get; set; }
        public Role Role { get; set; }
    }

    public enum Role
    {
        Admin,
        Contributor
    }
}
