using Microsoft.EntityFrameworkCore;

namespace BibliotecaISS.Models
{
    public class BibliotecaContext : DbContext, IBibliotecaContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options)
        {
        }

        // Definirea seturilor de date pentru fiecare tabel din baza de date
        public DbSet<Abonat> Abonati { get; set; }
        public DbSet<Carte> Carti { get; set; }
        public DbSet<ExemplarCarte> ExemplareCarti { get; set; }
        public DbSet<Imprumut> Imprumuturi { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Adaugă aici configurările suplimentare ale modelului, cum ar fi cheile primare, relațiile etc.
        }
    }
}
