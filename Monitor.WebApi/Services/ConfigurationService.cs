namespace Monitor.WebApi.Services
{
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Monitor.WebApi.Entities;
    using Monitor.WebApi.Helpers;
    using Monitor.WebApi.Models;
    using MySqlConnector;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;

    public interface IConfigurationService
    {

        IEnumerable<Configuration> GetAll();
        Configuration GetById(Guid id);
        void Add(Configuration u);
        void Update(Configuration u);
    }

    public class ConfigurationService : IConfigurationService
    {
        private IEnumerable<Configuration> _configurations
        {
            get
            {
                return this.GetAll().ToList();
            }
        }

        private readonly AppSettings _appSettings;

        private readonly IProjectService _projectService;

        public ConfigurationService(IOptions<AppSettings> appSettings, IProjectService projectService)
        {
            _appSettings = appSettings.Value;
            _projectService = projectService;
        }

        public IEnumerable<Configuration> GetAll()
        {
            var result = new List<Configuration>();
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM configurations order by ProjectId;";
            using var reader = command.ExecuteReader();
            Guid id = Guid.Empty;
            Configuration config = null;
            while (reader.Read())
            {
                var projId = reader.GetGuid("ProjectId");
                if (projId != id)
                {
                    if (config != null)
                    {
                        result.Add(config);
                    }
                    config = new Configuration();
                    config.ProjectId = projId;
                    id = projId;
                }
                var conf = reader.GetString("ConfigurationCode");
                var val = double.Parse(reader.GetString("Value"));
                switch (conf)
                {
                    case "SLOW_QUERY_THRESHOLD":
                        config.SlowQueryThreshold = val;
                        break;
                    case "ALLOCATED_MEMORY":
                        config.AllocatedMemory = val;
                        break;
                    case "BULK_INSERT_SIZE":
                        config.BulkInsertSize = val;
                        break;
                    case "FREQUENCY":
                        config.Frequency = val;
                        break;
                }

            }
            result.Add(config);

            connection.Close();
            return result;
        }

        public Configuration GetById(Guid id)
        {
            return _configurations.FirstOrDefault(x => x.ProjectId == id);
        }

        public void Add(Configuration u)
        {

            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"insert into  users (`Id`,`FirstName`,`LastName`,`Username`,`Password`) VALUES (@Id,@FirstName,@LastName,@Username,@Password);";
           /* command.Parameters.AddWithValue("@Id", u.Id.ToString("D"));
            command.Parameters.AddWithValue("@FirstName", u.FirstName);
            command.Parameters.AddWithValue("@LastName", u.LastName);
            command.Parameters.AddWithValue("@Username", u.Username);
            command.Parameters.AddWithValue("@Password", u.Password);*/
            command.ExecuteNonQuery();

            connection.Close();
        }

        public void Update(Configuration u)
        {
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"update configurations set `Value` = @AllocatedMemory where ProjectId=@ProjectId AND ConfigurationCode='ALLOCATED_MEMORY';
                                    update configurations set `Value` = @SlowQueryThreshold where ProjectId=@ProjectId AND ConfigurationCode='SLOW_QUERY_THRESHOLD';
                                    update configurations set `Value` = @Frequency where ProjectId=@ProjectId AND ConfigurationCode='FREQUENCY';
                                    update configurations set `Value` = @BulkInsertSize where ProjectId=@ProjectId AND ConfigurationCode='BULK_INSERT_SIZE';";
            command.Parameters.AddWithValue("@ProjectId", u.ProjectId.ToString("D"));
            command.Parameters.AddWithValue("@AllocatedMemory", u.AllocatedMemory);
            command.Parameters.AddWithValue("@SlowQueryThreshold", u.SlowQueryThreshold);
            command.Parameters.AddWithValue("@Frequency", u.Frequency);
            command.Parameters.AddWithValue("@BulkInsertSize", u.BulkInsertSize);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
