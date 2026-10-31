using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.CardNewSystem;

public interface ICardNewSystemProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
