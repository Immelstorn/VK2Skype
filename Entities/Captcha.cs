using System.Collections.Generic;

namespace VK2Skype.Entities
{
	class Captcha
	{
		public Error error { get; set; }
	}
	public class RequestParam
	{
		public string key { get; set; }
		public string value { get; set; }
	}

	public class Error
	{
		public int error_code { get; set; }
		public string error_msg { get; set; }
		public List<RequestParam> request_params { get; set; }
		public string captcha_sid { get; set; }
		public string captcha_img { get; set; }
	}
}
