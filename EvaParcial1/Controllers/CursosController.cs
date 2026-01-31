using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvaParcial1.Data;
using EvaParcial1.Models;

namespace EvaParcial1.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cursos
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos
                .OrderByDescending(c => c.FechaInicio)
                .ToListAsync();
            return View(cursos);
        }

        // GET: Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .Include(c => c.Inscripciones)
                    .ThenInclude(i => i.Estudiante)
                .FirstOrDefaultAsync(m => m.CursoId == id);

            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // GET: Cursos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CursoId,Nombre,Descripcion,FechaInicio,FechaFin")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                if (curso.FechaFin < curso.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de fin debe ser mayor o igual a la fecha de inicio");
                    return View(curso);
                }

                _context.Add(curso);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Curso creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // GET: Cursos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }
            return View(curso);
        }

        // POST: Cursos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CursoId,Nombre,Descripcion,FechaInicio,FechaFin")] Curso curso)
        {
            if (id != curso.CursoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (curso.FechaFin < curso.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de fin debe ser mayor o igual a la fecha de inicio");
                    return View(curso);
                }

                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Curso actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExists(curso.CursoId))
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
            return View(curso);
        }

        // GET: Cursos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(m => m.CursoId == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Curso eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CursoExists(int id)
        {
            return _context.Cursos.Any(e => e.CursoId == id);
        }
    }
}
