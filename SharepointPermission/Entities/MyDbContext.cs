using Microsoft.EntityFrameworkCore;

namespace SharepointPermission.Entities;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions options) : base(options)
    {
    }

    #region DbSet

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    #endregion
}