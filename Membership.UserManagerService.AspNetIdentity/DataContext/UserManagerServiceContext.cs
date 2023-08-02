namespace Membership.UserManagerService.AspNetIdentity.DataContext;
internal class UserManagerServiceContext : IdentityDbContext<User>
{
    readonly AspNetIdentityOptions Options;
    public UserManagerServiceContext(IOptions<AspNetIdentityOptions> options)
    {
        Options = options.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Options.ConnectionString);
        base.OnConfiguring(optionsBuilder);
    }
}
