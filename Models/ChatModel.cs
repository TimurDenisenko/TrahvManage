using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrahvManage.Models
{
    [Table("Chats")]
    public class ChatModel
    {
        [Key]
        public int Id { get; set; }
        public string FirstPersonalCode { get; set; }
        public string SecondPersonalCode { get; set; }
        public string History { get; set; }
    }
}