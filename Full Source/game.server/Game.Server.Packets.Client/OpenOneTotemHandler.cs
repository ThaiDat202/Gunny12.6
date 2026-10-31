using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.DragonBoat;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(136, "场景用户离开")]
public class OpenOneTotemHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.PlayerCharacter.Grade < 20)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("OpenOneTotemHandler.Msg1"));
			return 0;
		}
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 1;
		}
		int num = client.Player.PlayerCharacter.totemId + 1;
		if (num <= 10000)
		{
			num = 10001;
		}
		if (num > TotemMgr.MaxTotem())
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("OpenOneTotemHandler.Maxlevel"));
			client.Player.Out.SendPlayerRefreshTotem(client.Player.PlayerCharacter);
			return 1;
		}
		TotemInfo totemInfo = TotemMgr.FindTotemInfo(num);
		if (totemInfo == null)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("OpenOneTotemHandler.ErrorData"));
			client.Player.Out.SendPlayerRefreshTotem(client.Player.PlayerCharacter);
			return 1;
		}
		int num2 = totemInfo.ConsumeExp;
		int consumeHonor = totemInfo.ConsumeHonor;
		int num3 = DragonBoatMgr.ReduceToemUpGrace();
		if (num3 > 0)
		{
			num2 -= num2 * num3 / 100;
		}
		if (client.Player.PlayerCharacter.myHonor >= consumeHonor)
		{
			if (client.Player.MoneyDirect(num2))
			{
				client.Player.AddTotem(num);
				client.Player.RemovemyHonor(consumeHonor);
				client.Player.Out.SendPlayerRefreshTotem(client.Player.PlayerCharacter);
				client.Player.EquipBag.UpdatePlayerProperties();
				client.Player.OnUserToemGemstoneEvent(1);
			}
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("OpenOneTotemHandler.Msg2"));
		}
		return 0;
	}
}
