using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvaParcial1.Data;
using EvaParcial1.Models;

namespace EvaParcial1.Controllers
{
    public class EstudiantesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstudiantesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Estudiantes
        public async Task<IActionResult> Index()
        {
            var estudiantes = await _context.Estudiantes
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
            return View(estudiantes);
        }

        // GET: Estudiantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estudiante = await _context.Estudiantes
                .Include(e => e.Inscripciones)
                    .ThenInclude(i => i.Curso)
                .FirstOrDefaultAsync(m => m.EstudianteId == id);

            if (estudiante == null)
            {
                return NotFound();
            }

            return View(estudiante);
        }

        // GET: Estudiantes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estudiantes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EstudianteId,Nombre,Apellido,Email,FechaNacimiento")] Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                // Verificar email único
                if (await _context.Estudiantes.AnyAsync(e => e.Email == estudiante.Email))
                {
                    ModelState.AddModelError("Email", "Este correo electrónico ya está registrado");
                    return View(estudiante);
                }

                // Validar fecha de nacimiento
                if (estudiante.FechaNacimiento > DateTime.Now)
                {
                    ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser futura");
                    return View(estudiante);
                }

                _context.Add(estudiante);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Estudiante registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(estudiante);
        }

        // GET: Estudiantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null)
            {
                return NotFound();
            }
            return View(estudiante);
        }

        // POST: Estudiantes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstudianteId,Nombre,Apellido,Email,FechaNacimiento")] Estudiante estudiante)
        {
            if (id != estudiante.EstudianteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verificar email único (excluyendo el actual)
                if (await _context.Estudiantes.AnyAsync(e => e.Email == estudiante.Email && e.EstudianteId != id))
                {
                    ModelState.AddModelError("Email", "Este correo electrónico ya está registrado por otro estudiante");
                    return View(estudiante);
                }

                // Validar fecha de nacimiento
                if (estudiante.FechaNacimiento > DateTime.Now)
                {
                    ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser futura");
                    return View(estudiante);
                }

                try
                {
                    _context.Update(estudiante);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Estudiante actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstudianteExists(estudiante.EstudianteId))
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
            return View(estudiante);
        }

        // GET: Estudiantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(m => m.EstudianteId == id);
            if (estudiante == null)
            {
                return NotFound();
            }

            return View(estudiante);
        }

        // POST: Estudiantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante != null)
            {
                _context.Estudiantes.Remove(estudiante);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Estudiante eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EstudianteExists(int id)
        {
            return _context.Estudiantes.Any(e => e.EstudianteId == id);
        }
    }
}
