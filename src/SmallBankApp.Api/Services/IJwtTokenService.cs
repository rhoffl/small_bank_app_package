using SmallBankApp.Api.Models;
namespace SmallBankApp.Api.Services;
public interface IJwtTokenService { string CreateAccessToken(ApplicationUser user, IEnumerable<string> roles); }
