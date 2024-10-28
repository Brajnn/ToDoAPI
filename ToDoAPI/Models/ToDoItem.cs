namespace ToDoAPI.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public double PercentComplete { get; set; }
        public bool IsDone { get; set; }
        
    }
}
