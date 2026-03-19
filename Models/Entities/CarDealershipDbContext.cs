using Microsoft.EntityFrameworkCore;

namespace CarDealershipManager.Models.Entities
{
    public class CarDealershipDbContext : DbContext
    {
        public CarDealershipDbContext(DbContextOptions<CarDealershipDbContext> options)
            : base(options)
        {
        }

        public DbSet<CarModel> Cars { get; set; }
        public DbSet<BodyTypeModel> BodyTypes { get; set; }
        public DbSet<ColorModel> Colors { get; set; }
        public DbSet<DrivetrainModel> Drivetrains { get; set; }
        public DbSet<EuroClassModel> EuroClasses { get; set; }
        public DbSet<TransmissionTypeModel> TransmissionTypes { get; set; }
        public DbSet<MakeModel> Makes { get; set; }
        public DbSet<ModelModel> Models { get; set; }
        public DbSet<GenerationModel> Generations { get; set; }
        public DbSet<AdminModel> Admins { get; set; }
        public DbSet<GalleryModel> Galleries { get; set; }
        public DbSet<FuelTypeModel> FuelTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}