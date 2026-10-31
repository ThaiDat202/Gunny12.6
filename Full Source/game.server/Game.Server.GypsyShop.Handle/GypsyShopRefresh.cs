using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GypsyShop.Handle;

[GypsyShopHandleAttbute(5)]
public class GypsyShopRefresh : IGypsyShopCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int curRefreshedTimes = player.Actives.Info.CurRefreshedTimes;
		int num = curRefreshedTimes * curRefreshedTimes * 30 + 20000;
		if (player.PlayerCharacter.myHonor >= num)
		{
			player.Actives.RefreshMysteryShop();
			player.Actives.Info.CurRefreshedTimes++;
			player.Actives.SendGypsyShopPlayerInfo();
			player.RemoveHonor(num);
			player.SendMessage(LanguageMgr.GetTranslation("GypsyShopRefresh.Success"));
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("GypsyShopRefresh.Fail", num));
		}
		return false;
	}
}
