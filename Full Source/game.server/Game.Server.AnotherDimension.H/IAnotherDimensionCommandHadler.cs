using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AnotherDimension.Handle;

public interface IAnotherDimensionCommandHadler
{
	bool CommandHandler(GamePlayer Player, GSPacketIn packet);
}
