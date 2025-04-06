using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Identiity.Token
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime TokenExpirationDate { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }

    }
}

