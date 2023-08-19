using System.ComponentModel.DataAnnotations;

namespace Sabio.Models.Requests.Ratings
{
    public class RatingMergeRequest
    {
        [Required]
        public double Rating { get; set; }
        [Required]
        public int EntityId { get; set; }
        [Required]
        public int EntityTypeId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
