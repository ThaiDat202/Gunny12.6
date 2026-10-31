using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(77)]
public class DDPLayExchange : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = GameProperties.ExchangeFold * num;
		if (num2 > Player.Actives.Info.DDPlayPoint)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("DDPLayExchange.NotEnoughtPoint"));
		}
		else
		{
			Player.Actives.Info.DDPlayPoint -= num2;
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(201310);
			if (itemTemplateInfo != null)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
				itemInfo.IsBinds = true;
				itemInfo.Count = num;
				Player.AddTemplate(itemInfo);
				Player.SendMessage(LanguageMgr.GetTranslation("DDPLayExchange.Success"));
			}
			GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
			gSPacketIn.WriteByte(75);
			gSPacketIn.WriteInt(Player.Actives.Info.DDPlayPoint);
			Player.Out.SendTCP(gSPacketIn);
		}
		return true;
	}
}
