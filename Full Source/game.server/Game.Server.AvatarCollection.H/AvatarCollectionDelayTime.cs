using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.AvatarCollection.Handle;

[AvatarCollectionHandleAttbute(4)]
public class AvatarCollectionDelayTime : IAvatarCollectionCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int num3 = 1;
		if (GameProperties.VERSION >= 8300)
		{
			num3 = packet.ReadInt();
		}
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		ClothPropertyTemplateInfo clothPropertyTemplateInfo = AvatarColectionMgr.FindClothPropertyTemplate(num);
		if (clothPropertyTemplateInfo == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("AvatarColection.Msg3"));
			return false;
		}
		long num4 = clothPropertyTemplateInfo.Cost * num2;
		int num5 = (int)((num4 > int.MaxValue) ? int.MaxValue : num4);
		if (Player.PlayerCharacter.myHonor < num5)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("AvatarColection.Msg4"));
			return false;
		}
		if (!Player.AvatarBag.DelayAvatarColection(num, num2, num3))
		{
			Player.SendMessage(LanguageMgr.GetTranslation("AvatarColection.Msg5"));
		}
		else
		{
			Player.RemovemyHonor(num5);
			UserAvatarColectionInfo avatar = Player.AvatarBag.GetAvatar(num);
			if (avatar != null)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(402);
				gSPacketIn.WriteByte(4);
				gSPacketIn.WriteInt(num);
				gSPacketIn.WriteInt(avatar.Sex);
				if (GameProperties.VERSION >= 8300)
				{
					gSPacketIn.WriteInt(num3);
				}
				gSPacketIn.WriteDateTime(avatar.endTime);
				Player.SendTCP(gSPacketIn);
				Player.SendMessage(LanguageMgr.GetTranslation("AvatarColection.SuccessDelay"));
			}
		}
		return true;
	}
}
