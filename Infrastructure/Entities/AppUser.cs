using MRDb.Domain;
using MRDb.Infrastructure.Interface;
using MRDbIdentity.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Entities
{
    public class AppUser : User, IEntity
    {
        public List<UserSocial> Socials { get; set; }
    }

    public class UserSocial
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
