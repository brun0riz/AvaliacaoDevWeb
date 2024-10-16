using Microsoft.EntityFrameworkCore;

namespace YujiChikaraKiyota.Models;

public class AppDataContext : DbContext
{
    public DbSet<Folha> Folhas { get; set; }
    public DbSet<Funcionario> Funcionarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Bruno_Yuji.db");
    }
}