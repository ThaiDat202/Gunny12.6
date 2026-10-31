using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team.Handle;

[TeamHandle(17)]
public class CheckInput : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData db = new PlayerData();
		bool flag = packet.ReadBoolean();
		string name = packet.ReadString();
		int num = TeamMgr.CheckCreateTeam(name, flag, db);
		GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
		gSPacketIn.WriteByte(17);
		gSPacketIn.WriteBoolean(flag);
		gSPacketIn.WriteByte((byte)num);
		player.SendTCP(gSPacketIn);
		return 0;
	}
}
