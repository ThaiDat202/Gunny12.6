using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(15)]
public class BuyTimes : ISanXiaoCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		if (num > 0 && num <= 99)
		{
			int num2 = GameProperties.ThreeCleanBuyCost * num;
			if (num2 <= player.PlayerCharacter.Money)
			{
				player.RemoveMoney(num2);
				player.Actives.Info.SXStepRemain += num;
				player.Actives.SendSXBuyTime();
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
			}
		}
		return 1;
	}
}
