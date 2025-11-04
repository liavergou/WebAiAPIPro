using CoordExtractorApp.Core.Enums;

namespace CoordExtractorApp.Data
{
    public class User :BaseEntity
    {

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Firstname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public UserRole? UserRole { get; set; } = null!; // enum: Admin, Manager, Member)

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>(); //navigation property

        public virtual ICollection<ConversionJob> ConversionJobs { get;set; } = new List<ConversionJob>(); //navigation property

    }
}
