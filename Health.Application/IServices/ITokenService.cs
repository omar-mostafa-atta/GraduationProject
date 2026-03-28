using Health.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
