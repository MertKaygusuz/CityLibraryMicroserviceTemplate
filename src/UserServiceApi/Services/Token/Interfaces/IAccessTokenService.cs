using UserServiceApi.Dtos.Token;

namespace UserServiceApi.Services.Token.Interfaces
{
    public interface IAccessTokenService
    {
        CreateTokenResultDto CreateToken(CreateTokenDto dto);
    }
}
