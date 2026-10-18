using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data;

public class AppDataContext(DbContextOptions<AppDataContext> options) : DbContext(options) {
    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Projeto>().Property(p => p.Nome).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Categoria>().Property(c => c.Nome).IsRequired().HasMaxLength(100);

        var tarefa = modelBuilder.Entity<Tarefa>();
        tarefa.Property(t => t.Titulo).IsRequired().HasMaxLength(150);
        tarefa.Property(t => t.Descricao).HasMaxLength(2000);

        tarefa.Property(t => t.CriadaEmUtc).HasConversion(
            valor => valor,
            valor => DateTime.SpecifyKind(valor, DateTimeKind.Utc)
        );

        tarefa.HasOne<Categoria>().WithMany().HasForeignKey(t => t.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        tarefa.HasOne<Projeto>().WithMany().HasForeignKey(t => t.ProjetoId).OnDelete(DeleteBehavior.Restrict);
        
        tarefa.ToTable("Tarefas", tabela => {
            tabela.HasCheckConstraint("CK_Tarefas_Status", "Status IN (0, 1, 2)");
            tabela.HasCheckConstraint("CK_Tarefas_Prioridade", "Prioridade IN (0, 1, 2)");
        });
    }
}
