using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrahvManage.Models.Account
{
    [Table("Accounts")]
    public class AccountModel
    {
        [Key]
        public int Id { get; set; }

        [RegularExpression(@"^[A-Z][a-zA-Z]*(\s[A-Z][a-zA-Z]*)*$", ErrorMessage = "Vale nimi")]
        [Required(ErrorMessage = "Sisesta nimi")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[A-Z][a-zA-Z]*(\s|-)?[a-zA-Z]*$", ErrorMessage = "Vale perekonnanimi")]
        [Required(ErrorMessage = "Sisesta perekonnanimi")]
        public string LastName { get; set; }

        [RegularExpression(@"^(Male|Female|Other|Non-Binary)$", ErrorMessage = "Vale sugu")]
        [Required(ErrorMessage = "Sisesta sugu")]
        public string Gender { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Vale email")]
        [Required(ErrorMessage = "Sisesta email")]
        public string Email { get; set; }

        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "Vale pin-kood")]
        [Required(ErrorMessage = "Sisesta pin-kood")]
        public int PinCode { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Vale isikukood")]
        [Required(ErrorMessage = "Sisesta isikukood")]
        public string PersonalCode { get; set; }
        public string Role { get; set; }
    }
}