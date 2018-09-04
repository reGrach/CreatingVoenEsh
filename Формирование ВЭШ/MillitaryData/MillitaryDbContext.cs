namespace CreatingVoenEsh.MillitaryData
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MillitaryDbContext : DbContext
    {
        public MillitaryDbContext()
            : base("name=MillitaryDbContext")
        {
        }

        public DbSet<Brigade> Brigades { get; set; }
        public DbSet<Battalion> Battalions { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentBattalion> EquipmentBattalions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
