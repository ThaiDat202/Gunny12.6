using SqlDataProvider.Data;

namespace Center.Server
{
	public class Player
	{
		public int Id;

		public string Name;

		public int ZoneId;

		public string ZoneName;

		public string NickName;

		public string Password;

		public long LastTime;

		public bool IsFirst;

		public bool IsLoginDevice;

		public ePlayerState State;

		public ServerClient CurrentServer;

		public PlayerInfo Info;
	}
}
