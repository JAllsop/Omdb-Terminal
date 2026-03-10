using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OmdbTerminal.ApiService.Data;

public class OmdbDbContextFactory : IDesignTimeDbContextFactory<OmdbDbContext>
{
    public OmdbDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OmdbDbContext>();

        // We use a dummy connection string because we only need this to generate the code, 
        // not to actually talk to the database right now.
        optionsBuilder.UseMySql("Server=localhost;Database=dummy;", new MySqlServerVersion(new Version(8, 4, 8)));

        return new OmdbDbContext(optionsBuilder.Options);
    }
}