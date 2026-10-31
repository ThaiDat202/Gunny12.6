using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GypsyShop;

public interface IGypsyShopProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
