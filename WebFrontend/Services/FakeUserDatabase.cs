using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Projects.WebFrontend.Services;

public static class FakeUserDatabase
{
    private static readonly Dictionary<string, (string Password, string Role)> Users = new()
    {
        ["client1"] = ("password", "Client"),
        ["parent1"] = ("token", "Parent"),
        ["admin"] = ("adminpass", "Admin")
    };

    public static bool TryValidate(string username, string password, out IEnumerable<Claim> claims)
    {
        if (Users.TryGetValue(username, out var user) && user.Password == password)
        {
            claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            return true;
        }

        claims = Enumerable.Empty<Claim>();
        return false;
    }
}
