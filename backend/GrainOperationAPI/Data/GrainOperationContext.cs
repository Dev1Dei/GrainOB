using Microsoft.EntityFrameworkCore;
using GrainOperationAPI.Models;

namespace GrainOperationAPI.Data
{
    public class GrainOperationContext : DbContext
    {
        public GrainOperationContext(DbContextOptions<GrainOperationContext> options)
        : base(options)
        {
           
        }
        public DbSet<FarmerModel> Farmers { get; set; }
        public DbSet<TruckModel> Trucks { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }
        
        public DbSet<PriceModel> Prices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // FarmerModel configuration
            modelBuilder.Entity<FarmerModel>(entity =>
            {
                entity.ToTable("Farmers");
                entity.HasKey(e => e.FarmerId);
                entity.Property(e => e.FarmerId).HasColumnName("farmer_id").ValueGeneratedOnAdd();
                entity.Property(e => e.FarmerFirstName).IsRequired();
                entity.Property(e => e.FarmerLastName).IsRequired();
            });

            // TruckModel configuration
            modelBuilder.Entity<TruckModel>(entity =>
            {
                entity.ToTable("Trucks");
                entity.HasKey(e => e.TruckId);
                entity.Property(e => e.TruckId).HasColumnName("truck_id").ValueGeneratedOnAdd();
                entity.Property(e => e.FarmerId).HasColumnName("farmer_id");
                entity.HasIndex(e => e.FarmerId); // Create an index on the FarmerId column.
                entity.Property(e => e.TruckNumbers).IsRequired();
                entity.Property(e => e.TruckStorage).IsRequired();
                // Define the relationship with FarmerModel.
                entity.HasOne(t => t.Farmer);
            });

            // TransactionModel configuration
            modelBuilder.Entity<TransactionModel>(entity =>
            {
                entity.ToTable("Transactions");
                entity.HasKey(e => e.TransactionId);
                entity.Property(e => e.TransactionId).HasColumnName("transaction_id").ValueGeneratedOnAdd();
                entity.Property(e => e.TruckId).HasColumnName("truck_id");
                entity.HasIndex(e => e.TruckId); // Create an index on the TruckId column.
                                                 // Define the relationship with TruckModel.
                entity.HasOne(tr => tr.Truck);
                // Assuming the other fields are required, you would also set them as required.
                entity.Property(e => e.GrainType).IsRequired();
                entity.Property(e => e.GrainClass).IsRequired();
                entity.Property(e => e.Dryness).IsRequired();
                entity.Property(e => e.Cleanliness).IsRequired();
                entity.Property(e => e.GrainWeight).IsRequired();
                entity.Property(e => e.ArrivalTime).IsRequired();
                entity.Property(e => e.WantedPay).IsRequired();
                entity.Property(e => e.PricePerTonne).IsRequired();
                entity.Property(e => e.Status).IsRequired(false);
            });
        }
    }
}
