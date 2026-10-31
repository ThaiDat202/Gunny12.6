using System;
using System.Data.Entity;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Academy.Handle;

[AcademyHandle(13)]
public class FireApprentice : IAcademyCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		if (player.RemoveGold(20000) > 0)
		{
			if (player.PlayerCharacter.AcademyInfo.ApprenticeshipState >= AcademyMgr.MASTER_STATE && AcademyMgr.FireApprentice(player, num, isSilent: false))
			{
				player.PlayerCharacter.AcademyInfo.FreezesDate = DateTime.Now.AddHours(GameProperties.AcademyMasterFreezeHours);
				PlayerData playerData = new PlayerData();
				playerData.Entry(player.PlayerCharacter.AcademyInfo).State = EntityState.Modified;
				playerData.SaveChanges();
				player.Out.SendAcademyApprenticeState(player.PlayerCharacter, num);
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("BreakApprentice.FireApprenticeCD"));
			}
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("BreakApprentice.NotEnoughGold"));
		}
		return 0;
	}
}
