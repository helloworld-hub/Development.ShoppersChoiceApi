using ShoppersChoice.API.Models;

namespace ShoppersChoiceSevice.Authentication_Authorization
{
        public interface ITokenService
        {
            string GenerateToken(User user);
        }
    
}
