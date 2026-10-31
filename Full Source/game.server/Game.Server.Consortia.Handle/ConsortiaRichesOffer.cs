using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(6)]
public class ConsortiaRichesOffer : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		int num = packet.ReadInt();
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 1;
		}
		if (num < 1 || Player.PlayerCharacter.Money < num)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaRichesOfferHandler.NoMoney"));
			return 1;
		}
		int riches = num / GameProperties.ConsortiaRichesPlus;
		if (riches <= 0)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("捐献功能已关闭,财富请查询任务", GameProperties.ConsortiaRichesPlus));
			return 1;
		}
		bool val = false;
		string translateId = "ConsortiaRichesOfferHandler.Failed";
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			if (consortiaBussiness.ConsortiaRichAdd(Player.PlayerCharacter.ConsortiaID, ref riches, 5, Player.PlayerCharacter.NickName))
			{
				val = true;
				Player.PlayerCharacter.RichesOffer += riches;
				Player.PlayerCharacter.RichesRob += riches;
				Player.RemoveMoney(num);
				translateId = "ConsortiaRichesOfferHandler.Successed";
				GameServer.Instance.LoginServer.SendConsortiaRichesOffer(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, riches);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteBoolean(val);
		gSPacketIn.WriteString(LanguageMgr.GetTranslation(translateId));
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
