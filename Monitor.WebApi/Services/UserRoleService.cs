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

    public interface IUserRoleService
    {
        UserProfile GetById(Guid userId,Guid projectId);
      

        void AssignRoleToProject(UserRole u);
    }

    public class UserRoleService : IUserRoleService
    {
        private readonly AppSettings _appSettings;

        private readonly IProjectService _projectService;
        private readonly IUserService _userService;

        public UserRoleService(IOptions<AppSettings> appSettings, IProjectService projectService, IUserService userService)
        {
            _appSettings = appSettings.Value;
            _projectService = projectService;
            _userService = userService;
        }

        public UserProfile GetById(Guid userId,Guid projectId)
        {
            var result = new UserProfile
            {
                User = _userService.GetById(userId),
                Projects = _projectService.GetByUserId(userId)
            };
            result.ActiveProject = result.Projects.FirstOrDefault(x => x.Project.Id == projectId);

            return result;
        }

        public void AssignRoleToProject(UserRole u)
        {
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"insert into  userrole (`IdUser`,`IdProject`,`Role`) VALUES (@IdUser,@IdProject,@Role);";
            command.Parameters.AddWithValue("@IdUser", u.IdUser.ToString("D"));
            command.Parameters.AddWithValue("@IdProject", u.IdProject.ToString("D"));
            command.Parameters.AddWithValue("@Role", u.Role);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
