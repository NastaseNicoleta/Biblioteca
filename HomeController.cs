using BibliotecaISS.Models;
using BibliotecaISS.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using BibliotecaISS.ViewModels;

namespace BibliotecaISS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBibliotecaContext _context;
        private readonly CartiService _cartiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(IBibliotecaContext context, CartiService cartiService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cartiService = cartiService;
            _httpContextAccessor=httpContextAccessor;
        }
        // Mock data (înlocuiește cu interacțiunea cu baza de date)
        private static List<Abonat> abonati = new List<Abonat>();
        private static Abonat abonatAutentificat = null;

        private const string SessionName = "_Name";
        private const string SessionAge = "_Age";

        public IActionResult Index()
        {

            return View();
        }

        // Operații CRUD pentru Abonati
        public IActionResult Abonare()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcesareAbonare(Abonat abonat)
        {
            if (!ModelState.IsValid)
            {
                return View(abonat);
            }

            // Logică pentru salvarea abonatului în baza de date
            _context.Abonati.Add(abonat);
            _context.SaveChanges();
            return RedirectToAction("Index"); // Sau orice altă pagină de confirmare
        }

        // Autentificare
        public IActionResult Autentificare()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult ProcesareAutentificare(string cnp)
        {
            var abonat = _context.Abonati.FirstOrDefault(a => a.CNP == cnp);
            if (abonat != null)
            {
                HttpContext.Session.SetInt32("AbonatID", abonat.ID);
                HttpContext.Session.SetString("NumeAbonat", abonat.Nume);
                return RedirectToAction("PaginaUtilizator");
            }
            TempData["ErrorMessage"] = "CNP-ul introdus nu este valid. Vă rugăm să încercați din nou.";
            return RedirectToAction("Autentificare");
        }





        public IActionResult PaginaUtilizator()
        {
            // Înlocuiește acest cod cu logica de afișare a cărților disponibile
            var cartiDisponibile = _cartiService.GetCartiDisponibile();
            return View(cartiDisponibile);
        }

        public IActionResult ListaCartiDisponibile()
        {
            // Extrageți toate exemplarele de carte care sunt marcate ca disponibile
            var exemplareDisponibile = _context.ExemplareCarti.Where(exemplar => exemplar.Disponibil == true).ToList();

            // Inițializați o listă pentru a stoca informațiile despre cărțile disponibile
            List<Carte> cartiDisponibile = new List<Carte>();

            // Pentru fiecare exemplar disponibil, obțineți informațiile despre cartea corespunzătoare și adăugați-le în lista de cărți disponibile
            foreach (var exemplar in exemplareDisponibile)
            {
                var carte = _context.Carti.FirstOrDefault(c => c.ID == exemplar.CarteID);
                if (carte != null)
                {
                    cartiDisponibile.Add(carte);
                }
            }

            // Returnați lista de cărți disponibile către o viziune pentru a fi afișată utilizatorului
            return View(cartiDisponibile);
        }

        
        [HttpPost]
        public JsonResult ProcesareImprumut(int exemplarId)
        {
            int? abonatId = HttpContext.Session.GetInt32("AbonatID");
            if (!abonatId.HasValue)
            {
                return Json(new { success = false, message = "Utilizatorul nu este autentificat sau sesiunea a expirat." });
            }

            var exemplar = _context.ExemplareCarti.FirstOrDefault(e => e.ID == exemplarId);
            if (exemplar == null || !exemplar.Disponibil)
            {
                return Json(new { success = false, message = "Exemplarul nu este disponibil." });
            }

            exemplar.Disponibil = false;
            var imprumut = new Imprumut
            {
                AbonatID = abonatId.Value,
                ExemplarID = exemplar.ID,
                DataImprumut = DateTime.Now
            };

            _context.Imprumuturi.Add(imprumut);
            _context.SaveChanges();

            return Json(new { success = true });
        }



        
        [HttpPost]
        public JsonResult ProcesareRestituire(int imprumutId)
        {
            var imprumut = _context.Imprumuturi.FirstOrDefault(i => i.ID == imprumutId);
            if (imprumut == null)
            {
                return Json(new { success = false, message = "Împrumutul nu a fost găsit." });
            }

            if (!imprumut.DataReturnare.HasValue)
            {
                var exemplar = _context.ExemplareCarti.FirstOrDefault(e => e.ID == imprumut.ExemplarID);
                if (exemplar == null)
                {
                    return Json(new { success = false, message = "Exemplarul de carte nu a fost găsit." });
                }

                imprumut.DataReturnare = DateTime.Now;
                exemplar.Disponibil = true; // Marchează exemplarul ca disponibil
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Cartea a fost deja returnată." });
        }


        public ActionResult DetaliiImprumuturi()
        {
            // Obține ID-ul abonatului din sesiune
            int? abonatId = HttpContext.Session.GetInt32("AbonatID");
            if (!abonatId.HasValue)
            {
                // Dacă nu există un ID de abonat în sesiune, redirecționează la pagina de autentificare sau la o pagină de eroare
                return RedirectToAction("Autentificare");
            }

            var imprumuturiDetalii = _context.Imprumuturi
                .Where(i => i.AbonatID == abonatId.Value) // Filtrare după ID-ul abonatului
                .Join(_context.ExemplareCarti,
                    imprumut => imprumut.ExemplarID,
                    exemplar => exemplar.ID,
                    (imprumut, exemplar) => new { Imprumut = imprumut, Exemplar = exemplar })
                .Join(_context.Carti,
                    imprumutExemplar => imprumutExemplar.Exemplar.CarteID,
                    carte => carte.ID,
                    (imprumutExemplar, carte) => new ImprumutDetaliiViewModel
                    {
                        ID = imprumutExemplar.Imprumut.ID,
                        DataImprumut = imprumutExemplar.Imprumut.DataImprumut,
                        DataReturnare = imprumutExemplar.Imprumut.DataReturnare,
                        TitluCarte = carte.Titlu,
                        AutorCarte = carte.Autor
                    })
                .ToList();

            return View(imprumuturiDetalii);
        }


        // Delogare
        public IActionResult Delogare()
        {
            // Ștergeți datele de sesiune aici
            HttpContext.Session.Clear(); // Sau puteți șterge doar anumite chei de sesiune

            // Redirecționează către pagina de start sau altă pagină relevantă
            return RedirectToAction("Index", "Home");
        }
    }
}
