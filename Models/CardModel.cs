
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrahvManage.Models
{
    [Table("Cards")]
    public class CardModel
    {
        [Key]
        public int Id { get; set; }
        public string CreditCardNumber { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Ainult kuupäev")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string ExpirationDate { get; set; }
        public string CVV { get; set; }
    }
}