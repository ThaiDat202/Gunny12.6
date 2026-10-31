using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(21)]
public class ConsortiaLevelUp : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		byte b = packet.ReadByte();
		string msg = "";
		string text = "";
		byte val = 0;
		bool val2 = false;
		ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(Player.PlayerCharacter.ConsortiaID);
		switch (b)
		{
		case 1:
		{
			msg = "ConsortiaUpGradeHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness3 = new ConsortiaBussiness())
			{
				ConsortiaInfo consortiaSingle = consortiaBussiness3.GetConsortiaSingle(Player.PlayerCharacter.ConsortiaID);
				if (consortiaSingle == null)
				{
					msg = "ConsortiaUpGradeHandler.NoConsortia";
				}
				else
				{
					ConsortiaLevelInfo consortiaLevelInfo = ConsortiaExtraMgr.FindConsortiaLevelInfo(consortiaSingle.Level + 1);
					if (consortiaLevelInfo == null)
					{
						msg = "ConsortiaUpGradeHandler.NoUpGrade";
					}
					else if (consortiaLevelInfo.NeedGold > Player.PlayerCharacter.Gold)
					{
						msg = "ConsortiaUpGradeHandler.NoGold";
					}
					else
					{
						using ConsortiaBussiness consortiaBussiness4 = new ConsortiaBussiness();
						if (consortiaBussiness4.UpGradeConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
						{
							consortiaSingle.Level++;
							Player.RemoveGold(consortiaLevelInfo.NeedGold);
							GameServer.Instance.LoginServer.SendConsortiaUpGrade(consortiaSingle);
							msg = "ConsortiaUpGradeHandler.Success";
							val2 = true;
							val = (byte)consortiaSingle.Level;
						}
					}
				}
				if (consortiaSingle.Level >= 5)
				{
					text = LanguageMgr.GetTranslation("ConsortiaUpGradeHandler.Notice", consortiaSingle.ConsortiaName, consortiaSingle.Level);
				}
			}
			break;
		}
		case 2:
		{
			msg = "ConsortiaStoreUpGradeHandler.Failed";
			if (consortiaInfo == null)
			{
				msg = "ConsortiaStoreUpGradeHandler.NoConsortia";
				break;
			}
			using (ConsortiaBussiness consortiaBussiness6 = new ConsortiaBussiness())
			{
				if (consortiaBussiness6.UpGradeStoreConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
				{
					consortiaInfo.StoreLevel++;
					GameServer.Instance.LoginServer.SendConsortiaStoreUpGrade(consortiaInfo);
					msg = "ConsortiaStoreUpGradeHandler.Success";
					val2 = true;
					val = (byte)consortiaInfo.StoreLevel;
				}
			}
			break;
		}
		case 3:
			msg = "ConsortiaShopUpGradeHandler.Failed";
			if (consortiaInfo == null)
			{
				msg = "ConsortiaShopUpGradeHandler.NoConsortia";
			}
			else
			{
				using ConsortiaBussiness consortiaBussiness2 = new ConsortiaBussiness();
				if (consortiaBussiness2.UpGradeShopConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
				{
					consortiaInfo.ShopLevel++;
					GameServer.Instance.LoginServer.SendConsortiaShopUpGrade(consortiaInfo);
					msg = "ConsortiaShopUpGradeHandler.Success";
					val2 = true;
					val = (byte)consortiaInfo.ShopLevel;
				}
			}
			if (consortiaInfo.ShopLevel >= 2)
			{
				text = LanguageMgr.GetTranslation("ConsortiaShopUpGradeHandler.Notice", Player.PlayerCharacter.ConsortiaName, consortiaInfo.ShopLevel);
			}
			break;
		case 4:
			msg = "ConsortiaSmithUpGradeHandler.Failed";
			if (consortiaInfo == null)
			{
				msg = "ConsortiaSmithUpGradeHandler.NoConsortia";
			}
			else
			{
				using ConsortiaBussiness consortiaBussiness5 = new ConsortiaBussiness();
				if (consortiaBussiness5.UpGradeSmithConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
				{
					consortiaInfo.SmithLevel++;
					GameServer.Instance.LoginServer.SendConsortiaSmithUpGrade(consortiaInfo);
					msg = "ConsortiaSmithUpGradeHandler.Success";
					val2 = true;
					val = (byte)consortiaInfo.SmithLevel;
				}
			}
			if (consortiaInfo.SmithLevel >= 3)
			{
				text = LanguageMgr.GetTranslation("ConsortiaSmithUpGradeHandler.Notice", Player.PlayerCharacter.ConsortiaName, consortiaInfo.SmithLevel);
			}
			break;
		case 5:
			msg = "ConsortiaBufferUpGradeHandler.Failed";
			if (consortiaInfo == null)
			{
				msg = "ConsortiaUpGradeHandler.NoConsortia";
			}
			else
			{
				using ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
				if (consortiaBussiness.UpGradeSkillConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
				{
					consortiaInfo.SkillLevel++;
					GameServer.Instance.LoginServer.SendConsortiaKillUpGrade(consortiaInfo);
					msg = "ConsortiaBufferUpGradeHandler.Success";
					val2 = true;
					val = (byte)consortiaInfo.SkillLevel;
				}
			}
			if (consortiaInfo.SkillLevel >= 3)
			{
				text = LanguageMgr.GetTranslation("ConsortiaBufferUpGradeHandler.Notice", Player.PlayerCharacter.ConsortiaName, consortiaInfo.SmithLevel);
			}
			break;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(21);
		gSPacketIn.WriteByte(b);
		gSPacketIn.WriteByte(val);
		gSPacketIn.WriteBoolean(val2);
		gSPacketIn.WriteString(LanguageMgr.GetTranslation(msg));
		Player.Out.SendTCP(gSPacketIn);
		if (text != "")
		{
			GSPacketIn gSPacketIn2 = new GSPacketIn(10);
			gSPacketIn2.WriteInt(2);
			gSPacketIn2.WriteString(text);
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn2);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				if (gamePlayer != Player)
				{
					gamePlayer.Out.SendTCP(gSPacketIn2);
				}
			}
		}
		return 0;
	}
}
