namespace GrainOperationAPI.Models.DTOs
{
    public class StorageContainerDto
    {
        // We will only include properties that the client can set when creating a new storage container
        public string GrainType { get; set; } // This could be set to null if not required at creation time
        public string GrainClass { get; set; } // This could be set to null if not required at creation time
        public string Name { get; set; }
        public decimal TotalCapacity { get; set; } // This will be set based on the user selection from a dropdown
    }
}

