using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeAdorn.Handle;

[HomeAdornHandleAttbute(9)]
public class YardAdornSave : IHomeAdornCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int templateId = packet.ReadInt();
			if (Player.Home.CheckExitAdorn(templateId))
			{
				AdornInfo adorn = Player.Home.GetAdorn(templateId);
				Player.Home.SaveYardAdorn(adorn);
			}
		}
		return true;
	}
}
