namespace Monitor.WebApi.Entities
{
    using System;

    public class Query
    {
        public Guid IdUser { get; set; }
        public Guid IdProject { get; set; }
        public Role Role { get; set; }
    }
}


