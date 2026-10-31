using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Game.Base;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(529, "刻印系统")]
public class MarkHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		ThreadSafeRandom threadSafeRandom = new ThreadSafeRandom();
		switch (b)
		{
		case 22:
		{
			int num6 = packet.ReadInt();
			bool flag = packet.ReadBoolean();
			bool flag2 = packet.ReadBoolean();
			bool flag3 = false;
			List<int[]> list = GameProperties.EngraveVaults();
			if (num6 >= list.Count)
			{
				return 1;
			}
			if (num6 == 1 && flag2)
			{
				return 1;
			}
			int[] array = list[num6];
			if (!flag)
			{
				num6 = 0;
				array = list[num6];
				if (client.Player.Extra.Info.FreeTimeMark <= DateTime.Now && client.Player.Extra.Info.FreeCountMark > 0)
				{
					flag3 = true;
					client.Player.Extra.Info.FreeTimeMark = DateTime.Now.AddMinutes(GameProperties.EngraveVaultsTime);
					client.Player.Extra.Info.FreeCountMark--;
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mark.NoEnouchFree.Msg"));
				}
			}
			else if ((flag2 && client.Player.RemoveGiftToken(array[1]) > 0) || (!flag2 && client.Player.RemoveMoney(array[1]) > 0))
			{
				flag3 = true;
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mark.NoEnouchMoney.Msg"));
			}
			if (!flag3)
			{
				break;
			}
			List<MarkChipItemInfo> list2 = new List<MarkChipItemInfo>();
			List<MarkOpenBoxInfo> list3 = new List<MarkOpenBoxInfo>();
			for (int k = 0; k < array[2]; k++)
			{
				list3 = new List<MarkOpenBoxInfo>();
				ItemBoxMgr.CreateMarkBox(num6, list3);
				if (list3.Count > 0)
				{
					MarkChipItemInfo markChipItemInfo = client.Player.Marks.CreateMarkItem(list3[0].TemplateID, list3[0].IsBind, list3[0].BornLevel);
					if (markChipItemInfo != null)
					{
						client.Player.Marks.AddChipToBag(markChipItemInfo);
						list2.Add(markChipItemInfo);
					}
				}
			}
			ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(12202);
			client.Player.AddSoulCrystal(itemTemplateInfo2.Property3 * array[0]);
			GSPacketIn gSPacketIn2 = new GSPacketIn(529);
			gSPacketIn2.WriteByte(22);
			gSPacketIn2.WriteInt(num6);
			gSPacketIn2.WriteBoolean(val: false);
			gSPacketIn2.WriteInt(list2.Count);
			foreach (MarkChipItemInfo item in list2)
			{
				gSPacketIn2.WriteInt(item.TemplateID);
				gSPacketIn2.WriteInt(1);
				gSPacketIn2.WriteInt(item.ItemID);
			}
			client.SendTCP(gSPacketIn2);
			break;
		}
		case 9:
		{
			int num2 = packet.ReadInt();
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				int itemID3 = packet.ReadInt();
				int num4 = 0;
				MarkChipItemInfo chip3 = client.Player.Marks.GetChip(itemID3);
				if (chip3 != null)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(chip3.TemplateID);
					EngraveDebrisInfo singleEngraveDebris2 = EngraveMgr.GetSingleEngraveDebris(chip3.TemplateID);
					num4 += itemTemplateInfo.ReclaimValue;
					num4 += chip3.bornLv * GameProperties.EngraveSaleStarConfig;
					int num5 = 0;
					EngraveTemperConfigInfo engraveTemperConfigInfo = null;
					for (int j = 0; j < chip3.hLv; j++)
					{
						engraveTemperConfigInfo = EngraveMgr.GetTemperConfig(j, singleEngraveDebris2.Character);
						if (engraveTemperConfigInfo != null)
						{
							num5 += engraveTemperConfigInfo.Expend;
						}
					}
					num4 += num5 * GameProperties.EngraveSaleTemperConsumeConfig / 100;
					client.Player.Marks.RemoveChipFromBag(chip3);
				}
				num3 += num4;
			}
			if (num3 > 0)
			{
				client.Player.AddSoulCrystal(num3);
			}
			break;
		}
		case 23:
		{
			_ = client.Player.Extra.Info.FreeTimeMark;
			if (false)
			{
				client.Player.Extra.Info.FreeTimeMark = DateTime.Now;
			}
			GSPacketIn gSPacketIn3 = new GSPacketIn(529);
			gSPacketIn3.WriteByte(23);
			gSPacketIn3.WriteLong(Marshal.GetTimeStampMilliseconds(DateTime.Now));
			gSPacketIn3.WriteInt(client.Player.Extra.Info.FreeCountMark);
			client.SendTCP(gSPacketIn3);
			break;
		}
		case 7:
		{
			int num7 = packet.ReadInt();
			int num8 = packet.ReadInt();
			client.Player.MarkRefineItemTemp = null;
			MarkChipItemInfo chip4 = client.Player.Marks.GetChip(num7);
			if (chip4 == null)
			{
				break;
			}
			for (int l = 0; l < chip4.SecondProps.Count; l++)
			{
				if (chip4.SecondProps[l].type == num8)
				{
					ItemTemplateInfo itemTemplateInfo3 = ItemMgr.FindItemTemplate(chip4.TemplateID);
					EngraveRefineryConfigInfo refineryConfig = EngraveMgr.GetRefineryConfig(itemTemplateInfo3.Quality, chip4.bornLv + chip4.hammerLv);
					DebrisPropertyConfigInfo debrisProperty = EngraveMgr.GetDebrisProperty(chip4.TemplateID, chip4.SecondProps[l].type);
					if (refineryConfig != null && debrisProperty != null && refineryConfig.Expend <= client.Player.Extra.Info.SoulCrystal && refineryConfig.NeedMaterial <= client.Player.PropBag.GetItemCount(refineryConfig.Material))
					{
						client.Player.RemoveSoulCrystal(refineryConfig.Expend);
						client.Player.PropBag.RemoveTemplate(refineryConfig.Material, refineryConfig.NeedMaterial);
						client.Player.MarkRefineItemTemp = chip4.Clone();
						List<MarkProDataInfo> secondProps = client.Player.MarkRefineItemTemp.SecondProps;
						secondProps[l].value = threadSafeRandom.NextLow(debrisProperty.MinValue, debrisProperty.MaxValue);
						secondProps[l].hummerCount++;
						client.Player.MarkRefineItemTemp.SetSecondProps(secondProps);
						GSPacketIn gSPacketIn4 = new GSPacketIn(529);
						gSPacketIn4.WriteByte(7);
						gSPacketIn4.WriteInt(num7);
						gSPacketIn4.WriteInt(num8);
						gSPacketIn4.WriteInt(secondProps[l].type);
						gSPacketIn4.WriteInt(secondProps[l].value);
						gSPacketIn4.WriteInt(secondProps[l].hummerCount);
						client.SendTCP(gSPacketIn4);
					}
					else
					{
						client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mark.NotEnouchRefineResource.Msg"));
					}
					break;
				}
			}
			break;
		}
		case 8:
		{
			int num9 = packet.ReadInt();
			GSPacketIn gSPacketIn5 = new GSPacketIn(529);
			gSPacketIn5.WriteByte(8);
			if (num9 == 0 && client.Player.MarkRefineItemTemp != null)
			{
				MarkChipItemInfo chip5 = client.Player.Marks.GetChip(client.Player.MarkRefineItemTemp.ItemID);
				if (chip5 != null)
				{
					chip5.SetSecondProps(client.Player.MarkRefineItemTemp.SecondProps);
					client.Player.Marks.OnChipUpdate(chip5, sendToClient: true);
					client.Player.EquipBag.UpdatePlayerProperties();
					gSPacketIn5.WriteBoolean(val: true);
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mark.NotFoundRefineCopy.Msg"));
					gSPacketIn5.WriteBoolean(val: false);
				}
			}
			else
			{
				gSPacketIn5.WriteBoolean(val: false);
			}
			client.Player.MarkRefineItemTemp = null;
			client.Player.SendTCP(gSPacketIn5);
			break;
		}
		case 4:
		{
			int itemID2 = packet.ReadInt();
			int num = packet.ReadInt();
			if (num <= 0 || num > 10)
			{
				return 0;
			}
			MarkChipItemInfo chip2 = client.Player.Marks.GetChip(itemID2);
			if (chip2 == null)
			{
				break;
			}
			EngraveDebrisInfo singleEngraveDebris = EngraveMgr.GetSingleEngraveDebris(chip2.TemplateID);
			if (singleEngraveDebris == null)
			{
				break;
			}
			EngraveTemperConfigInfo temperConfig = EngraveMgr.GetTemperConfig(chip2.hLv, singleEngraveDebris.Character);
			if (chip2.hLv > EngraveMgr.GetTemperTopLevel(singleEngraveDebris.Character))
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mark.TemperMaxLevel.Msg"));
				return 0;
			}
			if (client.Player.RemoveSoulCrystal(temperConfig.Expend) > 0)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(529);
				gSPacketIn.WriteByte(4);
				if (threadSafeRandom.Next(1000) <= temperConfig.SuccessRate)
				{
					chip2.hLv++;
					MarkProDataInfo mainProp = chip2.MainProp;
					mainProp.attachValue = EngraveMgr.GetAttributeAdd(singleEngraveDebris, chip2.hLv, (eMarkProType)chip2.MainProp.type);
					mainProp.hummerCount++;
					chip2.SetMainProp(mainProp);
					client.Player.Marks.CreateSecondProp(chip2);
					client.Player.Marks.OnChipUpdate(chip2, sendToClient: true);
					gSPacketIn.WriteBoolean(val: true);
					client.Player.EquipBag.UpdatePlayerProperties();
				}
				else
				{
					gSPacketIn.WriteBoolean(val: false);
				}
				gSPacketIn.WriteInt(chip2.ItemID);
				gSPacketIn.WriteInt(num);
				client.Player.SendTCP(gSPacketIn);
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mark.NotEnouchSoulCrystal.Msg"));
			}
			break;
		}
		case 17:
		{
			int itemID = packet.ReadInt();
			int equipType = packet.ReadInt();
			MarkChipItemInfo chip = client.Player.Marks.GetChip(itemID);
			if (chip != null)
			{
				if (chip.Type != client.Player.Marks.TYPE_EQUIP)
				{
					if (!client.Player.Marks.EquipChip(chip, equipType))
					{
						client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mark.EquipFail.Msg"));
					}
				}
				else if (!client.Player.Marks.UnEquipChip(chip))
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mark.UnEquipFail.Msg"));
				}
				client.Player.EquipBag.UpdatePlayerProperties();
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Mark.EquipNotFound.Msg"));
			}
			break;
		}
		}
		return 1;
	}
}
