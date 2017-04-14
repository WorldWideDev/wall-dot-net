using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Wall.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Wall.Validators
{
    public class UserExistsAttribute : ValidationAttribute
    {
        private readonly DbConnector _dbConnector;
        private string _email;
        public UserExistsAttribute()
        {
            //_dbConnector = HttpContext.RequestServices.GetRequiredService<DbConnector>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            User user = (User)validationContext.ObjectInstance;
            List<Dictionary<string, object>> userQuery = _dbConnector.Query($"SELECT id FROM users WHERE email = '{user.Email}'");
            if(_email == (string)userQuery[0]["email"]){ return ValidationResult.Success; }
            else { return new ValidationResult("Email already exists"); }
        }

        // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // {
        //     User user = (User)validationContext.ObjectInstance;
        //     List<Dictionary<string, object>> userQuery = _dbConnector.Query($"SELECT id FROM users WHERE email = '{user.Email}'");
        //     if(_email == (string)userQuery[0]["email"])
        //     {
        //         yield return new ValidationResult("Email already exists");
        //     }
        // }

    }

}