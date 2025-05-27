using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialApi.Models
{
    public class TweetHashtag
    {
        public int TweetId { get; set; }

        [ForeignKey("UserId")]
        public Tweet Tweet { get; set; } = null!;

        public int HashtagId { get; set; }

        [ForeignKey("HashtagId")]
        public Hashtag Hashtag { get; set; } = null!;
    }
}