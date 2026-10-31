using System;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(295, "打造系统")]
public class FineStoreSuitHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		FineStorePackageType fineStorePackageType = (FineStorePackageType)packet.ReadByte();
		FineStorePackageType fineStorePackageType2 = fineStorePackageType;
		FineStorePackageType fineStorePackageType3 = fineStorePackageType2;
		if (fineStorePackageType3 == FineStorePackageType.FORGE_SUIT)
		{
			int num = packet.ReadInt();
			int num2 = client.Player.PlayerCharacter.fineSuitExp;
			bool flag = false;
			SetsBuildTempInfo setsBuildTempInfo = SetsBuildTempMgr.FindNextSetsBuildExp(num2);
			if (setsBuildTempInfo != null && num2 < SetsBuildTempMgr.SetsBuildMax())
			{
				ItemInfo itemByTemplateID = client.Player.GetItemByTemplateID(setsBuildTempInfo.UseItemTemplate);
				if (itemByTemplateID != null && itemByTemplateID.Count >= num)
				{
					if (num == 0)
					{
						num = itemByTemplateID.Count;
					}
					int num3 = SetsBuildTempMgr.SetsBuildMax();
					int num4 = (setsBuildTempInfo.Exp - client.Player.PlayerCharacter.fineSuitExp) / 10;
					if (num4 * 10 + client.Player.PlayerCharacter.fineSuitExp < setsBuildTempInfo.Exp)
					{
						num4++;
					}
					if (num4 > itemByTemplateID.Count)
					{
						num4 = itemByTemplateID.Count;
					}
					if (num == 1)
					{
						num4 = 1;
					}
					num2 += itemByTemplateID.Template.Property2 * num4;
					if (num2 > num3)
					{
						int num5 = num2 - num3;
						num2 = num3;
						if (num5 >= itemByTemplateID.Template.Property2)
						{
							num4 = num5 / itemByTemplateID.Template.Property2;
							ItemInfo cloneItem = ItemInfo.CreateFromTemplate(itemByTemplateID.Template, num4, 105);
							client.Player.AddTemplate(cloneItem);
						}
					}
					flag = client.Player.PropBag.RemoveTemplate(setsBuildTempInfo.UseItemTemplate, num4);
				}
				else
				{
					Console.WriteLine("FineStoreSuitHandler::item not found!");
				}
			}
			GSPacketIn gSPacketIn = new GSPacketIn(295);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteInt(num2);
			client.Player.SendTCP(gSPacketIn);
			if (flag)
			{
				client.Player.PlayerCharacter.fineSuitExp = num2;
				SetsBuildTempInfo setsBuildTempInfo2 = SetsBuildTempMgr.FindNextSetsBuildExp(num2);
				if (setsBuildTempInfo2 != null && setsBuildTempInfo.Level < setsBuildTempInfo2.Level)
				{
					client.Player.EquipBag.UpdatePlayerProperties();
					client.Player.OnBuildGpgrade(setsBuildTempInfo2.Level - 1);
				}
			}
		}
		else
		{
			Console.WriteLine("FineStoreSuitHandler cmd {0}, not found!", fineStorePackageType);
		}
		return 0;
	}
}
