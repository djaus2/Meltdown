using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazMail.Data
{
    public class EmailUser
    {
        public EmailUser (string name, string email)
        {
            Name = name;
            Email = email;
        }


        public EmailUser()
        {

        }

        public string Name { get; set; }
        public string Email { get; set; }

        public string Password { get; set; } // Only for sender


    }

}
