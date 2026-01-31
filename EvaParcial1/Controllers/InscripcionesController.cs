using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EvaParcial1.Data;
using EvaParcial1.Models;

namespace EvaParcial1.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InscripcionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inscripciones
        public async Task<IActionResult> Index()
        {
            var inscripciones = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Curso)
                .OrderByDescending(i => i.FechaInscripcion)
                .ToListAsync();
            return View(inscripciones);
        }

        // GET: Inscripciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Curso)
                .FirstOrDefaultAsync(m => m.InscripcionId == id);

            if (inscripcion == null)
            {
                return NotFound();
            }

            return View(inscripcion);
        }

        // GET: Inscripciones/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Inscripciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InscripcionId,EstudianteId,CursoId,FechaInscripcion,Estado")] Inscripcion inscripcion)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe la inscripción
                if (await _context.Inscripciones.AnyAsync(i => 
                    i.EstudianteId == inscripcion.EstudianteId && 
                    i.CursoId == inscripcion.CursoId))
                {
                    ModelState.AddModelError("", "Este estudiante ya está inscrito en este curso");
                    await PopulateDropdowns(inscripcion.EstudianteId, inscripcion.CursoId);
                    return View(inscripcion);
                }

                inscripcion.FechaInscripcion = DateTime.Now;
                _context.Add(inscripcion);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Inscripción realizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(inscripcion.EstudianteId, inscripcion.CursoId);
            return View(inscripcion);
        }

        // GET: Inscripciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null)
            {
                return NotFound();
            }

            await PopulateDropdowns(inscripcion.EstudianteId, inscripcion.CursoId);
            return View(inscripcion);
        }

        // POST: Inscripciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InscripcionId,EstudianteId,CursoId,FechaInscripcion,Estado")] Inscripcion inscripcion)
        {
            if (id != inscripcion.InscripcionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verificar si ya existe otra inscripción con la misma combinación
                if (await _context.Inscripciones.AnyAsync(i => 
                    i.EstudianteId == inscripcion.EstudianteId && 
                    i.CursoId == inscripcion.CursoId &&
                    i.InscripcionId != id))
                {
                    ModelState.AddModelError("", "Este estudiante ya está inscrito en este curso");
                    await PopulateDropdowns(inscripcion.EstudianteId, inscripcion.CursoId);
                    return View(inscripcion);
                }

                try
                {
                    _context.Update(inscripcion);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Inscripción actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InscripcionExists(inscripcion.InscripcionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(inscripcion.EstudianteId, inscripcion.CursoId);
            return View(inscripcion);
        }

        // GET: Inscripciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Curso)
                .FirstOrDefaultAsync(m => m.InscripcionId == id);

            if (inscripcion == null)
            {
                return NotFound();
            }

            return View(inscripcion);
        }

        // POST: Inscripciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion != null)
            {
                _context.Inscripciones.Remove(inscripcion);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Inscripción eliminada exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InscripcionExists(int id)
        {
            return _context.Inscripciones.Any(e => e.InscripcionId == id);
        }

        private async Task PopulateDropdowns(int? estudianteId = null, int? cursoId = null)
        {
            var estudiantes = await _context.Estudiantes
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            var cursos = await _context.Cursos
                .Where(c => c.FechaFin >= DateTime.Now)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.EstudianteId = new SelectList(
                estudiantes.Select(e => new { e.EstudianteId, NombreCompleto = $"{e.Apellido}, {e.Nombre}" }),
                "EstudianteId",
                "NombreCompleto",
                estudianteId);

            ViewBag.CursoId = new SelectList(cursos, "CursoId", "Nombre", cursoId);
        }
    }
}
