using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umob.GameHub.Domain.Entities;

namespace Umob.GameHub.Application.Abstractions.Authentication
{
    public interface IJwtToken
    {
        string GenerateToken(User user);
    }
}
