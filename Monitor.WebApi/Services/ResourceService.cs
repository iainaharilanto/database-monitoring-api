namespace Monitor.WebApi.Services
{
    using Microsoft.Extensions.Options;
    using Monitor.WebApi.Entities;
    using Monitor.WebApi.Helpers;
    using MySqlConnector;
    using System;
    using System.Collections.Generic;

    public interface IResourceService
    {

        IEnumerable<Resource> GetAll(Guid projectId, ResourceType type);

        Resource GetLatest(Guid projectId, ResourceType type);
    }

    public class ResourceService : IResourceService
    {
        private readonly AppSettings _appSettings;

        private readonly IProjectService _projectService;

        public ResourceService(IOptions<AppSettings> appSettings, IProjectService projectService)
        {
            _appSettings = appSettings.Value;
            _projectService = projectService;
        }

        public IEnumerable<Resource> GetAll(Guid projectId, ResourceType type)
        {
            var result = new List<Resource>();
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM resourceusage where ProjectId=@projectId AND Type=@type;";
            command.Parameters.AddWithValue("@projectId", projectId.ToString("D"));
            command.Parameters.AddWithValue("@type", type);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var user = new Resource
                {
                    Id = reader.GetGuid("Id"),
                    ProjectId = reader.GetGuid("ProjectId"),
                    Date = reader.GetDateTime("Date"),
                    Type = (ResourceType)Enum.Parse(typeof(ResourceType), reader.GetString("Value"), true),
                    Value = reader.GetDouble("Value")
                };
                result.Add(user);
            }
            connection.Close();
            return result;
        }

        public Resource GetLatest(Guid projectId, ResourceType type)
        {
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM resourceusage where ProjectId=@projectId AND Type=@type order by Date desc;";
            command.Parameters.AddWithValue("@projectId", projectId.ToString("D"));
            command.Parameters.AddWithValue("@type", type);

            using var reader = command.ExecuteReader();
            Resource user = null;
            while (reader.Read())
            {
                user = new Resource
                {
                    Id = reader.GetGuid("Id"),
                    ProjectId = reader.GetGuid("ProjectId"),
                    Date = reader.GetDateTime("Date"),
                    Type = (ResourceType)Enum.Parse(typeof(ResourceType), reader.GetString("Value"), true),
                    Value = reader.GetDouble("Value")
                };
                break;
            }
            connection.Close();
            return user;
        }
    }
}
