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

    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        void RemoveById(Guid id);
        void Add(User u);
        void ResetPwd(ResetPwdModel u);
        void Update(User u);

        void AssignRoleToProject(UserRole u);
    }

    public class UserService : IUserService
    {
        private IEnumerable<User> _users
        {
            get
            {
                return this.GetAll().ToList();
            }
        }

        private readonly AppSettings _appSettings;

        private readonly IProjectService _projectService;

        public UserService(IOptions<AppSettings> appSettings, IProjectService projectService)
        {
            _appSettings = appSettings.Value;
            _projectService = projectService;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            User user = null;
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users where Username=@username AND Password=@password";
            command.Parameters.AddWithValue("@username", model.Username);
            command.Parameters.AddWithValue("@password", model.Password);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                user = new User
                {
                    Id = reader.GetGuid("Id"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    LastName = reader.GetString("LastName"),
                    FirstName = reader.GetString("FirstName")
                };
                break;
            }

            connection.Close();

            if (user == null) return null;

            var projectRoles = _projectService.GetByUserId(user.Id);

            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token, projectRoles.Select(x => x.Project).ToList());
        }

        public IEnumerable<User> GetAll()
        {
            var result = new List<User>();
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var user = new User
                {
                    Id = reader.GetGuid("Id"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    LastName = reader.GetString("LastName"),
                    FirstName = reader.GetString("FirstName")
                };
                result.Add(user);
            }
            connection.Close();
            return result;
        }

        public User GetById(Guid id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveById(Guid id)
        {
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"delete FROM users where Id =@userId;";
            command.Parameters.AddWithValue("@userId", id.ToString("D"));
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void Add(User u)
        {

            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"insert into  users (`Id`,`FirstName`,`LastName`,`Username`,`Password`) VALUES (@Id,@FirstName,@LastName,@Username,@Password);";
            command.Parameters.AddWithValue("@Id", u.Id.ToString("D"));
            command.Parameters.AddWithValue("@FirstName", u.FirstName);
            command.Parameters.AddWithValue("@LastName", u.LastName);
            command.Parameters.AddWithValue("@Username", u.Username);
            command.Parameters.AddWithValue("@Password", u.Password);
            command.ExecuteNonQuery();

            connection.Close();
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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

        public void ResetPwd(ResetPwdModel u)
        {
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"update  users set `Password` = @password where Id=@IdUser;";
            command.Parameters.AddWithValue("@IdUser", u.Id.ToString("D"));
            command.Parameters.AddWithValue("@password", u.Password);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public void Update(User u)
        {
            using var connection = new MySqlConnection(this._appSettings.ConnectionStrings);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"update  users set `FirstName` = @FirstName, `LastName` = @LastName where Id=@IdUser;";
            command.Parameters.AddWithValue("@IdUser", u.Id.ToString("D"));
            command.Parameters.AddWithValue("@FirstName", u.FirstName);
            command.Parameters.AddWithValue("@LastName", u.LastName);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
