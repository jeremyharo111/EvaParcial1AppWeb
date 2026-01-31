using System.Diagnostics;
using EvaParcial1.Data;
using EvaParcial1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvaParcial1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get statistics for dashboard
            ViewBag.TotalCursos = await _context.Cursos.CountAsync();
            ViewBag.TotalEstudiantes = await _context.Estudiantes.CountAsync();
            ViewBag.TotalInscripciones = await _context.Inscripciones.CountAsync();
            
            // Get active courses (current date is between start and end)
            ViewBag.CursosActivos = await _context.Cursos
                .Where(c => c.FechaInicio <= DateTime.Now && c.FechaFin >= DateTime.Now)
                .CountAsync();

            // Get recent enrollments
            ViewBag.InscripcionesRecientes = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Curso)
                .OrderByDescending(i => i.FechaInscripcion)
                .Take(5)
                .ToListAsync();

            // Get upcoming courses
            ViewBag.CursosProximos = await _context.Cursos
                .Where(c => c.FechaInicio > DateTime.Now)
                .OrderBy(c => c.FechaInicio)
                .Take(3)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
