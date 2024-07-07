namespace BibliotecaISS.Models
{
    public class Imprumut
    {
        public int ID { get; set; }
        public int AbonatID { get; set; } // ID-ul abonatului care a împrumutat cartea
        public int ExemplarID { get; set; } // ID-ul exemplarului de carte împrumutat
        public DateTime DataImprumut { get; set; }
        public DateTime? DataReturnare { get; set; }

    }
}
