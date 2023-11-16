using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Entities;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder) //usar el api fluent para configurar la base de datos
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("security");// definir un esquema para las tablas que se vana crear

        builder.Entity<IdentityUser>().ToTable("users");// cambiar el nombre de la tabla de usuarios
        builder.Entity<IdentityRole>().ToTable("roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("users_roles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("users_claims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("users_logins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("roles_claims");
        builder.Entity<IdentityUserToken<string>>().ToTable("users_tokens");
        
        // para afectar la propiedad de la tabla de books y que sea unica
        builder.Entity<Book>()
            .HasIndex(x => x.ISBN)
            .IsUnique(true);
    }

    public DbSet<Autor> Autores { get; set; }
    public DbSet<Book> Books { get; set; }
}