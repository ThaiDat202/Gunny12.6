using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeAdorn.Handle;

[HomeAdornHandleAttbute(7)]
public class YardAdornInfo : IHomeAdornCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(292);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(num);
		if (num == Player.PlayerId)
		{
			gSPacketIn.WriteInt(Player.Home.Data.houseStage);
			AdornInfo[] yardAdorn = Player.Home.YardAdorn;
			foreach (AdornInfo adornInfo in yardAdorn)
			{
				gSPacketIn.WriteInt(adornInfo.TemplateId);
			}
		}
		else
		{
			gSPacketIn.WriteInt(Player.Home.FriendData.houseStage);
			AdornInfo[] friendYardAdorn = Player.Home.FriendYardAdorn;
			foreach (AdornInfo adornInfo2 in friendYardAdorn)
			{
				gSPacketIn.WriteInt(adornInfo2.TemplateId);
			}
		}
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
