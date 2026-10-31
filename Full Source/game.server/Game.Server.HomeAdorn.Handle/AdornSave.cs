using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeAdorn.Handle;

[HomeAdornHandleAttbute(6)]
public class AdornSave : IHomeAdornCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int templateId = packet.ReadInt();
			int num2 = packet.ReadInt();
			if (Player.Home.CheckExitAdorn(templateId))
			{
				AdornInfo adorn = Player.Home.GetAdorn(templateId);
				Player.Home.SaveAdorn(adorn);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(292);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteBoolean(val: true);
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
