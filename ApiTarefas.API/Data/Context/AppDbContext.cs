using Microsoft.EntityFrameworkCore;
using ApiTarefas.API.Models.Entities;

namespace ApiTarefas.API.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Tarefa> Tarefas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da tabela Tarefas
            modelBuilder.Entity<Tarefa>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Descricao)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.Categoria)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Tags)
                    .HasMaxLength(500);
                
                entity.Property(e => e.DataCriacao)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");
                
                entity.Property(e => e.DataAtualizacao)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<int>();
                
                entity.Property(e => e.Ativo)
                    .IsRequired()
                    .HasDefaultValue(true);
                
                // Índices para performance
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Categoria);
                entity.HasIndex(e => e.DataVencimento);
                entity.HasIndex(e => e.Prioridade);
                entity.HasIndex(e => e.Ativo);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Tarefa && 
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((Tarefa)entityEntry.Entity).DataCriacao = DateTime.Now;
                }

                ((Tarefa)entityEntry.Entity).DataAtualizacao = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}