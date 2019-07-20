namespace ProcessService.Entities
{
    public class Profile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool LockedOut { get; set; }
        public string ProfilePicsture { get; set; }
        public bool RestaurantFeatured { get; set; }

        //TODO be sure what you want to see when you're an user of the platform
    }
}