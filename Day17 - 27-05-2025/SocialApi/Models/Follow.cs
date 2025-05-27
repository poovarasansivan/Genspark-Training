using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialApi.Models
{
    public class Follow
    {
        [Key]
        public int FollowerId { get; set; }

        [ForeignKey("FollowerId")]
        public User Follower { get; set; } = null!;

        public int FollowingId { get; set; }

        [ForeignKey("FollowingId")]
        public User Following { get; set; } = null!;
    }
}