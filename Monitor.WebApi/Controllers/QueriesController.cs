namespace Monitor.WebApi.Controllers
{
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Monitor.WebApi.Entities;
    using Monitor.WebApi.Helpers;
    using Monitor.WebApi.Models;
    using Monitor.WebApi.Services;
    using MySqlConnector;
    using System;

    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    public class QueriesController : ControllerBase
    {
        private IUserService _userService;

        public QueriesController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [IsAdmin]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult GetAll(UserRequest u)
        {
            try      
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    LastName = u.LastName,
                    FirstName = u.FirstName,
                    Username = u.Username,
                    Password = u.Password
                };
                _userService.Add(user);
                return Ok(user);
            }
            catch (MySqlException ex)
            {
                if(ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return BadRequest("Username already exists");
                }
                return BadRequest(ex);
            }
            
        }
        
        [HttpPut("{id}")]
        public IActionResult Update(UserRequest u)
        {
            try      
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    LastName = u.LastName,
                    FirstName = u.FirstName,
                    Username = u.Username,
                    Password = u.Password
                };
                _userService.Add(user);
                return Ok(user);
            }
            catch (MySqlException ex)
            {
                if(ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return BadRequest("Username already exists");
                }
                return BadRequest(ex);
            }
            
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(Guid id)
        {
             _userService.RemoveById(id);
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user =_userService.GetById(id);
            return Ok(user);
        }

        [HttpPost("assign")]
        public IActionResult Assign(UserRole u)
        {
            try
            {
   
                _userService.AssignRoleToProject(u);
                return Ok(u);
            }
            catch (MySqlException ex)
            {
                if (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return BadRequest("Username already exists");
                }
                return BadRequest(ex);
            }

        }
    }
}
