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
    public class ConfigurationsController : ControllerBase
    {
        private IConfigurationService _configurationService;

        public ConfigurationsController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }     

        [IsAdmin]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _configurationService.GetAll();
            return Ok(users);
        }

        /*[HttpPost]
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
                _configurationService.Add(user);
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
            
        }*/
        
        
        [HttpPut("{id}")]
        public IActionResult Update(Configuration u)
        {
            try      
            {
               
                _configurationService.Update(u);
                return Ok();
            }
            catch (MySqlException ex)
            {
               
                return BadRequest(ex);
            }
            
        }

        
        [HttpGet("{id}")]
        [IsAuthorized]
        public IActionResult GetById(Guid id)
        {
            var config =_configurationService.GetById(id);
            return Ok(config);
        }

    }
}
