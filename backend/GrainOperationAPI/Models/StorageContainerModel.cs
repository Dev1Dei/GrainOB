namespace GrainOperationAPI.Models
{
    public class StorageContainerModel
    {
        public int ContainerId { get; set; }  // Auto-incremented primary key
        public int UserId { get; set; }
        public string Name { get; set; }
        public string GrainType { get; set; }
        public string GrainClass { get; set; }
        public decimal Weight { get; set; }
        public decimal Dryness { get; set; }
        public decimal Cleanliness { get; set; }
        public decimal TotalCapacity { get; set; }
        public decimal FreeSpace { get; set; }
        public decimal StoredSpace {  get; set; }

        // Navigation property to UserBalance
        public UserBalanceModel User { get; set; }
    }

}
