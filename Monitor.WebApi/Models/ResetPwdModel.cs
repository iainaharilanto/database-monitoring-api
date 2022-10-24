namespace Monitor.WebApi.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ResetPwdModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
