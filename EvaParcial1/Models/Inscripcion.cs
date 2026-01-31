using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvaParcial1.Models
{
    [Table("Inscripciones")]
    public class Inscripcion
    {
        [Key]
        [Column("inscripcion_id")]
        public int InscripcionId { get; set; }

        [Required(ErrorMessage = "El estudiante es obligatorio")]
        [Column("estudiante_id")]
        [Display(Name = "Estudiante")]
        public int EstudianteId { get; set; }

        [Required(ErrorMessage = "El curso es obligatorio")]
        [Column("curso_id")]
        [Display(Name = "Curso")]
        public int CursoId { get; set; }

        [Column("fecha_inscripcion")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inscripción")]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        [StringLength(20)]
        [Column("estado")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activo";

        // Navegación
        [ForeignKey("EstudianteId")]
        public virtual Estudiante? Estudiante { get; set; }

        [ForeignKey("CursoId")]
        public virtual Curso? Curso { get; set; }
    }
}
