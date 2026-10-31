using Bussiness;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(26)]
public class SkillSocket : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		bool flag = packet.ReadBoolean();
		int id = packet.ReadInt();
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		if (num < 0)
		{
			num = 1;
		}
		ConsortiaBufferTempInfo consortiaBufferTempInfo = ConsortiaExtraMgr.FindConsortiaBuffInfo(id);
		ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(Player.PlayerCharacter.ConsortiaID);
		if (consortiaBufferTempInfo == null || consortiaInfo == null)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg1"));
			return 0;
		}
		bool flag2 = true;
		int num3 = num * consortiaBufferTempInfo.riches;
		int validMinutes = 1440 * num;
		if (consortiaBufferTempInfo.type == 2)
		{
			switch (num2)
			{
			case 1:
				if (Player.PlayerCharacter.UseOffer < num3)
				{
					flag2 = false;
				}
				break;
			case 2:
				num3 = num * consortiaBufferTempInfo.metal;
				if (Player.PlayerCharacter.DDTMoney < num3)
				{
					flag2 = false;
				}
				break;
			}
		}
		else if (consortiaInfo.Riches < num3)
		{
			flag2 = false;
		}
		if (flag2)
		{
			if (consortiaBufferTempInfo.level <= consortiaInfo.Level)
			{
				switch (consortiaBufferTempInfo.group)
				{
				case 1:
					BufferList.CreatePayBuffer(101, consortiaBufferTempInfo.value, validMinutes, id)?.Start(Player);
					break;
				case 3:
					BufferList.CreatePayBuffer(103, consortiaBufferTempInfo.value, validMinutes, id)?.Start(Player);
					break;
				case 6:
					BufferList.CreatePayBuffer(106, consortiaBufferTempInfo.value, validMinutes, id)?.Start(Player);
					break;
				case 11:
					BufferList.CreatePayBuffer(111, consortiaBufferTempInfo.value, validMinutes, id)?.Start(Player);
					break;
				case 12:
					BufferList.CreatePayBuffer(112, consortiaBufferTempInfo.value, validMinutes, id)?.Start(Player);
					break;
				case 8:
					Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg2"));
					return 0;
				default:
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						ConsortiaUserInfo[] allMemberByConsortia = playerBussiness.GetAllMemberByConsortia(Player.PlayerCharacter.ConsortiaID);
						AbstractBuffer abstractBuffer = null;
						switch (consortiaBufferTempInfo.group)
						{
						case 2:
							abstractBuffer = BufferList.CreatePayBuffer(102, consortiaBufferTempInfo.value, validMinutes, id);
							break;
						case 4:
							abstractBuffer = BufferList.CreatePayBuffer(104, consortiaBufferTempInfo.value, validMinutes, id);
							break;
						case 5:
							abstractBuffer = BufferList.CreatePayBuffer(105, consortiaBufferTempInfo.value, validMinutes, id);
							break;
						case 7:
							abstractBuffer = BufferList.CreatePayBuffer(107, consortiaBufferTempInfo.value, validMinutes, id);
							break;
						case 9:
							abstractBuffer = BufferList.CreatePayBuffer(109, consortiaBufferTempInfo.value, validMinutes, id);
							break;
						case 10:
							abstractBuffer = BufferList.CreatePayBuffer(110, consortiaBufferTempInfo.value, validMinutes, id);
							break;
						}
						ConsortiaUserInfo[] array = allMemberByConsortia;
						foreach (ConsortiaUserInfo consortiaUserInfo in array)
						{
							GamePlayer playerById = WorldMgr.GetPlayerById(consortiaUserInfo.UserID);
							if (playerById != null)
							{
								abstractBuffer?.Start(playerById);
								if (playerById != Player)
								{
									playerById.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg3"));
								}
							}
						}
						if (abstractBuffer != null)
						{
							ConsortiaBufferInfo consortiaBufferInfo = playerBussiness.GetUserConsortiaBufferSingle(consortiaBufferTempInfo.id);
							if (consortiaBufferInfo == null)
							{
								consortiaBufferInfo = new ConsortiaBufferInfo();
								consortiaBufferInfo.ConsortiaID = Player.PlayerCharacter.ConsortiaID;
								consortiaBufferInfo.IsOpen = true;
								consortiaBufferInfo.BufferID = consortiaBufferTempInfo.id;
								consortiaBufferInfo.Type = abstractBuffer.Info.Type;
								consortiaBufferInfo.Value = abstractBuffer.Info.Value;
								consortiaBufferInfo.ValidDate = abstractBuffer.Info.ValidDate;
								consortiaBufferInfo.BeginDate = abstractBuffer.Info.BeginDate;
							}
							else
							{
								consortiaBufferInfo.BufferID = consortiaBufferTempInfo.id;
								consortiaBufferInfo.Value = abstractBuffer.Info.Value;
								consortiaBufferInfo.ValidDate += abstractBuffer.Info.ValidDate;
							}
							playerBussiness.SaveConsortiaBuffer(consortiaBufferInfo);
						}
					}
					break;
				}
				}
				if (num2 == 1)
				{
					if (consortiaBufferTempInfo.type == 1)
					{
						using ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
						int riches = num3;
						consortiaBussiness.ConsortiaRichRemove(Player.PlayerCharacter.ConsortiaID, ref riches);
					}
					else
					{
						Player.RemoveUserOffer(num3);
					}
				}
				else
				{
					Player.RemoveGiftToken(num3);
				}
				Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg4"));
			}
			else
			{
				Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg5"));
			}
		}
		else
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg6"));
		}
		return 0;
	}
}
