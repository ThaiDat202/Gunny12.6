using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(303, "场景用户离开")]
public class RedPackageHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		switch (packet.ReadByte())
		{
		case 1:
		{
			int id2 = packet.ReadInt();
			if (!ConsortiaMgr.ReceiveRedPackage(client.Player, id2))
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("RedPackage.Gain.Fail"));
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("RedPackage.Gain.Success"));
			}
			break;
		}
		case 2:
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			string text = packet.ReadString();
			int num3 = num * num2;
			if (num3 <= 0 || client.Player.PlayerCharacter.Money < num3)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("RedPackage.Gain.MoneyFail", num3));
				return 0;
			}
			if (client.Player.RemoveMoney(num3) > 0)
			{
				ConsortiaRedPackageInfo consortiaRedPackageInfo = new ConsortiaRedPackageInfo();
				consortiaRedPackageInfo.UserID = client.Player.PlayerCharacter.ID;
				consortiaRedPackageInfo.NickName = client.Player.PlayerCharacter.NickName;
				consortiaRedPackageInfo.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
				consortiaRedPackageInfo.Money = num;
				consortiaRedPackageInfo.Package = num2;
				consortiaRedPackageInfo.WishWord = text;
				consortiaRedPackageInfo.ReceiveData = "";
				consortiaRedPackageInfo.TimeCreate = DateTime.Now;
				ConsortiaMgr.AddRedPackage(consortiaRedPackageInfo);
				client.Player.SendMessage(LanguageMgr.GetTranslation("RedPackage.Send.Success"));
				WorldMgr.SendSysRedPackageNotice(client.Player.PlayerCharacter.ConsortiaID, LanguageMgr.GetTranslation("RedPackage.Send.SysNotice", client.Player.PlayerCharacter.NickName, num2, text));
			}
			break;
		}
		case 3:
		{
			List<ConsortiaRedPackageInfo> redPackageList = ConsortiaMgr.GetRedPackageList(client.Player.PlayerCharacter.ConsortiaID);
			GSPacketIn gSPacketIn2 = new GSPacketIn(303);
			gSPacketIn2.WriteByte(3);
			gSPacketIn2.WriteByte(0);
			gSPacketIn2.WriteInt(redPackageList.Count);
			foreach (ConsortiaRedPackageInfo item in redPackageList)
			{
				gSPacketIn2.WriteInt(item.ID);
				gSPacketIn2.WriteBoolean(ConsortiaMgr.CheckGainRedPackage(item.ID, client.Player.PlayerId));
				gSPacketIn2.WriteDateTime(item.TimeCreate);
				gSPacketIn2.WriteString(item.NickName);
				gSPacketIn2.WriteInt(item.Package);
			}
			client.Player.SendTCP(gSPacketIn2);
			break;
		}
		case 4:
		{
			int id = packet.ReadInt();
			ConsortiaRedPackageInfo redPackage = ConsortiaMgr.GetRedPackage(id);
			if (redPackage == null)
			{
				break;
			}
			Dictionary<int, ConsortiaRedPackageGainInfo> gainRedPackageInfo = ConsortiaMgr.GetGainRedPackageInfo(redPackage);
			GSPacketIn gSPacketIn = new GSPacketIn(303);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteString(redPackage.NickName);
			gSPacketIn.WriteInt(redPackage.Money);
			gSPacketIn.WriteString(redPackage.WishWord);
			gSPacketIn.WriteInt(gainRedPackageInfo.Count);
			foreach (ConsortiaRedPackageGainInfo item2 in gainRedPackageInfo.Values.OrderBy((ConsortiaRedPackageGainInfo a) => a.TimeGain))
			{
				gSPacketIn.WriteString(item2.NickName);
				gSPacketIn.WriteInt(item2.Money);
				gSPacketIn.WriteDateTime(item2.TimeGain);
			}
			client.Player.SendTCP(gSPacketIn);
			break;
		}
		}
		return 1;
	}
}
