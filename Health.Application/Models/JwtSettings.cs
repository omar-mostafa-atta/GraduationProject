using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class JwtSettings
    {
        public string Secret { get; set; }      // The key used to sign tokens
        public string Issuer { get; set; }      // Who issues the token
        public string Audience { get; set; }    // Who the token is for
        public int ExpiryMinutes { get; set; }  // Token expiration in minutes
    }

}
