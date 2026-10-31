using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.GuardCore.Handle;

[GuardCoreHandleAttbute(1)]
public class LevelUp : IGuardCoreCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (player.PlayerCharacter.guardCoreGrade >= GuardCoreMgr.MaxLevel())
		{
			player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.LevelUp.MaxLevel"));
			return 0;
		}
		GuardCoreLevelTemplateInfo guardCoreLevelTemplateInfo = GuardCoreMgr.FindGuardCoreLevelTemplate(player.PlayerCharacter.guardCoreGrade + 1);
		if (guardCoreLevelTemplateInfo == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.Equip.DataError"));
			return 0;
		}
		ItemInfo itemByTemplateID = player.PropBag.GetItemByTemplateID(12540);
		if (itemByTemplateID == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.LevelUp.GuardItemNotFound"));
			return 0;
		}
		if (guardCoreLevelTemplateInfo.Gold > player.PlayerCharacter.Gold)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.LevelUp.GoldNotEnough"));
			return 0;
		}
		long num = player.PlayerCharacter.GP - LevelMgr.FindLevel(player.Level).GP;
		if (guardCoreLevelTemplateInfo.Exp > num)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.LevelUp.GPNotEnough"));
			return 0;
		}
		if (guardCoreLevelTemplateInfo.Guard > itemByTemplateID.Count)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.LevelUp.ItemNotEnough", itemByTemplateID.Template.Name));
			return 0;
		}
		player.BeginChanges();
		player.RemoveGold(guardCoreLevelTemplateInfo.Gold);
		player.RemoveGP(guardCoreLevelTemplateInfo.Exp);
		player.RemoveTemplate(itemByTemplateID.TemplateID, guardCoreLevelTemplateInfo.Guard);
		player.PlayerCharacter.guardCoreGrade++;
		player.CommitChanges();
		player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.LevelUp.Success"));
		return 1;
	}
}
