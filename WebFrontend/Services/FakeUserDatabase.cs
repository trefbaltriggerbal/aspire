using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Projects.WebFrontend.Services;

public class FakeUserDatabase
{
    private readonly ILogger<FakeUserDatabase> _logger;
    private readonly Dictionary<string, (string Password, string Role)> _users = new()
    {
        ["client1"] = ("password", "Client"),
        ["parent1"] = ("token", "Parent"),
        ["admin"] = ("adminpass", "Admin")
    };

    public FakeUserDatabase(ILogger<FakeUserDatabase> logger)
    {
        _logger = logger;
    }

    public bool TryValidate(string username, string password, out IEnumerable<Claim> claims)
    {
        _logger.LogInformation("Login attempt for {Username}", username);
        if (_users.TryGetValue(username, out var user) && user.Password == password)
        {
            claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            _logger.LogInformation("Login succeeded for {Username}", username);
            return true;
        }

        _logger.LogWarning("Invalid credentials for {Username}", username);
        claims = Enumerable.Empty<Claim>();
        return false;
    }
}
