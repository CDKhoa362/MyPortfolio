using System.ComponentModel.DataAnnotations;
namespace MyPortfolio.Models.Location
{
    public class Wards
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required]
        public string WardName { get; set; } = null!;

        // One To Many relationship with Districts (Many)
        public string DistrictId { get; set; } = null!;
        public virtual Districts District { get; set; } = null!;

    }
}
