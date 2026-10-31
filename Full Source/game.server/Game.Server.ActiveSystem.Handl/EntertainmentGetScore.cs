using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(71)]
public class EntertainmentGetScore : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		if (!Player.IsUpdateEnterModePoint)
		{
			Player.IsUpdateEnterModePoint = true;
			WorldMgr.ChatEntertamentModeGetPoint(Player.PlayerCharacter, Player.Actives.Info.EntertamentPoint);
		}
		gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(72);
		gSPacketIn.WriteInt(Player.Actives.Info.EntertamentPoint);
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
