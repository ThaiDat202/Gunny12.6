using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Helpers;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Server;
using Game.Server.Achievements;
using Game.Server.Buffer;
using Game.Server.DragonBoat;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.LotteryTicket;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Base.Packets;

[PacketLib(1)]
public class AbstractPacketLib : IPacketLib
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    protected readonly GameClient m_gameClient;

    public AbstractPacketLib(GameClient client)
    {
        m_gameClient = client;
    }

    public GSPacketIn SendFarmPoultryLevel(int PlayerID, UserFarmInfo farm, int nextLevel)
    {
        GSPacketIn result = new GSPacketIn(81, PlayerID);
        GSPacketIn gSPacketIn = new GSPacketIn(81);
        gSPacketIn.WriteByte(21);
        gSPacketIn.WriteInt(farm.FarmID);
        gSPacketIn.WriteInt(farm.TreeLevel);
        gSPacketIn.WriteInt(farm.MonsterExp);
        gSPacketIn.WriteInt(nextLevel);
        gSPacketIn.WriteByte((byte)farm.PoultryState);
        gSPacketIn.WriteDateTime(farm.CountDownTime);
        SendTCP(gSPacketIn);
        return result;
    }

    public GSPacketIn SendEnterFarm(PlayerInfo Player, UserFarmInfo farm, UserFieldInfo[] fields)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteInt(farm.FarmID);
        gSPacketIn.WriteBoolean(farm.isFarmHelper);
        gSPacketIn.WriteInt(farm.isAutoId);
        gSPacketIn.WriteDateTime(farm.AutoPayTime);
        gSPacketIn.WriteInt(farm.AutoValidDate);
        gSPacketIn.WriteInt(farm.GainFieldId);
        gSPacketIn.WriteInt(farm.KillCropId);
        gSPacketIn.WriteInt(fields.Length);
        foreach (UserFieldInfo userFieldInfo in fields)
        {
            gSPacketIn.WriteInt(userFieldInfo.FieldID);
            gSPacketIn.WriteInt(userFieldInfo.SeedID);
            gSPacketIn.WriteDateTime(userFieldInfo.PayTime);
            gSPacketIn.WriteDateTime(userFieldInfo.PlantTime);
            gSPacketIn.WriteInt(userFieldInfo.GainCount);
            gSPacketIn.WriteInt(userFieldInfo.FieldValidDate);
            gSPacketIn.WriteInt(userFieldInfo.AccelerateTime);
        }
        if (farm.FarmID == Player.ID)
        {
            gSPacketIn.WriteInt(GameProperties.FastGrowNeedMoney);
            gSPacketIn.WriteString(farm.PayFieldMoney);
            gSPacketIn.WriteString(farm.PayAutoMoney);
            gSPacketIn.WriteDateTime(farm.AutoPayTime);
            gSPacketIn.WriteInt(farm.AutoValidDate);
            gSPacketIn.WriteInt(Player.VIPLevel);
            gSPacketIn.WriteInt(farm.buyExpRemainNum);
        }
        else
        {
            gSPacketIn.WriteBoolean(farm.isArrange);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendFarmLandInfo(PlayerFarm farm)
    {
        UserFieldInfo[] fields = farm.GetFields();
        GSPacketIn gSPacketIn = new GSPacketIn(81, farm.CurrentFarm.FarmID);
        gSPacketIn.WriteByte(17);
        gSPacketIn.WriteInt(fields.Length);
        UserFieldInfo[] array = fields;
        foreach (UserFieldInfo userFieldInfo in array)
        {
            gSPacketIn.WriteInt(userFieldInfo.FieldID);
            gSPacketIn.WriteInt(userFieldInfo.SeedID);
            gSPacketIn.WriteDateTime(userFieldInfo.PlantTime);
            gSPacketIn.WriteInt(userFieldInfo.GainCount);
            gSPacketIn.WriteInt(userFieldInfo.AccelerateTime);
        }
        gSPacketIn.WriteBoolean(farm.midAutumnFlag);
        gSPacketIn.WriteBoolean(farm.CurrentFarm.isFarmHelper);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendHelperSwitchField(PlayerInfo Player, UserFarmInfo farm)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
        gSPacketIn.WriteByte(9);
        gSPacketIn.WriteBoolean(farm.isFarmHelper);
        gSPacketIn.WriteInt(farm.isAutoId);
        gSPacketIn.WriteDateTime(farm.AutoPayTime);
        gSPacketIn.WriteInt(farm.AutoValidDate);
        gSPacketIn.WriteInt(farm.GainFieldId);
        gSPacketIn.WriteInt(farm.KillCropId);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendSeeding(PlayerInfo Player, UserFieldInfo field)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
        gSPacketIn.WriteByte(2);
        gSPacketIn.WriteInt(field.FieldID);
        gSPacketIn.WriteInt(field.SeedID);
        gSPacketIn.WriteDateTime(field.PlantTime);
        gSPacketIn.WriteDateTime(field.PayTime);
        gSPacketIn.WriteInt(field.GainCount);
        gSPacketIn.WriteInt(field.FieldValidDate);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPayFields(GamePlayer Player, List<int> fieldIds)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(81, Player.PlayerCharacter.ID);
        gSPacketIn.WriteByte(6);
        gSPacketIn.WriteInt(Player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(fieldIds.Count);
        foreach (int fieldId in fieldIds)
        {
            UserFieldInfo fieldAt = Player.Farm.GetFieldAt(fieldId);
            gSPacketIn.WriteInt(fieldAt.FieldID);
            gSPacketIn.WriteInt(fieldAt.SeedID);
            gSPacketIn.WriteDateTime(fieldAt.PayTime);
            gSPacketIn.WriteDateTime(fieldAt.PlantTime);
            gSPacketIn.WriteInt(fieldAt.GainCount);
            gSPacketIn.WriteInt(fieldAt.FieldValidDate);
            gSPacketIn.WriteInt(fieldAt.AccelerateTime);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendKillCropField(PlayerInfo Player, UserFieldInfo field)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
        gSPacketIn.WriteByte(7);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteInt(field.FieldID);
        gSPacketIn.WriteInt(field.SeedID);
        gSPacketIn.WriteInt(field.AccelerateTime);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendCompose(GamePlayer Player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(81, Player.PlayerCharacter.ID);
        gSPacketIn.WriteByte(5);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SenddoMature(PlayerFarm farm)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(81, farm.Player.PlayerCharacter.ID);
        gSPacketIn.WriteByte(3);
        gSPacketIn.WriteInt(farm.CurrentFields.Length);
        UserFieldInfo[] currentFields = farm.CurrentFields;
        foreach (UserFieldInfo userFieldInfo in currentFields)
        {
            if (userFieldInfo != null)
            {
                gSPacketIn.WriteBoolean(val: true);
                gSPacketIn.WriteInt(userFieldInfo.FieldID);
                gSPacketIn.WriteInt(userFieldInfo.GainCount);
                gSPacketIn.WriteInt(userFieldInfo.AccelerateTime);
            }
            else
            {
                gSPacketIn.WriteBoolean(val: false);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendtoGather(PlayerInfo Player, UserFieldInfo field)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
        gSPacketIn.WriteByte(4);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteInt(field.FieldID);
        gSPacketIn.WriteInt(field.SeedID);
        gSPacketIn.WriteDateTime(field.PlantTime);
        gSPacketIn.WriteInt(field.GainCount);
        gSPacketIn.WriteInt(field.AccelerateTime);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendPetGuildOptionChange()
    {
        GSPacketIn gSPacketIn = new GSPacketIn(158);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteInt(8);
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendNewPacket(GamePlayer Player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(102, Player.PlayerCharacter.ID);
        gSPacketIn.WriteByte(0);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendOpenWorldBoss(int pX, int pY)
    {
        BaseWorldBossRoom worldBossRoom = RoomMgr.WorldBossRoom;
        GSPacketIn gSPacketIn = new GSPacketIn(102);
        gSPacketIn.WriteByte(0);
        gSPacketIn.WriteString(worldBossRoom.bossResourceId);
        gSPacketIn.WriteInt(worldBossRoom.currentPVE);
        gSPacketIn.WriteString("Thần thú");
        gSPacketIn.WriteString(worldBossRoom.name);
        gSPacketIn.WriteLong(worldBossRoom.MaxBlood);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt((pX == 0) ? worldBossRoom.playerDefaultPosX : pX);
        gSPacketIn.WriteInt((pY == 0) ? worldBossRoom.playerDefaultPosY : pY);
        gSPacketIn.WriteDateTime(worldBossRoom.begin_time);
        gSPacketIn.WriteDateTime(worldBossRoom.end_time);
        gSPacketIn.WriteInt(worldBossRoom.fight_time);
        gSPacketIn.WriteBoolean(worldBossRoom.fightOver);
        gSPacketIn.WriteBoolean(worldBossRoom.roomClose);
        gSPacketIn.WriteInt(worldBossRoom.ticketID);
        gSPacketIn.WriteInt(worldBossRoom.need_ticket_count);
        gSPacketIn.WriteInt(worldBossRoom.timeCD);
        gSPacketIn.WriteInt(worldBossRoom.reviveMoney);
        gSPacketIn.WriteInt(worldBossRoom.reFightMoney);
        gSPacketIn.WriteInt(worldBossRoom.addInjureBuffMoney);
        gSPacketIn.WriteInt(worldBossRoom.addInjureValue);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteBoolean(val: false);
        SendTCP(gSPacketIn);
    }

    public void SendTotemUpgradeInfo(Dictionary<int, int> infos)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(619);
        gSPacketIn.WriteInt(infos.Count);
        foreach (KeyValuePair<int, int> info in infos)
        {
            gSPacketIn.WriteInt(info.Key);
            gSPacketIn.WriteInt(info.Value);
        }
        SendTCP(gSPacketIn);
    }

    public void SendTotemUpgradeUpdate(int templateId, int level)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(617);
        gSPacketIn.WriteInt(templateId);
        gSPacketIn.WriteInt(level);
        SendTCP(gSPacketIn);
    }

    public void SendPolarRegion(bool isOpen)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(305);
        gSPacketIn.WriteByte(0);
        gSPacketIn.WriteBoolean(isOpen);
        SendTCP(gSPacketIn);
    }

    public void SendPvePowerBuffResponse(UserPvePowerBuffInfo pveBuffInfo)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(84);
        gSPacketIn.WriteInt(21);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(pveBuffInfo.refreshCount);
        gSPacketIn.WriteDateTime(pveBuffInfo.refreshDate);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffCount);
        gSPacketIn.WriteDateTime(pveBuffInfo.getBuffDate);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffIndex);
        gSPacketIn.WriteInt(pveBuffInfo.ListIndexBuff.Count);
        foreach (UserPvePowerBuffTempInfo item in pveBuffInfo.ListIndexBuff)
        {
            gSPacketIn.WriteInt(item.ID);
            gSPacketIn.WriteInt(item.Attack);
            gSPacketIn.WriteInt(item.Defence);
            gSPacketIn.WriteInt(item.Agility);
            gSPacketIn.WriteInt(item.Luck);
            gSPacketIn.WriteInt(item.Damage);
            gSPacketIn.WriteInt(item.Guard);
            gSPacketIn.WriteInt(item.HP);
            gSPacketIn.WriteInt(item.MagicDefence);
            gSPacketIn.WriteInt(item.MagicAttack);
            gSPacketIn.WriteInt(item.Grade);
            gSPacketIn.WriteInt(item.FightPower);
            gSPacketIn.WriteString(item.NickName);
            gSPacketIn.WriteInt(item.Sex);
            gSPacketIn.WriteString(item.Style);
            gSPacketIn.WriteString(item.Colors);
            gSPacketIn.WriteString(item.Skin);
            gSPacketIn.WriteInt(item.Hide);
        }
        gSPacketIn.WriteInt(pveBuffInfo.getBuffAtk);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffDef);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffAgl);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffLuck);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffDmg);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffAr);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffHp);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffMAtk);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffMDef);
        SendTCP(gSPacketIn);
    }

    public void SendPvePowerBuffRefresh(UserPvePowerBuffInfo pveBuffInfo)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(84);
        gSPacketIn.WriteInt(21);
        gSPacketIn.WriteInt(2);
        gSPacketIn.WriteInt(pveBuffInfo.refreshCount);
        gSPacketIn.WriteDateTime(pveBuffInfo.refreshDate);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffIndex);
        gSPacketIn.WriteInt(pveBuffInfo.ListIndexBuff.Count);
        foreach (UserPvePowerBuffTempInfo item in pveBuffInfo.ListIndexBuff)
        {
            gSPacketIn.WriteInt(item.ID);
            gSPacketIn.WriteInt(item.Attack);
            gSPacketIn.WriteInt(item.Defence);
            gSPacketIn.WriteInt(item.Agility);
            gSPacketIn.WriteInt(item.Luck);
            gSPacketIn.WriteInt(item.Damage);
            gSPacketIn.WriteInt(item.Guard);
            gSPacketIn.WriteInt(item.HP);
            gSPacketIn.WriteInt(item.MagicDefence);
            gSPacketIn.WriteInt(item.MagicAttack);
            gSPacketIn.WriteInt(item.Grade);
            gSPacketIn.WriteInt(item.FightPower);
            gSPacketIn.WriteString(item.NickName);
            gSPacketIn.WriteInt(item.Sex);
            gSPacketIn.WriteString(item.Style);
            gSPacketIn.WriteString(item.Colors);
            gSPacketIn.WriteString(item.Skin);
            gSPacketIn.WriteInt(item.Hide);
        }
        SendTCP(gSPacketIn);
    }

    public void SendPvePowerGetBuff(UserPvePowerBuffInfo pveBuffInfo)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(84);
        gSPacketIn.WriteInt(21);
        gSPacketIn.WriteInt(3);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffCount);
        gSPacketIn.WriteDateTime(pveBuffInfo.getBuffDate);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffIndex);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffAtk);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffDef);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffAgl);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffLuck);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffDmg);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffAr);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffHp);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffMAtk);
        gSPacketIn.WriteInt(pveBuffInfo.getBuffMDef);
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendUpdateRoomBorden(List<ItemInfo> list, bool isAdd, int currentItemId)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(390);
        gSPacketIn.WriteByte(20);
        gSPacketIn.WriteBoolean(isAdd);
        gSPacketIn.WriteInt(list.Count);
        foreach (ItemInfo item in list)
        {
            gSPacketIn.WriteInt(item.ItemID);
            gSPacketIn.WriteInt(item.TemplateID);
            gSPacketIn.WriteInt(item.ValidDate);
            gSPacketIn.WriteDateTime(item.BeginDate);
            gSPacketIn.WriteBoolean(item.IsUsed);
        }
        gSPacketIn.WriteInt(currentItemId);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendMinesShopInfo()
    {
        GSPacketIn gSPacketIn = new GSPacketIn(414);
        gSPacketIn.WriteByte(8);
        gSPacketIn.WriteBoolean(val: true);
        List<MineManageItemInfo> listDrop = MinesMgr.GetListDrop();
        gSPacketIn.WriteInt(listDrop.Count);
        foreach (MineManageItemInfo item in listDrop)
        {
            gSPacketIn.WriteInt(item.TemplateID);
            gSPacketIn.WriteInt(item.Price);
            gSPacketIn.WriteInt(item.CurrentCount);
        }
        List<MineManageExchangeInfo> listExchange = MinesMgr.GetListExchange();
        gSPacketIn.WriteInt(listExchange.Count);
        foreach (MineManageExchangeInfo item2 in listExchange)
        {
            gSPacketIn.WriteInt(item2.ID);
            gSPacketIn.WriteInt(item2.activityType);
            gSPacketIn.WriteInt(item2.Quality);
            gSPacketIn.WriteInt(item2.TemplateID);
            gSPacketIn.WriteString(item2.Name);
            gSPacketIn.WriteInt(item2.Price);
            gSPacketIn.WriteInt(item2.strengthLevel);
            gSPacketIn.WriteInt(item2.attackCompose);
            gSPacketIn.WriteInt(item2.defendCompose);
            gSPacketIn.WriteInt(item2.agilityCompose);
            gSPacketIn.WriteInt(item2.luckCompose);
            gSPacketIn.WriteBoolean(item2.isBind);
            gSPacketIn.WriteInt(item2.validDate);
            gSPacketIn.WriteInt(item2.Count);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteBoolean(val: false);
            gSPacketIn.WriteBoolean(val: false);
            gSPacketIn.WriteBoolean(val: false);
            gSPacketIn.WriteInt(item2.Position);
            gSPacketIn.WriteInt(item2.LimitCount);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendMinesLevelUpArm(UserMinesInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(414);
        gSPacketIn.WriteByte(6);
        gSPacketIn.WriteInt(info.HeadExp);
        gSPacketIn.WriteInt(info.ClothExp);
        gSPacketIn.WriteInt(info.WeaponExp);
        gSPacketIn.WriteInt(info.ShieldExp);
        SendTCP(gSPacketIn);
    }

    public void SendMinesLevelUpTool(UserMinesInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(414);
        gSPacketIn.WriteByte(3);
        gSPacketIn.WriteInt(info.ToolExp);
        gSPacketIn.WriteInt(0);
        SendTCP(gSPacketIn);
    }

    public void SendOpenMines(UserMinesInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(414);
        gSPacketIn.WriteByte(2);
        gSPacketIn.WriteInt(info.ToolExp);
        gSPacketIn.WriteInt(info.HeadExp);
        gSPacketIn.WriteInt(info.ClothExp);
        gSPacketIn.WriteInt(info.WeaponExp);
        gSPacketIn.WriteInt(info.ShieldExp);
        SendTCP(gSPacketIn);
    }

    public void SendOpenOrCloseStock(bool isOpen)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(411);
        gSPacketIn.WriteByte(8);
        gSPacketIn.WriteBoolean(isOpen);
        SendTCP(gSPacketIn);
    }

    public void SendOpenOrCloseChristmas(int lastPacks, bool isOpen)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(145);
        gSPacketIn.WriteByte(16);
        gSPacketIn.WriteBoolean(isOpen);
        if (isOpen)
        {
            DateTime date = DateTime.Parse(GameProperties.ChristmasBeginDate);
            DateTime date2 = DateTime.Parse(GameProperties.ChristmasEndDate);
            gSPacketIn.WriteDateTime(date);
            gSPacketIn.WriteDateTime(date2);
            string[] array = GameProperties.ChristmasGifts.Split('|');
            gSPacketIn.WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(',');
                gSPacketIn.WriteInt(int.Parse(array2[0]));
                gSPacketIn.WriteInt(int.Parse(array2[1]));
            }
            gSPacketIn.WriteInt(lastPacks);
            gSPacketIn.WriteInt(GameProperties.ChristmasBuildSnowmanDoubleMoney);
        }
        SendTCP(gSPacketIn);
    }

    public void SendLittleGameActived()
    {
        GSPacketIn gSPacketIn = new GSPacketIn(80);
        gSPacketIn.WriteBoolean(val: true);
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendOpenVIP(PlayerInfo Player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(92, Player.ID);
        gSPacketIn.WriteByte(Player.typeVIP);
        gSPacketIn.WriteInt(Player.VIPLevel);
        gSPacketIn.WriteInt(Player.VIPExp);
        gSPacketIn.WriteDateTime(Player.VIPExpireDay);
        gSPacketIn.WriteDateTime(Player.LastVIPPackTime);
        gSPacketIn.WriteInt(Player.VIPNextLevelDaysNeeded);
        gSPacketIn.WriteBoolean(Player.CanTakeVipReward);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendSevenDoubleOpenClose(int ID, bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(148, ID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteBoolean(result);
        if (result)
        {
            gSPacketIn.WriteBoolean(val: false);
            gSPacketIn.WriteInt(3);
            gSPacketIn.WriteInt(33);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(3);
            for (int i = 0; i < 3; i++)
            {
                gSPacketIn.WriteInt(i);
                gSPacketIn.WriteInt(3000);
                gSPacketIn.WriteInt(3 * i);
                gSPacketIn.WriteInt(1);
                gSPacketIn.WriteInt(100100);
                gSPacketIn.WriteInt(10 * i);
            }
            gSPacketIn.WriteInt(11);
            gSPacketIn.WriteInt(22);
            gSPacketIn.WriteInt(33);
            gSPacketIn.WriteInt(5);
            for (int j = 0; j < 5; j++)
            {
                gSPacketIn.WriteInt(j);
            }
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(44);
            gSPacketIn.WriteString("10:20|11:10");
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendConsortiaBattleOpenClose(int ID, bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(153, ID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteBoolean(result);
        if (result)
        {
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteDateTime(DateTime.Now.AddMinutes(90.0));
            gSPacketIn.WriteBoolean(result);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendFightFootballTimeOpenClose(int ID, bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(151, ID);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendGetBoxTime(int ID, int receiebox, bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(53, ID);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteInt(receiebox);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public static IPacketLib CreatePacketLibForVersion(int rawVersion, GameClient client)
    {
        Type[] derivedClasses = ScriptMgr.GetDerivedClasses(typeof(IPacketLib));
        foreach (Type type in derivedClasses)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(PacketLibAttribute), inherit: false);
            for (int j = 0; j < customAttributes.Length; j++)
            {
                PacketLibAttribute packetLibAttribute = (PacketLibAttribute)customAttributes[j];
                if (packetLibAttribute.RawVersion != rawVersion)
                {
                    continue;
                }
                try
                {
                    return (IPacketLib)Activator.CreateInstance(type, client);
                }
                catch (Exception exception)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("error creating packetlib (" + type.FullName + ") for raw version " + rawVersion, exception);
                    }
                }
            }
        }
        return null;
    }

    public void SendTCP(GSPacketIn packet)
    {
        m_gameClient.SendTCP(packet);
    }

    public void SendWeaklessGuildProgress(PlayerInfo player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(15, player.ID);
        gSPacketIn.WriteInt(player.weaklessGuildProgress.Length);
        for (int i = 0; i < player.weaklessGuildProgress.Length; i++)
        {
            gSPacketIn.WriteByte(player.weaklessGuildProgress[i]);
        }
        SendTCP(gSPacketIn);
    }

    public void SendLoginFailed(string msg)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(1);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
    }

    //ServerManager.as -> __onLoginComplete
    public void SendLoginSuccess()
    {
        if (m_gameClient.Player == null)
        {
            return;
        }
        GSPacketIn gSPacketIn = new GSPacketIn(1, m_gameClient.Player.PlayerCharacter.ID);
        gSPacketIn.WriteByte(0);
        if (!m_gameClient.LoginMobileAceito)
        {
            gSPacketIn.WriteInt(m_gameClient.Player.ZoneId);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Attack);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Defence);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Agility);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Luck);
            gSPacketIn.WriteInt((m_gameClient.Player.PlayerCharacter.GP > int.MaxValue) ? int.MaxValue : Convert.ToInt32(m_gameClient.Player.PlayerCharacter.GP));
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Repute);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Gold);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Money);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.DDTMoney);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.medal);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Score);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Hide);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.FightPower);
            if (GameProperties.VERSION >= 11000)
            {
                gSPacketIn.WriteLong(0L);
            }
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.AcademyInfo.ApprenticeshipState);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.AcademyInfo.MasterID);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.AcademyInfo.MasterOrApprentices);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.AcademyInfo.GraduatesCount);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.AcademyInfo.HonourOfMaster);
            gSPacketIn.WriteDateTime(m_gameClient.Player.PlayerCharacter.AcademyInfo.FreezesDate);
            gSPacketIn.WriteByte(m_gameClient.Player.PlayerCharacter.typeVIP);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.VIPLevel);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.VIPExp);
            gSPacketIn.WriteDateTime(m_gameClient.Player.PlayerCharacter.VIPExpireDay);
            gSPacketIn.WriteDateTime(m_gameClient.Player.PlayerCharacter.LastDate);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.VIPNextLevelDaysNeeded);
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteBoolean(m_gameClient.Player.PlayerCharacter.CanTakeVipReward);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.OptionOnOff);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.AchievementPoint);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.Honor);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.honorId);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.OnlineTime);
            gSPacketIn.WriteBoolean(m_gameClient.Player.PlayerCharacter.Sex);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.Style + "&" + m_gameClient.Player.PlayerCharacter.Colors);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.Skin);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.ConsortiaID);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.ConsortiaName);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.badgeID);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.DutyLevel);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.DutyName);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Right);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.ChairmanName);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.ConsortiaHonor);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.ConsortiaRiches);
            gSPacketIn.WriteBoolean(m_gameClient.Player.PlayerCharacter.HasBagPassword);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.PasswordQuest1);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.PasswordQuest2);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.FailedPasswordAttemptCount);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.UserName);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Nimbus);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.PvePermission);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.FightLabPermission);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.AnswerSite);
            gSPacketIn.WriteDateTime(m_gameClient.Player.PlayerCharacter.LastSpaDate);
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.UseOffer);
            gSPacketIn.WriteInt(m_gameClient.Player.BattleData.MatchInfo.dailyScore);
            gSPacketIn.WriteInt(m_gameClient.Player.BattleData.MatchInfo.dailyWinCount);
            gSPacketIn.WriteInt(m_gameClient.Player.BattleData.MatchInfo.dailyGameCount);
            gSPacketIn.WriteBoolean(m_gameClient.Player.BattleData.MatchInfo.DailyLeagueFirst);
            gSPacketIn.WriteInt(m_gameClient.Player.BattleData.MatchInfo.DailyLeagueLastScore);
            gSPacketIn.WriteInt(m_gameClient.Player.BattleData.MatchInfo.weeklyScore);
            gSPacketIn.WriteInt(m_gameClient.Player.BattleData.MatchInfo.weeklyGameCount);
            gSPacketIn.WriteInt(m_gameClient.Player.BattleData.MatchInfo.weeklyRanking);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.spdTexpExp);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.attTexpExp);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.defTexpExp);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.hpTexpExp);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.lukTexpExp);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.magicAtkTexpExp);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.magicDefTexpExp);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.texpTaskCount);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.texpCount);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.magicTexpCount);
            gSPacketIn.WriteDateTime(m_gameClient.Player.PlayerCharacter.Texp.texpTaskDate);
            gSPacketIn.WriteBoolean(m_gameClient.Player.PlayerCharacter.isOldPlayerHasValidEquitAtLogin);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.badLuckNumber);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.luckyNum);
            gSPacketIn.WriteDateTime(m_gameClient.Player.PlayerCharacter.lastLuckyNumDate);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.lastLuckNum);
            gSPacketIn.WriteBoolean(m_gameClient.Player.PlayerCharacter.IsOldPlayer);
            if (GameProperties.VERSION >= 8100)
            {
                gSPacketIn.WriteBoolean(m_gameClient.Player.PlayerCharacter.IsOldPlayer);
            }
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.CardSoul);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.GetSoulCount);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.uesedFinishTime);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.totemId);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.necklaceExp);
            if (GameProperties.VERSION >= 10900)
            {
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.necklaceCastLevel);
            }
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.accumulativeLoginDays);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.accumulativeAwardDays);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.isFirstDivorce);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.PveEpicPermission);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.MountsType);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.PetsID);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.horseRaceCanRaceTime);
            gSPacketIn.WriteBoolean(m_gameClient.Player.PlayerCharacter.isAttest);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.ImagePath);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.SubID);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.SubName);
            gSPacketIn.WriteBoolean(m_gameClient.Player.PlayerCharacter.IsShow);
            if (GameProperties.VERSION >= 8100)
            {
                gSPacketIn.WriteBoolean(val: false);
                gSPacketIn.WriteDateTime(m_gameClient.Player.PlayerCharacter.createPlayerDate);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.vipDiscount);
                gSPacketIn.WriteDateTime(m_gameClient.Player.PlayerCharacter.vipDiscountValidity);
            }
            if (GameProperties.VERSION >= 12100)
            {
                //Cần sửa
                gSPacketIn.WriteInt(0); //_loc6_.ddtkingDiscount = _loc5_.readInt()
                gSPacketIn.WriteDateTime(DateTime.Now); //_loc6_.ddtkingDiscountValidity = _loc5_.readDate()
            }
            if (GameProperties.VERSION >= 8300)
            {
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.stive);
            }
            if (GameProperties.VERSION >= 8400)
            {
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.manualLevel);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Agility);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Armor);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Attack);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Damage);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Defense);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.HP);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Lucky);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.MagicAttack);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.MagicResistance);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Stamina);
            }
            if (GameProperties.VERSION >= 8800)
            {
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Team.teamID);
                gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.Team.teamName);
                gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.Team.teamTag);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Team.teamGrade);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Team.teamWinTime);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Team.teamTotalTime);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Team.teamDivision);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Team.teamScore);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Team.teamDuty);
                if (m_gameClient.Player.PlayerCharacter.TeamUser != null)
                {
                    gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.TeamUser.BattleScore);
                }
                else
                {
                    gSPacketIn.WriteInt(0);
                }
            }
            if (GameProperties.VERSION >= 8900)
            {
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Team.freeInvitedUsedCnt);
            }
            if (GameProperties.VERSION >= 9500)
            {
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainCritTexpExp);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainSunderTexpExp);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainCritDmgTexpExp);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainSpeedTexpExp);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainTricKillTexpExp);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainDmgTexpExp);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainArmorTexpExp);
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.SecondTexpCount);
            }
            if (GameProperties.VERSION >= 12100)
            {
                //Cần sửa
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainAvoidDmgTextExp); //_loc6_.avoidInjury = _loc5_.readInt();
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainTricDefTexpExp); //_loc6_.mustResist = _loc5_.readInt();
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainTenacTexpExp); // _loc6_.toughness = _loc5_.readInt();
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainGuardTexpExp); //_loc6_.Guard = _loc5_.readInt();
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainFireDefTexpExp); //_loc6_.fireResist = _loc5_.readInt();
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainWaterDefTexpExp); // _loc6_.waterResist = _loc5_.readInt();
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainWindDefTexpExp); //_loc6_.windResist = _loc5_.readInt();
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.TrainLandDefTexpExp); //_loc6_.soilResist = _loc5_.readInt();
                gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.ThirdTexpCount); //_loc6_.thirdTexpCount = _loc5_.readInt();
            }
            if (GameProperties.VERSION >= 10400)
            {
                gSPacketIn.WriteString("");
            }
        }
        else
        {
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.ID);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.NickName);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.ImagePath ?? "");
            gSPacketIn.WriteLong(m_gameClient.Player.PlayerCharacter.FightPower);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Attack);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Defence);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Agility);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.Luck);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.Honor);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.honorId);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.OnlineTime);
            gSPacketIn.WriteBoolean(m_gameClient.Player.PlayerCharacter.Sex);
            gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.ConsortiaID);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.ConsortiaName);
            gSPacketIn.WriteDateTime(m_gameClient.Player.PlayerCharacter.createPlayerDate);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.Style);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.Colors);
            gSPacketIn.WriteString(m_gameClient.Player.PlayerCharacter.Skin);
        }
        SendTCP(gSPacketIn);
    }

    public void SendRSAKey(byte[] m, byte[] e)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(7);
        gSPacketIn.Write(m);
        gSPacketIn.Write(e);
        SendTCP(gSPacketIn);
    }

    public void SendUpdateCountHorseRace(int count)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(282);
        gSPacketIn.WriteByte(2);
        gSPacketIn.WriteInt(count);
        SendTCP(gSPacketIn);
    }

    public void SendNotEnoughEnergyBuy(bool MustBuy)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(20);
        gSPacketIn.WriteBoolean(MustBuy);
        SendTCP(gSPacketIn);
    }

    public void SendEdictumVersion()
    {
        EdictumInfo[] allEdictumVersion = WorldMgr.GetAllEdictumVersion();
        Random random = new Random();
        if (allEdictumVersion.Length == 0)
        {
            return;
        }
        GSPacketIn gSPacketIn = new GSPacketIn(75);
        gSPacketIn.WriteInt(allEdictumVersion.Length);
        EdictumInfo[] array = allEdictumVersion;
        foreach (EdictumInfo edictumInfo in array)
        {
            if (GameProperties.AlwaysShowNoticeUpdate)
            {
                gSPacketIn.WriteInt(edictumInfo.ID + random.Next(10000));
            }
            else
            {
                gSPacketIn.WriteInt(edictumInfo.ID);
            }
        }
        SendTCP(gSPacketIn);
    }

    public void SendOpenNoviceActive(int channel, int activeId, int condition, int awardGot, DateTime startTime, DateTime endTime)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(258);
        gSPacketIn.WriteInt(channel);
        switch (channel)
        {
            case 1:
                gSPacketIn.WriteBoolean(val: false);
                break;
            case 0:
                gSPacketIn.WriteInt(activeId);
                gSPacketIn.WriteInt(condition);
                gSPacketIn.WriteInt(awardGot);
                gSPacketIn.WriteDateTime(startTime);
                gSPacketIn.WriteDateTime(endTime);
                break;
        }
        SendTCP(gSPacketIn);
    }

    public void SendUpdateAreaInfo()
    {
        AreaConfigInfo[] allAreaConfig = WorldMgr.GetAllAreaConfig();
        GSPacketIn gSPacketIn = new GSPacketIn(210);
        gSPacketIn.WriteInt(allAreaConfig.Length);
        AreaConfigInfo[] array = allAreaConfig;
        foreach (AreaConfigInfo areaConfigInfo in array)
        {
            gSPacketIn.WriteInt(areaConfigInfo.AreaID);
            gSPacketIn.WriteString(areaConfigInfo.AreaName);
        }
        SendTCP(gSPacketIn);
    }

    public void SendCheckCode()
    {
        if (m_gameClient.Player != null && m_gameClient.Player.PlayerCharacter.CheckCount >= GameProperties.CHECK_MAX_FAILED_COUNT)
        {
            if (m_gameClient.Player.PlayerCharacter.CheckError == 0)
            {
                m_gameClient.Player.PlayerCharacter.CheckCount += 10000;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(200, m_gameClient.Player.PlayerCharacter.ID, 10240);
            if (m_gameClient.Player.PlayerCharacter.CheckError < 1)
            {
                gSPacketIn.WriteByte(0);
            }
            else
            {
                gSPacketIn.WriteByte(2);
            }
            gSPacketIn.WriteBoolean(val: true);
            m_gameClient.Player.PlayerCharacter.CheckCode = CheckCode.GenerateCheckCode();
            gSPacketIn.Write(CheckCode.CreateImage(m_gameClient.Player.PlayerCharacter.CheckCode));
            SendTCP(gSPacketIn);
        }
    }

    public void SendKitoff(string msg)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(2);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
    }

    public void SendEditionError(string msg)
    {
    }

    public void SendWaitingRoom(bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(16);
        gSPacketIn.WriteByte((byte)(result ? 1u : 0u));
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendCollectInfor(int id, byte state)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(32, id);
        gSPacketIn.WriteByte(state);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public virtual GSPacketIn SendMessage(eMessageType type, string message)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(3);
        gSPacketIn.WriteInt((int)type);
        gSPacketIn.WriteString(message);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendReady()
    {
        GSPacketIn packet = new GSPacketIn(0);
        SendTCP(packet);
    }

    public void SendUpdatePrivateInfo(PlayerInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(38, info.ID);
        gSPacketIn.WriteInt(info.Money);
        gSPacketIn.WriteInt(info.medal);
        gSPacketIn.WriteInt(info.DDTMoney);
        gSPacketIn.WriteInt(info.Score);
        gSPacketIn.WriteInt(info.Gold);
        gSPacketIn.WriteInt(info.badLuckNumber);
        gSPacketIn.WriteInt(info.damageScores);
        if (GameProperties.IsOpenPetScore || GameProperties.VERSION >= 7600)
        {
            gSPacketIn.WriteInt(info.petScore);
        }
        gSPacketIn.WriteInt(info.myHonor);
        gSPacketIn.WriteInt(info.hardCurrency);
        if (GameProperties.VERSION >= 8400)
        {
            gSPacketIn.WriteInt(info.jampsCurrency);
        }
        if (GameProperties.VERSION >= 12100)
        {
            //Cần sửa
            gSPacketIn.WriteInt(0); //_self.jampsBattleCount = param1.pkg.readInt();
        }
        SendTCP(gSPacketIn);
    }

    public void SendLeagueNotice(int id, int restCount, int maxCount, byte type)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(42, id);
        gSPacketIn.WriteByte(type);
        if (type == 1)
        {
            gSPacketIn.WriteInt(restCount);
            gSPacketIn.WriteInt(maxCount);
        }
        else
        {
            gSPacketIn.WriteInt(restCount);
        }
        SendTCP(gSPacketIn);
    }

    public void SendDragonBoat(PlayerInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(100, info.ID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(DragonBoatMgr.periodType());
        gSPacketIn.WriteInt(DragonBoatMgr.boatCompleteExp());
        gSPacketIn.WriteDateTime(DragonBoatMgr.BeginDate());
        gSPacketIn.WriteDateTime(DragonBoatMgr.EndDate());
        SendTCP(gSPacketIn);
    }

    public void SendSignBuffInfo(ActiveSystemInfo activeUser)
    {
        int[] array = JsonConvert.DeserializeObject<int[]>(activeUser.fightPowerSignData);
        GSPacketIn gSPacketIn = new GSPacketIn(84);
        gSPacketIn.WriteInt(12);
        gSPacketIn.WriteInt(activeUser.loginDays);
        gSPacketIn.WriteInt(activeUser.wellAddRate);
        int[] array2 = array;
        foreach (int val in array2)
        {
            gSPacketIn.WriteInt(val);
        }
        SendTCP(gSPacketIn);
    }

    public void SendGMBuff(ActiveSystemInfo activeUser)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(314);
        gSPacketIn.WriteInt(activeUser.wellAddRate * 100);
        gSPacketIn.WriteInt(activeUser.wellAddRate * 100);
        SendTCP(gSPacketIn);
    }

    public void SendLotteryTicketInfo(int userId, LotteryTicketMgr lottInfo)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(84);
        gSPacketIn.WriteInt(13);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteBoolean(lottInfo.Data.Status != LotteryTicketStatus.CLOSE);
        gSPacketIn.WriteBoolean(lottInfo.Data.Status == LotteryTicketStatus.OPEN);
        gSPacketIn.WriteInt(1);
        List<string> list = new List<string>();
        UserLotteryTicketInfo userLotteryTicketInfo = lottInfo.FindPlayer(userId);
        if (userLotteryTicketInfo != null)
        {
            list.AddRange(userLotteryTicketInfo.PoolList);
        }
        for (int i = 0; i < 8; i++)
        {
            if (i < list.Count)
            {
                gSPacketIn.WriteString(list[i]);
            }
            else
            {
                gSPacketIn.WriteString("");
            }
        }
        SendTCP(gSPacketIn);
    }

    public void SendSuperWinnerOpen(int playerID, bool isOpen)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(145, playerID);
        gSPacketIn.WriteByte(48);
        gSPacketIn.WriteBoolean(isOpen);
        SendTCP(gSPacketIn);
    }

    public void SendCatchBeastOpen(int playerID, bool isOpen)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(145, playerID);
        gSPacketIn.WriteByte(32);
        gSPacketIn.WriteBoolean(isOpen);
        SendTCP(gSPacketIn);
    }

    public void SendLanternriddlesOpen(int playerID, bool isOpen)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(145, playerID);
        gSPacketIn.WriteByte(37);
        gSPacketIn.WriteBoolean(isOpen);
        SendTCP(gSPacketIn);
    }

    public void SendGuildMemberWeekOpenClose(PyramidConfigInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(145, info.UserID);
        gSPacketIn.WriteByte(7);
        gSPacketIn.WriteBoolean(info.isOpen);
        gSPacketIn.WriteString(info.beginTime.ToString());
        gSPacketIn.WriteString(info.endTime.ToString());
        SendTCP(gSPacketIn);
    }

    public void SendQQtips(QQtipsMessagesInfo QQTips)
    {
        if (QQTips != null)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(99);
            gSPacketIn.WriteString(QQTips.title);
            gSPacketIn.WriteString(QQTips.content);
            gSPacketIn.WriteInt(QQTips.maxLevel);
            gSPacketIn.WriteInt(QQTips.minLevel);
            gSPacketIn.WriteInt(QQTips.outInType);
            if (QQTips.outInType == 0)
            {
                gSPacketIn.WriteInt(QQTips.moduleType);
                gSPacketIn.WriteInt(QQTips.inItemID);
            }
            else
            {
                gSPacketIn.WriteString(QQTips.url);
            }
            SendTCP(gSPacketIn);
        }
    }

    public void SendCSMBox(int UserID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(360, UserID);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(60);
        SendTCP(gSPacketIn);
    }

    public void SendTreasureHunting(PyramidConfigInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(110, info.UserID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteDateTime(DateTime.Now);
        gSPacketIn.WriteDateTime(DateTime.Now.AddDays(1.0));
        gSPacketIn.WriteInt(10000);
        SendTCP(gSPacketIn);
        SendMysteriousActivity(info);
    }

    public void SendMysteriousActivity(PyramidConfigInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(110, info.UserID);
        gSPacketIn.WriteByte(16);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(2);
        gSPacketIn.WriteInt(3);
        gSPacketIn.WriteDateTime(DateTime.Now);
        gSPacketIn.WriteDateTime(DateTime.Now.AddDays(1.0));
        SendTCP(gSPacketIn);
    }

    public void SendPyramidOpenClose(PyramidConfigInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(145, info.UserID);
        gSPacketIn.WriteByte(0);
        gSPacketIn.WriteBoolean(info.isOpen);
        gSPacketIn.WriteBoolean(info.isScoreExchange);
        gSPacketIn.WriteDateTime(info.beginTime);
        gSPacketIn.WriteDateTime(info.endTime);
        gSPacketIn.WriteInt(info.freeCount);
        gSPacketIn.WriteInt(info.turnCardPrice);
        gSPacketIn.WriteInt(info.revivePrice.Length);
        for (int i = 0; i < info.revivePrice.Length; i++)
        {
            gSPacketIn.WriteInt(info.revivePrice[i]);
        }
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendOtherPlayerProperty(PlayerInfo info, PlayerProperty PlayerProp, Sys_Users_CardInfo cardInfo)
    {
        string[] array = new string[4] { "Attack", "Defence", "Agility", "Luck" };
        PlayerData playerData = new PlayerData();
        GSPacketIn gSPacketIn = new GSPacketIn(164, info.ID);
        gSPacketIn.WriteInt(info.ID);
        gSPacketIn.WriteInt(info.ZoneID);
        string[] array2 = array;
        foreach (string key in array2)
        {
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(PlayerProp.Current["Texp"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Card"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Pet"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Suit"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Gem"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Bead"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Avatar"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicStone"][key]);
            if (GameProperties.VERSION >= 7600)
            {
                gSPacketIn.WriteInt(PlayerProp.Current["Temple"][key]);
            }
            if (GameProperties.VERSION >= 8200)
            {
                gSPacketIn.WriteInt(PlayerProp.Current["PetAtlas"][key]);
            }
        }
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["MagicStone"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Horse"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["HorsePicCherish"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Enchant"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Suit"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Texp"]);
        if (GameProperties.VERSION >= 8200)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Card"]);
        }
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["MagicStone"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Horse"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["HorsePicCherish"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Enchant"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Suit"]);
        if (GameProperties.VERSION >= 7600)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Temple"]);
        }
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Texp"]);
        if (GameProperties.VERSION >= 8200)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Card"]);
        }
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(PlayerProp.Current["Texp"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Pet"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Bead"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Suit"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Gem"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Avatar"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Horse"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["HorsePicCherish"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Temple"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Suit"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Suit"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Bead"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Avatar"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Horse"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["HorsePicCherish"]);
        if (GameProperties.VERSION >= 9500)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Texp"]);
        }
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Bead"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Avatar"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Horse"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["HorsePicCherish"]);
        if (GameProperties.VERSION >= 7600)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Temple"]);
        }
        if (GameProperties.VERSION >= 9500)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Texp"]);
        }
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Pet"]);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(info.fineSuitExp);
        if (GameProperties.VERSION >= 9300)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["CardAchievement"]["HP"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Card"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Card"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Mark"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Mark"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["Attack"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["Defence"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["Agility"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["Luck"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["HP"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Mark"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Mark"]);
            gSPacketIn.WriteInt(PlayerProp.Current["marKing"]["Attack"]);
            gSPacketIn.WriteInt(PlayerProp.Current["marKing"]["Defence"]);
            gSPacketIn.WriteInt(PlayerProp.Current["marKing"]["Agility"]);
            gSPacketIn.WriteInt(PlayerProp.Current["marKing"]["Luck"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["marKing"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["marKing"]);
        }
        if (GameProperties.VERSION >= 9500)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["titleAdd"]["Attack"]);
            gSPacketIn.WriteInt(PlayerProp.Current["titleAdd"]["Defence"]);
            gSPacketIn.WriteInt(PlayerProp.Current["titleAdd"]["Agility"]);
            gSPacketIn.WriteInt(PlayerProp.Current["titleAdd"]["Luck"]);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["magicHouse"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["magicHouse"]);
            info.SecondPro = playerData.Sys_Users_SecondPro.FirstOrDefault((Sys_Users_SecondPro p) => p.UserID == info.ID);
            if (info.SecondPro != null)
            {
                gSPacketIn.WriteInt(info.SecondPro.Crit);
                gSPacketIn.WriteInt(info.SecondPro.Tenacity);
                gSPacketIn.WriteInt(info.SecondPro.SunderArmor);
                gSPacketIn.WriteInt(info.SecondPro.AvoidInjury);
                gSPacketIn.WriteInt(info.SecondPro.Kill);
                gSPacketIn.WriteInt(info.SecondPro.WillFight);
                gSPacketIn.WriteInt(info.SecondPro.ViolenceInjury);
                gSPacketIn.WriteInt(info.SecondPro.Speed);
                gSPacketIn.WriteInt(info.SecondPro.Guard);
            }
            else
            {
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
            }
        }
        if (GameProperties.VERSION >= 10100)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["Attack"]);
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["Defence"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["SeloPet"]);
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["Agility"]);
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["Luck"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["SeloPet"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["SeloPet"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["SeloPet"]);
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["HP"]);
        }
        if (GameProperties.VERSION >= 10500)
        {
            gSPacketIn.WriteInt(cardInfo.CardLevel);
            gSPacketIn.WriteString(cardInfo.CardMainStr);
            gSPacketIn.WriteString(cardInfo.CardSuitStr);
        }
        if (GameProperties.VERSION >= 10600)
        {
            info.HonorInfo = playerData.Sys_Users_Honor.FirstOrDefault((Sys_Users_Honor p) => p.UserID == info.ID);
            if (info.HonorInfo != null)
            {
                gSPacketIn.WriteInt(info.HonorInfo.HonorLevel);
                gSPacketIn.WriteInt(PlayerProp.Current["honor"]["HP"]);
            }
            else
            {
                gSPacketIn.WriteInt(1);
                gSPacketIn.WriteInt(0);
            }
        }
        if (GameProperties.VERSION >= 10900)
        {
            TS_NecklaceCasting tS_NecklaceCasting = NecklaceStrengthMgr.ts_NecklaceCasting.FirstOrDefault((TS_NecklaceCasting p) => p.Level == info.necklaceCastLevel);
            gSPacketIn.WriteInt(tS_NecklaceCasting.HP);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    //PlayerManager.as -> __updatePlayerProperty
    public GSPacketIn SendUpdatePlayerProperty(PlayerInfo info, PlayerProperty PlayerProp, Sys_Users_CardInfo cardInfo)
    {
        string[] array = new string[4] { "Attack", "Defence", "Agility", "Luck" };
        GSPacketIn gSPacketIn = new GSPacketIn(164, info.ID);
        gSPacketIn.WriteInt(info.ID);
        gSPacketIn.WriteInt(info.ZoneID);
        string[] array2 = array;
        foreach (string key in array2)
        {
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(PlayerProp.Current["Texp"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Card"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Pet"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Suit"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Gem"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Bead"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["Avatar"][key]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicStone"][key]);
            if (GameProperties.VERSION >= 7600)
            {
                gSPacketIn.WriteInt(PlayerProp.Current["Temple"][key]);
            }
            if (GameProperties.VERSION >= 8200)
            {
                gSPacketIn.WriteInt(PlayerProp.Current["PetAtlas"][key]);
            }
            if (GameProperties.VERSION >= 12100)
            {
                //Cần sửa
                gSPacketIn.WriteInt(0); //_loc11_["ElfEquip"] = _loc4_.readInt();
            }
        }
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["MagicStone"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Horse"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["HorsePicCherish"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Enchant"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Suit"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Texp"]);
        if (GameProperties.VERSION >= 8200)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Card"]);
        }
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["MagicStone"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Horse"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["HorsePicCherish"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Enchant"]);
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Suit"]);
        if (GameProperties.VERSION >= 7600)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Temple"]);
        }
        gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Texp"]);
        if (GameProperties.VERSION >= 8200)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Card"]);
        }
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(PlayerProp.Current["Texp"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Pet"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Bead"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Suit"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Gem"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Avatar"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Horse"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["HorsePicCherish"]["HP"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Temple"]["HP"]);
        if (GameProperties.VERSION >= 12100)
        {
            //Cần sửa
            gSPacketIn.WriteInt(0); //_loc11_["ElfEquip"] = _loc4_.readInt();
        }
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Suit"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Suit"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Bead"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Avatar"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Horse"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["HorsePicCherish"]);
        if (GameProperties.VERSION >= 9500)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Texp"]);
        }
        if (GameProperties.VERSION >= 12100)
        {
            //Cần sửa
            gSPacketIn.WriteInt(0); //_loc3_["Damage"]["ElfEquip"] = _loc4_.readInt();
        }
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Bead"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Avatar"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Horse"]);
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["HorsePicCherish"]);
        if (GameProperties.VERSION >= 7600)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Temple"]);
        }
        if (GameProperties.VERSION >= 9500)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Texp"]);
        }
        gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Pet"]);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(info.fineSuitExp);
        if (GameProperties.VERSION >= 9300)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["CardAchievement"]["HP"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Card"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Card"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["Mark"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["Mark"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["Attack"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["Defence"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["Agility"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["Luck"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Mark"]["HP"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["Mark"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["Mark"]);
            gSPacketIn.WriteInt(PlayerProp.Current["marKing"]["Attack"]);
            gSPacketIn.WriteInt(PlayerProp.Current["marKing"]["Defence"]);
            gSPacketIn.WriteInt(PlayerProp.Current["marKing"]["Agility"]);
            gSPacketIn.WriteInt(PlayerProp.Current["marKing"]["Luck"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["marKing"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["marKing"]);
        }
        if (GameProperties.VERSION >= 9500)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["titleAdd"]["Attack"]);
            gSPacketIn.WriteInt(PlayerProp.Current["titleAdd"]["Defence"]);
            gSPacketIn.WriteInt(PlayerProp.Current["titleAdd"]["Agility"]);
            gSPacketIn.WriteInt(PlayerProp.Current["titleAdd"]["Luck"]);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["magicHouse"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["magicHouse"]);
            gSPacketIn.WriteInt(info.SecondPro.Crit);
            gSPacketIn.WriteInt(info.SecondPro.Tenacity);
            gSPacketIn.WriteInt(info.SecondPro.SunderArmor);
            gSPacketIn.WriteInt(info.SecondPro.AvoidInjury);
            gSPacketIn.WriteInt(info.SecondPro.Kill);
            gSPacketIn.WriteInt(info.SecondPro.WillFight);
            gSPacketIn.WriteInt(info.SecondPro.ViolenceInjury);
            gSPacketIn.WriteInt(info.SecondPro.Speed);
            gSPacketIn.WriteInt(info.SecondPro.Guard);
        }
        if (GameProperties.VERSION >= 10100)
        {
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["Attack"]);
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["Defence"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicAttack"]["SeloPet"]);
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["Agility"]);
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["Luck"]);
            gSPacketIn.WriteInt(PlayerProp.Current["MagicDefence"]["SeloPet"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Damage"]["SeloPet"]);
            gSPacketIn.WriteInt(PlayerProp.Current["Armor"]["SeloPet"]);
            gSPacketIn.WriteInt(PlayerProp.Current["SeloPet"]["HP"]);
        }
        if (GameProperties.VERSION >= 10500)
        {
            gSPacketIn.WriteInt(cardInfo.CardLevel);
            gSPacketIn.WriteString(cardInfo.CardMainStr);
            gSPacketIn.WriteString(cardInfo.CardSuitStr);
        }
        if (GameProperties.VERSION >= 10600)
        {
            gSPacketIn.WriteInt(info.HonorInfo.HonorLevel);
            gSPacketIn.WriteInt(PlayerProp.Current["honor"]["HP"]);
        }
        if (GameProperties.VERSION >= 10900)
        {
            TS_NecklaceCasting tS_NecklaceCasting = NecklaceStrengthMgr.ts_NecklaceCasting.FirstOrDefault((TS_NecklaceCasting p) => p.Level == info.necklaceCastLevel);
            gSPacketIn.WriteInt(tS_NecklaceCasting.HP);
        }
        if (GameProperties.VERSION >= 12100)
        {
            gSPacketIn.WriteInt(info.ElementPro.FireAttack); //_loc7_.elementHuoAtk = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.WindAttack); //_loc7_.elementFengAtk = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.LandAttack); //_loc7_.elementTuAtk = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.WaterAttack); //_loc7_.elementShuiAtk = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.LighAttack); //_loc7_.elementGuangAtk = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.DrakAttack); //_loc7_.elementAnAtk = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.FireDefence); //_loc7_.elementHuoDef = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.WindDefence); //_loc7_.elementFengDef = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.LandDefence); //_loc7_.elementTuDef = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.WaterDefence); //_loc7_.elementShuiDef = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.LighDefence); //_loc7_.elementGuangDef = _loc4_.readInt();
            gSPacketIn.WriteInt(info.ElementPro.DrakDefence); //_loc7_.elementAnDef = _loc4_.readInt();
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    gSPacketIn.WriteInt(info.EmblemInfoAttackSKill);
                    if (info.EmblemInfoAttackSKill != 0)
                    {
                        gSPacketIn.WriteInt(info.EmblemInfoAttackSKill_Value);
                        gSPacketIn.WriteInt(info.EmblemInfoAttackSKill_Rate);
                    }

                }
                else
                {
                    gSPacketIn.WriteInt(info.EmblemInfoDefendSKill);
                    if (info.EmblemInfoDefendSKill != 0)
                    {
                        gSPacketIn.WriteInt(info.EmblemInfoDefendSKill_Value);
                        gSPacketIn.WriteInt(info.EmblemInfoDefendSKill_Rate);
                    }

                }
            }
            gSPacketIn.WriteInt(0); //var _loc5_:int = _loc4_.readInt();
            for (int i = 0; i < 0; i++)
            {
                gSPacketIn.WriteInt(0); //_loc15_ = _loc4_.readInt();
                gSPacketIn.WriteInt(0); //_loc7_.cultureMachiningDic[_loc15_] = _loc4_.readInt();
                gSPacketIn.WriteInt(0); //_loc7_.cultureQuenchingDic[_loc15_] = _loc4_.readInt();
                gSPacketIn.WriteString(""); //_loc7_.cultureQuenchingSkillDic[_loc15_] = _loc4_.readUTF();
            }
            gSPacketIn.WriteInt(0); //_loc7_.cultureAddDamage = _loc4_.readInt();
            gSPacketIn.WriteString(""); //_loc7_.lingStoneStr = _loc4_.readUTF();
            gSPacketIn.WriteInt(0); //_loc7_.plotDamage = _loc4_.readInt();
            gSPacketIn.WriteInt(0); //_loc7_.plotDefence = _loc4_.readInt();


        }

        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info, UserMatchInfo matchInfo)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(67, info.ID);
        gSPacketIn.WriteInt((info.GP > int.MaxValue) ? int.MaxValue : Convert.ToInt32(info.GP));
        if (GameProperties.VERSION >= 8200)
        {
            gSPacketIn.WriteInt(info.ddtKingGrade);
        }
        gSPacketIn.WriteInt(info.Offer);
        gSPacketIn.WriteInt(info.RichesOffer);
        gSPacketIn.WriteInt(info.RichesRob);
        gSPacketIn.WriteInt(info.Win);
        gSPacketIn.WriteInt(info.Total);
        gSPacketIn.WriteInt(info.Escape);
        gSPacketIn.WriteInt(info.Attack);
        gSPacketIn.WriteInt(info.Defence);
        gSPacketIn.WriteInt(info.Agility);
        gSPacketIn.WriteInt(info.Luck);
        gSPacketIn.WriteInt(info.MagicAttack);
        gSPacketIn.WriteInt(info.MagicDefence);
        gSPacketIn.WriteInt(info.hp);
        gSPacketIn.WriteInt(info.Hide);
        gSPacketIn.WriteString(string.IsNullOrEmpty(info.tempStyle) ? info.Style : info.tempStyle);
        gSPacketIn.WriteString(info.Colors);
        gSPacketIn.WriteString(info.Skin);
        gSPacketIn.WriteBoolean(info.IsShowConsortia);
        gSPacketIn.WriteInt(info.ConsortiaID);
        gSPacketIn.WriteString(info.ConsortiaName);
        gSPacketIn.WriteInt(info.badgeID);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(info.Nimbus);
        gSPacketIn.WriteString(info.PvePermission);
        gSPacketIn.WriteString(info.FightLabPermission);
        gSPacketIn.WriteInt(info.FightPower);
        if (GameProperties.VERSION >= 11000)
        {
            gSPacketIn.WriteLong(0L);
        }
        gSPacketIn.WriteInt(info.AcademyInfo.ApprenticeshipState);
        gSPacketIn.WriteInt(info.AcademyInfo.MasterID);
        gSPacketIn.WriteString(info.AcademyInfo.MasterOrApprentices);
        gSPacketIn.WriteInt(info.AcademyInfo.GraduatesCount);
        gSPacketIn.WriteString(info.AcademyInfo.HonourOfMaster);
        gSPacketIn.WriteInt(info.AchievementPoint);
        gSPacketIn.WriteString(info.Honor);
        gSPacketIn.WriteInt(info.honorId);
        gSPacketIn.WriteDateTime(info.LastSpaDate);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteDateTime(DateTime.Now);
        gSPacketIn.WriteInt(info.UseOffer);
        gSPacketIn.WriteInt(matchInfo.dailyScore);
        gSPacketIn.WriteInt(matchInfo.dailyWinCount);
        gSPacketIn.WriteInt(matchInfo.dailyGameCount);
        gSPacketIn.WriteInt(matchInfo.weeklyScore);
        gSPacketIn.WriteInt(matchInfo.weeklyGameCount);
        gSPacketIn.WriteInt(info.LeagueMoney);
        gSPacketIn.WriteInt(info.Texp.spdTexpExp);
        gSPacketIn.WriteInt(info.Texp.attTexpExp);
        gSPacketIn.WriteInt(info.Texp.defTexpExp);
        gSPacketIn.WriteInt(info.Texp.hpTexpExp);
        gSPacketIn.WriteInt(info.Texp.lukTexpExp);
        gSPacketIn.WriteInt(info.Texp.magicAtkTexpExp);
        gSPacketIn.WriteInt(info.Texp.magicDefTexpExp);
        gSPacketIn.WriteInt(info.Texp.texpTaskCount);
        gSPacketIn.WriteInt(info.Texp.texpCount);
        gSPacketIn.WriteInt(info.Texp.magicTexpCount);
        gSPacketIn.WriteDateTime(info.Texp.texpTaskDate);
        gSPacketIn.WriteInt(10);
        for (int i = 1; i <= 10; i++)
        {
            gSPacketIn.WriteInt(i);
            gSPacketIn.WriteByte(10);
        }
        gSPacketIn.WriteString(info.PveEpicPermission);
        gSPacketIn.WriteInt(info.evolutionGrade);
        gSPacketIn.WriteInt(info.evolutionExp);
        if (GameProperties.VERSION >= 7700)
        {
            gSPacketIn.WriteInt(info.desertEnterCount);
        }
        gSPacketIn.WriteInt(info.BattleCount);
        gSPacketIn.WriteInt(info.VipIntegral);
        if (GameProperties.VERSION >= 7600)
        {
            gSPacketIn.WriteInt(info.RingExp);
            gSPacketIn.WriteInt(info.guardCoreID);
            gSPacketIn.WriteInt(info.guardCoreGrade);
        }
        if (GameProperties.VERSION >= 8800)
        {
            gSPacketIn.WriteInt(info.Team.teamID);
            gSPacketIn.WriteString(info.Team.teamName);
            gSPacketIn.WriteString(info.Team.teamTag);
            gSPacketIn.WriteInt(info.Team.teamGrade);
            gSPacketIn.WriteInt(info.Team.teamWinTime);
            gSPacketIn.WriteInt(info.Team.teamTotalTime);
            gSPacketIn.WriteInt(info.Team.teamDivision);
            gSPacketIn.WriteInt(info.Team.teamScore);
            gSPacketIn.WriteInt(info.Team.teamDuty);
            if (info.TeamUser != null)
            {
                gSPacketIn.WriteInt(info.TeamUser.BattleScore);
            }
            else
            {
                gSPacketIn.WriteInt(0);
            }
        }
        if (GameProperties.VERSION >= 10400)
        {
            gSPacketIn.WriteInt(info.magicWardrobeID);
        }
        if (GameProperties.VERSION >= 9500)
        {
            gSPacketIn.WriteInt(info.Texp.TrainCritTexpExp);
            gSPacketIn.WriteInt(info.Texp.TrainSunderTexpExp);
            gSPacketIn.WriteInt(info.Texp.TrainCritDmgTexpExp);
            gSPacketIn.WriteInt(info.Texp.TrainSpeedTexpExp);
            gSPacketIn.WriteInt(info.Texp.TrainTricKillTexpExp);
            gSPacketIn.WriteInt(info.Texp.TrainDmgTexpExp);
            gSPacketIn.WriteInt(info.Texp.TrainArmorTexpExp);
            gSPacketIn.WriteInt(info.Texp.SecondTexpCount);
        }

        if (GameProperties.VERSION >= 12100)
        {
            //Cần sửa
            gSPacketIn.WriteInt(info.Texp.TrainAvoidDmgTextExp); //_loc6_.avoidInjury = _loc4_.readInt();
            gSPacketIn.WriteInt(info.Texp.TrainTricDefTexpExp); //_loc6_.mustResist = _loc4_.readInt();
            gSPacketIn.WriteInt(info.Texp.TrainTenacTexpExp);   //_loc6_.toughness = _loc4_.readInt();
            gSPacketIn.WriteInt(info.Texp.TrainGuardTexpExp);   //_loc6_.Guard = _loc4_.readInt();
            gSPacketIn.WriteInt(info.Texp.TrainFireDefTexpExp); //_loc6_.fireResist = _loc4_.readInt();
            gSPacketIn.WriteInt(info.Texp.TrainWaterDefTexpExp);    //_loc6_.waterResist = _loc4_.readInt();
            gSPacketIn.WriteInt(info.Texp.TrainWindDefTexpExp); //_loc6_.windResist = _loc4_.readInt();
            gSPacketIn.WriteInt(info.Texp.TrainLandDefTexpExp); //_loc6_.soilResist = _loc4_.readInt();
            gSPacketIn.WriteInt(info.Texp.ThirdTexpCount);	//_loc6_.thirdTexpCount = _loc4_.readInt();

        }
        if (GameProperties.VERSION >= 11000)
        {
            if (info.IsHideElf)
            {
                gSPacketIn.WriteInt(0);
            }
            else
            {
                gSPacketIn.WriteInt((info?.ElfInfo?.FirstOrDefault((Sys_Users_Elf e) => e.IsFight)?.TempID).GetValueOrDefault());
            }
            gSPacketIn.WriteInt(info.SwornInfo.swornCount);
        }
        if (GameProperties.VERSION >= 12100)
        {
            //Cần sửa
            gSPacketIn.WriteInt(0); //_loc6_.elfEquipType = _loc4_.readInt();
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendPingTime(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(4);
        player.PingStart = DateTime.Now.Ticks;
        gSPacketIn.WriteInt(player.PlayerCharacter.AntiAddiction);
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendNetWork(GamePlayer player, long delay)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(6, player.PlayerId);
        gSPacketIn.WriteInt((int)delay / 1000 / 10);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    //PlayerManager.as -> __itemEquip
    public GSPacketIn SendUserEquip(PlayerInfo player, List<ItemInfo> items, List<UserGemStone> userGemStone, List<ItemInfo> beadItems, List<ItemInfo> magicStones, ExplorerManualInfo explorerInfo, List<MarkChipItemInfo> playerMark, List<UsersPetLacrarInfo> lacres, List<Sys_Users_Emblem> emblemInfos, ICollection<Sys_Users_Elf> elfinfo, Sys_Users_Academy academyInfo)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(74, player.ID);
        gSPacketIn.WriteInt(player.ID);
        gSPacketIn.WriteInt(player.ZoneID);
        gSPacketIn.WriteString(player.NickName);
        gSPacketIn.WriteInt(player.Agility);
        gSPacketIn.WriteInt(player.Attack);
        gSPacketIn.WriteString(player.Colors);
        gSPacketIn.WriteString(player.Skin);
        gSPacketIn.WriteInt(player.Defence);
        gSPacketIn.WriteInt((player.GP > int.MaxValue) ? int.MaxValue : Convert.ToInt32(player.GP));
        gSPacketIn.WriteInt(player.Grade);
        if (GameProperties.VERSION >= 8200)
        {
            gSPacketIn.WriteInt(player.ddtKingGrade);
        }
        gSPacketIn.WriteInt(player.Luck);
        gSPacketIn.WriteInt(player.hp);
        gSPacketIn.WriteInt(player.Hide);
        gSPacketIn.WriteInt(player.Repute);
        gSPacketIn.WriteBoolean(player.Sex);
        gSPacketIn.WriteString(player.Style);
        gSPacketIn.WriteInt(player.Offer);
        gSPacketIn.WriteByte(player.typeVIP);
        gSPacketIn.WriteInt(player.VIPLevel);
        gSPacketIn.WriteBoolean(player.isOpenKingBless);
        gSPacketIn.WriteInt(player.Win);
        gSPacketIn.WriteInt(player.Total);
        gSPacketIn.WriteInt(player.Escape);
        gSPacketIn.WriteInt(player.ConsortiaID);
        gSPacketIn.WriteString(player.ConsortiaName);
        gSPacketIn.WriteInt(player.badgeID);
        gSPacketIn.WriteInt(player.RichesOffer);
        gSPacketIn.WriteInt(player.RichesRob);
        gSPacketIn.WriteBoolean(player.IsMarried);
        gSPacketIn.WriteInt(player.SpouseID);
        gSPacketIn.WriteString(player.SpouseName);
        gSPacketIn.WriteString(player.DutyName);
        gSPacketIn.WriteInt(player.Nimbus);
        gSPacketIn.WriteInt(player.FightPower);
        if (GameProperties.VERSION >= 11000)
        {
            gSPacketIn.WriteLong(0L);
        }
        gSPacketIn.WriteInt(academyInfo.ApprenticeshipState);
        gSPacketIn.WriteInt(academyInfo.MasterID);
        gSPacketIn.WriteString(academyInfo.MasterOrApprentices);
        gSPacketIn.WriteInt(academyInfo.GraduatesCount);
        gSPacketIn.WriteString(academyInfo.HonourOfMaster);
        gSPacketIn.WriteInt(player.AchievementPoint);
        gSPacketIn.WriteString(player.Honor);
        gSPacketIn.WriteDateTime(DateTime.Now.AddDays(-2.0));
        gSPacketIn.WriteInt(player.Texp.spdTexpExp);
        gSPacketIn.WriteInt(player.Texp.attTexpExp);
        gSPacketIn.WriteInt(player.Texp.defTexpExp);
        gSPacketIn.WriteInt(player.Texp.hpTexpExp);
        gSPacketIn.WriteInt(player.Texp.lukTexpExp);
        gSPacketIn.WriteInt(player.Texp.magicAtkTexpExp);
        gSPacketIn.WriteInt(player.Texp.magicDefTexpExp);
        gSPacketIn.WriteBoolean(val: false);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(player.totemId);
        gSPacketIn.WriteInt(player.necklaceExp);
        if (GameProperties.VERSION >= 11000)
        {
            gSPacketIn.WriteInt(player.necklaceCastLevel);
        }
        gSPacketIn.WriteBoolean(player.isAttest);
        gSPacketIn.WriteInt(player.MountLv);
        if (GameProperties.VERSION >= 7600)
        {
            gSPacketIn.WriteInt(player.RingExp);
        }
        gSPacketIn.WriteInt(items.Count);
        foreach (ItemInfo item in items)
        {
            gSPacketIn.WriteByte((byte)item.BagType);
            gSPacketIn.WriteInt(item.UserID);
            gSPacketIn.WriteInt(item.ItemID);
            gSPacketIn.WriteInt(item.Count);
            gSPacketIn.WriteInt(item.Place);
            gSPacketIn.WriteInt(item.TemplateID);
            gSPacketIn.WriteInt(item.AttackCompose);
            gSPacketIn.WriteInt(item.DefendCompose);
            gSPacketIn.WriteInt(item.AgilityCompose);
            gSPacketIn.WriteInt(item.LuckCompose);
            gSPacketIn.WriteInt(item.StrengthenLevel);
            gSPacketIn.WriteBoolean(item.IsBinds);
            gSPacketIn.WriteBoolean(item.IsJudge);
            gSPacketIn.WriteDateTime(item.BeginDate);
            gSPacketIn.WriteInt(item.ValidDate);
            gSPacketIn.WriteString(item.Color);
            gSPacketIn.WriteString(item.Skin);
            gSPacketIn.WriteBoolean(item.IsUsed);
            gSPacketIn.WriteInt(item.Hole1);
            gSPacketIn.WriteInt(item.Hole2);
            gSPacketIn.WriteInt(item.Hole3);
            gSPacketIn.WriteInt(item.Hole4);
            gSPacketIn.WriteInt(item.Hole5);
            gSPacketIn.WriteInt(item.Hole6);
            gSPacketIn.WriteString(item.Pic);
            gSPacketIn.WriteInt(item.RefineryLevel);
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteByte((byte)item.Hole5Level);
            gSPacketIn.WriteInt(item.Hole5Exp);
            gSPacketIn.WriteByte((byte)item.Hole6Level);
            gSPacketIn.WriteInt(item.Hole6Exp);
            if (GameProperties.VERSION >= 8400)
            {
                gSPacketIn.WriteInt(item.curExp);
            }
            gSPacketIn.WriteBoolean(item.GoldValidDate());
            if (item.GoldValidDate())
            {
                gSPacketIn.WriteInt(item.goldValidDate);
                gSPacketIn.WriteDateTime(item.goldBeginTime);
            }
            gSPacketIn.WriteString(item.latentEnergyCurStr);
            gSPacketIn.WriteString(item.latentEnergyNewStr);
            gSPacketIn.WriteDateTime(item.latentEnergyEndTime);
            gSPacketIn.WriteInt(item.MagicLevel);
            if (GameProperties.VERSION >= 10200)
            {
                gSPacketIn.WriteInt(item.enchantSpiritLevel);
            }
        }
        gSPacketIn.WriteInt(beadItems.Count);
        foreach (ItemInfo beadItem in beadItems)
        {
            gSPacketIn.WriteByte((byte)beadItem.BagType);
            gSPacketIn.WriteInt(beadItem.UserID);
            gSPacketIn.WriteInt(beadItem.ItemID);
            gSPacketIn.WriteInt(beadItem.Count);
            gSPacketIn.WriteInt(beadItem.Place);
            gSPacketIn.WriteInt(beadItem.TemplateID);
            gSPacketIn.WriteInt(beadItem.AttackCompose);
            gSPacketIn.WriteInt(beadItem.DefendCompose);
            gSPacketIn.WriteInt(beadItem.AgilityCompose);
            gSPacketIn.WriteInt(beadItem.LuckCompose);
            gSPacketIn.WriteInt(beadItem.StrengthenLevel);
            gSPacketIn.WriteBoolean(beadItem.IsBinds);
            gSPacketIn.WriteBoolean(beadItem.IsJudge);
            gSPacketIn.WriteDateTime(beadItem.BeginDate);
            gSPacketIn.WriteInt(beadItem.ValidDate);
            gSPacketIn.WriteString(beadItem.Color);
            gSPacketIn.WriteString(beadItem.Skin);
            gSPacketIn.WriteBoolean(beadItem.IsUsed);
            gSPacketIn.WriteInt(beadItem.Hole1);
            gSPacketIn.WriteInt(beadItem.Hole2);
            gSPacketIn.WriteInt(beadItem.Hole3);
            gSPacketIn.WriteInt(beadItem.Hole4);
            gSPacketIn.WriteInt(beadItem.Hole5);
            gSPacketIn.WriteInt(beadItem.Hole6);
            gSPacketIn.WriteString(beadItem.Pic);
            gSPacketIn.WriteInt(beadItem.RefineryLevel);
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteByte((byte)beadItem.Hole5Level);
            gSPacketIn.WriteInt(beadItem.Hole5Exp);
            gSPacketIn.WriteByte((byte)beadItem.Hole6Level);
            gSPacketIn.WriteInt(beadItem.Hole6Exp);
            gSPacketIn.WriteBoolean(beadItem.GoldValidDate());
        }
        gSPacketIn.WriteInt(userGemStone.Count);
        for (int i = 0; i < userGemStone.Count; i++)
        {
            gSPacketIn.WriteInt(userGemStone[i].FigSpiritId);
            gSPacketIn.WriteString(userGemStone[i].FigSpiritIdValue);
            gSPacketIn.WriteInt(userGemStone[i].EquipPlace);
        }
        gSPacketIn.WriteInt(player.evolutionGrade);
        gSPacketIn.WriteInt(player.evolutionExp);
        gSPacketIn.WriteInt(player.MagicAttack);
        gSPacketIn.WriteInt(player.MagicDefence);
        gSPacketIn.WriteInt(magicStones.Count);
        foreach (ItemInfo magicStone in magicStones)
        {
            gSPacketIn.WriteInt(magicStone.Place);
            gSPacketIn.WriteInt(magicStone.TemplateID);
            gSPacketIn.WriteInt(magicStone.StrengthenLevel);
            gSPacketIn.WriteInt(magicStone.AttackCompose);
            gSPacketIn.WriteInt(magicStone.DefendCompose);
            gSPacketIn.WriteInt(magicStone.AgilityCompose);
            gSPacketIn.WriteInt(magicStone.LuckCompose);
            gSPacketIn.WriteInt(magicStone.MagicAttack);
            gSPacketIn.WriteInt(magicStone.MagicDefence);
        }
        gSPacketIn.WriteInt(player.MountsType);
        for (int j = 0; j < 9; j++)
        {
            gSPacketIn.WriteInt(0);
        }
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(explorerInfo.manualLevel);
        gSPacketIn.WriteInt(explorerInfo.Agility);
        gSPacketIn.WriteInt(explorerInfo.Armor);
        gSPacketIn.WriteInt(explorerInfo.Attack);
        gSPacketIn.WriteInt(explorerInfo.Damage);
        gSPacketIn.WriteInt(explorerInfo.Defense);
        gSPacketIn.WriteInt(explorerInfo.HP);
        gSPacketIn.WriteInt(explorerInfo.Lucky);
        gSPacketIn.WriteInt(explorerInfo.MagicAttack);
        gSPacketIn.WriteInt(explorerInfo.MagicResistance);
        gSPacketIn.WriteInt(explorerInfo.Stamina);
        gSPacketIn.WriteInt(player.guardCoreGrade);
        gSPacketIn.WriteInt(player.guardCoreID);
        if (GameProperties.VERSION >= 8700)
        {
            if (player.GhostEquipList != null)
            {
                Dictionary<string, UserEquipGhostInfo> dictionary = JsonConvert.DeserializeObject<Dictionary<string, UserEquipGhostInfo>>(player.GhostEquipList);
                if (dictionary == null)
                {
                    gSPacketIn.WriteInt(0);
                }
                else
                {
                    gSPacketIn.WriteInt(dictionary.Count);
                    foreach (UserEquipGhostInfo value in dictionary.Values)
                    {
                        gSPacketIn.WriteInt(value.BagType);
                        gSPacketIn.WriteInt(value.Place);
                        gSPacketIn.WriteInt(value.Level);
                        gSPacketIn.WriteInt(value.TotalGhost);
                    }
                }
            }
            else
            {
                gSPacketIn.WriteInt(0);
            }
            gSPacketIn.WriteInt(player.Team.teamID);
            gSPacketIn.WriteString(player.Team.teamName);
            gSPacketIn.WriteString(player.Team.teamTag);
            gSPacketIn.WriteInt(player.Team.teamGrade);
            gSPacketIn.WriteInt(player.Team.teamWinTime);
            gSPacketIn.WriteInt(player.Team.teamTotalTime);
            gSPacketIn.WriteInt(player.Team.teamDivision);
            gSPacketIn.WriteInt(player.Team.teamScore);
            gSPacketIn.WriteInt(player.Team.teamDuty);
            if (player.TeamUser != null)
            {
                gSPacketIn.WriteInt(player.TeamUser.BattleScore);
            }
            else
            {
                gSPacketIn.WriteInt(0);
            }
        }
        if (GameProperties.VERSION >= 9400)
        {
            gSPacketIn.WriteInt(playerMark.Count);
            foreach (MarkChipItemInfo item2 in playerMark)
            {
                gSPacketIn.WriteInt(item2.ItemID);
                gSPacketIn.WriteInt(item2.TemplateID);
                gSPacketIn.WriteInt(item2.Place);
                gSPacketIn.WriteBoolean(item2.IsExist);
                if (item2.IsExist)
                {
                    gSPacketIn.WriteBoolean(val: false);
                    gSPacketIn.WriteBoolean(item2.IsBind);
                    gSPacketIn.WriteInt(item2.bornLv);
                    gSPacketIn.WriteInt(item2.hammerLv);
                    gSPacketIn.WriteInt(item2.hLv);
                    MarkProDataInfo mainProp = item2.MainProp;
                    gSPacketIn.WriteInt(mainProp.type);
                    gSPacketIn.WriteInt(mainProp.value);
                    gSPacketIn.WriteInt(mainProp.attachValue);
                    List<MarkProDataInfo> secondProps = item2.SecondProps;
                    foreach (MarkProDataInfo item3 in secondProps)
                    {
                        gSPacketIn.WriteInt(item3.type);
                        gSPacketIn.WriteInt(item3.value);
                        gSPacketIn.WriteInt(item3.attachValue);
                        gSPacketIn.WriteInt(item3.hummerCount);
                    }
                }
                else
                {
                    gSPacketIn.WriteBoolean(val: false);
                }
            }
        }
        if (GameProperties.VERSION >= 10300)
        {
            gSPacketIn.WriteInt(player.magicWardrobeID);
        }
        if (GameProperties.VERSION >= 9500)
        {
            gSPacketIn.WriteInt(player.GetTotemUpgrade().Count);
            foreach (KeyValuePair<int, int> item4 in player.GetTotemUpgrade())
            {
                gSPacketIn.WriteInt(item4.Key);
                gSPacketIn.WriteInt(item4.Value);
            }
        }
        if (GameProperties.VERSION >= 10100)
        {
            if (lacres != null)
            {
                gSPacketIn.WriteInt(lacres.Count);
                foreach (UsersPetLacrarInfo lacre in lacres)
                {
                    gSPacketIn.WriteInt(lacre.place);
                    gSPacketIn.WriteBoolean(lacre.isSeal);
                    if (lacre.isSeal)
                    {
                        gSPacketIn.WriteInt(lacre.petId);
                        gSPacketIn.WriteString(lacre.GetProStr());
                    }
                }
            }
            else
            {
                gSPacketIn.WriteInt(0);
            }
        }
        if (GameProperties.VERSION >= 10700)
        {
            if (emblemInfos == null)
            {
                gSPacketIn.WriteInt(0);
            }
            else
            {
                gSPacketIn.WriteInt(emblemInfos.Count);
                foreach (Sys_Users_Emblem emblemInfo in emblemInfos)
                {
                    gSPacketIn.WriteInt(emblemInfo.ID);
                    gSPacketIn.WriteInt(emblemInfo.tempId);
                    gSPacketIn.WriteInt(emblemInfo.equipPos);
                    gSPacketIn.WriteInt(emblemInfo.equipIndex);
                    gSPacketIn.WriteInt(emblemInfo.mainType);
                    gSPacketIn.WriteInt(emblemInfo.mainValue);
                    if (!string.IsNullOrEmpty(emblemInfo.strvalue))
                    {
                        string[] array = emblemInfo.strvalue.Split('|');
                        gSPacketIn.WriteInt(array.Length);
                        for (int k = 0; k < array.Length; k++)
                        {
                            gSPacketIn.WriteInt(int.Parse(array[k].Split(',')[0]));
                            gSPacketIn.WriteInt(int.Parse(array[k].Split(',')[1]));
                        }
                    }
                    else
                    {
                        gSPacketIn.WriteInt(0);
                    }
                }
            }
            if (GameProperties.VERSION >= 12100)
            {
                //Cần sửa
                gSPacketIn.WriteInt(player.ElementPro.FireAttack); //_loc7_.elementHuoAtk = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.WindAttack); //_loc7_.elementFengAtk = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.LandAttack); //_loc7_.elementTuAtk = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.WaterAttack); //_loc7_.elementShuiAtk = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.LighAttack); //_loc7_.elementGuangAtk = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.DrakAttack); //_loc7_.elementAnAtk = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.FireDefence); //_loc7_.elementHuoDef = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.WindDefence); //_loc7_.elementFengDef = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.LandDefence); //_loc7_.elementTuDef = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.WaterDefence); //_loc7_.elementShuiDef = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.LighDefence); //_loc7_.elementGuangDef = _loc18_.readInt();
                gSPacketIn.WriteInt(player.ElementPro.DrakDefence); //_loc7_.elementAnDef = _loc18_.readInt();

            }
            for (int l = 0; l < 2; l++)
            {
                if (l == 0)
                {
                    gSPacketIn.WriteInt(player.EmblemInfoAttackSKill);
                    if (player.EmblemInfoAttackSKill != 0)
                    {
                        gSPacketIn.WriteInt(player.EmblemInfoAttackSKill_Value);
                        gSPacketIn.WriteInt(player.EmblemInfoAttackSKill_Rate);
                    }
                }
                else
                {
                    gSPacketIn.WriteInt(player.EmblemInfoDefendSKill);
                    if (player.EmblemInfoDefendSKill != 0)
                    {
                        gSPacketIn.WriteInt(player.EmblemInfoDefendSKill_Value);
                        gSPacketIn.WriteInt(player.EmblemInfoDefendSKill_Rate);
                    }
                }
            }
        }
        if (GameProperties.VERSION >= 11000)
        {
            Sys_Users_Elf sys_Users_Elf = elfinfo.FirstOrDefault((Sys_Users_Elf e) => e.IsFight);
            gSPacketIn.WriteInt(sys_Users_Elf?.TempID ?? 0);
            if (sys_Users_Elf != null && sys_Users_Elf.TempID != 0)
            {
                gSPacketIn.WriteInt(sys_Users_Elf.GoodFeling);
                gSPacketIn.WriteInt(sys_Users_Elf.Grade);
                gSPacketIn.WriteInt(sys_Users_Elf.Quality);
                gSPacketIn.WriteInt(sys_Users_Elf.HpValue);
                gSPacketIn.WriteInt(sys_Users_Elf.AttackValue);
                gSPacketIn.WriteInt(sys_Users_Elf.HuoValue);
                gSPacketIn.WriteInt(sys_Users_Elf.ShuiValue);
                gSPacketIn.WriteInt(sys_Users_Elf.TuValue);
                gSPacketIn.WriteInt(sys_Users_Elf.FengValue);
                if (GameProperties.VERSION >= 12100)
                {
                    gSPacketIn.WriteInt(sys_Users_Elf.GuangValue); //_loc23_.guangValue = _loc18_.readInt();
                    gSPacketIn.WriteInt(sys_Users_Elf.AnValue); //_loc23_.anValue = _loc18_.readInt();
                }
                gSPacketIn.WriteInt(sys_Users_Elf.HpGrowth);
                gSPacketIn.WriteInt(sys_Users_Elf.AttackGrowth);
                gSPacketIn.WriteInt(sys_Users_Elf.HuoGrowth);
                gSPacketIn.WriteInt(sys_Users_Elf.ShuiGrowth);
                gSPacketIn.WriteInt(sys_Users_Elf.TuGrowth);
                gSPacketIn.WriteInt(sys_Users_Elf.FengGrowth);
                if (GameProperties.VERSION >= 12100)
                {
                    gSPacketIn.WriteInt(sys_Users_Elf.GuangGrowth); //_loc23_.guangGrowth = _loc18_.readInt
                    gSPacketIn.WriteInt(sys_Users_Elf.AnGrowth); //_loc23_.anGrowth = _loc18_.readInt();
                }
                gSPacketIn.WriteString(sys_Users_Elf.SkillList);
            }
        }
        if (GameProperties.VERSION >= 11000)
        {
            if (player.SwornInfo != null)
            {
                gSPacketIn.WriteInt(player.SwornInfo.swornCount);
            }
            else
            {
                PlayerData playerData = new PlayerData();
                int val = playerData.Sys_Users_Sworn.FirstOrDefault((Sys_Users_Sworn p) => p.UserID == player.ID)?.swornCount ?? 0;
                gSPacketIn.WriteInt(val);
            }
        }
        if (GameProperties.VERSION >= 12100)
        {
            Sys_Users_Elf sys_Users_Elf = elfinfo.FirstOrDefault((Sys_Users_Elf e) => e.IsFight);
            gSPacketIn.WriteInt(sys_Users_Elf.EquipTempId); //_loc7_.elfEquipTempId = _loc18_.readInt();
            gSPacketIn.WriteInt(0); //_loc21_ = _loc18_.readInt();
            for (int i = 0; i < 0; i++)
            {
                gSPacketIn.WriteInt(0); //_loc12_ = _loc18_.readInt();
                gSPacketIn.WriteInt(0); //_loc7_.cultureMachiningDic[_loc12_] = _loc18_.readInt();
                gSPacketIn.WriteInt(0); //_loc7_.cultureQuenchingDic[_loc12_] = _loc18_.readInt();
                gSPacketIn.WriteString(""); //_loc7_.cultureQuenchingSkillDic[_loc12_] = _loc18_.readUTF();
            }
            gSPacketIn.WriteInt(0); //_loc7_.cultureAddDamage = _loc18_.readInt();
            gSPacketIn.WriteInt(0); //_loc7_.lingStoneStr = _loc18_.readUTF();
            gSPacketIn.WriteInt(0); //_loc7_.plotDamage = _loc18_.readInt();
            gSPacketIn.WriteInt(0); //_loc7_.plotDefence = _loc18_.readInt();
        }
        gSPacketIn.Compress();
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPetLacrarInfo(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(68);
        gSPacketIn.WriteByte(48);
        gSPacketIn.WriteInt(player.PetBag.lacresPet.Count);
        foreach (UsersPetLacrarInfo item in player.PetBag.lacresPet)
        {
            gSPacketIn.WriteInt(item.place);
            gSPacketIn.WriteBoolean(item.isSeal);
            if (!item.isSeal)
            {
                continue;
            }
            gSPacketIn.WriteInt(item.petId);
            gSPacketIn.WriteInt(item.lockArr.Count);
            foreach (StoreFineSealLockInfo item2 in item.lockArr)
            {
                gSPacketIn.WriteInt(item2.proType);
                if (item2.proType == 39)
                {
                    gSPacketIn.WriteInt(item2.skillId);
                    gSPacketIn.WriteInt(item2.min);
                    gSPacketIn.WriteInt(item2.max);
                }
                else
                {
                    gSPacketIn.WriteInt(item2.proNum);
                    gSPacketIn.WriteInt(item2.min);
                    gSPacketIn.WriteInt(item2.max);
                }
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendOpenWorshipTheMoon(bool open)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(281);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteBoolean(open);
        if (open)
        {
            gSPacketIn.WriteString(GameProperties.WorshipMoonBeginDate);
            gSPacketIn.WriteString(GameProperties.WorshipMoonEndDate);
        }
        SendTCP(gSPacketIn);
    }

    public void SendOpenEntertainmentMode()
    {
        GSPacketIn gSPacketIn = new GSPacketIn(145);
        gSPacketIn.WriteByte(71);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteDateTime(DateTime.Now);
        gSPacketIn.WriteDateTime(DateTime.Now.AddDays(7.0));
        SendTCP(gSPacketIn);
    }

    public void SendUesedFinishTime(int uesedFinishTime)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(86);
        gSPacketIn.WriteInt(uesedFinishTime);
        SendTCP(gSPacketIn);
    }

    public void SendDateTime()
    {
        GSPacketIn gSPacketIn = new GSPacketIn(5);
        gSPacketIn.WriteDateTime(DateTime.Now);
        SendTCP(gSPacketIn);
    }

    public void SendchangeHorse(int curIndex)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(260);
        gSPacketIn.WriteByte(2);
        gSPacketIn.WriteInt(curIndex);
        SendTCP(gSPacketIn);
    }

    public void SendPicCherishInfo(int playerId, MountDrawDataInfo[] draws)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(260, playerId);
        gSPacketIn.WriteByte(8);
        gSPacketIn.WriteInt(draws.Length);
        foreach (MountDrawDataInfo mountDrawDataInfo in draws)
        {
            gSPacketIn.WriteInt(mountDrawDataInfo.ID);
            gSPacketIn.WriteInt(mountDrawDataInfo.Active);
            if (GameProperties.VERSION >= 7600)
            {
                gSPacketIn.WriteDateTime(DateTime.Now);
                gSPacketIn.WriteInt(0);
            }
            if (GameProperties.VERSION >= 9800)
            {
                gSPacketIn.WriteInt(mountDrawDataInfo.qiLingGrade);
            }
        }
        SendTCP(gSPacketIn);
    }

    public void SendHorseInitAllData(MountDataInfo info, MountSkillDataInfo[] skills)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(260, info.UserID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteInt(info.curUseHorse);
        gSPacketIn.WriteInt(info.curLevel);
        gSPacketIn.WriteInt(info.curExp);
        if (GameProperties.VERSION >= 7600)
        {
            gSPacketIn.WriteInt(0);
        }
        gSPacketIn.WriteInt(skills.Length);
        foreach (MountSkillDataInfo mountSkillDataInfo in skills)
        {
            gSPacketIn.WriteInt(mountSkillDataInfo.SKillID);
            gSPacketIn.WriteInt(mountSkillDataInfo.Exp);
            gSPacketIn.WriteInt(mountSkillDataInfo.IsUse);
        }
        SendTCP(gSPacketIn);
    }

    public void SendUpHorse(bool isHasLevelUp, int grade, int exp)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(260);
        gSPacketIn.WriteByte(3);
        gSPacketIn.WriteBoolean(isHasLevelUp);
        gSPacketIn.WriteInt(grade);
        gSPacketIn.WriteInt(exp);
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendDailyAward(PlayerInfo player)
    {
        bool val = false;
        if (DateTime.Now.Date != player.LastAward.Date)
        {
            val = true;
        }
        GSPacketIn gSPacketIn = new GSPacketIn(13, player.ID);
        gSPacketIn.WriteBoolean(val);
        gSPacketIn.WriteInt(0);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateGoodsCount(PlayerInfo player, ShopItemInfo[] BagList, ShopItemInfo[] ConsoList)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(168, player.ID);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(player.ConsortiaID);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(0);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateRoomList(List<BaseRoom> roomlist)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(9);
        gSPacketIn.WriteInt(roomlist.Count);
        int num = ((roomlist.Count < 8) ? roomlist.Count : 8);
        gSPacketIn.WriteInt(num);
        for (int i = 0; i < num; i++)
        {
            BaseRoom baseRoom = roomlist[i];
            gSPacketIn.WriteInt(baseRoom.RoomId);
            gSPacketIn.WriteByte((byte)baseRoom.RoomType);
            gSPacketIn.WriteByte(baseRoom.TimeMode);
            gSPacketIn.WriteByte((byte)baseRoom.PlayerCount);
            gSPacketIn.WriteByte((byte)baseRoom.viewerCnt);
            gSPacketIn.WriteByte((byte)baseRoom.maxViewerCnt);
            gSPacketIn.WriteByte((byte)baseRoom.PlacesCount);
            gSPacketIn.WriteBoolean((!string.IsNullOrEmpty(baseRoom.Password)) ? true : false);
            gSPacketIn.WriteInt(baseRoom.MapId);
            gSPacketIn.WriteBoolean(baseRoom.IsPlaying);
            gSPacketIn.WriteString(baseRoom.Name);
            gSPacketIn.WriteByte((byte)baseRoom.GameType);
            gSPacketIn.WriteByte((byte)baseRoom.HardLevel);
            gSPacketIn.WriteInt(baseRoom.LevelLimits);
            gSPacketIn.WriteBoolean(baseRoom.isOpenBoss);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendSceneAddPlayer(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(18, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
        if (GameProperties.VERSION >= 8200)
        {
            gSPacketIn.WriteInt(player.PlayerCharacter.ddtKingGrade);
        }
        gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
        gSPacketIn.WriteString(player.PlayerCharacter.NickName);
        gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
        gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
        gSPacketIn.WriteString(player.PlayerCharacter.ConsortiaName);
        gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
        gSPacketIn.WriteInt(player.PlayerCharacter.Win);
        gSPacketIn.WriteInt(player.PlayerCharacter.Total);
        gSPacketIn.WriteInt(player.PlayerCharacter.Escape);
        gSPacketIn.WriteInt(player.PlayerCharacter.ConsortiaID);
        gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
        gSPacketIn.WriteBoolean(player.PlayerCharacter.IsMarried);
        if (player.PlayerCharacter.IsMarried)
        {
            gSPacketIn.WriteInt(player.PlayerCharacter.SpouseID);
            gSPacketIn.WriteString(player.PlayerCharacter.SpouseName);
        }
        gSPacketIn.WriteString(player.PlayerCharacter.UserName);
        gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
        if (player.PlayerCharacter.AcademyInfo == null)
        {
            gSPacketIn.WriteInt(0);
        }
        else
        {
            gSPacketIn.WriteInt(player.PlayerCharacter.AcademyInfo.ApprenticeshipState);
        }
        gSPacketIn.WriteBoolean(player.PlayerCharacter.IsOldPlayer);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendSceneRemovePlayer(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(21, player.PlayerCharacter.ID);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendGameMissionStart()
    {
        GSPacketIn gSPacketIn = new GSPacketIn(82);
        gSPacketIn.WriteBoolean(val: true);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendGameMissionPrepare()
    {
        GSPacketIn gSPacketIn = new GSPacketIn(116);
        gSPacketIn.WriteBoolean(val: true);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomPlayerAdd(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94, player.PlayerId);
        gSPacketIn.WriteByte(4);
        bool val = player.CurrentRoom.Game != null;
        gSPacketIn.WriteBoolean(val);
        gSPacketIn.WriteByte((byte)player.CurrentRoomIndex);
        gSPacketIn.WriteByte((byte)player.CurrentRoomTeam);
        gSPacketIn.WriteBoolean(val: false);
        gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
        if (GameProperties.VERSION >= 8200)
        {
            gSPacketIn.WriteInt(player.PlayerCharacter.ddtKingGrade);
        }
        gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
        int num = player.PlayerCharacter.TempHideCategory(4, hide: true);
        if (num == 0)
        {
            num = player.PlayerCharacter.Hide;
        }
        gSPacketIn.WriteInt(num);
        gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
        gSPacketIn.WriteInt((int)player.PingTime / 1000 / 10);
        gSPacketIn.WriteInt(player.ZoneId);
        gSPacketIn.WriteInt(player.Actives.Info.activityTanabataNum);
        if (GameProperties.VERSION >= 11000)
        {
            if (player.PlayerCharacter.IsHideElf)
            {
                gSPacketIn.WriteInt(0);
            }
            else
            {
                gSPacketIn.WriteInt((player.PlayerCharacter?.ElfInfo?.FirstOrDefault((Sys_Users_Elf e) => e.IsFight)?.TempID).GetValueOrDefault());
            }
            gSPacketIn.WriteString(player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.IsFight)?.SkillList ?? "");
        }
        gSPacketIn.WriteInt(player.PlayerCharacter.ID);
        gSPacketIn.WriteString(player.PlayerCharacter.NickName);
        gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
        gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
        gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
        gSPacketIn.WriteString(player.PlayerCharacter.Style);
        gSPacketIn.WriteString(player.PlayerCharacter.Colors);
        gSPacketIn.WriteString(player.PlayerCharacter.Skin);
        gSPacketIn.WriteInt((player.MainWeapon == null) ? 7008 : player.MainWeapon.TemplateID);
        if (player.SecondWeapon == null)
        {
            gSPacketIn.WriteInt(0);
        }
        else
        {
            gSPacketIn.WriteInt(player.SecondWeapon.TemplateID);
        }
        gSPacketIn.WriteInt(player.PlayerCharacter.ConsortiaID);
        gSPacketIn.WriteString(player.PlayerCharacter.ConsortiaName);
        gSPacketIn.WriteInt(player.PlayerCharacter.badgeID);
        gSPacketIn.WriteInt(player.PlayerCharacter.Win);
        gSPacketIn.WriteInt(player.PlayerCharacter.Total);
        gSPacketIn.WriteInt(player.PlayerCharacter.Escape);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteBoolean(player.PlayerCharacter.IsMarried);
        if (player.PlayerCharacter.IsMarried)
        {
            gSPacketIn.WriteInt(player.PlayerCharacter.SpouseID);
            gSPacketIn.WriteString(player.PlayerCharacter.SpouseName);
        }
        gSPacketIn.WriteString(player.PlayerCharacter.UserName);
        gSPacketIn.WriteInt(player.PlayerCharacter.Nimbus);
        gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
        gSPacketIn.WriteInt(player.PlayerCharacter.AcademyInfo.ApprenticeshipState);
        gSPacketIn.WriteInt(player.PlayerCharacter.AcademyInfo.MasterID);
        gSPacketIn.WriteString(player.PlayerCharacter.AcademyInfo.MasterOrApprentices);
        gSPacketIn.WriteInt(player.PlayerCharacter.AcademyInfo.GraduatesCount);
        gSPacketIn.WriteString(player.PlayerCharacter.AcademyInfo.HonourOfMaster);
        gSPacketIn.WriteBoolean(val: false);
        gSPacketIn.WriteInt(0);
        if (GameProperties.VERSION >= 7600)
        {
            gSPacketIn.WriteInt(player.PlayerCharacter.guardCoreID);
        }
        gSPacketIn.WriteBoolean(player.PlayerCharacter.IsOldPlayer);
        if (player.Pet == null)
        {
            gSPacketIn.WriteInt(0);
        }
        else
        {
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(player.Pet.Place);
            gSPacketIn.WriteInt(player.Pet.TemplateID);
            gSPacketIn.WriteInt(player.Pet.ID);
            gSPacketIn.WriteString(player.Pet.Name);
            gSPacketIn.WriteInt(player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(player.Pet.Level);
            string[] array = player.Pet.SkillEquip.Split('|');
            gSPacketIn.WriteInt(array.Length);
            string[] array2 = array;
            foreach (string text in array2)
            {
                gSPacketIn.WriteInt(int.Parse(text.Split(',')[1]));
                gSPacketIn.WriteInt(int.Parse(text.Split(',')[0]));
            }
        }
        if (GameProperties.VERSION >= 8100 && GameProperties.VERSION < 8800)
        {
            gSPacketIn.WriteInt(player.TrailElite.Data.trailEliteLevel);
        }
        else if (GameProperties.VERSION >= 8800)
        {
            gSPacketIn.WriteInt(player.Extra.Info.curcentRoomBordenTemplateId);
        }
        if (GameProperties.VERSION >= 11000)
        {
            if (player.PlayerCharacter.IsHideElf)
            {
                gSPacketIn.WriteInt(0);
            }
            else
            {
                gSPacketIn.WriteInt(player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.IsFight)?.TempID ?? 0);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomPlayerRemove(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94, player.PlayerId);
        gSPacketIn.WriteByte(5);
        gSPacketIn.Parameter1 = player.PlayerId;
        gSPacketIn.WriteInt(player.ZoneId);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(15);
        for (int i = 0; i < states.Length; i++)
        {
            gSPacketIn.WriteByte(states[i]);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(10);
        for (int i = 0; i < states.Length; i++)
        {
            gSPacketIn.WriteInt(states[i]);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94, player.PlayerId);
        gSPacketIn.WriteByte(6);
        gSPacketIn.WriteByte((byte)player.CurrentRoomTeam);
        gSPacketIn.WriteByte((byte)player.CurrentRoomIndex);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomCreate(BaseRoom room)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(0);
        gSPacketIn.WriteInt(room.RoomId);
        gSPacketIn.WriteByte((byte)room.RoomType);
        gSPacketIn.WriteByte((byte)room.HardLevel);
        gSPacketIn.WriteByte(room.TimeMode);
        gSPacketIn.WriteByte((byte)room.PlayerCount);
        gSPacketIn.WriteByte((byte)room.viewerCnt);
        gSPacketIn.WriteByte((byte)room.PlacesCount);
        gSPacketIn.WriteBoolean(!string.IsNullOrEmpty(room.Password));
        gSPacketIn.WriteInt(room.MapId);
        gSPacketIn.WriteBoolean(room.IsPlaying);
        gSPacketIn.WriteString(room.Name);
        gSPacketIn.WriteByte((byte)room.GameType);
        gSPacketIn.WriteInt(room.LevelLimits);
        gSPacketIn.WriteBoolean(room.isCrosszone);
        gSPacketIn.WriteBoolean(room.isWithinLeageTime);
        gSPacketIn.WriteBoolean(room.isOpenBoss);
        gSPacketIn.WriteString(room.Pic);
        gSPacketIn.WriteBoolean(room.IsShowLoading);
        if (GameProperties.VERSION >= 12100)
        {
            gSPacketIn.WriteInt(0);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendSingleRoomCreate(BaseRoom room)
    {
        return SendSingleRoomCreate(room, null);
    }

    public GSPacketIn SendSingleRoomCreate(BaseRoom room, GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(18);
        gSPacketIn.WriteInt(room.RoomId);
        gSPacketIn.WriteByte((byte)room.RoomType);
        gSPacketIn.WriteBoolean(room.IsPlaying);
        gSPacketIn.WriteByte((byte)room.GameType);
        gSPacketIn.WriteInt(room.MapId);
        gSPacketIn.WriteBoolean(room.isCrosszone);
        gSPacketIn.WriteInt(room.ZoneId);
        gSPacketIn.WriteBoolean(room.IsShowLoading);
        gSPacketIn.WriteByte((byte)room.HardLevel);
        if (player == null)
        {
            SendTCP(gSPacketIn);
        }
        else
        {
            player.SendTCP(gSPacketIn);
        }
        return gSPacketIn;
    }

    public GSPacketIn SendRoomLoginResult(bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomRobotInfo(bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(24);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomPairUpStart(BaseRoom room)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(13);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomType(GamePlayer player, BaseRoom game)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(12);
        gSPacketIn.WriteByte((byte)game.GameStyle);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRoomPairUpCancel(BaseRoom room)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(11);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(66, player.PlayerCharacter.ID);
        gSPacketIn.WriteByte((byte)place);
        gSPacketIn.WriteInt(goodsID);
        gSPacketIn.WriteString(style);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    //__settingRoom PVE
    public GSPacketIn SendGameRoomSetupChange(BaseRoom room)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(94);
        gSPacketIn.WriteByte(2);
        gSPacketIn.WriteBoolean(room.isOpenBoss);
        if (room.isOpenBoss)
        {
            gSPacketIn.WriteString(room.Pic);
        }
        if (GameProperties.VERSION >= 8700)
        {
            gSPacketIn.WriteInt((int)room.GameType);
        }
        gSPacketIn.WriteInt(room.MapId);
        gSPacketIn.WriteByte((byte)room.RoomType);
        gSPacketIn.WriteString((room.Password == null) ? "" : room.Password);
        gSPacketIn.WriteString((room.Name == null) ? "GunnyII" : room.Name);
        gSPacketIn.WriteByte(room.TimeMode);
        gSPacketIn.WriteByte((byte)room.HardLevel);
        gSPacketIn.WriteInt(room.LevelLimits);
        gSPacketIn.WriteBoolean(room.isCrosszone);
        if (GameProperties.VERSION >= 12100)
        {
            gSPacketIn.WriteInt(2);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendSetDressModelArr(int playerId, Dictionary<int, List<ItemInfo>> dresses)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(237, playerId);
        gSPacketIn.WriteByte(1);
        if (GameProperties.VERSION >= 8700)
        {
            gSPacketIn.WriteInt(6);
        }
        gSPacketIn.WriteInt(dresses.Count);
        foreach (int key in dresses.Keys)
        {
            List<ItemInfo> list = dresses[key];
            gSPacketIn.WriteInt(key);
            gSPacketIn.WriteInt(dresses[key].Count);
            foreach (ItemInfo item in list)
            {
                gSPacketIn.WriteInt(item.ItemID);
                gSPacketIn.WriteInt(item.TemplateID);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendCurentDressModel(int playerId, int current)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(237, playerId);
        gSPacketIn.WriteByte(2);
        gSPacketIn.WriteInt(current);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendAvatarColectionAllInfo(int PlayerId, Dictionary<int, UserAvatarColectionInfo> infos)
    {
        Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
        List<int> list = new List<int>();
        List<int> list2 = new List<int>();
        foreach (int key in infos.Keys)
        {
            if (!string.IsNullOrEmpty(infos[key].ActiveDress))
            {
                if (infos[key].Sex == 3)
                {
                    list2.Add(key);
                }
                else
                {
                    list.Add(key);
                }
            }
        }
        GSPacketIn gSPacketIn = new GSPacketIn(402, PlayerId);
        gSPacketIn.WriteByte(5);
        if (GameProperties.VERSION >= 8300)
        {
            dictionary.Add(1, list);
            dictionary.Add(2, list2);
            gSPacketIn.WriteInt(dictionary.Count);
            foreach (int key2 in dictionary.Keys)
            {
                gSPacketIn.WriteInt(key2);
                gSPacketIn.WriteInt(dictionary[key2].Count);
                foreach (int item in dictionary[key2])
                {
                    gSPacketIn.WriteInt(item);
                    gSPacketIn.WriteInt(infos[item].Sex);
                    gSPacketIn.WriteInt(infos[item].CurrentGroup.Length);
                    string[] currentGroup = infos[item].CurrentGroup;
                    foreach (string s in currentGroup)
                    {
                        gSPacketIn.WriteInt(int.Parse(s));
                    }
                    gSPacketIn.WriteDateTime(infos[item].endTime);
                }
            }
        }
        else
        {
            gSPacketIn.WriteInt(list.Count);
            foreach (int item2 in list)
            {
                gSPacketIn.WriteInt(item2);
                gSPacketIn.WriteInt(infos[item2].Sex);
                gSPacketIn.WriteInt(infos[item2].CurrentGroup.Length);
                string[] currentGroup2 = infos[item2].CurrentGroup;
                foreach (string s2 in currentGroup2)
                {
                    gSPacketIn.WriteInt(int.Parse(s2));
                }
                gSPacketIn.WriteDateTime(infos[item2].endTime);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isbind, int MinValid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(76, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(previewItemList.Count);
        foreach (KeyValuePair<int, double> previewItem in previewItemList)
        {
            gSPacketIn.WriteInt(previewItem.Key);
            gSPacketIn.WriteInt(MinValid);
            int num = (int)previewItem.Value;
            gSPacketIn.WriteInt((num > 100) ? 100 : ((num >= 0) ? num : 0));
        }
        gSPacketIn.WriteBoolean(isbind);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendFusionResult(GamePlayer player, bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(78, player.PlayerCharacter.ID);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendOpenHoleComplete(GamePlayer player, int type, bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(217, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(type);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendOpenCloseBraveDoor(GamePlayer player, bool isOpen)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(315, player.PlayerId);
        gSPacketIn.WriteBoolean(isOpen);
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendRuneOpenPackage(GamePlayer player, int rand)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(121, player.PlayerCharacter.ID);
        gSPacketIn.WriteByte(3);
        gSPacketIn.WriteInt(rand);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, ItemInfo item)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(111, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(templateid);
        gSPacketIn.WriteInt(item.ValidDate);
        gSPacketIn.WriteBoolean(isbind);
        gSPacketIn.WriteInt(item.AgilityCompose);
        gSPacketIn.WriteInt(item.AttackCompose);
        gSPacketIn.WriteInt(item.DefendCompose);
        gSPacketIn.WriteInt(item.LuckCompose);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots)
    {
        if (m_gameClient.Player == null)
        {
            return;
        }
        int num = updatedSlots.Length;
        if (num > 40)
        {
            IEnumerable<IEnumerable<int>> enumerable = updatedSlots.Split(40);
            {
                foreach (IEnumerable<int> item in enumerable)
                {
                    SendUpdateInventorySlot(bag, item.ToArray());
                }
                return;
            }
        }
        GSPacketIn gSPacketIn = new GSPacketIn(64, m_gameClient.Player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(bag.BagType);
        gSPacketIn.WriteInt(num);
        foreach (int num2 in updatedSlots)
        {
            gSPacketIn.WriteInt(num2);
            ItemInfo itemAt = bag.GetItemAt(num2);
            if (itemAt == null)
            {
                gSPacketIn.WriteBoolean(val: false);
                continue;
            }
            gSPacketIn.WriteBoolean(val: true);
            gSPacketIn.WriteInt(itemAt.UserID);
            gSPacketIn.WriteInt(itemAt.ItemID);
            gSPacketIn.WriteInt(itemAt.Count);
            gSPacketIn.WriteInt(itemAt.Place);
            gSPacketIn.WriteInt(itemAt.TemplateID);
            gSPacketIn.WriteInt(itemAt.AttackCompose);
            gSPacketIn.WriteInt(itemAt.DefendCompose);
            gSPacketIn.WriteInt(itemAt.AgilityCompose);
            gSPacketIn.WriteInt(itemAt.LuckCompose);
            gSPacketIn.WriteInt(itemAt.StrengthenLevel);
            gSPacketIn.WriteInt(itemAt.StrengthenExp);
            gSPacketIn.WriteBoolean(itemAt.IsBinds);
            gSPacketIn.WriteBoolean(itemAt.IsJudge);
            gSPacketIn.WriteDateTime(itemAt.BeginDate);
            gSPacketIn.WriteInt(itemAt.ValidDate);
            gSPacketIn.WriteString((itemAt.Color == null) ? "" : itemAt.Color);
            gSPacketIn.WriteString((itemAt.Skin == null) ? "" : itemAt.Skin);
            gSPacketIn.WriteBoolean(itemAt.IsUsed);
            gSPacketIn.WriteInt(itemAt.Hole1);
            gSPacketIn.WriteInt(itemAt.Hole2);
            gSPacketIn.WriteInt(itemAt.Hole3);
            gSPacketIn.WriteInt(itemAt.Hole4);
            gSPacketIn.WriteInt(itemAt.Hole5);
            gSPacketIn.WriteInt(itemAt.Hole6);
            gSPacketIn.WriteString(itemAt.Pic);
            gSPacketIn.WriteInt(itemAt.RefineryLevel);
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteInt(itemAt.StrengthenTimes);
            gSPacketIn.WriteByte((byte)itemAt.Hole5Level);
            gSPacketIn.WriteInt(itemAt.Hole5Exp);
            gSPacketIn.WriteByte((byte)itemAt.Hole6Level);
            gSPacketIn.WriteInt(itemAt.Hole6Exp);
            gSPacketIn.WriteInt(itemAt.curExp);
            gSPacketIn.WriteBoolean(itemAt.cellLocked);
            gSPacketIn.WriteBoolean(itemAt.GoldValidDate());
            if (itemAt.GoldValidDate())
            {
                gSPacketIn.WriteInt(itemAt.goldValidDate);
                gSPacketIn.WriteDateTime(itemAt.goldBeginTime);
            }
            gSPacketIn.WriteString(itemAt.latentEnergyCurStr);
            gSPacketIn.WriteString(itemAt.latentEnergyCurStr);
            gSPacketIn.WriteDateTime(itemAt.latentEnergyEndTime);
            if (Equip.isMagicStone(itemAt.Template.CategoryID))
            {
                gSPacketIn.WriteInt(itemAt.MagicAttack);
                gSPacketIn.WriteInt(itemAt.MagicDefence);
            }
            else
            {
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
            }
            gSPacketIn.WriteBoolean(itemAt.goodsLock);
            gSPacketIn.WriteInt(itemAt.MagicExp);
            gSPacketIn.WriteInt(itemAt.MagicLevel);
            if (GameProperties.VERSION >= 10200)
            {
                gSPacketIn.WriteInt(itemAt.enchantSpiritLevel);
            }
            if (GameProperties.VERSION >= 12100)
            {
                //Cần sửa
                gSPacketIn.WriteString("");//_loc5_.magicJadeStr = _loc6_.readUTF();
            }
        }
        SendTCP(gSPacketIn);
    }

    public void SendPlayerCardEquip(PlayerInfo player, List<UsersCardInfo> cards)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(216, player.ID);
        gSPacketIn.WriteInt(player.ID);
        gSPacketIn.WriteInt(player.ZoneID);
        gSPacketIn.WriteInt(cards.Count);
        foreach (UsersCardInfo card in cards)
        {
            gSPacketIn.WriteInt(card.Place);
            if (card.TemplateID == 0)
            {
                gSPacketIn.WriteBoolean(val: false);
                continue;
            }
            gSPacketIn.WriteBoolean(val: true);
            gSPacketIn.WriteInt(card.CardID);
            gSPacketIn.WriteInt(card.CardType);
            gSPacketIn.WriteInt(card.UserID);
            gSPacketIn.WriteInt(card.Place);
            gSPacketIn.WriteInt(card.TemplateID);
            gSPacketIn.WriteBoolean(card.isFirstGet);
            gSPacketIn.WriteInt(card.Attack);
            gSPacketIn.WriteInt(card.Defence);
            gSPacketIn.WriteInt(card.Agility);
            gSPacketIn.WriteInt(card.Luck);
            gSPacketIn.WriteInt(card.Damage);
            gSPacketIn.WriteInt(card.Guard);
        }
        SendTCP(gSPacketIn);
    }

    public void SendPlayerCardInfo(CardInventory bag, int[] updatedSlots)
    {
        if (m_gameClient.Player == null)
        {
            return;
        }
        GSPacketIn gSPacketIn = new GSPacketIn(216, m_gameClient.Player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(m_gameClient.Player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(m_gameClient.Player.ZoneId);
        gSPacketIn.WriteInt(updatedSlots.Length);
        foreach (int num in updatedSlots)
        {
            gSPacketIn.WriteInt(num);
            UsersCardInfo itemAt = bag.GetItemAt(num);
            if (itemAt == null)
            {
                gSPacketIn.WriteBoolean(val: false);
                continue;
            }
            gSPacketIn.WriteBoolean(val: true);
            gSPacketIn.WriteInt(itemAt.CardID);
            gSPacketIn.WriteInt(itemAt.CardType);
            gSPacketIn.WriteInt(itemAt.UserID);
            gSPacketIn.WriteInt(itemAt.Place);
            gSPacketIn.WriteInt(itemAt.TemplateID);
            gSPacketIn.WriteBoolean(itemAt.isFirstGet);
            gSPacketIn.WriteInt(itemAt.Attack);
            gSPacketIn.WriteInt(itemAt.Defence);
            gSPacketIn.WriteInt(itemAt.Agility);
            gSPacketIn.WriteInt(itemAt.Luck);
            gSPacketIn.WriteInt(itemAt.Damage);
            gSPacketIn.WriteInt(itemAt.Guard);
        }
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendGetCard(PlayerInfo player, UsersCardInfo card)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(216, player.ID);
        gSPacketIn.WriteInt(player.ID);
        gSPacketIn.WriteInt(player.ZoneID);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(card.Place);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteInt(card.CardID);
        gSPacketIn.WriteInt(card.CardType);
        gSPacketIn.WriteInt(card.UserID);
        gSPacketIn.WriteInt(card.Place);
        gSPacketIn.WriteInt(card.TemplateID);
        gSPacketIn.WriteBoolean(card.isFirstGet);
        gSPacketIn.WriteInt(card.Attack);
        gSPacketIn.WriteInt(card.Defence);
        gSPacketIn.WriteInt(card.Agility);
        gSPacketIn.WriteInt(card.Luck);
        gSPacketIn.WriteInt(card.Damage);
        gSPacketIn.WriteInt(card.Guard);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendPlayerCardSlot(PlayerInfo player, List<UsersCardInfo> cardslots)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(170, player.ID);
        gSPacketIn.WriteInt(player.ID);
        gSPacketIn.WriteInt(player.ZoneID);
        gSPacketIn.WriteInt(player.CardSoul);
        gSPacketIn.WriteInt(cardslots.Count);
        foreach (UsersCardInfo cardslot in cardslots)
        {
            gSPacketIn.WriteInt(cardslot.Place);
            gSPacketIn.WriteInt(cardslot.Type);
            gSPacketIn.WriteInt(cardslot.Level);
            gSPacketIn.WriteInt(cardslot.CardGP);
        }
        SendTCP(gSPacketIn);
    }

    public void SendCardAchievementUpdate(int playerId, int[] Property, List<CardAchievementDataInfo> cardAchievements)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(362, playerId);
        gSPacketIn.WriteInt(Property[0]);
        gSPacketIn.WriteInt(Property[1]);
        gSPacketIn.WriteInt(Property[2]);
        gSPacketIn.WriteInt(Property[3]);
        gSPacketIn.WriteInt(Property[4]);
        gSPacketIn.WriteInt(Property[5]);
        gSPacketIn.WriteInt(Property[6]);
        gSPacketIn.WriteInt(Property[7]);
        gSPacketIn.WriteInt(cardAchievements.Count);
        foreach (CardAchievementDataInfo cardAchievement in cardAchievements)
        {
            gSPacketIn.WriteInt(cardAchievement.IsComplete);
            gSPacketIn.WriteInt(cardAchievement.AchievementID);
        }
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendGetPlayerCard(int playerId)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(18, playerId);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerCardSlot(PlayerInfo player, UsersCardInfo card)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(170, player.ID);
        gSPacketIn.WriteInt(player.ID);
        gSPacketIn.WriteInt(player.ZoneID);
        gSPacketIn.WriteInt(player.CardSoul);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(card.Place);
        gSPacketIn.WriteInt(card.Type);
        gSPacketIn.WriteInt(card.Level);
        gSPacketIn.WriteInt(card.CardGP);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerCardReset(PlayerInfo player, List<int> points)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(196, player.ID);
        gSPacketIn.WriteInt(points.Count);
        foreach (int point in points)
        {
            gSPacketIn.WriteInt(point);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerCardSoul(PlayerInfo player, bool isSoul, int soul)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(208, player.ID);
        gSPacketIn.WriteBoolean(isSoul);
        if (isSoul)
        {
            gSPacketIn.WriteInt(soul);
            gSPacketIn.WriteInt(player.GetSoulCount);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendEatPetsInfo(EatPetsInfo info)
    {
        if (info == null)
        {
            return null;
        }
        GSPacketIn gSPacketIn = new GSPacketIn(68, m_gameClient.Player.PlayerId);
        gSPacketIn.WriteByte(33);
        gSPacketIn.WriteInt(info.weaponExp);
        gSPacketIn.WriteInt(info.weaponLevel);
        gSPacketIn.WriteInt(info.clothesExp);
        gSPacketIn.WriteInt(info.clothesLevel);
        gSPacketIn.WriteInt(info.hatExp);
        gSPacketIn.WriteInt(info.hatLevel);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateUserPet(PetInventory bag, int[] slots)
    {
        if (m_gameClient.Player == null)
        {
            return null;
        }
        GSPacketIn gSPacketIn = new GSPacketIn(68, m_gameClient.Player.PlayerId);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteInt(m_gameClient.Player.PlayerId);
        gSPacketIn.WriteInt(m_gameClient.Player.ZoneId);
        gSPacketIn.WriteInt(slots.Length);
        foreach (int num in slots)
        {
            gSPacketIn.WriteInt(num);
            UsersPetInfo petAt = bag.GetPetAt(num);
            if (petAt == null)
            {
                gSPacketIn.WriteBoolean(val: false);
                continue;
            }
            gSPacketIn.WriteBoolean(val: true);
            gSPacketIn.WriteInt(petAt.ID);
            gSPacketIn.WriteInt(petAt.TemplateID);
            gSPacketIn.WriteString(petAt.Name);
            gSPacketIn.WriteInt(petAt.UserID);
            gSPacketIn.WriteInt(petAt.TotalAttack);
            gSPacketIn.WriteInt(petAt.TotalDefence);
            gSPacketIn.WriteInt(petAt.TotalLuck);
            gSPacketIn.WriteInt(petAt.TotalAgility);
            gSPacketIn.WriteInt(petAt.TotalBlood);
            gSPacketIn.WriteInt(petAt.Damage);
            gSPacketIn.WriteInt(petAt.Guard);
            gSPacketIn.WriteInt(petAt.AttackGrow);
            gSPacketIn.WriteInt(petAt.DefenceGrow);
            gSPacketIn.WriteInt(petAt.LuckGrow);
            gSPacketIn.WriteInt(petAt.AgilityGrow);
            gSPacketIn.WriteInt(petAt.BloodGrow);
            gSPacketIn.WriteInt(petAt.DamageGrow);
            gSPacketIn.WriteInt(petAt.GuardGrow);
            gSPacketIn.WriteInt(petAt.Level);
            gSPacketIn.WriteInt(petAt.GP);
            gSPacketIn.WriteInt(petAt.MaxGP);
            gSPacketIn.WriteInt(petAt.Hunger);
            gSPacketIn.WriteInt(petAt.breakGrade);
            gSPacketIn.WriteInt(petAt.breakBlood);
            gSPacketIn.WriteInt(petAt.breakAttack);
            gSPacketIn.WriteInt(petAt.breakDefence);
            gSPacketIn.WriteInt(petAt.breakAgility);
            gSPacketIn.WriteInt(petAt.breakLuck);
            gSPacketIn.WriteInt(petAt.PetHappyStar);
            gSPacketIn.WriteInt(petAt.MP);
            if (petAt.Skill != null)
            {
                string[] array = petAt.Skill.Split('|');
                gSPacketIn.WriteInt(array.Length);
                string[] array2 = array;
                foreach (string text in array2)
                {
                    gSPacketIn.WriteInt(int.Parse(text.Split(',')[0]));
                    gSPacketIn.WriteInt(int.Parse(text.Split(',')[1]));
                }
            }
            else
            {
                gSPacketIn.WriteInt(0);
            }
            if (petAt.SkillEquip != null)
            {
                string[] array3 = petAt.SkillEquip.Split('|');
                gSPacketIn.WriteInt(array3.Length);
                string[] array4 = array3;
                foreach (string text2 in array4)
                {
                    gSPacketIn.WriteInt(int.Parse(text2.Split(',')[1]));
                    gSPacketIn.WriteInt(int.Parse(text2.Split(',')[0]));
                }
            }
            else
            {
                gSPacketIn.WriteInt(0);
            }
            gSPacketIn.WriteBoolean(petAt.IsEquip);
            gSPacketIn.WriteInt(petAt.PetEquips.Count);
            foreach (PetEquipInfo petEquip in petAt.PetEquips)
            {
                gSPacketIn.WriteInt(petEquip.eqType);
                gSPacketIn.WriteInt(petEquip.eqTemplateID);
                gSPacketIn.WriteDateTime(petEquip.startTime);
                gSPacketIn.WriteInt(petEquip.ValidDate);
                if (GameProperties.VERSION >= 8200)
                {
                    gSPacketIn.WriteInt(petEquip.AttackCompose);
                    gSPacketIn.WriteInt(petEquip.DefendCompose);
                    gSPacketIn.WriteInt(petEquip.AgilityCompose);
                    gSPacketIn.WriteInt(petEquip.LuckCompose);
                    gSPacketIn.WriteInt(petEquip.itemID);
                }
            }
            gSPacketIn.WriteInt(petAt.currentStarExp);
            if (GameProperties.VERSION >= 10100)
            {
                gSPacketIn.WriteBoolean(petAt.IsSeal);
            }
        }
        gSPacketIn.WriteInt(m_gameClient.Player.PetBag.EatPets.weaponLevel);
        gSPacketIn.WriteInt(m_gameClient.Player.PetBag.EatPets.clothesLevel);
        gSPacketIn.WriteInt(m_gameClient.Player.PetBag.EatPets.hatLevel);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPetInfo(int id, int zoneId, UsersPetInfo[] pets, EatPetsInfo eatpet)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(68, id);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteInt(id);
        gSPacketIn.WriteInt(zoneId);
        gSPacketIn.WriteInt(pets.Length);
        foreach (UsersPetInfo usersPetInfo in pets)
        {
            gSPacketIn.WriteInt(usersPetInfo.Place);
            gSPacketIn.WriteBoolean(val: true);
            gSPacketIn.WriteInt(usersPetInfo.ID);
            gSPacketIn.WriteInt(usersPetInfo.TemplateID);
            gSPacketIn.WriteString(usersPetInfo.Name);
            gSPacketIn.WriteInt(usersPetInfo.UserID);
            gSPacketIn.WriteInt(usersPetInfo.TotalAttack);
            gSPacketIn.WriteInt(usersPetInfo.TotalDefence);
            gSPacketIn.WriteInt(usersPetInfo.TotalLuck);
            gSPacketIn.WriteInt(usersPetInfo.TotalAgility);
            gSPacketIn.WriteInt(usersPetInfo.TotalBlood);
            gSPacketIn.WriteInt(usersPetInfo.Damage);
            gSPacketIn.WriteInt(usersPetInfo.Guard);
            gSPacketIn.WriteInt(usersPetInfo.AttackGrow);
            gSPacketIn.WriteInt(usersPetInfo.DefenceGrow);
            gSPacketIn.WriteInt(usersPetInfo.LuckGrow);
            gSPacketIn.WriteInt(usersPetInfo.AgilityGrow);
            gSPacketIn.WriteInt(usersPetInfo.BloodGrow);
            gSPacketIn.WriteInt(usersPetInfo.DamageGrow);
            gSPacketIn.WriteInt(usersPetInfo.GuardGrow);
            gSPacketIn.WriteInt(usersPetInfo.Level);
            gSPacketIn.WriteInt(usersPetInfo.GP);
            gSPacketIn.WriteInt(usersPetInfo.MaxGP);
            gSPacketIn.WriteInt(usersPetInfo.Hunger);
            gSPacketIn.WriteInt(usersPetInfo.breakGrade);
            gSPacketIn.WriteInt(usersPetInfo.breakBlood);
            gSPacketIn.WriteInt(usersPetInfo.breakAttack);
            gSPacketIn.WriteInt(usersPetInfo.breakDefence);
            gSPacketIn.WriteInt(usersPetInfo.breakAgility);
            gSPacketIn.WriteInt(usersPetInfo.breakLuck);
            gSPacketIn.WriteInt(usersPetInfo.PetHappyStar);
            gSPacketIn.WriteInt(usersPetInfo.MP);
            string[] array = usersPetInfo.Skill.Split('|');
            string[] array2 = usersPetInfo.SkillEquip.Split('|');
            gSPacketIn.WriteInt(array.Length);
            string[] array3 = array;
            foreach (string text in array3)
            {
                gSPacketIn.WriteInt(int.Parse(text.Split(',')[0]));
                gSPacketIn.WriteInt(int.Parse(text.Split(',')[1]));
            }
            gSPacketIn.WriteInt(array2.Length);
            string[] array4 = array2;
            foreach (string text2 in array4)
            {
                gSPacketIn.WriteInt(int.Parse(text2.Split(',')[1]));
                gSPacketIn.WriteInt(int.Parse(text2.Split(',')[0]));
            }
            gSPacketIn.WriteBoolean(usersPetInfo.IsEquip);
            gSPacketIn.WriteInt(usersPetInfo.PetEquips.Count);
            foreach (PetEquipInfo petEquip in usersPetInfo.PetEquips)
            {
                gSPacketIn.WriteInt(petEquip.eqType);
                gSPacketIn.WriteInt(petEquip.eqTemplateID);
                gSPacketIn.WriteDateTime(petEquip.startTime);
                gSPacketIn.WriteInt(petEquip.ValidDate);
                if (GameProperties.VERSION >= 8200)
                {
                    gSPacketIn.WriteInt(petEquip.AttackCompose);
                    gSPacketIn.WriteInt(petEquip.DefendCompose);
                    gSPacketIn.WriteInt(petEquip.AgilityCompose);
                    gSPacketIn.WriteInt(petEquip.LuckCompose);
                    gSPacketIn.WriteInt(petEquip.itemID);
                }
            }
            gSPacketIn.WriteInt(usersPetInfo.currentStarExp);
            if (GameProperties.VERSION >= 10100)
            {
                gSPacketIn.WriteBoolean(usersPetInfo.IsSeal);
            }
        }
        gSPacketIn.WriteInt(eatpet?.weaponLevel ?? 0);
        gSPacketIn.WriteInt(eatpet?.clothesLevel ?? 0);
        gSPacketIn.WriteInt(eatpet?.hatLevel ?? 0);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRefreshPet(GamePlayer player, UsersPetInfo[] pets, ItemInfo[] items, bool refreshBtn)
    {
        if (pets.Length == 0)
        {
            return null;
        }
        int val = 10;
        int val2 = 10;
        int val3 = 100;
        GSPacketIn gSPacketIn = new GSPacketIn(68, player.PlayerCharacter.ID);
        gSPacketIn.WriteByte(5);
        gSPacketIn.WriteBoolean(refreshBtn);
        gSPacketIn.WriteInt(pets.Length);
        foreach (UsersPetInfo usersPetInfo in pets)
        {
            gSPacketIn.WriteInt(usersPetInfo.Place);
            gSPacketIn.WriteInt(usersPetInfo.TemplateID);
            gSPacketIn.WriteString(usersPetInfo.Name);
            gSPacketIn.WriteInt(usersPetInfo.Attack);
            gSPacketIn.WriteInt(usersPetInfo.Defence);
            gSPacketIn.WriteInt(usersPetInfo.Luck);
            gSPacketIn.WriteInt(usersPetInfo.Agility);
            gSPacketIn.WriteInt(usersPetInfo.Blood);
            gSPacketIn.WriteInt(usersPetInfo.Damage);
            gSPacketIn.WriteInt(usersPetInfo.Guard);
            gSPacketIn.WriteInt(usersPetInfo.AttackGrow);
            gSPacketIn.WriteInt(usersPetInfo.DefenceGrow);
            gSPacketIn.WriteInt(usersPetInfo.LuckGrow);
            gSPacketIn.WriteInt(usersPetInfo.AgilityGrow);
            gSPacketIn.WriteInt(usersPetInfo.BloodGrow);
            gSPacketIn.WriteInt(usersPetInfo.DamageGrow);
            gSPacketIn.WriteInt(usersPetInfo.GuardGrow);
            gSPacketIn.WriteInt(usersPetInfo.Level);
            gSPacketIn.WriteInt(usersPetInfo.GP);
            gSPacketIn.WriteInt(usersPetInfo.MaxGP);
            gSPacketIn.WriteInt(usersPetInfo.Hunger);
            gSPacketIn.WriteInt(usersPetInfo.MP);
            string[] array = usersPetInfo.Skill.Split('|');
            gSPacketIn.WriteInt(array.Length);
            string[] array2 = array;
            foreach (string text in array2)
            {
                gSPacketIn.WriteInt(int.Parse(text.Split(',')[0]));
                gSPacketIn.WriteInt(int.Parse(text.Split(',')[1]));
            }
            gSPacketIn.WriteInt(val);
            gSPacketIn.WriteInt(val2);
            gSPacketIn.WriteInt(val3);
        }
        if (!player.PlayerCharacter.IsFistGetPet)
        {
            gSPacketIn.WriteInt(0);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendAddFriend(PlayerInfo user, int relation, bool state)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(160, user.ID);
        gSPacketIn.WriteByte(160);
        gSPacketIn.WriteBoolean(state);
        gSPacketIn.WriteInt(user.ID);
        gSPacketIn.WriteString(user.NickName);
        gSPacketIn.WriteByte(user.typeVIP);
        gSPacketIn.WriteInt(user.VIPLevel);
        gSPacketIn.WriteBoolean(user.Sex);
        gSPacketIn.WriteString(user.Style);
        gSPacketIn.WriteString(user.Colors);
        gSPacketIn.WriteString(user.Skin);
        gSPacketIn.WriteInt((user.State == 1) ? 1 : 0);
        gSPacketIn.WriteInt(user.Grade);
        if (GameProperties.VERSION >= 8200)
        {
            gSPacketIn.WriteInt(user.ddtKingGrade);
        }
        gSPacketIn.WriteInt(user.Hide);
        gSPacketIn.WriteString(user.ConsortiaName);
        gSPacketIn.WriteInt(user.Total);
        gSPacketIn.WriteInt(user.Escape);
        gSPacketIn.WriteInt(user.Win);
        gSPacketIn.WriteInt(user.Offer);
        gSPacketIn.WriteInt(user.Repute);
        gSPacketIn.WriteInt(relation);
        gSPacketIn.WriteString(user.UserName);
        gSPacketIn.WriteInt(user.Nimbus);
        gSPacketIn.WriteInt(user.FightPower);
        gSPacketIn.WriteInt(user.AcademyInfo.ApprenticeshipState);
        gSPacketIn.WriteInt(user.AcademyInfo.MasterID);
        gSPacketIn.WriteString(user.AcademyInfo.MasterOrApprentices);
        gSPacketIn.WriteInt(user.AcademyInfo.GraduatesCount);
        gSPacketIn.WriteString(user.AcademyInfo.HonourOfMaster);
        gSPacketIn.WriteInt(user.AchievementPoint);
        gSPacketIn.WriteString(user.Honor);
        gSPacketIn.WriteBoolean(user.IsMarried);
        gSPacketIn.WriteBoolean(user.IsOldPlayer);
        gSPacketIn.WriteDateTime(user.LastDate);
        gSPacketIn.WriteString(user.ImagePath);
        gSPacketIn.WriteBoolean(user.IsShow);
        gSPacketIn.WriteBoolean(user.isAttest);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendFriendRemove(int FriendID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(160, FriendID);
        gSPacketIn.WriteByte(161);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendFriendState(int playerID, int state, byte typeVip, int viplevel)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(160, playerID);
        gSPacketIn.WriteByte(165);
        gSPacketIn.WriteInt(state);
        gSPacketIn.WriteByte(typeVip);
        gSPacketIn.WriteInt(viplevel);
        gSPacketIn.WriteBoolean(val: true);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateAllData(PlayerInfo player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(142, player.ID);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteDateTime(DateTime.Now.AddDays(7.0));
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendGetSpree(PlayerInfo player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(157, player.ID);
        gSPacketIn.WriteBoolean(val: true);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateUpCount(PlayerInfo player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(96, player.ID);
        gSPacketIn.WriteInt(player.MaxBuyHonor);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerRefreshTotem(PlayerInfo player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(136, player.ID);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(player.myHonor);
        gSPacketIn.WriteInt(player.totemId);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendLabyrinthUpdataInfo(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(131, player.PlayerId);
        gSPacketIn.WriteByte(2);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.maxFinishLevel);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.currentFinishLevel);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.sweepTotalTimeRemaining);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.sweepTotalExp);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.sweepTotalTime);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.sweepNeedMoney);
        gSPacketIn.WriteBoolean(player.PlayerCharacter.LabyrinthInfo.isUseDoubleProp);
        gSPacketIn.WriteBoolean(player.PlayerCharacter.LabyrinthInfo.isPlayGame);
        gSPacketIn.WriteBoolean(player.PlayerCharacter.LabyrinthInfo.isSweep);
        gSPacketIn.WriteBoolean(player.PlayerCharacter.LabyrinthInfo.isMultiple);
        gSPacketIn.WriteString(player.PlayerCharacter.LabyrinthInfo.buffList);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.nowContinueCount);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.maxFinishLevelTurn);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.maxWeekFinishLevel);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.tryAgainMoney);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendLabyrinthUpdataInfoOld(int ID, UserLabyrinthInfo laby)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(131, ID);
        gSPacketIn.WriteByte(2);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(laby.myProgress);
        gSPacketIn.WriteInt(laby.currentFloor);
        gSPacketIn.WriteBoolean(laby.completeChallenge);
        gSPacketIn.WriteInt(laby.remainTime);
        gSPacketIn.WriteInt(laby.accumulateExp);
        gSPacketIn.WriteInt(laby.cleanOutAllTime);
        gSPacketIn.WriteInt(laby.cleanOutGold);
        gSPacketIn.WriteInt(laby.myRanking);
        gSPacketIn.WriteBoolean(laby.isDoubleAward);
        gSPacketIn.WriteBoolean(laby.isInGame);
        gSPacketIn.WriteBoolean(laby.isCleanOut);
        gSPacketIn.WriteBoolean(laby.serverMultiplyingPower);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerFigSpiritinit(int ID, List<UserGemStone> gems)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(209, ID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteInt(gems.Count);
        foreach (UserGemStone gem in gems)
        {
            gSPacketIn.WriteInt(gem.UserID);
            gSPacketIn.WriteInt(gem.FigSpiritId);
            gSPacketIn.WriteString(gem.FigSpiritIdValue);
            gSPacketIn.WriteInt(gem.EquipPlace);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerFigSpiritUp(int ID, UserGemStone gem, bool isUp, bool isMaxLevel, bool isFall, int num, int dir)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(209, ID);
        gSPacketIn.WriteByte(2);
        string[] array = gem.FigSpiritIdValue.Split('|');
        gSPacketIn.WriteBoolean(isUp);
        gSPacketIn.WriteBoolean(isMaxLevel);
        gSPacketIn.WriteBoolean(isFall);
        gSPacketIn.WriteInt(num);
        gSPacketIn.WriteInt(array.Length);
        foreach (string text in array)
        {
            gSPacketIn.WriteInt(gem.FigSpiritId);
            gSPacketIn.WriteInt(Convert.ToInt32(text.Split(',')[0]));
            gSPacketIn.WriteInt(Convert.ToInt32(text.Split(',')[1]));
            gSPacketIn.WriteInt(Convert.ToInt32(text.Split(',')[2]));
        }
        gSPacketIn.WriteInt(gem.EquipPlace);
        gSPacketIn.WriteInt(dir);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendTrusteeshipStart(int ID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(140, ID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(0);
        for (int i = 0; i < 0; i++)
        {
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteDateTime(DateTime.Now);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendKingBlessMain(PlayerExtra Extra)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(142, Extra.Info.UserID);
        gSPacketIn.WriteInt(Extra.Info.KingBlessIndex);
        gSPacketIn.WriteDateTime(Extra.Info.KingBlessEnddate);
        if (Extra.Info.KingBlessIndex > 0)
        {
            gSPacketIn.WriteInt(Extra.KingBlessInfo.Count);
            foreach (int key in Extra.KingBlessInfo.Keys)
            {
                gSPacketIn.WriteInt(key);
                gSPacketIn.WriteInt(Extra.KingBlessInfo[key]);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendKingBlessUpdateBuffData(int UserID, int data, int value)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(143, UserID);
        gSPacketIn.WriteInt(data);
        gSPacketIn.WriteInt(value);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendDeedMain(PlayerExtra Extra)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(2272, Extra.Info.UserID);
        gSPacketIn.WriteInt(Extra.Info.DeedIndex);
        gSPacketIn.WriteDateTime(Extra.Info.DeedEnddate);
        if (Extra.Info.DeedIndex > 0)
        {
            gSPacketIn.WriteInt(Extra.DeedInfo.Count);
            foreach (int key in Extra.DeedInfo.Keys)
            {
                gSPacketIn.WriteInt(key);
                gSPacketIn.WriteInt(Extra.DeedInfo[key]);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendDeedUpdateBuffData(int UserID, int data, int value)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(2273, UserID);
        gSPacketIn.WriteInt(data);
        gSPacketIn.WriteInt(value);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendGrowthPackageIsOpen(int ID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(84, ID);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(3);
        gSPacketIn.WriteBoolean(GameProperties.IsPromotePackageOpen);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendGrowthPackageOpen(int ID, int isBuy)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(84, ID);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(isBuy);
        for (int i = 0; i < 9; i++)
        {
            if (i < isBuy)
            {
                gSPacketIn.WriteInt(1);
            }
            else
            {
                gSPacketIn.WriteInt(0);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendGrowthPackageUpadte(int ID, int isBuy)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(84, ID);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(2);
        gSPacketIn.WriteInt(isBuy);
        for (int i = 0; i < 9; i++)
        {
            if (i < isBuy)
            {
                gSPacketIn.WriteInt(1);
            }
            else
            {
                gSPacketIn.WriteInt(0);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendDiceActiveClose(int ID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(134, ID);
        gSPacketIn.WriteByte(2);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendDiceReceiveData(PlayerDice Dice)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(134, Dice.Data.UserID);
        gSPacketIn.WriteByte(3);
        gSPacketIn.WriteBoolean(Dice.Data.UserFirstCell);
        gSPacketIn.WriteInt(Dice.Data.CurrentPosition);
        gSPacketIn.WriteInt(Dice.Data.LuckIntegralLevel);
        gSPacketIn.WriteInt(Dice.Data.LuckIntegral);
        gSPacketIn.WriteInt(Dice.Data.FreeCount);
        gSPacketIn.WriteInt(Dice.RewardItem.Count);
        for (int i = 0; i < Dice.RewardItem.Count; i++)
        {
            gSPacketIn.WriteInt(Dice.RewardItem[i].TemplateID);
            gSPacketIn.WriteInt(i);
            gSPacketIn.WriteInt(Dice.RewardItem[i].StrengthenLevel);
            gSPacketIn.WriteInt(Dice.RewardItem[i].Count);
            gSPacketIn.WriteInt(Dice.RewardItem[i].ValidDate);
            gSPacketIn.WriteBoolean(Dice.RewardItem[i].IsBinds);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendDiceReceiveResult(PlayerDice Dice)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(134, Dice.Data.UserID);
        gSPacketIn.WriteByte(4);
        gSPacketIn.WriteInt(Dice.Data.CurrentPosition);
        gSPacketIn.WriteInt(Dice.result);
        gSPacketIn.WriteInt(Dice.Data.LuckIntegral);
        gSPacketIn.WriteInt(Dice.Data.Level);
        gSPacketIn.WriteInt(Dice.Data.FreeCount);
        gSPacketIn.WriteString(Dice.RewardName);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendDiceActiveOpen(PlayerDice Dice)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(134, Dice.Data.UserID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteInt(Dice.Data.FreeCount);
        gSPacketIn.WriteInt(Dice.refreshPrice);
        gSPacketIn.WriteInt(Dice.commonDicePrice);
        gSPacketIn.WriteInt(Dice.doubleDicePrice);
        gSPacketIn.WriteInt(Dice.bigDicePrice);
        gSPacketIn.WriteInt(Dice.smallDicePrice);
        gSPacketIn.WriteInt(Dice.MAX_LEVEL);
        for (int i = 0; i < Dice.MAX_LEVEL; i++)
        {
            List<DiceLevelAwardInfo> list = Dice.LevelAward[i];
            gSPacketIn.WriteInt(Dice.IntegralPoint[i]);
            gSPacketIn.WriteInt(list.Count);
            for (int j = 0; j < list.Count; j++)
            {
                gSPacketIn.WriteInt(list[j].TemplateID);
                gSPacketIn.WriteInt(list[j].Count);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendChickenBoxOpen(int ID, int flushPrice, int[] openCardPrice, int[] eagleEyePrice)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(87, ID);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(openCardPrice.Length);
        for (int num = openCardPrice.Length; num > 0; num--)
        {
            gSPacketIn.WriteInt(openCardPrice[num - 1]);
        }
        gSPacketIn.WriteInt(eagleEyePrice.Length);
        for (int num2 = eagleEyePrice.Length; num2 > 0; num2--)
        {
            gSPacketIn.WriteInt(eagleEyePrice[num2 - 1]);
        }
        gSPacketIn.WriteInt(flushPrice);
        gSPacketIn.WriteDateTime(DateTime.Parse(GameProperties.NewChickenEndTime));
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendLuckStarOpen(int ID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(87, ID);
        gSPacketIn.WriteInt(25);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendLuckStoneEnable(int ID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(165, ID);
        gSPacketIn.WriteDateTime(DateTime.Now);
        gSPacketIn.WriteDateTime(DateTime.Now);
        gSPacketIn.WriteBoolean(val: false);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendBattleGoundOpen(int ID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(132, ID);
        gSPacketIn.WriteByte(1);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendBattleGoundOver(int ID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(132, ID);
        gSPacketIn.WriteByte(2);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendActivityList(int ID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(155, ID);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(1);
        for (int i = 0; i < 1; i++)
        {
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(1);
        }
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteInt(6);
        gSPacketIn.WriteBoolean(val: false);
        gSPacketIn.WriteBoolean(val: false);
        gSPacketIn.WriteString("");
        if (GameProperties.VERSION >= 9500)
        {
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteByte(0);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendExpBlessedData(int PlayerId)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(155, PlayerId);
        gSPacketIn.WriteByte(8);
        gSPacketIn.WriteInt(0);
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendFindBackIncome(int ID)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(147, ID);
        gSPacketIn.WriteInt(6);
        gSPacketIn.WriteBoolean(val: false);
        gSPacketIn.WriteBoolean(val: false);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerDrill(int ID, Dictionary<int, UserDrillInfo> drills)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(121, ID);
        gSPacketIn.WriteByte(6);
        gSPacketIn.WriteInt(ID);
        gSPacketIn.WriteInt(drills[0].HoleExp);
        gSPacketIn.WriteInt(drills[1].HoleExp);
        gSPacketIn.WriteInt(drills[2].HoleExp);
        gSPacketIn.WriteInt(drills[3].HoleExp);
        gSPacketIn.WriteInt(drills[4].HoleExp);
        gSPacketIn.WriteInt(drills[5].HoleExp);
        gSPacketIn.WriteInt(drills[0].HoleLv);
        gSPacketIn.WriteInt(drills[1].HoleLv);
        gSPacketIn.WriteInt(drills[2].HoleLv);
        gSPacketIn.WriteInt(drills[3].HoleLv);
        gSPacketIn.WriteInt(drills[4].HoleLv);
        gSPacketIn.WriteInt(drills[5].HoleLv);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUserRanks(int Id, List<UserRankInfo> ranks)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(34, Id);
        gSPacketIn.WriteInt(ranks.Count);
        foreach (UserRankInfo rank in ranks)
        {
            gSPacketIn.WriteInt(rank.NewTitleID);
            gSPacketIn.WriteString(rank.Name);
            gSPacketIn.WriteDateTime(rank.BeginDate);
            if (rank.Validate == 0)
            {
                gSPacketIn.WriteDateTime(DateTime.Now.AddYears(1));
            }
            else
            {
                gSPacketIn.WriteDateTime(rank.EndDate);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateAchievements(GamePlayer player, BaseAchievement[] infos)
    {
        if (player == null || infos == null)
        {
            return null;
        }
        GSPacketIn gSPacketIn = new GSPacketIn(228, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(infos.Length);
        foreach (BaseAchievement baseAchievement in infos)
        {
            gSPacketIn.WriteInt(baseAchievement.Data.AchievementID);
            gSPacketIn.WriteInt(1);
        }
        return gSPacketIn;
    }

    public GSPacketIn SendAchievementDatas(GamePlayer player, BaseAchievement[] infos)
    {
        if (player == null || infos == null)
        {
            return null;
        }
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;
        GSPacketIn gSPacketIn = new GSPacketIn(231, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(infos.Length);
        foreach (BaseAchievement baseAchievement in infos)
        {
            gSPacketIn.WriteInt(baseAchievement.Data.AchievementID);
            gSPacketIn.WriteInt(year);
            gSPacketIn.WriteInt(month);
            gSPacketIn.WriteInt(day);
        }
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] infos)
    {
        if (player == null || states == null || infos == null)
        {
            return null;
        }
        if (infos.Length > 5)
        {
            IEnumerable<IEnumerable<BaseQuest>> enumerable = infos.Split(5);
            foreach (IEnumerable<BaseQuest> item in enumerable)
            {
                SendUpdateQuests(player, states, item.ToArray());
            }
            return null;
        }
        GSPacketIn gSPacketIn = new GSPacketIn(178, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(infos.Length);
        foreach (BaseQuest baseQuest in infos)
        {
            gSPacketIn.WriteInt(baseQuest.Data.QuestID);
            gSPacketIn.WriteBoolean(baseQuest.Data.IsComplete);
            gSPacketIn.WriteInt(baseQuest.Data.Condition1);
            gSPacketIn.WriteInt(baseQuest.Data.Condition2);
            gSPacketIn.WriteInt(baseQuest.Data.Condition3);
            gSPacketIn.WriteInt(baseQuest.Data.Condition4);
            gSPacketIn.WriteDateTime(baseQuest.Data.CompletedDate);
            gSPacketIn.WriteInt(baseQuest.Data.RepeatFinish);
            gSPacketIn.WriteInt(baseQuest.Data.RandDobule);
            gSPacketIn.WriteBoolean(baseQuest.Data.IsExist);
            gSPacketIn.WriteInt(baseQuest.Data.QuestLevel);
            if (GameProperties.VERSION >= 8400)
            {
                gSPacketIn.WriteDateTime(baseQuest.Data.CompletedDate);
            }
            if (GameProperties.VERSION >= 7700)
            {
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
            }
            int[] array = new int[4]
            {
                baseQuest.Data.Condition5,
                baseQuest.Data.Condition6,
                baseQuest.Data.Condition7,
                baseQuest.Data.Condition8
            };
            int num = ((baseQuest.Data.Condition5 + baseQuest.Data.Condition6 + baseQuest.Data.Condition7 + baseQuest.Data.Condition8 > 0) ? array.Length : 0);
            gSPacketIn.WriteInt(num);
            for (int j = 0; j < num; j++)
            {
                gSPacketIn.WriteInt(j + 4);
                gSPacketIn.WriteInt(array[j]);
                gSPacketIn.WriteInt(0);
            }
            gSPacketIn.WriteBoolean(baseQuest.Info.IsAccept);
        }
        gSPacketIn.WriteInt(0);
        gSPacketIn.Write(states);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendLoginDeviceCheck(GamePlayer p, bool isLogin)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(84);
        gSPacketIn.WriteInt(11);
        gSPacketIn.WriteBoolean(isLogin);
        gSPacketIn.WriteInt(p.Extra.Info.loginDeviceDownAward ? 1 : 0);
        gSPacketIn.WriteInt(p.Extra.Info.loginDeviceDailyAward ? 1 : 0);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUserBankUpdate(UserBankInfo bank)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(389);
        gSPacketIn.WriteInt(3);
        gSPacketIn.WriteInt(bank.BankID);
        gSPacketIn.WriteInt(bank.Amount);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUserSyncEquipGhost(GamePlayer p)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(392);
        List<UserEquipGhostInfo> allEquipGhost = p.GetAllEquipGhost();
        gSPacketIn.WriteInt(allEquipGhost.Count);
        foreach (UserEquipGhostInfo item in allEquipGhost)
        {
            gSPacketIn.WriteInt(item.BagType);
            gSPacketIn.WriteInt(item.Place);
            gSPacketIn.WriteInt(item.Level);
            gSPacketIn.WriteInt(item.TotalGhost);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateBuffer(GamePlayer player, BufferInfo[] infos)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(185, player.PlayerId);
        gSPacketIn.WriteInt(infos.Length);
        foreach (BufferInfo bufferInfo in infos)
        {
            gSPacketIn.WriteInt(bufferInfo.Type);
            gSPacketIn.WriteBoolean(bufferInfo.IsExist);
            gSPacketIn.WriteDateTime(bufferInfo.BeginDate);
            gSPacketIn.WriteInt(bufferInfo.ValidDate);
            gSPacketIn.WriteInt(bufferInfo.Value);
            gSPacketIn.WriteInt(bufferInfo.ValidCount);
            gSPacketIn.WriteInt(bufferInfo.TemplateID);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateConsotiaBuffer(GamePlayer player, Dictionary<int, BufferInfo> bufflist)
    {
        List<ConsortiaBufferTempInfo> allConsortiaBuff = ConsortiaExtraMgr.GetAllConsortiaBuff();
        GSPacketIn gSPacketIn = new GSPacketIn(129, player.PlayerId);
        gSPacketIn.WriteByte(26);
        gSPacketIn.WriteInt(allConsortiaBuff.Count);
        foreach (ConsortiaBufferTempInfo item in allConsortiaBuff)
        {
            if (bufflist.ContainsKey(item.id))
            {
                BufferInfo bufferInfo = bufflist[item.id];
                gSPacketIn.WriteInt(bufferInfo.TemplateID);
                gSPacketIn.WriteBoolean(val: true);
                gSPacketIn.WriteDateTime(bufferInfo.BeginDate);
                gSPacketIn.WriteInt(bufferInfo.ValidDate / 24 / 60);
            }
            else
            {
                gSPacketIn.WriteInt(item.id);
                gSPacketIn.WriteBoolean(val: false);
                gSPacketIn.WriteDateTime(DateTime.Now);
                gSPacketIn.WriteInt(0);
            }
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(186, player.PlayerId);
        gSPacketIn.WriteInt(infos.Count);
        foreach (AbstractBuffer info2 in infos)
        {
            BufferInfo info = info2.Info;
            gSPacketIn.WriteInt(info.Type);
            gSPacketIn.WriteBoolean(info.IsExist);
            gSPacketIn.WriteDateTime(info.BeginDate);
            gSPacketIn.WriteInt(info.ValidDate);
            gSPacketIn.WriteInt(info.Value);
            gSPacketIn.WriteInt(info.ValidCount);
            gSPacketIn.WriteInt(info.TemplateID);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(117);
        gSPacketIn.WriteInt(playerID);
        gSPacketIn.WriteInt((int)type);
        GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, ItemInfo item)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(195);
        gSPacketIn.WriteInt(auctionID);
        gSPacketIn.WriteBoolean(isExist);
        if (isExist)
        {
            gSPacketIn.WriteInt(info.AuctioneerID);
            gSPacketIn.WriteString(info.AuctioneerName);
            gSPacketIn.WriteDateTime(info.BeginDate);
            gSPacketIn.WriteInt(info.BuyerID);
            gSPacketIn.WriteString(info.BuyerName);
            gSPacketIn.WriteInt(info.ItemID);
            gSPacketIn.WriteInt(info.Mouthful);
            gSPacketIn.WriteInt(info.PayType);
            gSPacketIn.WriteInt(info.Price);
            gSPacketIn.WriteInt(info.Rise);
            gSPacketIn.WriteInt(info.ValidDate);
            gSPacketIn.WriteBoolean(item != null);
            if (item != null)
            {
                gSPacketIn.WriteInt(item.Count);
                gSPacketIn.WriteInt(item.TemplateID);
                gSPacketIn.WriteInt(item.AttackCompose);
                gSPacketIn.WriteInt(item.DefendCompose);
                gSPacketIn.WriteInt(item.AgilityCompose);
                gSPacketIn.WriteInt(item.LuckCompose);
                gSPacketIn.WriteInt(item.StrengthenLevel);
                gSPacketIn.WriteBoolean(item.IsBinds);
                gSPacketIn.WriteBoolean(item.IsJudge);
                gSPacketIn.WriteDateTime(item.BeginDate);
                gSPacketIn.WriteInt(item.ValidDate);
                gSPacketIn.WriteString(item.Color);
                gSPacketIn.WriteString(item.Skin);
                gSPacketIn.WriteBoolean(item.IsUsed);
                gSPacketIn.WriteInt(item.Hole1);
                gSPacketIn.WriteInt(item.Hole2);
                gSPacketIn.WriteInt(item.Hole3);
                gSPacketIn.WriteInt(item.Hole4);
                gSPacketIn.WriteInt(item.Hole5);
                gSPacketIn.WriteInt(item.Hole6);
                gSPacketIn.WriteString(item.Pic);
                gSPacketIn.WriteInt(item.RefineryLevel);
                gSPacketIn.WriteDateTime(DateTime.Now);
                gSPacketIn.WriteByte((byte)item.Hole5Level);
                gSPacketIn.WriteInt(item.Hole5Exp);
                gSPacketIn.WriteByte((byte)item.Hole6Level);
                gSPacketIn.WriteInt(item.Hole6Exp);
                gSPacketIn.WriteInt(item.MagicLevel);
                gSPacketIn.WriteInt(item.MagicExp);
            }
        }
        gSPacketIn.Compress();
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendAASState(bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(224);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendIDNumberCheck(bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(226);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendAASInfoSet(bool result)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(224);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(227);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteInt(1);
        gSPacketIn.WriteBoolean(val: true);
        gSPacketIn.WriteBoolean(IsMinor);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(241, player.PlayerCharacter.ID);
        bool flag = room != null;
        gSPacketIn.WriteBoolean(flag);
        if (flag)
        {
            gSPacketIn.WriteInt(room.Info.ID);
            gSPacketIn.WriteBoolean(room.Info.IsHymeneal);
            gSPacketIn.WriteString(room.Info.Name);
            gSPacketIn.WriteBoolean((!(room.Info.Pwd == "")) ? true : false);
            gSPacketIn.WriteInt(room.Info.MapIndex);
            gSPacketIn.WriteInt(room.Info.AvailTime);
            gSPacketIn.WriteInt(room.Count);
            gSPacketIn.WriteInt(room.Info.PlayerID);
            gSPacketIn.WriteString(room.Info.PlayerName);
            gSPacketIn.WriteInt(room.Info.GroomID);
            gSPacketIn.WriteString(room.Info.GroomName);
            gSPacketIn.WriteInt(room.Info.BrideID);
            gSPacketIn.WriteString(room.Info.BrideName);
            gSPacketIn.WriteDateTime(room.Info.BeginTime);
            gSPacketIn.WriteByte((byte)room.RoomState);
            gSPacketIn.WriteString(room.Info.RoomIntroduction);
            gSPacketIn.WriteInt(room.Info.SeniorType);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result, int failType)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(242, player.PlayerCharacter.ID);
        gSPacketIn.WriteBoolean(result);
        if (result)
        {
            gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.ID);
            gSPacketIn.WriteString(player.CurrentMarryRoom.Info.Name);
            gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.MapIndex);
            gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.AvailTime);
            gSPacketIn.WriteInt(player.CurrentMarryRoom.Count);
            gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.PlayerID);
            gSPacketIn.WriteString(player.CurrentMarryRoom.Info.PlayerName);
            gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.GroomID);
            gSPacketIn.WriteString(player.CurrentMarryRoom.Info.GroomName);
            gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.BrideID);
            gSPacketIn.WriteString(player.CurrentMarryRoom.Info.BrideName);
            gSPacketIn.WriteDateTime(player.CurrentMarryRoom.Info.BeginTime);
            gSPacketIn.WriteBoolean(player.CurrentMarryRoom.Info.IsHymeneal);
            gSPacketIn.WriteByte((byte)player.CurrentMarryRoom.RoomState);
            gSPacketIn.WriteString(player.CurrentMarryRoom.Info.RoomIntroduction);
            gSPacketIn.WriteBoolean(player.CurrentMarryRoom.Info.GuestInvite);
            gSPacketIn.WriteInt(player.MarryMap);
            gSPacketIn.WriteBoolean(player.CurrentMarryRoom.Info.IsGunsaluteUsed);
            gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.SeniorType);
        }
        if (!result && GameProperties.VERSION >= 7300)
        {
            gSPacketIn.WriteInt(failType);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(243, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
        gSPacketIn.WriteInt(player.PlayerCharacter.Hide);
        gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
        gSPacketIn.WriteInt(player.PlayerCharacter.ID);
        gSPacketIn.WriteString(player.PlayerCharacter.NickName);
        gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
        gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
        gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
        gSPacketIn.WriteString(player.PlayerCharacter.Style);
        gSPacketIn.WriteString(player.PlayerCharacter.Colors);
        gSPacketIn.WriteString(player.PlayerCharacter.Skin);
        gSPacketIn.WriteInt(player.X);
        gSPacketIn.WriteInt(player.Y);
        gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
        gSPacketIn.WriteInt(player.PlayerCharacter.Win);
        gSPacketIn.WriteInt(player.PlayerCharacter.Total);
        gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
        gSPacketIn.WriteBoolean(player.PlayerCharacter.IsOldPlayer);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(239);
        gSPacketIn.WriteInt(ID);
        gSPacketIn.WriteBoolean(isExist);
        if (isExist)
        {
            gSPacketIn.WriteInt(info.UserID);
            gSPacketIn.WriteBoolean(info.IsPublishEquip);
            gSPacketIn.WriteString(info.Introduction);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(246, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(userID);
        gSPacketIn.WriteBoolean(isMarried);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int id)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(247, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(userID);
        gSPacketIn.WriteString(userName);
        gSPacketIn.WriteString(loveProclamation);
        gSPacketIn.WriteInt(id);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(248, player.PlayerCharacter.ID);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteBoolean(isProposer);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int id)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(250, player.PlayerCharacter.ID);
        gSPacketIn.WriteInt(UserID);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(UserName);
        gSPacketIn.WriteBoolean(isApplicant);
        gSPacketIn.WriteInt(id);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(244, player.PlayerCharacter.ID);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(235, player.PlayerCharacter.ID);
        gSPacketIn.WriteString(info.Introduction);
        gSPacketIn.WriteBoolean(info.IsPublishEquip);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(249, player.PlayerCharacter.ID);
        gSPacketIn.WriteByte(3);
        gSPacketIn.WriteInt(info.AvailTime);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(234, player.PlayerCharacter.ID);
        gSPacketIn.WriteBoolean(info.IsMarried);
        gSPacketIn.WriteInt(info.SpouseID);
        gSPacketIn.WriteString(info.SpouseName);
        gSPacketIn.WriteBoolean(info.IsCreatedMarryRoom);
        gSPacketIn.WriteInt(info.SelfMarryRoomID);
        gSPacketIn.WriteBoolean(info.IsGotRing);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendConsortiaCreate(string name1, bool result, int id, string name2, string msg, int dutyLevel, string DutyName, int dutyRight, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(1);
        gSPacketIn.WriteString(name1);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteInt(id);
        gSPacketIn.WriteString(name2);
        gSPacketIn.WriteString(msg);
        gSPacketIn.WriteInt(dutyLevel);
        gSPacketIn.WriteString((DutyName == null) ? "" : DutyName);
        gSPacketIn.WriteInt(dutyRight);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendConsortiaRichesOffer(int money, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(6);
        gSPacketIn.WriteInt(money);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendConsortiaInvite(string username, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(11);
        gSPacketIn.WriteString(username);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaInvitePass(int id, bool result, int consortiaid, string consortianame, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(12);
        gSPacketIn.WriteInt(id);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaInviteDel(int id, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(13);
        gSPacketIn.WriteInt(id);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendConsortiaLevelUp(byte type, byte level, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(21);
        gSPacketIn.WriteByte(type);
        gSPacketIn.WriteByte(level);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaTryIn(int id, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(0);
        gSPacketIn.WriteInt(id);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaTryInPass(int id, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(4);
        gSPacketIn.WriteInt(id);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaTryInDel(int id, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(5);
        gSPacketIn.WriteInt(id);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaUpdateDescription(string description, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(14);
        gSPacketIn.WriteString(description);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaEquipConstrol(bool result, List<int> riches, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(24);
        gSPacketIn.WriteBoolean(result);
        foreach (int rich in riches)
        {
            gSPacketIn.WriteInt(rich);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendConsortiaMemberGrade(int id, bool update, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(18);
        gSPacketIn.WriteInt(id);
        gSPacketIn.WriteBoolean(update);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaOut(int id, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(3);
        gSPacketIn.WriteInt(id);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaChangeChairman(string nick, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(14);
        gSPacketIn.WriteString(nick);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaUpdatePlacard(string description, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(15);
        gSPacketIn.WriteString(description);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteString(msg);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendConsortiaApplyStatusOut(bool state, bool result, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(7);
        gSPacketIn.WriteBoolean(result);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendBuyBadge(int consortiaID, int BadgeID, int ValidDate, bool result, string BadgeBuyTime, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(28);
        gSPacketIn.WriteInt(consortiaID);
        gSPacketIn.WriteInt(BadgeID);
        gSPacketIn.WriteInt(ValidDate);
        gSPacketIn.WriteDateTime(Convert.ToDateTime(BadgeBuyTime));
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendConsortia(int money, bool result, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(6);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendConsortiaMail(bool result, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(129, playerid);
        gSPacketIn.WriteByte(29);
        gSPacketIn.WriteBoolean(result);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn sendOneOnOneTalk(int receiverID, bool isAutoReply, string SenderNickName, string msg, int playerid)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(160, playerid);
        gSPacketIn.WriteByte(51);
        gSPacketIn.WriteInt(receiverID);
        gSPacketIn.WriteString(SenderNickName);
        gSPacketIn.WriteDateTime(DateTime.Now);
        gSPacketIn.WriteString(msg);
        gSPacketIn.WriteBoolean(isAutoReply);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendNecklaceStrength(PlayerInfo player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(95, player.ID);
        gSPacketIn.WriteInt(player.necklaceExp);
        gSPacketIn.WriteInt(player.necklaceExpAdd);
        if (GameProperties.VERSION >= 10900)
        {
            gSPacketIn.WriteInt(player.necklaceArtificeNeedCount);
        }
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendMissionEnergy(UsersExtraInfo extra)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(105, extra.UserID);
        gSPacketIn.WriteInt(extra.MissionEnergy);
        gSPacketIn.WriteInt(extra.buyEnergyCount);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateOneKeyFinish(PlayerInfo player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(86, player.ID);
        gSPacketIn.WriteInt(10000);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendUpdateSoulCrystal(UsersExtraInfo extra)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(529, extra.UserID);
        gSPacketIn.WriteByte(16);
        gSPacketIn.WriteInt(extra.SoulCrystal);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendRenshenOpenClose()
    {
        GSPacketIn gSPacketIn = new GSPacketIn(298);
        gSPacketIn.WriteByte(13);
        gSPacketIn.WriteBoolean(val: true);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    void IPacketLib.SendRenshenOpenClose()
    {
        throw new NotImplementedException();
    }

    public GSPacketIn SendUpdateCoin(UsersExtraInfo extra)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(664);
        gSPacketIn.WriteByte(14);
        gSPacketIn.WriteInt(extra.Coin);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendSummonResult(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(664);
        gSPacketIn.WriteByte(8);
        gSPacketIn.WriteInt(player.GetSummonCount(1));
        gSPacketIn.WriteInt(player.GetSummonCount(2));
        gSPacketIn.WriteInt(0);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendGourdExpStorage(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(393);
        gSPacketIn.WriteInt(player.PlayerCharacter.GourdExpInfo.UserID);
        gSPacketIn.WriteInt(player.PlayerCharacter.GourdExpInfo.TemplateID);
        gSPacketIn.WriteInt(player.PlayerCharacter.GourdExpInfo.Exp);
        gSPacketIn.WriteInt(player.PlayerCharacter.GourdExpInfo.Stage);
        gSPacketIn.WriteInt(player.PlayerCharacter.GourdExpInfo.IsFrist);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendLabyrinthStartSweep(GamePlayer player)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(131, player.PlayerId);
        gSPacketIn.WriteByte(3);
        gSPacketIn.WriteInt(0);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.sweepTotalTimeRemaining);
        gSPacketIn.WriteInt(player.PlayerCharacter.LabyrinthInfo.sweepTotalTime);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public GSPacketIn SendLabyrinthSweepInfo(GamePlayer player, List<ItemInfo> items, int exp, int money)
    {
        GSPacketIn pkg = new GSPacketIn(131, player.PlayerId);
        pkg.WriteByte(7);
        pkg.WriteInt(0);
        pkg.WriteInt(player.PlayerCharacter.LabyrinthInfo.currentFinishLevel);
        pkg.WriteInt(exp);
        pkg.WriteInt(items.Count);
        items.ForEach(delegate (ItemInfo a)
        {
            pkg.WriteInt(a.TemplateID);
            pkg.WriteInt(a.Count);
        });
        pkg.WriteInt(money);
        SendTCP(pkg);
        return pkg;
    }

    public GSPacketIn SendAcademySystemNotice(string text, bool isAlert)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(141);
        gSPacketIn.WriteByte(17);
        gSPacketIn.WriteString(text);
        gSPacketIn.WriteBoolean(isAlert);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendAcademyGradute(GamePlayer app, int type)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(141);
        gSPacketIn.WriteByte(11);
        gSPacketIn.WriteInt(type);
        gSPacketIn.WriteInt(app.PlayerId);
        gSPacketIn.WriteString(app.PlayerCharacter.NickName);
        SendTCP(gSPacketIn);
    }

    public GSPacketIn SendAcademyApprenticeState(PlayerInfo player, int removeUserId)
    {
        GSPacketIn gSPacketIn = new GSPacketIn(141);
        gSPacketIn.WriteByte(10);
        gSPacketIn.WriteInt(player.AcademyInfo.ApprenticeshipState);
        gSPacketIn.WriteInt(player.AcademyInfo.MasterID);
        gSPacketIn.WriteString(player.AcademyInfo.MasterOrApprentices);
        gSPacketIn.WriteInt(removeUserId);
        gSPacketIn.WriteInt(player.AcademyInfo.GraduatesCount);
        gSPacketIn.WriteString(player.AcademyInfo.HonourOfMaster);
        gSPacketIn.WriteDateTime(player.AcademyInfo.FreezesDate);
        SendTCP(gSPacketIn);
        return gSPacketIn;
    }

    public void SendLabyrinthUpdataInfo(int id, UserLabyrinthInfo laby)
    {
        throw new NotImplementedException();
    }
}
