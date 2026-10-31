using System;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Emblem.Handle;

[EmblemHandle(5)]
public class Compose : IEmblemCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int id1 = packet.ReadInt();
		int id2 = packet.ReadInt();
		int id3 = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		bool flag2 = packet.ReadBoolean();
		Sys_Users_Emblem firstEmblemInfo = player.EmblemInfos.FirstOrDefault((Sys_Users_Emblem p) => p.ID == id1 && p.isExist);
		Sys_Users_Emblem secondEmblemInfo = player.EmblemInfos.FirstOrDefault((Sys_Users_Emblem p) => p.ID == id2 && p.isExist);
		Sys_Users_Emblem thirdEmblemInfo = player.EmblemInfos.FirstOrDefault((Sys_Users_Emblem p) => p.ID == id3 && p.isExist);
		if (firstEmblemInfo == null || secondEmblemInfo == null || thirdEmblemInfo == null)
		{
			return 0;
		}
		TS_Emblem tS_Emblem = EmblemMgr.Emblems.FirstOrDefault((TS_Emblem p) => p.TemplateID == firstEmblemInfo.tempId);
		TS_Emblem tS_Emblem2 = EmblemMgr.Emblems.FirstOrDefault((TS_Emblem p) => p.TemplateID == secondEmblemInfo.tempId);
		TS_Emblem tS_Emblem3 = EmblemMgr.Emblems.FirstOrDefault((TS_Emblem p) => p.TemplateID == thirdEmblemInfo.tempId);
		if (tS_Emblem == null || tS_Emblem2 == null || tS_Emblem3 == null)
		{
			return 0;
		}
		int currentProfile = tS_Emblem.Profile;
		if (tS_Emblem2.Profile != currentProfile || tS_Emblem2.Profile != currentProfile || currentProfile > 2)
		{
			return 0;
		}
		int num = int.Parse(GameProperties.EmblemComposeRandom.Split(',')[0]);
		if (currentProfile == 2)
		{
			num = int.Parse(GameProperties.EmblemComposeRandom.Split(',')[1]);
		}
		bool isSuccess = false;
		int newId = 0;
		if (new Random().Next(1000) < num)
		{
			TS_Emblem tS_Emblem4 = EmblemMgr.Emblems.FirstOrDefault((TS_Emblem p) => p.Profile == currentProfile + 1 && p.MainType == firstEmblemInfo.mainType);
			if (tS_Emblem4 == null)
			{
				return 0;
			}
			isSuccess = true;
			newId = EmblemMgr.AddItem(player, tS_Emblem4.TemplateID);
			player.SendMessage(LanguageMgr.GetTranslation("Emblem.Compose.Success"));
			firstEmblemInfo.isExist = false;
			secondEmblemInfo.isExist = false;
			thirdEmblemInfo.isExist = false;
			EmblemMgr.Out.SendUpdateEmblem(player, firstEmblemInfo);
			EmblemMgr.Out.SendUpdateEmblem(player, secondEmblemInfo);
			EmblemMgr.Out.SendUpdateEmblem(player, thirdEmblemInfo);
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("Emblem.Compose.Failed"));
			if (flag2)
			{
				player.RemoveMoney(1000);
			}
			else
			{
				firstEmblemInfo.isExist = false;
				secondEmblemInfo.isExist = false;
				thirdEmblemInfo.isExist = false;
				EmblemMgr.Out.SendUpdateEmblem(player, firstEmblemInfo);
				EmblemMgr.Out.SendUpdateEmblem(player, secondEmblemInfo);
				EmblemMgr.Out.SendUpdateEmblem(player, thirdEmblemInfo);
			}
		}
		EmblemMgr.Out.SendComposeSuccess(player, isSuccess, newId);
		return 1;
	}
}
