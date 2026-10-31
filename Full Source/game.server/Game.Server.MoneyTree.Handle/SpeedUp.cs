using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.MoneyTree.Handle;

[MoneyTreeHandleAttbute(2)]
public class SpeedUp : IMoneyTreeCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int pos = packet.ReadInt();
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		MoneyTreeInfo moneyTreeInfo = Player.Home.GetMoneyTreeInfo(pos);
		if (moneyTreeInfo == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("SpeedUp.GetFail"));
		}
		else
		{
			int value = num * moneyTreeInfo.priceSpeedUp;
			if (Player.MoneyDirect(value))
			{
				moneyTreeInfo.timeGrown = moneyTreeInfo.timeGrown.AddMinutes(-(num * 60));
				Player.Home.SendMoneyTreeInfo();
				Player.SendMessage(LanguageMgr.GetTranslation("SpeedUp.Success"));
			}
		}
		return false;
	}
}
