using System.Collections.Generic;

namespace VK2Skype
{
	public class Message
	{
		public int mid { get; set; }
		public int date { get; set; }
		public int @out { get; set; }
		public int uid { get; set; }
		public int read_state { get; set; }
		public string title { get; set; }
		public string body { get; set; }
		public int chat_id { get; set; }
		public string chat_active { get; set; }
		public int users_count { get; set; }
		public int admin_id { get; set; }
		public Attachment attachment { get; set; }
		public List<Attachment2> attachments { get; set; }
	}

	public class Photo
	{
		public int pid { get; set; }
		public int aid { get; set; }
		public int owner_id { get; set; }
		public string src { get; set; }
		public string src_big { get; set; }
		public string src_small { get; set; }
		public string src_xbig { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public string text { get; set; }
		public int created { get; set; }
		public string access_key { get; set; }
	}

	public class Attachment
	{
		public string type { get; set; }
		public Photo photo { get; set; }
	}

	public class Photo2
	{
		public int pid { get; set; }
		public int aid { get; set; }
		public int owner_id { get; set; }
		public string src { get; set; }
		public string src_big { get; set; }
		public string src_small { get; set; }
		public string src_xbig { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public string text { get; set; }
		public int created { get; set; }
		public string access_key { get; set; }
	}

	public class Attachment2
	{
		public string type { get; set; }
		public Photo2 photo { get; set; }
	}
}
