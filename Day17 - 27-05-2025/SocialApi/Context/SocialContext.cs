using Microsoft.EntityFrameworkCore;
using SocialApi.Models;

namespace SocialApi.Context
{
    public class SocialContext : DbContext
    {
        public SocialContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Tweet> Tweets { get; set; } = null!;
        public DbSet<Hashtag> Hashtags { get; set; } = null!;
        public DbSet<Follow> Follows { get; set; } = null!;
        public DbSet<TweetHashtag> TweetHashtags { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;

       
    }
}