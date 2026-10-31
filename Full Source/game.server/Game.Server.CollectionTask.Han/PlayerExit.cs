using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.CollectionTask.Handle;

[CollectionTaskHandleAttbute(4)]
public class PlayerExit : ICollectionTaskCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		WorldMgr.CollectionTaskRooms.RemovePlayer(id);
		return true;
	}
}
