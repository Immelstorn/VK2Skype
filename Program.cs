using System;
using System.Threading;
using SKYPE4COMLib;

namespace VK2Skype
{
	internal class Program
	{
		private static void Main()
		{
			var skypeApi=new SkypeApi();
		    Logs.WriteLog("log.txt","Started.");
			while (true)
			{
				skypeApi.CheckNewMessages();
				Thread.Sleep(3000);
			}
		}
	}
}