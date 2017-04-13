using System.ComponentModel.DataAnnotations;
using LoginReg.Validators;
namespace LoginReg.Models
{
    public abstract class BaseEntity
    {

    }
    public class User
    {
        [DisplayAttribute(Name = "First Name")]
        [RequiredAttribute(ErrorMessage = "This field is required")]
        [MinLengthAttribute(3, ErrorMessage = "Field must be at least 3 characters long")]
        public string FirstName {get;set;}

        [DisplayAttribute(Name = "Last Name")]
        [RequiredAttribute(ErrorMessage = "This field is required")]
        [MinLengthAttribute(3, ErrorMessage = "Field must be at least 3 characters long")]
        public string LastName {get;set;}

        [DisplayAttribute(Name = "Email")]
        // [UserExistsAttribute()]
        [RequiredAttribute(ErrorMessage = "This field is required")]
        [EmailAddressAttribute(ErrorMessage = "Invalid email address")]
        public string Email {get;set;}

        [DisplayAttribute(Name = "Password")]
        [RequiredAttribute(ErrorMessage = "This field is required")]
        [DataTypeAttribute(DataType.Password)]
        [CompareAttribute("ConfirmPassword")]
        public string Password {get;set;}

        [DisplayAttribute(Name = "Confirm Password")]
        [RequiredAttribute(ErrorMessage = "This field is required")]
        [DataTypeAttribute(DataType.Password)]
        [CompareAttribute("Password")]
        public string ConfirmPassword {get;set;}
        // public string CreatedAt {get;set;}
    }
}