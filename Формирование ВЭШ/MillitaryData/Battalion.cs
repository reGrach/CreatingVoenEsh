namespace CreatingVoenEsh.MillitaryData
{
    public class Battalion
    {
        public int BattalionId { get; set; }
        public string Name { get; set; }
        public bool Load { get; set; }
        public int Officer { get; set; }
        public int Serjant { get; set; }
        public int Soldat { get; set; }
        public int Civilian { get; set; }

        //Связь один ко многим
        public int BrigadeId { get; set; }
        public virtual Brigade Brigade { get; set; }

        //Связь многие ко многим
        //public virtual List<Equipment> Equipments { get; set; }

    }
}
