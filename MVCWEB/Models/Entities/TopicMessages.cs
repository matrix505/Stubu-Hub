namespace MVCWEB.Models.Entities
{
    public class TopicMessages
    {
        public int? Message_id { get; set; }
        public int? Sender_id { get; set; }
        public int? Topic_id { get; set; }
        public string? SenderName { get; set; }
        public String? Message { get; set; }
        public TimeOnly? Timestamp { get; set; }
        
    }
}
