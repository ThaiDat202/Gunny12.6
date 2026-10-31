using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(96, "荣誉提升")]
public class HonorUpHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		bool flag = packet.ReadBoolean();
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		int num2 = num;
		int num3 = num2;
		if (num3 != 1 && num3 == 2)
		{
			int iD = client.Player.PlayerCharacter.MaxBuyHonor + 1;
			TotemHonorTemplateInfo totemHonorTemplateInfo = TotemHonorMgr.FindTotemHonorTemplateInfo(iD);
			if (totemHonorTemplateInfo == null)
			{
				return 0;
			}
			int needMoney = totemHonorTemplateInfo.NeedMoney;
			int addHonor = totemHonorTemplateInfo.AddHonor;
			if (client.Player.MoneyDirect(needMoney))
			{
				client.Player.AddHonor(addHonor);
				client.Player.AddMaxHonor(1);
			}
		}
		client.Player.Out.SendUpdateUpCount(client.Player.PlayerCharacter);
		return 0;
	}
}
