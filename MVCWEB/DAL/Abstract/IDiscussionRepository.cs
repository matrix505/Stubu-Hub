using MVCWEB.Models.Entities;

namespace MVCWEB.DAL.Abstract
{
    public interface IDiscussionRepository 
    {
        Task<bool> CreateDiscussion(Discussions ds);
        Task<List<Discussions>?> GetAllDiscussions(int ProjectId);
        Task<Discussions?> GetDiscussionById(int TopicId);
        Task<List<TopicMessages>?> GetTopicMessages(int TopicId);
    }
}
