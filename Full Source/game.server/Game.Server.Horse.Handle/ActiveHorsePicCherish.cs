using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Horse.Handle;

[HorseHandleAttbute(7)]
public class ActiveHorsePicCherish : IHorseCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int place = packet.ReadInt();
		ItemInfo itemAt = Player.GetItemAt(eBageType.PropBag, place);
		if (itemAt != null)
		{
			MountDrawTemplateInfo mountDrawTemplateInfo = MountMgr.FindMountDrawByTemplate(itemAt.TemplateID);
			if (mountDrawTemplateInfo == null)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("PicCherishInfo.Msg1"));
			}
			else
			{
				Player.PlayerCharacter.MountsType = mountDrawTemplateInfo.ID;
				Player.Horse.Info.curUseHorse = mountDrawTemplateInfo.ID;
				Player.Horse.ActiveHorsePicCherish(mountDrawTemplateInfo.ID);
				GSPacketIn gSPacketIn = new GSPacketIn(260);
				gSPacketIn.WriteByte(7);
				gSPacketIn.WriteInt(mountDrawTemplateInfo.ID);
				gSPacketIn.WriteInt(1);
				if (GameProperties.VERSION >= 7600)
				{
					gSPacketIn.WriteDateTime(DateTime.Now);
					gSPacketIn.WriteInt(0);
				}
				if (GameProperties.VERSION >= 9800)
				{
					gSPacketIn.WriteInt(0);
				}
				Player.SendTCP(gSPacketIn);
				Player.Out.SendchangeHorse(mountDrawTemplateInfo.ID);
				Player.SendMessage(LanguageMgr.GetTranslation("PicCherishInfo.Msg2", mountDrawTemplateInfo.Name));
				Player.RemoveTemplate(itemAt.TemplateID, 1);
			}
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("PicCherishInfo.Msg1"));
		}
		return true;
	}
}
