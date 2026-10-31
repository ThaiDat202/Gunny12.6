using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GuardCore.Handle;

[GuardCoreHandleAttbute(2)]
public class Equip : IGuardCoreCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GuardCoreTemplateInfo guardCoreTemplateInfo = GuardCoreMgr.FindGuardCoreTemplate(num);
		if (guardCoreTemplateInfo == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.Equip.DataError"));
			return 0;
		}
		if (guardCoreTemplateInfo.GainGrade > player.Level)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.Equip.PlayerLevelNotEngough"));
			return 0;
		}
		if (guardCoreTemplateInfo.GuardGrade > player.PlayerCharacter.guardCoreGrade)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.Equip.GuardGradeNotEnough"));
			return 0;
		}
		player.PlayerCharacter.guardCoreID = num;
		player.SendMessage(LanguageMgr.GetTranslation("GuardCore.Handle.Equip.Success"));
		player.UpdateProperties();
		return 1;
	}
}
