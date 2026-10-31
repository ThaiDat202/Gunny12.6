using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GypsyShop.Handle;

[GypsyShopHandleAttbute(2)]
public class GypsyShopPlayerInfo : IGypsyShopCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (player.Actives.LoadGypsyItemDataFromDatabase())
		{
			player.Actives.SendGypsyShopPlayerInfo();
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("GypsyShopPlayerInfo.LoadDataFail"));
		}
		return false;
	}
}
