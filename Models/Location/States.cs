using System.ComponentModel.DataAnnotations;
namespace MyPortfolio.Models.Location
{
    public class States
    {
        [Key]        
        public string Id { get; set; } = null!;

        [Required]
        public string StateName { get; set; } = null!;

        // One to Many relationship with Districts (One)
        public virtual ICollection<Districts> Districts { get; set; } = new HashSet<Districts>();

    }
}
