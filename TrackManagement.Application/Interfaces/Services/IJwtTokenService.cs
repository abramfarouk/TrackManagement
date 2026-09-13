using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackManagement.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        (string Token, DateTime ExpiresAtUtc) GenerateToken(string username);
    }
}
