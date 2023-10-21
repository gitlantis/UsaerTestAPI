using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserTestMonnitorAPI.Models
{
    public class OrgUserModel
    {        
        public Guid? UserGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Username { get; set; }
        [EmailAddress(ErrorMessage = "Not a valid email")]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Password { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }
}   
