using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Academy.Handle;

[AcademyHandle(4)]
public class AcademyForApprentice : IAcademyCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		string str = packet.ReadString();
		if (AcademyMgr.GetRequest(player.PlayerId, num) != null)
		{
			return 0;
		}
		if (player.PlayerCharacter.AcademyInfo.FreezesDate <= DateTime.Now)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(num);
			if (playerById != null && playerById.PlayerCharacter.AcademyInfo.IsMaster && playerById.PlayerCharacter.AcademyInfo.ApprenticeshipState < AcademyMgr.MASTER_FULL_STATE && AcademyMgr.CheckCanMaster(playerById.PlayerCharacter.FightPower))
			{
				AcademyMgr.AddRequest(new AcademyRequestInfo
				{
					SenderID = player.PlayerId,
					ReceiderID = num,
					Type = 1,
					CreateTime = DateTime.Now
				});
				GSPacketIn gSPacketIn = new GSPacketIn(141);
				gSPacketIn.WriteByte(4);
				gSPacketIn.WriteInt(player.PlayerId);
				gSPacketIn.WriteString(player.PlayerCharacter.NickName);
				gSPacketIn.WriteString(str);
				playerById.SendTCP(gSPacketIn);
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("LoginServerConnector.HandleSysMess.Msg2"));
			}
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.TakeApprentice.Frozen", AcademyMgr.CheckDate(player.PlayerCharacter.AcademyInfo.FreezesDate)));
		}
		return 0;
	}
}
