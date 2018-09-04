using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CreatingVoenEsh.MillitaryData
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int Lenght { get; set; }
        public int Mass { get; set; }
        public bool TypeEquip { get; set; }

        //Связь многие ко многим
        //public virtual List<Battalion> Battalions { get; set; }
    }
}
