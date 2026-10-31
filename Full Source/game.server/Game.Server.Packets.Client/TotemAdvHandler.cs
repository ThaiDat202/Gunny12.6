using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(616, "图腾升阶")]
public class TotemAdvHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		Dictionary<int, int> totemUpgrade = client.Player.PlayerCharacter.GetTotemUpgrade();
		int num2 = 0;
		if (totemUpgrade.ContainsKey(num))
		{
			num2 = totemUpgrade[num];
		}
		TS_UpgradeTemplate totemUpGradeInfo = ItemMgr.GetTotemUpGradeInfo(num, num2 + 1);
		if (totemUpGradeInfo == null)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("已达到最高等级"));
			return 1;
		}
		int itemCount = client.Player.PropBag.GetItemCount(totemUpGradeInfo.ItemTempId2);
		if (totemUpGradeInfo.Param1 <= client.Player.PlayerCharacter.myHonor && totemUpGradeInfo.Param2 <= itemCount)
		{
			client.Player.RemovemyHonor(totemUpGradeInfo.Param1);
			client.Player.PropBag.RemoveTemplate(totemUpGradeInfo.ItemTempId2, totemUpGradeInfo.Param2);
			if (!totemUpgrade.ContainsKey(num))
			{
				totemUpgrade.Add(num, 0);
			}
			totemUpgrade[num]++;
			client.Player.PlayerCharacter.SetTotemUpgrade(totemUpgrade);
			client.Out.SendTotemUpgradeUpdate(num, totemUpgrade[num]);
			client.Player.EquipBag.UpdatePlayerProperties();
			client.Player.SendMessage(LanguageMgr.GetTranslation("TotemUpgradeSystem.Success"));
			return 0;
		}
		client.Player.SendMessage(LanguageMgr.GetTranslation("TotemUpgradeSystem.MateryNotEnouch"));
		return 1;
	}
}
