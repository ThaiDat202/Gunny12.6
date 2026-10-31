using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeAdorn.Handle;

[HomeAdornHandleAttbute(4)]
public class HouseAdornInfo : IHomeAdornCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(292);
		int num = packet.ReadInt();
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(num);
		if (num == Player.PlayerId)
		{
			gSPacketIn.WriteInt(Player.Home.Data.houseStage);
			AdornInfo[] houseAdorn = Player.Home.HouseAdorn;
			foreach (AdornInfo adornInfo in houseAdorn)
			{
				gSPacketIn.WriteInt(adornInfo.TemplateId);
			}
		}
		else
		{
			gSPacketIn.WriteInt(Player.Home.FriendData.houseStage);
			AdornInfo[] friendHouseAdorn = Player.Home.FriendHouseAdorn;
			foreach (AdornInfo adornInfo2 in friendHouseAdorn)
			{
				gSPacketIn.WriteInt(adornInfo2.TemplateId);
			}
		}
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
