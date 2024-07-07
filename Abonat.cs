using System.ComponentModel.DataAnnotations;

namespace BibliotecaISS.Models
{
    public class Abonat
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "CNP-ul este obligatoriu.")]
        [RegularExpression("^[0-9]{13}$", ErrorMessage = "CNP-ul trebuie să conțină exact 13 cifre.")]
        public string CNP { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu.")]
        [RegularExpression("^[a-zA-ZăîâșțĂÎÂȘȚ -]+$", ErrorMessage = "Numele poate conține doar litere și spații.")]
        public string Nume { get; set; }

        public string Adresa { get; set; }

        [Required(ErrorMessage = "Numărul de telefon este obligatoriu.")]
        [RegularExpression("^(07[0-9]{8})$", ErrorMessage = "Numărul de telefon trebuie să înceapă cu 07 și să fie format din 10 cifre.")]
        public string Telefon { get; set; }
    }
}
