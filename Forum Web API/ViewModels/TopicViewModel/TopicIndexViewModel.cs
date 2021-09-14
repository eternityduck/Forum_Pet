using System.Collections.Generic;

namespace Forum.ViewModels.TopicViewModel
{
    public class TopicIndexViewModel
    {
        public int NumberOfTopics { get; set; }
        public IEnumerable<TopicListViewModel> TopicList { get; set; }
    }
}