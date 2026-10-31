using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(49)]
public class ClearColor : ISanXiaoCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (player.Actives.Info.SXStepRemain <= 0)
		{
			player.SendMessage(LanguageMgr.GetTranslation("SanXiao.NoEnouchStepRemain.Msg"));
			return 1;
		}
		if (player.PlayerCharacter.Money >= GameProperties.ThreeClearColourCleanCost)
		{
			player.RemoveMoney(GameProperties.ThreeClearColourCleanCost);
			player.Actives.Info.SXScore += GameProperties.ThreeClearColourCleanPoint;
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
		}
		return 1;
	}
}
