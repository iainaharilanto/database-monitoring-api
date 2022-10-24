namespace Monitor.WebApi.Entities
{
    using System;

    public class Project
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Database { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ServerUrl { get; set; }

        public int Port { get; set; }
    }
}
