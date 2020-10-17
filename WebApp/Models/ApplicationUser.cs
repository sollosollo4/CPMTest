using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string GivenName { get; set; }
        public string SurName { get; set; }
        public string MiddleName { get; set; }

        public virtual HashSet<UserFile> UserFiles { get; set; }

        public ApplicationUser()
        {
            UserFiles = new HashSet<UserFile>();
        }
    }
}
