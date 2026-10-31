using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(0)]
public class HomeEnter : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		UserHomeInfo userHomeInfo = null;
		bool flag = false;
		int val = 1;
		if (num == Player.PlayerId)
		{
			flag = Player.Home.LoadFromDatabase(num);
			userHomeInfo = Player.Home.Data;
		}
		else
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(num);
			if (playerById != null)
			{
				userHomeInfo = playerById.Home.FriendData;
			}
			else
			{
				using (new PlayerBussiness())
				{
					if (userHomeInfo == null)
					{
						userHomeInfo = Player.Home.CreateFriendHomeInfo(num);
					}
				}
			}
		}
		if (userHomeInfo != null)
		{
			flag = true;
			val = userHomeInfo.enableFight;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(285);
		gSPacketIn.WriteByte(0);
		gSPacketIn.WriteBoolean(flag);
		if (flag)
		{
			gSPacketIn.WriteInt(num);
		}
		gSPacketIn.WriteInt(val);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
