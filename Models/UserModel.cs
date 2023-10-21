using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserTestMonnitorAPI.Models
{
    public class UserModel
    {        
        public string username { get; set; }        
        public string password { get; set; }
        public string token { get; set; }
        public DateTime? expires { get; set; }
    }
}
