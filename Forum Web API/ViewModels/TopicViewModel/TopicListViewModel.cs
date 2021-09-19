﻿using System.Collections.Generic;
using Forum_Web_API.ViewModels.PostViewModel;

namespace Forum_Web_API.ViewModels.TopicViewModel
{
    public class TopicListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfPosts { get; set; }
        public int NumberOfUsers { get; set; }
      
        public bool HasRecentPost { get; set; }

        public PostListViewModel Latest { get; set; }
        public IEnumerable<PostListViewModel> AllPosts { get; set; }
    }
}