using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Renshen;

public interface IRenshenProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
