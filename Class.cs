namespace BibliotecaISS.ViewModels
{
    public class ImprumutDetaliiViewModel
    {
        public int ID { get; set; }
        public DateTime DataImprumut { get; set; }
        public DateTime? DataReturnare { get; set; }
        public string TitluCarte { get; set; }
        public string AutorCarte { get; set; }
    }
}
