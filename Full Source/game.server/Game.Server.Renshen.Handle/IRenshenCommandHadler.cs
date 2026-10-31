using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Renshen.Handle;

public interface IRenshenCommandHadler
{
	bool CommandHandler(GamePlayer Player, GSPacketIn packet);
}
