using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MemoryGame.Handle;

[MemoryGameHandleAttbute(14)]
public class BuyCount : IMemoryGameCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		if (num <= 0 || num > 999)
		{
			return 1;
		}
		int num2 = num * GameProperties.PairUpCardCost;
		if (player.PlayerCharacter.Money >= num2)
		{
			player.RemoveMoney(num2);
			player.Actives.Info.MemoryGameCount += num;
			GSPacketIn gSPacketIn = new GSPacketIn(329);
			gSPacketIn.WriteByte(14);
			gSPacketIn.WriteInt(player.Actives.Info.MemoryGameCount);
			player.SendTCP(gSPacketIn);
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
		}
		return 1;
	}
}
