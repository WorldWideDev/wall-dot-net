using System;
using System.ComponentModel.DataAnnotations;
namespace Wall.Models
{
    public class Message
    {
        public int ID { get; set; }

        [DisplayAttribute(Name = "New Wall Post")]
        [RequiredAttribute(ErrorMessage = "Field is Required")]
        [MinLengthAttribute(3, ErrorMessage = "Field must be at least 3 characters long")]
        public string Content { get; set;}
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserId { get; set; }
    }
}