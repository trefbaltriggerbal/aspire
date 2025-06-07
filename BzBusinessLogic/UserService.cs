using BzData;
using BzData.Entities;
using Microsoft.EntityFrameworkCore;

namespace BzBusinessLogic;

public class UserService
{
    private readonly BzDbContext _db;

    public UserService(BzDbContext db)
    {
        _db = db;
    }

    public Task<BzUser?> FindUserAsync(string userName) =>
        _db.Users.SingleOrDefaultAsync(u => u.UserName == userName);
}
