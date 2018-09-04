using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreatingVoenEsh.MillitaryData
{
    public class EquipmentBattalion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BattalionId { get; set; }
        [Required]
        public int EquipmentId { get; set; }
        public int Count { get; set; }

        [ForeignKey("BattalionId")]
        public virtual Battalion Battalions { get; set; }
        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipments { get; set; }
    }
}
