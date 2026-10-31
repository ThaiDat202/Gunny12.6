using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Academy.Handle;

[AcademyHandle(6)]
public class MasterConfirm : IAcademyCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		AcademyRequestInfo request = AcademyMgr.GetRequest(num, player.PlayerId);
		if (request != null && request.Type == 1)
		{
			AcademyMgr.RemoveRequest(request);
			if (player.PlayerCharacter.AcademyInfo.FreezesDate <= DateTime.Now)
			{
				if (player.PlayerCharacter.AcademyInfo.ApprenticeshipState < AcademyMgr.MASTER_FULL_STATE && AcademyMgr.CheckCanMaster(player.PlayerCharacter.FightPower) && player.PlayerCharacter.AcademyInfo.IsMaster)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(num);
					if (playerById != null && AcademyMgr.CheckCanApprentice(playerById.PlayerCharacter.FightPower))
					{
						if (AcademyMgr.AddApprentice(player, playerById))
						{
							playerById.Out.SendAcademySystemNotice(LanguageMgr.GetTranslation("MasterConfirm", player.PlayerCharacter.NickName), isAlert: true);
							player.SendMessage(LanguageMgr.GetTranslation("ApprenticeConfirm", playerById.PlayerCharacter.NickName));
						}
						else
						{
							player.SendMessage(LanguageMgr.GetTranslation("TakeApprentice.Failed.AlreadyHasMaster"));
						}
					}
					else
					{
						player.SendMessage(LanguageMgr.GetTranslation("LoginServerConnector.HandleSysMess.Msg2"));
					}
				}
				else
				{
					player.SendMessage(LanguageMgr.GetTranslation("TakeApprentice.Failed"));
				}
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.TakeApprentice.Frozen", AcademyMgr.CheckDate(player.PlayerCharacter.AcademyInfo.FreezesDate)));
			}
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("AppClub.RemoveInfo.RecordNotFound"));
		}
		return 0;
	}
}
