using System.Collections.Generic;

namespace CreatingVoenEsh.MillitaryData
{
    public class Brigade
    {
        public int BrigadeId { get; set; }
        public string Name { get; set; }

        public virtual List<Battalion> Battalions { get; set; }
    }
}
