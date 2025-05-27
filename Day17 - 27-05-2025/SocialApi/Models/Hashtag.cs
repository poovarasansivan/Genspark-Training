using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SocialApi.Models
{
    public class Hashtag
    {
        public int Id { get; set; }
        public string Tag { get; set; } = null!;

        public ICollection<TweetHashtag> TweetHashtags { get; set; } = new List<TweetHashtag>();
    }
}