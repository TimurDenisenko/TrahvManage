using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrahvManage.Models
{
    [Table("Fines")]
    public class FineModel
    {
        [Key]
        public int Id { get; set; }
        public string PersonalCode { get; set; }
        public string AutoNumber { get; set; }
        public string Incident { get; set; }
        public string IncidentPlace { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Ainult kuupäev")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string IncidentDate { get; set; }
        public float FineAmount { get; set; }
        public string GifUrl { get; set; }
    }
}