using Microsoft.EntityFrameworkCore;
using EvaParcial1.Models;

namespace EvaParcial1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Curso
            modelBuilder.Entity<Curso>(entity =>
            {
                entity.HasKey(e => e.CursoId);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
            });

            // Configuración de Estudiante
            modelBuilder.Entity<Estudiante>(entity =>
            {
                entity.HasKey(e => e.EstudianteId);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuración de Inscripcion
            modelBuilder.Entity<Inscripcion>(entity =>
            {
                entity.HasKey(e => e.InscripcionId);
                entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Activo");
                entity.Property(e => e.FechaInscripcion).HasDefaultValueSql("GETDATE()");

                // Índice único para evitar duplicados
                entity.HasIndex(e => new { e.EstudianteId, e.CursoId }).IsUnique();

                // Relaciones
                entity.HasOne(e => e.Estudiante)
                    .WithMany(s => s.Inscripciones)
                    .HasForeignKey(e => e.EstudianteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Curso)
                    .WithMany(c => c.Inscripciones)
                    .HasForeignKey(e => e.CursoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
