namespace Monitor.WebApi.Services
{
    using Microsoft.Extensions.Options;
    using Monitor.WebApi.Entities;
    using Monitor.WebApi.Helpers;
    using Monitor.WebApi.Models;
    using MySqlConnector;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProjectService
    {
        IEnumerable<Project> GetAll();
        Project GetById(Guid id);
        IList<ProjectRole> GetByUserId(Guid id);
        void RemoveById(Guid id);
        void Add(Project u);
    }

    public class ProjectService : IProjectService
    {
        private IEnumerable<Project> _projects
        {
            get
            {
                return this.GetAll().ToList();
            }
        }

        private readonly AppSettings _appSettings;

        public ProjectService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }


        public IEnumerable<Project> GetAll()
        {
            var result = new List<Project>();
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM projects;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var project = new Project
                {
                    Id = reader.GetGuid("Id"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    Name = reader.GetString("Name"),
                    Database = reader.GetString("Database"),
                    ServerUrl = reader.GetString("ServerUrl"),
                    Port = reader.GetInt32("Port"),
                };
                result.Add(project);
            }
            connection.Close();
            return result;
        }

        public Project GetById(Guid id)
        {
            return _projects.FirstOrDefault(x => x.Id == id);
        }

        public IList<ProjectRole> GetByUserId(Guid id)
        {
            List<ProjectRole> result = new List<ProjectRole>();
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM projects p inner join userrole u on p.Id=u.IdProject Where IdUser=@IdUser";
            command.Parameters.AddWithValue("@IdUser", id.ToString("D"));

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var projectRole = new ProjectRole
                {
                    Project = new Project
                    {
                        Id = reader.GetGuid("Id"),
                        Username = reader.GetString("Username"),
                        Password = reader.GetString("Password"),
                        Name = reader.GetString("Name"),
                        Database = reader.GetString("Database"),
                        ServerUrl = reader.GetString("ServerUrl"),
                        Port = reader.GetInt32("Port"),
                    },
                    Role = (Role)Enum.Parse(typeof(Role), reader.GetString("Role"), true)
                };
                result.Add(projectRole);
            }
            connection.Close();

            return result;
        }

        public void RemoveById(Guid id)
        {
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"delete FROM projects where Id =@userId;";
            command.Parameters.AddWithValue("@userId", id.ToString("D"));
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void Add(Project p)
        {

            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"insert into projects (`Id`,`Name`,`Database`,`Username`,`Password`,`ServerUrl`,`Port`) VALUES (@Id,@Name,@Database,@Username,@Password,@ServerUrl,@Port);";
            command.Parameters.AddWithValue("@Id", p.Id.ToString("D"));
            command.Parameters.AddWithValue("@Name", p.Name);
            command.Parameters.AddWithValue("@Database", p.Database);
            command.Parameters.AddWithValue("@Username", p.Username);
            command.Parameters.AddWithValue("@Password", p.Password);
            command.Parameters.AddWithValue("@ServerUrl", p.ServerUrl);
            command.Parameters.AddWithValue("@Port", p.Port);
            command.ExecuteNonQuery();

            connection.Close();
        }

    }
}
