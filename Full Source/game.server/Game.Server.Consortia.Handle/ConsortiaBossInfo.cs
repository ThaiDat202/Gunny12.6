using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(30)]
public class ConsortiaBossInfo : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		int num = packet.ReadInt();
		int consortiaID = Player.PlayerCharacter.ConsortiaID;
		ConsortiaInfo consortiaInfo = ConsortiaBossMgr.GetConsortiaById(consortiaID);
		if (consortiaInfo == null || consortiaInfo.LastOpenBoss.Date < DateTime.Now.Date)
		{
			using ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
			consortiaInfo = consortiaBussiness.GetConsortiaSingle(consortiaID);
		}
		if (consortiaInfo == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Consortia.Msg8"));
			return 0;
		}
		if (consortiaInfo.Level < 3)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Consortia.Msg9"));
			return 0;
		}
		int num2 = ConsortiaMgr.FindConsortiaBossMaxLevel(0, consortiaInfo);
		ConsortiaBossConfigInfo consortiaBossConfigInfo = ConsortiaMgr.FindConsortiaBossConfig(num2);
		if (consortiaBossConfigInfo == null || (consortiaBossConfigInfo.Level != num && num > 1))
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Consortia.Msg10"));
			return 0;
		}
		switch (b)
		{
		case 0:
		{
			if (consortiaInfo.Riches < consortiaBossConfigInfo.CostRich)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("Consortia.Msg11"));
				return 0;
			}
			consortiaInfo.bossState = 1;
			consortiaInfo.endTime = DateTime.Now.AddMinutes(20.0);
			consortiaInfo.LastOpenBoss = DateTime.Now;
			using (ConsortiaBussiness consortiaBussiness3 = new ConsortiaBussiness())
			{
				int riches2 = consortiaBossConfigInfo.CostRich;
				if (consortiaBussiness3.ConsortiaRichRemove(Player.PlayerCharacter.ConsortiaID, ref riches2))
				{
					consortiaInfo.Riches = riches2;
				}
			}
			ConsortiaBossMgr.CreateBoss(consortiaInfo, consortiaBossConfigInfo.NpcID);
			break;
		}
		case 2:
		{
			if (consortiaInfo.Riches < consortiaBossConfigInfo.ProlongRich)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("Consortia.Msg12"));
				return 0;
			}
			using (ConsortiaBussiness consortiaBussiness2 = new ConsortiaBussiness())
			{
				int riches = consortiaBossConfigInfo.ProlongRich;
				if (consortiaBussiness2.ConsortiaRichRemove(Player.PlayerCharacter.ConsortiaID, ref riches))
				{
					consortiaInfo.Riches = riches;
				}
			}
			ConsortiaBossMgr.ExtendAvailable(consortiaID, consortiaInfo.Riches);
			break;
		}
		default:
			if (ConsortiaBossMgr.GetConsortiaExit(consortiaID))
			{
				ConsortiaBossMgr.reload(consortiaID);
				break;
			}
			consortiaInfo.bossState = 0;
			consortiaInfo.endTime = DateTime.Now;
			consortiaInfo.extendAvailableNum = 3;
			consortiaInfo.callBossLevel = num2;
			ConsortiaBossMgr.AddConsortia(consortiaInfo);
			break;
		}
		return 0;
	}
}
