using System;
using System.Collections.Generic;
using System.ComponentModel;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
  
  [PacketHandler(131, "新版秘境")]
  public class LabyrinthHandler : IPacketHandler
  {
    public int HandlePacket(GameClient client, GSPacketIn packet)
    {
      LabyrinthPackageType labyrinthPackageType = (LabyrinthPackageType) packet.ReadInt();
      int sType = packet.ReadInt();
      UserLabyrinthInfo laby = client.Player.Labyrinth ?? client.Player.LoadLabyrinth(sType);
      int id = client.Player.PlayerCharacter.ID;
      int num1;
      switch (labyrinthPackageType)
      {
        case LabyrinthPackageType.DOUBLE_REWARD:
          bool flag1 = packet.ReadBoolean();
          if (client.Player.PropBag.GetItemByTemplateID(0, 11916) == null)
          {
            num1 = 0;
            goto label_39;
          }
          else
          {
            if ((!flag1 || laby.isDoubleAward ? 0 : (client.Player.RemoveTemplate(11916, 1) ? 1 : 0)) != 0)
              laby.isDoubleAward = flag1;
            client.Player.Out.SendLabyrinthUpdataInfo(id, laby);
            break;
          }
        case LabyrinthPackageType.REQUEST_UPDATE:
          if (laby.isValidDate())
          {
            laby.completeChallenge = true;
            laby.accumulateExp = 0;
            laby.isInGame = false;
            laby.currentFloor = 1;
            laby.tryAgainComplete = true;
            laby.LastDate = DateTime.Now;
            laby.ProcessAward = client.Player.InitProcessAward();
          }
          client.Player.CalculatorClearnOutLabyrinth();
          client.Player.Out.SendLabyrinthUpdataInfo(id, laby);
          break;
        case LabyrinthPackageType.START_SWEEP:
          int warriorFamRaidDdtPrice = GameProperties.WarriorFamRaidDDTPrice;
          if (client.Player.PlayerCharacter.DDTMoney < warriorFamRaidDdtPrice)
          {
            client.Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.Msg1"));
            client.Player.Actives.StopCleantOutLabyrinth();
            break;
          }
          laby.isCleanOut = true;
          client.Player.RemoveGiftToken(warriorFamRaidDdtPrice);
          client.Player.Actives.CleantOutLabyrinth();
          break;
        case LabyrinthPackageType.QUICK_SWEEP:
          if (!laby.isCleanOut)
          {
            client.Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.Msg2"));
            num1 = 0;
            goto label_39;
          }
          else
          {
            int num2 = Math.Abs(laby.currentRemainTime / 60);
            int num3 = GameProperties.WarriorFamRaidPricePerMin * num2;
            if (client.Player.Extra.UseKingBless(7))
            {
              client.Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.Msg3", (object) num3));
              client.Player.Actives.SpeededUpCleantOutLabyrinth();
              break;
            }
            if (client.Player.MoneyDirect(num3))
            {
              client.Player.Actives.SpeededUpCleantOutLabyrinth();
              break;
            }
            break;
          }
        case LabyrinthPackageType.STOP_SWEEP:
          client.Player.Actives.StopCleantOutLabyrinth();
          break;
        case LabyrinthPackageType.RESET_LABYRINTH:
          if (laby.tryAgainComplete)
          {
            laby.currentFloor = 1;
            laby.accumulateExp = 0;
            laby.tryAgainComplete = false;
            laby.ProcessAward = client.Player.InitProcessAward();
            client.Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.Msg4"));
            client.Player.Out.SendLabyrinthUpdataInfo(id, laby);
            break;
          }
          client.Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.Msg5"));
          break;
        case LabyrinthPackageType.TRY_AGAIN:
          bool flag2 = packet.ReadBoolean();
          packet.ReadBoolean();
          if (flag2)
          {
            int num2 = client.Player.LabyrinthTryAgainMoney();
            if (client.Player.MoneyDirect(num2))
            {
              laby.completeChallenge = true;
              laby.isInGame = false;
              client.Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.Msg6"));
              client.Player.Out.SendLabyrinthUpdataInfo(id, laby);
              break;
            }
            break;
          }
          client.Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.Msg4"));
          break;
        case LabyrinthPackageType.REQUEST_RANK:
          int val1 = packet.ReadInt();
          int val2 = packet.ReadInt();
          List<WarriorFamRankInfo> warriorFamRankInfoList = new List<WarriorFamRankInfo>();
          string str = "千秋岁引弹弹堂";
          int val3 = 1001;
          int val4 = 0;
          using (PlayerBussiness playerBussiness = new PlayerBussiness())
          {
            GSPacketIn pkg = new GSPacketIn((short) 131);
            pkg.WriteByte((byte) 10);
            pkg.WriteInt(val1);
            pkg.WriteInt(val2);
            WarriorFamRankInfo[] allWarriorFamRank = playerBussiness.GetAllWarriorFamRank();
            int length = allWarriorFamRank.Length;
            for (int index = 0; index < length; ++index)
              warriorFamRankInfoList.Add(allWarriorFamRank[index]);
            int count = warriorFamRankInfoList.Count;
            pkg.WriteInt(count);
            for (int index = 0; index < count; ++index)
            {
              pkg.WriteInt(warriorFamRankInfoList[index].PlayerRank);
              pkg.WriteInt(val3);
              pkg.WriteString(str);
              pkg.WriteInt(warriorFamRankInfoList[index].UserID);
              pkg.WriteString(warriorFamRankInfoList[index].PlayerName);
              pkg.WriteInt(warriorFamRankInfoList[index].FamLevel);
              pkg.WriteInt(val4);
            }
            client.SendTCP(pkg);
            break;
          }
        default:
          Console.WriteLine("新版秘境类型: " + labyrinthPackageType.ToString());
          break;
      }
      num1 = 0;
label_39:
      return num1;
    }

    static LabyrinthHandler() => LicenseManager.Validate(typeof (LabyrinthHandler));
  }
}
