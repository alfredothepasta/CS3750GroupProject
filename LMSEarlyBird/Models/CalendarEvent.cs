namespace LMSEarlyBird.Models
{
	public class CalendarEvent
	{
		public int Id { get; set; }
		public string title { get; set; }
		public DateTime start { get; set; }
		public DateTime end { get; set; }
		public string backgroundColor { get; set; }
		public string borderColor { get; set; }
        public string textColor { get; set; }
		public string url { get; set; }
	}
}
