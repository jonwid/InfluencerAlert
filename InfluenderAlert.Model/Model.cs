using System;
using System.Collections.Generic;

namespace InfluenderAlert.Model
{
    public class Post
    {
        public string Url { get; set; }
        public string Source { get; set; }
        public string Influencer { get; set; }
        public int Followers { get; set; }
        public int Likes { get; set; }
        public string Views { get; set; }
        public DateTime Date { get; set; }
        public string Image { get; set; }
        public string Headline { get; set; }
    }

    public class Product
    {
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
    }

    public class Feed
    {
        public List<Product> Products { get; set; }
    }
}
