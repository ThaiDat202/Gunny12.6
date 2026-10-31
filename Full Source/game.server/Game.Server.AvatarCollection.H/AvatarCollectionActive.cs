using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.AvatarCollection.Handle;

[AvatarCollectionHandleAttbute(3)]
public class AvatarCollectionActive : IAvatarCollectionCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		int num4 = 1;
		if (GameProperties.VERSION >= 8300)
		{
			num4 = packet.ReadInt();
		}
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		ClothGroupTemplateInfo clothGroupTemplateInfo = AvatarColectionMgr.FindClothGroupTemplateInfo(num, num2, num3);
		if (clothGroupTemplateInfo == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("AvatarColection.Msg2"));
			return false;
		}
		int cost = clothGroupTemplateInfo.Cost;
		if (Player.PlayerCharacter.Gold < cost)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("AvatarColection.GoldNotEnough"));
			return false;
		}
		List<int> list = new List<int>();
		list.Add(num2);
		if (!string.IsNullOrEmpty(clothGroupTemplateInfo.OtherTemplateID))
		{
			string[] array = clothGroupTemplateInfo.OtherTemplateID.Split('|');
			foreach (string s in array)
			{
				list.Add(int.Parse(s));
			}
		}
		if (!Player.AvatarBag.ActiveAvatarColection(num, list, num3, num4))
		{
			Player.SendMessage(LanguageMgr.GetTranslation("AvatarColection.Msg1"));
		}
		else
		{
			UserAvatarColectionInfo avatar = Player.AvatarBag.GetAvatar(num);
			GSPacketIn gSPacketIn = new GSPacketIn(402);
			if (avatar != null)
			{
				if (avatar.FullActive())
				{
					Player.EquipBag.UpdatePlayerProperties();
					Player.SendMessage(LanguageMgr.GetTranslation("AvatarColection.ActiveFullProp"));
				}
				else if (avatar.HaftActive() && !avatar.isValidate)
				{
					avatar.endTime = DateTime.Now;
					avatar.endTime = avatar.endTime.AddDays(7.0);
					Player.EquipBag.UpdatePlayerProperties();
					gSPacketIn.WriteByte(4);
					gSPacketIn.WriteInt(num);
					gSPacketIn.WriteInt(num3);
					if (GameProperties.VERSION >= 8300)
					{
						gSPacketIn.WriteInt(num4);
					}
					gSPacketIn.WriteDateTime(avatar.endTime);
					Player.SendTCP(gSPacketIn);
					Player.SendMessage(LanguageMgr.GetTranslation("AvatarColection.ActiveProp"));
				}
			}
			Player.RemoveGold(cost);
			gSPacketIn = new GSPacketIn(402);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteInt(num2);
			gSPacketIn.WriteInt(num3);
			if (GameProperties.VERSION >= 8300)
			{
				gSPacketIn.WriteInt(num4);
			}
			Player.SendTCP(gSPacketIn);
		}
		return true;
	}
}
