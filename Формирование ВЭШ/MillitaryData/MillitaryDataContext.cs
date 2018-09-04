namespace Формирование_ВЭШ.MillitaryData
{
    using System.Data.Entity;

    public partial class MillitaryDataContext : DbContext
    {
        public MillitaryDataContext()
            : base("name=MillitaryDataContext")
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
