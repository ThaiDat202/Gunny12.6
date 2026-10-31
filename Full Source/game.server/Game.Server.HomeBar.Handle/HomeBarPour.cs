using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeBar.Handle;

[HomeBarHandleAttbute(3)]
public class HomeBarPour : IHomeBarCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		UserHomeInfo data = Player.Home.Data;
		if (data != null)
		{
			data.pourCount++;
			int value = data.pourCount * data.pourCount * data.pourCount + GameProperties.HomeBarPourBasePrice;
			if (Player.MoneyDirect(value))
			{
				Player.AddHomeBarEnergy(GameProperties.HomeBarPourPower);
			}
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("HomeBarInfo.LoadDataFail"));
		}
		return true;
	}
}
