using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;

namespace CoreXF.Store.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            this.Extensions = new List<Extension>();
            this.RegisteredOn = DateTime.UtcNow;
        }

        public DateTime RegisteredOn { get; set; }

        public ICollection<Extension> Extensions { get; set; }
    }
}
