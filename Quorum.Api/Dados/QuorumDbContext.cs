using Microsoft.EntityFrameworkCore;
using Quorum.Api.Dominio;

namespace Quorum.Api.Dados;

public class QuorumDbContext(DbContextOptions<QuorumDbContext> options) : DbContext(options)
{
    public DbSet<UnidadeConsumidora> Unidades => Set<UnidadeConsumidora>();
    public DbSet<Carregador> Carregadores => Set<Carregador>();
    public DbSet<MedicaoDemanda> Medicoes => Set<MedicaoDemanda>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        var unidades = model.Entity<UnidadeConsumidora>();
        var carregadores = model.Entity<Carregador>();
        var medicoes = model.Entity<MedicaoDemanda>();

        unidades.HasMany(u => u.Carregadores).WithOne()
            .HasForeignKey(c => c.UnidadeConsumidoraId).OnDelete(DeleteBehavior.Cascade);
        unidades.HasMany(u => u.Medicoes).WithOne()
            .HasForeignKey(m => m.UnidadeConsumidoraId).OnDelete(DeleteBehavior.Cascade);

        medicoes.HasIndex(m => new { m.UnidadeConsumidoraId, m.RegistradaEm, m.Id });
        // O SQLite armazena a data sem fuso. A leitura restaura a indicação de UTC.
        medicoes.Property(m => m.RegistradaEm)
            .HasConversion(data => data, data => DateTime.SpecifyKind(data, DateTimeKind.Utc));

        unidades.HasData(new UnidadeConsumidora("Unidade de demonstração", 50) { Id = 1 });
        carregadores.HasData(
            new Carregador(1, "Carregador 11 kW", 11, true) { Id = 1 },
            new Carregador(1, "Carregador 22 kW", 22, true) { Id = 2 });
        medicoes.HasData(new MedicaoDemanda(1, 32, new DateTime(2026, 11, 15, 12, 0, 0, DateTimeKind.Utc)) { Id = 1 });
    }
}
