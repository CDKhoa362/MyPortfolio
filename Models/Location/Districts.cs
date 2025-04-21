using System.ComponentModel.DataAnnotations;
namespace MyPortfolio.Models.Location
{
    public class Districts
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required]
        public string DistrictName { get; set; } = null!;

        // <Navigation Property> <The name of key in Principal table>
        // One to Many relationship with States (Many)
        public string StateId { get; set; } = null!;
        public virtual States State { get; set; } = null!;

        // One to many relationship with Wards (One)
        public virtual ICollection<Wards> Wards { get; set; } = new HashSet<Wards>();

    }
}
