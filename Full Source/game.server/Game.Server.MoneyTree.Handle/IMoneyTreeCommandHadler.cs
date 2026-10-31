using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MoneyTree.Handle;

public interface IMoneyTreeCommandHadler
{
	bool CommandHandler(GamePlayer Player, GSPacketIn packet);
}
