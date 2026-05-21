using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SmallBankApp.Api.DTOs;
using SmallBankApp.Api.Models;
using SmallBankApp.Api.Services;

namespace SmallBankApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly IJwtTokenService _jwt;
    private readonly IAlertService _alerts;

    public AuthController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, IJwtTokenService jwt, IAlertService alerts)
    { _users = users; _signIn = signIn; _jwt = jwt; _alerts = alerts; }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, PhoneNumber = request.PhoneNumber,
            LegalFirstName = request.LegalFirstName, LegalLastName = request.LegalLastName, DateOfBirth = request.DateOfBirth };
        var result = await _users.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        await _alerts.SendSecurityAlertAsync(user.Id, "Welcome. Please complete MFA setup and e-KYC verification.", ct);
        return Created($"/api/users/{user.Id}", new { user.Id, user.Email, user.KycStatus });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(request.Email);
        if (user is null) return Unauthorized();
        var check = await _signIn.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!check.Succeeded) return Unauthorized();
        // Require real MFA validation before token issuance in production.
        if (string.IsNullOrWhiteSpace(request.MfaCode)) return Unauthorized(new { requiresMfa = true });
        await _alerts.SendSecurityAlertAsync(user.Id, "New login detected.", ct);
        return Ok(new { accessToken = _jwt.CreateAccessToken(user, Array.Empty<string>()), tokenType = "Bearer" });
    }
}
