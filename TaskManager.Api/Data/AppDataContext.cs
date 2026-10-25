using Microsoft.EntityFrameworkCore;

public class AppDataContext : DbContext{
    public DbSet<Tarefa> Tarefas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Projeto> Projetos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=TaskManager.db");
    }

    // Configurações de relacionamento entre as entidades 
    protected override void OnModelCreating(ModelBuilder modelBuilder) {

        // Uma tarefa pode ter uma categoria, mas uma categoria pode ter várias tarefas.
        modelBuilder.Entity<Tarefa>()
            .HasOne(t => t.Categoria)
            .WithMany()
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.SetNull); // Se a categoria for deletada, o campo CategoriaId fica null

        // Uma tarefa pode ter um projeto, mas um projeto pode ter várias tarefas.
        modelBuilder.Entity<Tarefa>()
            .HasOne(t => t.Projeto)
            .WithMany()
            .HasForeignKey(t => t.ProjetoId)
            .OnDelete(DeleteBehavior.SetNull); // Se o projeto for deletado, o campo ProjetoId fica null
}

}
