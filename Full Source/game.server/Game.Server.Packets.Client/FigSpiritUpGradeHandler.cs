using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(209, "场景用户离开")]
public class FigSpiritUpGradeHandler : IPacketHandler
{
	private static int[] exps = FightSpiritTemplateMgr.Exps();

	private static int golddenLv = GameProperties.FightSpiritMaxLevel;

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.PlayerCharacter.Grade < 30)
		{
			return 0;
		}
		int iD = client.Player.PlayerCharacter.ID;
		packet.ReadByte();
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		int templateId = packet.ReadInt();
		int figSpiritId = packet.ReadInt();
		int place = packet.ReadInt();
		int place2 = packet.ReadInt();
		int num4 = packet.ReadInt();
		ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
		int itemCount = client.Player.PropBag.GetItemCount(templateId);
		UserGemStone gemStone = client.Player.GetGemStone(place);
		if (gemStone == null)
		{
			client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, isUp: false, isMaxLevel: true, isFall: true, 0, 0);
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FigSpiritUpGradeHandler.Msg"));
			return 0;
		}
		string[] array = gemStone.FigSpiritIdValue.Split('|');
		bool flag = false;
		bool flag2 = IsMaxLv(array);
		bool isFall = true;
		int num5 = 1;
		int dir = 0;
		FigSpiritUpInfo[] array2 = new FigSpiritUpInfo[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			FigSpiritUpInfo figSpiritUpInfo = new FigSpiritUpInfo();
			figSpiritUpInfo.level = Convert.ToInt32(array[i].Split(',')[0]);
			figSpiritUpInfo.exp = Convert.ToInt32(array[i].Split(',')[1]);
			figSpiritUpInfo.place = Convert.ToInt32(array[i].Split(',')[2]);
			array2[i] = figSpiritUpInfo;
		}
		if (itemCount <= 0 || itemByTemplateID == null)
		{
			client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, flag2, isFall, 0, dir);
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FigSpiritUpGradeHandler.Msg"));
			return 0;
		}
		if (!itemByTemplateID.isGemStone())
		{
			client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, flag2, isFall, 0, dir);
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FigSpiritUpGradeHandler.Msg"));
			return 0;
		}
		if (!flag2)
		{
			FigSpiritUpInfo curGem = GetCurGem(array2, place2);
			if (curGem == null)
			{
				client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, flag2, isFall, 0, dir);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FigSpiritUpGradeHandler.WrongPlace"));
				return 0;
			}
			int num6 = 0;
			int num7 = num;
			int num8 = num7;
			if (num8 == 1)
			{
				int num9 = getNeedExp(curGem.exp, curGem.level) / itemByTemplateID.Template.Property2;
				if (itemCount < num9)
				{
					num9 = itemCount;
				}
				num6 = itemByTemplateID.Template.Property2 * num9;
				client.Player.PropBag.RemoveTemplate(templateId, num9);
			}
			else
			{
				num6 = itemByTemplateID.Template.Property2;
				client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
			}
			if (curGem.level < golddenLv)
			{
				curGem.exp += num6;
				flag = canUpLv(curGem.exp, curGem.level);
				if (flag)
				{
					curGem.level++;
					curGem.exp = 0;
				}
			}
			array2 = UpdateGem(array2, curGem, flag);
			if (flag)
			{
				isFall = false;
				client.Player.EquipBag.UpdatePlayerProperties();
				dir = 1;
			}
			string text = array2[0].level + "," + array2[0].exp + "," + array2[0].place;
			for (int j = 1; j < array2.Length; j++)
			{
				text = text + "|" + array2[j].level + "," + array2[j].exp + "," + array2[j].place;
			}
			gemStone.FigSpiritId = figSpiritId;
			gemStone.FigSpiritIdValue = text;
			client.Player.UpdateGemStone(place, gemStone);
			client.Player.OnUserToemGemstoneEvent(2);
			client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, flag2, isFall, num5, dir);
			return 0;
		}
		client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, flag2, isFall, 0, dir);
		return 0;
	}

	private int getNeedExp(int _curExp, int _curLv)
	{
		int num = exps[_curLv + 1];
		int num2 = _curExp + exps[_curLv];
		return num - num2;
	}

	private bool IsMaxLv(string[] SpiritIdValue)
	{
		return SpiritIdValue[0].Split(',')[0] == golddenLv.ToString() && SpiritIdValue[1].Split(',')[0] == golddenLv.ToString() && SpiritIdValue[2].Split(',')[0] == golddenLv.ToString();
	}

	private bool canUpLv(int exp, int curLv)
	{
		for (int i = 1; i < exps.Length; i++)
		{
			if (exp >= exps[i] - exps[i - 1] && curLv == i - 1)
			{
				return true;
			}
		}
		return false;
	}

	private FigSpiritUpInfo GetCurGem(FigSpiritUpInfo[] gems, int place)
	{
		foreach (FigSpiritUpInfo figSpiritUpInfo in gems)
		{
			if (figSpiritUpInfo.place == place)
			{
				return figSpiritUpInfo;
			}
		}
		return null;
	}

	private FigSpiritUpInfo[] UpdateGem(FigSpiritUpInfo[] gems, FigSpiritUpInfo gem, bool isUp)
	{
		for (int i = 0; i < gems.Length; i++)
		{
			if (gems[i].place == gem.place)
			{
				gems[i] = gem;
			}
		}
		if (isUp)
		{
			IEnumerable<FigSpiritUpInfo> enumerable = from p in gems
				orderby p.level, p.exp
				select p;
			FigSpiritUpInfo[] array = new FigSpiritUpInfo[gems.Length];
			int num = 0;
			foreach (FigSpiritUpInfo item in enumerable)
			{
				switch (num)
				{
				case 0:
					item.place = 2;
					break;
				case 1:
					item.place = 1;
					break;
				case 2:
					item.place = 0;
					break;
				}
				array[num] = item;
				num++;
			}
			return array;
		}
		return gems;
	}
}
