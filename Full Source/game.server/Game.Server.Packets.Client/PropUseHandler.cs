using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(66, "场景用户离开")]
public class PropUseHandler : IPacketHandler
{
	private Random rand = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eBageType bageType = (eBageType)packet.ReadInt();
		int slot = packet.ReadInt();
		PlayerInventory inventory = client.Player.GetInventory(bageType);
		if (inventory != null)
		{
			ItemInfo itemAt = inventory.GetItemAt(slot);
			if (itemAt != null)
			{
				int num = packet.ReadInt();
				for (int i = 0; i < num; i++)
				{
					switch (packet.ReadInt())
					{
					case 201316:
					{
						UserChickActiveInfo chickActiveData = client.Player.Actives.GetChickActiveData();
						if (chickActiveData.IsKeyOpened == 0 && itemAt != null && itemAt.Count >= 1)
						{
							inventory.RemoveCountFromStack(itemAt, 1);
							chickActiveData.Active((client.Player.PlayerCharacter.Grade <= 15) ? 1 : 2);
							client.Player.Actives.SaveChickActiveData(chickActiveData);
							client.Player.SendMessage(LanguageMgr.GetTranslation("PropUseHandler.ChickActivation.Success"));
						}
						else
						{
							client.Player.SendMessage(LanguageMgr.GetTranslation("PropUseHandler.ChickActivation.Fail"));
						}
						break;
					}
					case 11963:
					{
						int num2 = rand.Next(1, 1000);
						client.Player.AddGiftToken(num2);
						client.Player.SendMessage(LanguageMgr.GetTranslation("PropUseHandler.GiftToken", num2));
						inventory.RemoveCountFromStack(itemAt, 1);
						break;
					}
					}
				}
				int num3 = packet.ReadInt();
				bool flag = packet.ReadBoolean();
			}
		}
		return 0;
	}
}
