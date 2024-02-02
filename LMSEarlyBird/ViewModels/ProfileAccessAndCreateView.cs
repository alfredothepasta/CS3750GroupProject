namespace LMSEarlyBird.ViewModels
{
    public class ProfileAccessAndCreateView
    {
        public IEnumerable<ProfileViewModel> ProfileUpdater { get; set; }
        public IEnumerable<Models.Address> Addresses { get; set; }
    }
}
