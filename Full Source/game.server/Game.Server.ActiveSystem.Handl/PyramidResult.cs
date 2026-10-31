using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(3)]
public class PyramidResult : IActiveSystemCommandHadler
{
	public static Random random = new Random();

	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		PyramidInfo pyramid = Player.Actives.Pyramid;
		if (pyramid == null)
		{
			Player.Actives.LoadPyramid();
			pyramid = Player.Actives.Pyramid;
		}
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		if (pyramid.currentFreeCount < Player.Actives.PyramidConfig.freeCount)
		{
			pyramid.currentFreeCount++;
		}
		else
		{
			int turnCardPrice = Player.Actives.PyramidConfig.turnCardPrice;
			if (!Player.MoneyDirect(turnCardPrice))
			{
				return false;
			}
		}
		bool flag = false;
		bool flag2 = false;
		string text = "";
		bool flag3 = true;
		if (num < 8)
		{
			List<ItemInfo> pyramidAward = ActiveSystemMgr.GetPyramidAward(num);
			int index = random.Next(pyramidAward.Count);
			ItemInfo itemInfo = pyramidAward[index];
			int templateID = itemInfo.TemplateID;
			flag = templateID == 201083;
			flag2 = templateID == 201082;
			text = LanguageMgr.GetTranslation("ActiveSystemHandler.Msg11", itemInfo.Template.Name, itemInfo.Count);
			if (flag2)
			{
				text = LanguageMgr.GetTranslation("ActiveSystemHandler.Msg12");
				pyramid.currentLayer++;
				if (pyramid.currentLayer > pyramid.maxLayer)
				{
					pyramid.maxLayer++;
				}
				flag3 = false;
			}
			pyramid.totalPoint += 10;
			switch (templateID)
			{
			case 201079:
				pyramid.turnPoint += 10;
				text = LanguageMgr.GetTranslation("ActiveSystemHandler.Msg13");
				flag3 = false;
				break;
			case 201080:
				pyramid.turnPoint += 30;
				text = LanguageMgr.GetTranslation("ActiveSystemHandler.Msg14");
				flag3 = false;
				break;
			case 201081:
				pyramid.turnPoint += 50;
				text = LanguageMgr.GetTranslation("ActiveSystemHandler.Msg15");
				flag3 = false;
				break;
			case 201077:
				pyramid.pointRatio += 5;
				text = LanguageMgr.GetTranslation("ActiveSystemHandler.Msg16");
				flag3 = false;
				break;
			case 201078:
				pyramid.pointRatio += 10;
				text = LanguageMgr.GetTranslation("ActiveSystemHandler.Msg17");
				flag3 = false;
				break;
			}
			if (flag3)
			{
				Player.AddTemplate(itemInfo, LanguageMgr.GetTranslation("ActiveSystemHandler.Msg18"));
			}
			string text2 = $"{num}-{templateID}-{num2}";
			if (pyramid.LayerItems == "")
			{
				pyramid.LayerItems = text2;
			}
			else
			{
				PyramidInfo pyramidInfo = pyramid;
				pyramidInfo.LayerItems = pyramidInfo.LayerItems + "|" + text2;
			}
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(templateID);
			gSPacketIn.WriteInt(num2);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteBoolean(flag2);
			gSPacketIn.WriteInt(pyramid.currentLayer);
			gSPacketIn.WriteInt(pyramid.maxLayer);
			gSPacketIn.WriteInt(pyramid.totalPoint);
			gSPacketIn.WriteInt(pyramid.turnPoint);
			gSPacketIn.WriteInt(pyramid.pointRatio);
			gSPacketIn.WriteInt(pyramid.currentFreeCount);
			Player.SendTCP(gSPacketIn);
		}
		else
		{
			int num3 = random.Next(49, 501);
			pyramid.turnPoint += num3;
			pyramid.turnPoint += pyramid.turnPoint * pyramid.pointRatio / 100;
			text = LanguageMgr.GetTranslation("ActiveSystemHandler.Msg19", num3);
			pyramid.isPyramidStart = false;
			pyramid.currentLayer = 1;
			pyramid.currentReviveCount = 0;
			pyramid.totalPoint += pyramid.turnPoint;
			pyramid.turnPoint = 0;
			pyramid.pointRatio = 0;
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteBoolean(pyramid.isPyramidStart);
			gSPacketIn.WriteInt(pyramid.totalPoint);
			gSPacketIn.WriteInt(pyramid.turnPoint);
			gSPacketIn.WriteInt(pyramid.pointRatio);
			gSPacketIn.WriteInt(pyramid.currentLayer);
			Player.SendTCP(gSPacketIn);
		}
		Player.SendMessage(text);
		return true;
	}
}
