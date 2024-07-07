using System.Collections.Generic;
using System.Linq;
using BibliotecaISS.Models;

namespace BibliotecaISS.Services
{
    public class CartiService
    {
        private readonly BibliotecaContext _context;

        public CartiService(BibliotecaContext context)
        {
            _context = context;
        }
        public class CarteDisponibila
        {
            public Carte Carte { get; set; }
            public int NumarExemplareDisponibile { get; set; }
        }
        public List<Carte> GetCartiDisponibile()
        {
            // Obținem toate ID-urile cărților care au cel puțin un exemplar disponibil
            var cartiDisponibileIds = _context.ExemplareCarti
                                               .Where(exemplar => exemplar.Disponibil)
                                               .Select(exemplar => exemplar.CarteID)
                                               .Distinct()
                                               .ToList();

            // Obținem cărțile corespunzătoare ID-urilor obținute
            var cartiDisponibile = _context.Carti
                                           .Where(carte => cartiDisponibileIds.Contains(carte.ID))
                                           .ToList();

            return cartiDisponibile;
        }

    }
}
