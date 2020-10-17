using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class UserFile
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        
        public virtual ApplicationUser User { get; set; }
    }
}
