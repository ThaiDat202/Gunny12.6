using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Academy.Handle;

[AcademyHandle(7)]
public class ApprenticeConfirm : IAcademyCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		AcademyRequestInfo request = AcademyMgr.GetRequest(num, player.PlayerId);
		if (request != null && request.Type == 0)
		{
			AcademyMgr.RemoveRequest(request);
			if (player.PlayerCharacter.AcademyInfo.FreezesDate <= DateTime.Now)
			{
				if (player.PlayerCharacter.AcademyInfo.MasterID == 0 && AcademyMgr.CheckCanApprentice(player.PlayerCharacter.FightPower) && !player.PlayerCharacter.AcademyInfo.IsMaster)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(num);
					if (playerById != null && playerById.PlayerCharacter.FightPower >= AcademyMgr.Academy_FightPower && AcademyMgr.CheckCanMaster(playerById.PlayerCharacter.FightPower))
					{
						if (AcademyMgr.AddApprentice(playerById, player))
						{
							playerById.Out.SendAcademySystemNotice(LanguageMgr.GetTranslation("ApprenticeConfirm", player.PlayerCharacter.NickName), isAlert: true);
							player.SendMessage(LanguageMgr.GetTranslation("MasterConfirm", playerById.PlayerCharacter.NickName));
						}
						else
						{
							player.SendMessage(LanguageMgr.GetTranslation("AlreadlyHasRelationship.Apprentice"));
						}
					}
					else
					{
						player.SendMessage(LanguageMgr.GetTranslation("LoginServerConnector.HandleSysMess.Msg2"));
					}
				}
				else
				{
					player.SendMessage(LanguageMgr.GetTranslation("BeApprentice.Failed"));
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
