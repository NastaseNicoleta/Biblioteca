using Microsoft.EntityFrameworkCore;

namespace BibliotecaISS.Models
{
    public interface IBibliotecaContext
    {
        DbSet<Abonat> Abonati { get; set; }
        DbSet<Carte> Carti { get; set; }
        DbSet<ExemplarCarte> ExemplareCarti { get; set; }
        DbSet<Imprumut> Imprumuturi { get; set; }


        int SaveChanges();
    }
}
