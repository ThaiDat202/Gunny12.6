using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeAdorn.Handle;

[HomeAdornHandleAttbute(8)]
public class YardAdornListInfo : IHomeAdornCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(292);
		gSPacketIn.WriteByte(8);
		List<AdornInfo> adornByType = Player.Home.GetAdornByType("yard");
		gSPacketIn.WriteInt(adornByType.Count);
		foreach (AdornInfo item in adornByType)
		{
			gSPacketIn.WriteInt(item.TemplateId);
			gSPacketIn.WriteInt(item.Category);
		}
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
