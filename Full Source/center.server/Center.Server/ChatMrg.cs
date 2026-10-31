using System.Collections.Generic;

namespace Center.Server
{
	public class ChatMrg
	{
		private static List<ChatInfo> _chatInfos = new List<ChatInfo>();

		private static object _sycObj = new object();

		public static void AddChat(ChatInfo info)
		{
			lock (_sycObj)
			{
				_chatInfos.Add(info);
			}
			if (_chatInfos.Count > 100)
			{
				ClearAllChat();
			}
		}

		public static List<ChatInfo> GetAllChat()
		{
			lock (_sycObj)
			{
				return _chatInfos;
			}
		}

		public static void ClearAllChat()
		{
			lock (_sycObj)
			{
				_chatInfos.Clear();
			}
		}
	}
}
