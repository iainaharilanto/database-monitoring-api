namespace Monitor.WebApi.Models
{
    using Monitor.WebApi.Entities;
    using System;
    using System.Collections.Generic;

    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }

        public List<KeyValuePair<string, string>> Projects { get; set; } = new List<KeyValuePair<string, string>>();

        public AuthenticateResponse(User user, string token, IList<Project> projects)
        {
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Username = user.Username;
            this.Token = token;
            foreach (var project in projects)
            {
                this.Projects.Add(new KeyValuePair<string, string>(project.Id.ToString(), project.Name));
            }
        }
    }
}
