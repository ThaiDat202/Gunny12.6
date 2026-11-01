using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Helpers;
using Bussiness.Managers;
using DDtankHelp;
using Game.Logic;
using Game.Server.GameObjects;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PetInventory : PetAbstractInventory
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private bool m_saveToDb;

    private List<UsersPetInfo> m_removedList;

    protected GamePlayer m_player;

    private UsersPetFormInfo m_petForm;

    private List<PetFormActiveListInfo> m_activeList;

    private EatPetsInfo m_eatPets;

    public int lastAwakenId;

    public List<ItemAwakenPet> ItemAwakens;

    public GamePlayer Player => m_player;

    public UsersPetFormInfo PetForm => m_petForm;

    public List<PetFormActiveListInfo> ActiveList => m_activeList;

    public EatPetsInfo EatPets => m_eatPets;

    public int MaxLevel => Convert.ToInt32(PetMgr.FindConfig("MaxLevel").Value);

    public int MaxLevelByGrade
    {
        get
        {
            if (m_player == null)
            {
                return MaxLevel;
            }
            return (m_player.Level > MaxLevel) ? MaxLevel : m_player.Level;
        }
    }

    public PetInventory(GamePlayer player, bool saveTodb, int capibility, int aCapability, int beginSlot)
        : base(capibility, aCapability, beginSlot)
    {
        m_player = player;
        m_saveToDb = saveTodb;
        m_removedList = new List<UsersPetInfo>();
        m_activeList = new List<PetFormActiveListInfo>();
        lastAwakenId = 0;
        ItemAwakens = new List<ItemAwakenPet>();
    }

    public virtual void saveEqAwakenPet(PlayerBussiness pb)
    {
        for (int i = 0; i < ItemAwakens.Count; i++)
        {
            ItemAwakenPet itemAwakenPet = ItemAwakens[i];
            if (itemAwakenPet != null)
            {
                if (itemAwakenPet.isSave)
                {
                    pb.UpdateEqAwakenPet(itemAwakenPet, m_player.PlayerCharacter.ID);
                }
                else
                {
                    pb.AddEqAwakenPet(itemAwakenPet, m_player.PlayerCharacter.ID);
                }
            }
        }
    }

    public virtual void LoadFromDatabase()
    {
        if (!m_saveToDb)
        {
            return;
        }
        using PlayerBussiness playerBussiness = new PlayerBussiness();
        int iD = m_player.PlayerCharacter.ID;
        UsersPetInfo[] userPetSingles = playerBussiness.GetUserPetSingles(iD, m_player.PlayerCharacter.VIPLevel);
        ItemAwakenPet[] eqPetWaken = playerBussiness.GetEqPetWaken(iD);
        if (GameProperties.VERSION < 7600)
        {
            UsersPetInfo[] userAdoptPetSingles = playerBussiness.GetUserAdoptPetSingles(iD);
            UsersPetInfo[] array = userAdoptPetSingles;
            foreach (UsersPetInfo usersPetInfo in array)
            {
                AddAdoptPetTo(usersPetInfo, usersPetInfo.Place);
            }
        }
        if (GameProperties.VERSION >= 10100)
        {
            lacresPet = playerBussiness.GetUserLacresPet(iD);
            if (lacresPet == null || lacresPet.Count == 0)
            {
                List<UsersPetLacrarInfo> list = new List<UsersPetLacrarInfo>();
                UsersPetLacrarInfo usersPetLacrarInfo = new UsersPetLacrarInfo();
                usersPetLacrarInfo.place = 6;
                usersPetLacrarInfo.lockArr = new List<StoreFineSealLockInfo>();
                usersPetLacrarInfo.isSeal = false;
                list.Add(usersPetLacrarInfo);
                UsersPetLacrarInfo usersPetLacrarInfo2 = new UsersPetLacrarInfo();
                usersPetLacrarInfo2.place = 15;
                usersPetLacrarInfo2.lockArr = new List<StoreFineSealLockInfo>();
                usersPetLacrarInfo2.isSeal = false;
                list.Add(usersPetLacrarInfo2);
                lacresPet = list;
            }
        }
        UsersPetFormInfo allUsersPetFormByID = playerBussiness.GetAllUsersPetFormByID(iD);
        EatPetsInfo allEatPetsByID = playerBussiness.GetAllEatPetsByID(iD);
        BeginChanges();
        try
        {
            for (int j = 0; j < eqPetWaken.Length; j++)
            {
                ItemAwakens.Add(eqPetWaken[j]);
                lastAwakenId = ItemAwakens[j].itemId;
            }
            UsersPetInfo[] array2 = userPetSingles;
            foreach (UsersPetInfo pet in array2)
            {
                if (string.IsNullOrEmpty(pet.BaseProp))
                {
                    string text = pet.TemplateID.ToString();
                    int num = -1;
                    num = ((text.Substring(text.Length - 1, 1) == "1") ? ((pet.Level < 30) ? pet.TemplateID : ((pet.Level >= 50) ? (pet.TemplateID - 2) : (pet.TemplateID - 1))) : ((!(text.Substring(text.Length - 1, 1) == "2")) ? (pet.TemplateID - 2) : (pet.TemplateID - 1)));
                    PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(num);
                    if (petTemplateInfo != null)
                    {
                        UsersPetInfo usersPetInfo2 = PetMgr.CreatePet(petTemplateInfo, pet.UserID, pet.Place, pet.Level, Player.PlayerCharacter.VIPLevel);
                        pet.BaseProp = JsonConvert.SerializeObject(usersPetInfo2);
                        UpdateEvolutionPet(usersPetInfo2, pet.Level, MaxLevelByGrade, Player.PlayerCharacter.VIPLevel);
                        pet.AttackGrow = usersPetInfo2.AttackGrow;
                        pet.DefenceGrow = usersPetInfo2.DefenceGrow;
                        pet.AgilityGrow = usersPetInfo2.AgilityGrow;
                        pet.LuckGrow = usersPetInfo2.LuckGrow;
                        pet.BloodGrow = usersPetInfo2.BloodGrow;
                        pet.DamageGrow = usersPetInfo2.DamageGrow;
                        pet.GuardGrow = usersPetInfo2.GuardGrow;
                    }
                }
                pet.GetSkillEquip();
                if (pet.IsSeal && !lacresPet.Any((UsersPetLacrarInfo info) => info.isSeal && info.petId == pet.ID))
                {
                    pet.IsSeal = false;
                    Debugger.Log("Lacre para o pet não foi encontrado, então o pet foi liberado", "LogLiberarPet" + iD);
                }
                AddPetTo(pet, pet.Place);
            }
            foreach (UsersPetLacrarInfo item in lacresPet)
            {
                if (!item.isSeal)
                {
                    continue;
                }
                UsersPetInfo petById = GetPetById(item.petId);
                if (petById == null)
                {
                    Debugger.Log("O pet do lacre não foi encontrado, então o lacre foi liberado", "LogLiberarLacre" + iD);
                    item.isSeal = false;
                }
                else if (!petById.IsSeal)
                {
                    if (petById.IsEquip)
                    {
                        petById.IsEquip = false;
                    }
                    petById.IsSeal = true;
                    OnPlaceChanged(petById.Place);
                }
            }
            if (allUsersPetFormByID == null)
            {
                lock (m_lock)
                {
                    m_petForm = new UsersPetFormInfo
                    {
                        UserID = iD,
                        PetsID = -1,
                        ActivePets = "[]"
                    };
                }
            }
            else
            {
                lock (m_lock)
                {
                    m_petForm = allUsersPetFormByID;
                }
            }
            if (allEatPetsByID == null)
            {
                lock (m_lock)
                {
                    m_eatPets = new EatPetsInfo
                    {
                        UserID = iD
                    };
                    return;
                }
            }
            lock (m_lock)
            {
                m_eatPets = allEatPetsByID;
            }
        }
        finally
        {
            DeserializePetForms();
            CommitChanges();
        }
    }

    public virtual void SaveToDatabase(bool saveAdopt)
    {
        if (!m_saveToDb)
        {
            return;
        }
        using PlayerBussiness playerBussiness = new PlayerBussiness();
        lock (m_lock)
        {
            for (int i = 0; i < m_pets.Length; i++)
            {
                UsersPetInfo usersPetInfo = m_pets[i];
                if (usersPetInfo != null && usersPetInfo.IsDirty)
                {
                    usersPetInfo.eQPets = SerializePetEquip(usersPetInfo.PetEquips);
                    if (usersPetInfo.ID > 0)
                    {
                        playerBussiness.UpdateUserPet(usersPetInfo);
                    }
                    else
                    {
                        playerBussiness.AddUserPet(usersPetInfo);
                    }
                }
            }
            saveEqAwakenPet(playerBussiness);
        }
        if (saveAdopt)
        {
            lock (m_lock)
            {
                for (int j = 0; j < m_adoptPets.Length; j++)
                {
                    UsersPetInfo usersPetInfo2 = m_adoptPets[j];
                    if (usersPetInfo2 != null && usersPetInfo2.IsDirty && usersPetInfo2.ID == 0)
                    {
                        playerBussiness.AddUserAdoptPet(usersPetInfo2, isUse: false);
                    }
                }
            }
        }
        lock (m_lock)
        {
            if (m_petForm != null && m_petForm.IsDirty)
            {
                SerializePetForms();
                if (m_petForm.ID == 0)
                {
                    playerBussiness.AddUsersPetForm(m_petForm);
                }
                else
                {
                    playerBussiness.UpdateUsersPetForm(m_petForm);
                }
            }
        }
        lock (m_lock)
        {
            if (m_eatPets != null && m_eatPets.IsDirty)
            {
                if (m_eatPets.ID == 0)
                {
                    playerBussiness.AddEatPets(m_eatPets);
                }
                else
                {
                    playerBussiness.UpdateEatPets(m_eatPets);
                }
            }
        }
        lock (m_lock)
        {
            foreach (UsersPetInfo removed in m_removedList)
            {
                playerBussiness.UpdateUserPet(removed);
            }
            m_removedList.Clear();
        }
    }

    public void DeserializePetForms()
    {
        try
        {
            if (PetForm == null || string.IsNullOrEmpty(PetForm.ActivePets))
            {
                return;
            }
            lock (m_lock)
            {
                m_activeList = (from p in JsonConvert.DeserializeObject<List<PetFormActiveListInfo>>(PetForm.ActivePets)
                                group p by p.TemplateID into g
                                select g.First()).ToList();
            }
        }
        catch (Exception exception)
        {
            log.Error("DeserializeMagicHouse fail: ", exception);
        }
    }

    public void SerializePetForms()
    {
        try
        {
            lock (m_lock)
            {
                m_petForm.ActivePets = JsonConvert.SerializeObject(m_activeList);
            }
        }
        catch (Exception exception)
        {
            log.Error("DeserializeMagicHouse fail: ", exception);
        }
    }

    public void UpdatePetFormProp(int hp, ref int enhanceHp, ref int reduceDamagePow)
    {
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        foreach (PetFormActiveListInfo active in ActiveList)
        {
            PetFormDataInfo petFormDataInfo = PetMgr.FindPetFormData(active.TemplateID);
            if (petFormDataInfo != null)
            {
                num += petFormDataInfo.HeathUp;
                num2 += petFormDataInfo.DamageReduce;
                num3 += petFormDataInfo.NoBombReduceDamage;
                num4 += petFormDataInfo.BurningResistance;
            }
        }
        enhanceHp = ((hp * (num / 100)) / 2);
        reduceDamagePow = num2;
    }

    public override bool AddPetTo(UsersPetInfo pet, int place)
    {
        if (base.AddPetTo(pet, place))
        {
            pet.UserID = m_player.PlayerCharacter.ID;
            pet.PetEquips = DeserializePetEquip(pet.eQPets);
            return true;
        }
        return false;
    }

    public bool AddPetForm(int petId)
    {
        if (petId > 0)
        {
            if (m_activeList.Find((PetFormActiveListInfo o) => o.TemplateID == petId) != null)
            {
                return false;
            }
            for (int i = 0; i < ActiveList.Count; i++)
            {
                ActiveList[i].IsFollow = false;
            }
            PetFormActiveListInfo petFormActiveListInfo = new PetFormActiveListInfo();
            petFormActiveListInfo.TemplateID = petId;
            petFormActiveListInfo.IsFollow = true;
            lock (m_lock)
            {
                m_activeList.Add(petFormActiveListInfo);
            }
            return true;
        }
        return false;
    }

    public List<PetFormActiveListInfo> GetAllPetForms()
    {
        List<PetFormActiveListInfo> list = new List<PetFormActiveListInfo>();
        foreach (PetFormActiveListInfo active in ActiveList)
        {
            if (PetMgr.FindPetFormData(active.TemplateID) != null)
            {
                list.Add(active);
            }
        }
        return list;
    }

    public PetFormActiveListInfo GetPetForm(int petId)
    {
        foreach (PetFormActiveListInfo active in ActiveList)
        {
            if (active.TemplateID == petId)
            {
                return active;
            }
        }
        return null;
    }

    public bool PetFormFollow(int petId, bool isFollow)
    {
        for (int i = 0; i < ActiveList.Count; i++)
        {
            ActiveList[i].IsFollow = false;
            if (ActiveList[i].TemplateID == petId)
            {
                ActiveList[i].IsFollow = isFollow;
                m_petForm.PetsID = petId;
            }
        }
        return false;
    }

    public List<PetEquipInfo> DeserializePetEquip(string eqString)
    {
        if (string.IsNullOrEmpty(eqString))
        {
            return new List<PetEquipInfo>();
        }
        List<PetEquipInfo> list = JsonConvert.DeserializeObject<List<PetEquipInfo>>(eqString);
        List<PetEquipInfo> list2 = new List<PetEquipInfo>();
        foreach (PetEquipInfo item in list)
        {
            if (item.Template == null)
            {
                ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item.eqTemplateID);
                if (itemTemplateInfo != null)
                {
                    PetEquipInfo petEquipInfo = new PetEquipInfo(itemTemplateInfo);
                    petEquipInfo.eqTemplateID = item.eqTemplateID;
                    petEquipInfo.eqType = item.eqType;
                    petEquipInfo.ValidDate = item.ValidDate;
                    petEquipInfo.startTime = item.startTime;
                    petEquipInfo.AttackCompose = item.AttackCompose;
                    petEquipInfo.DefendCompose = item.DefendCompose;
                    petEquipInfo.AgilityCompose = item.AgilityCompose;
                    petEquipInfo.LuckCompose = item.LuckCompose;
                    petEquipInfo.itemID = item.itemID;
                    list2.Add(petEquipInfo);
                }
            }
            else
            {
                list2.Add(item);
            }
        }
        return list2;
    }

    public string SerializePetEquip(List<PetEquipInfo> eqs)
    {
        return JsonConvert.SerializeObject(eqs);
    }

    public void UpdateEatPets()
    {
        m_player.Out.SendEatPetsInfo(EatPets);
    }

    public virtual bool OnChangedPetEquip(int place)
    {
        lock (m_lock)
        {
            if (m_pets[place] != null && m_pets[place].IsEquip)
            {
                m_player.EquipBag.UpdatePlayerProperties();
            }
        }
        OnPlaceChanged(place);
        return true;
    }

    public virtual void ReduceHunger()
    {
        UsersPetInfo petIsEquip = GetPetIsEquip();
        if (petIsEquip == null)
        {
            return;
        }
        int num = 0;
        int num2 = 0;
        if (petIsEquip.Hunger >= num2)
        {
            if (petIsEquip.Level >= 60)
            {
                petIsEquip.Hunger -= num2;
            }
            else
            {
                petIsEquip.Hunger -= num;
            }
            UpdatePet(petIsEquip, petIsEquip.Place);
        }
    }

    public bool CanAdd(ItemInfo item, List<PetEquipInfo> infos)
    {
        if (infos.Count == 3 || item.Template == null)
        {
            return false;
        }
        foreach (PetEquipInfo info in infos)
        {
            if (item.eqType() == info.eqType)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckEqPetLevel(int place, ItemInfo item)
    {
        UsersPetInfo petAt = GetPetAt(place);
        if (petAt == null)
        {
            return true;
        }
        if (petAt.Level < item.Template.Property2)
        {
            return true;
        }
        return false;
    }

    public bool AddEqPet(int place, ItemInfo item)
    {
        UsersPetInfo petAt = GetPetAt(place);
        if (petAt == null)
        {
            return false;
        }
        if (CanAdd(item, m_pets[place].PetEquips))
        {
            PetEquipInfo item2 = new PetEquipInfo(item.Template)
            {
                eqTemplateID = item.TemplateID,
                eqType = item.eqType(),
                ValidDate = item.ValidDate,
                startTime = item.BeginDate,
                AttackCompose = item.AttackCompose,
                DefendCompose = item.DefendCompose,
                AgilityCompose = item.AgilityCompose,
                LuckCompose = item.LuckCompose,
                itemID = item.ItemID
            };
            petAt.PetEquips.Add(item2);
            return true;
        }
        return false;
    }

    public PetEquipInfo GetEqPet(List<PetEquipInfo> infos, int place)
    {
        foreach (PetEquipInfo info in infos)
        {
            if (info.eqType == place)
            {
                return info;
            }
        }
        return null;
    }

    public bool RemoveEqPet(int petPlace, int eqPlace)
    {
        UsersPetInfo petAt = GetPetAt(petPlace);
        if (petAt == null)
        {
            return false;
        }
        PetEquipInfo eqPet = GetEqPet(petAt.PetEquips, eqPlace);
        if (eqPet != null)
        {
            ChangeEqPetToItem(eqPet);
            return petAt.PetEquips.Remove(eqPet);
        }
        return false;
    }

    public void ChangeEqPetToItem(PetEquipInfo eq)
    {
        if (eq.Template != null)
        {
            ItemInfo itemInfo = ItemInfo.CreateFromTemplate(eq.Template, 1, 105);
            itemInfo.IsBinds = true;
            itemInfo.IsUsed = true;
            itemInfo.ValidDate = eq.ValidDate;
            itemInfo.BeginDate = eq.startTime;
            itemInfo.AttackCompose = eq.AttackCompose;
            itemInfo.DefendCompose = eq.DefendCompose;
            itemInfo.AgilityCompose = eq.AgilityCompose;
            itemInfo.LuckCompose = eq.LuckCompose;
            itemInfo.ItemID = eq.itemID;
            m_player.AddTemplate(itemInfo);
        }
    }

    public void RemoveAllEqPet(List<PetEquipInfo> infos)
    {
        foreach (PetEquipInfo info in infos)
        {
            ChangeEqPetToItem(info);
        }
    }

    public override bool RemovePet(UsersPetInfo pet)
    {
        if (base.RemovePet(pet))
        {
            if (pet.PetEquips != null && pet.PetEquips.Count > 0)
            {
                RemoveAllEqPet(pet.PetEquips);
            }
            lock (m_removedList)
            {
                pet.IsExit = false;
                m_removedList.Add(pet);
            }
            return true;
        }
        return false;
    }

    public override void UpdateChangedPlaces()
    {
        if (m_changedPlaces.Count > 20)
        {
            IEnumerable<IEnumerable<int>> enumerable = m_changedPlaces.ToArray().Split(20);
            foreach (IEnumerable<int> item in enumerable)
            {
                m_player.Out.SendUpdateUserPet(this, item.ToArray());
            }
        }
        else
        {
            m_player.Out.SendUpdateUserPet(this, m_changedPlaces.ToArray());
        }
        base.UpdateChangedPlaces();
    }

    public virtual void ClearAdoptPets()
    {
        using PlayerBussiness playerBussiness = new PlayerBussiness();
        lock (m_lock)
        {
            for (int i = 0; i < base.ACapalility; i++)
            {
                if (m_adoptPets[i] != null && m_adoptPets[i].ID > 0)
                {
                    playerBussiness.ClearAdoptPet(m_adoptPets[i].ID);
                }
                m_adoptPets[i] = null;
            }
        }
    }

    public UsersPetLacrarInfo GetLacreByPlace(int place)
    {
        lock (m_lock)
        {
            foreach (UsersPetLacrarInfo item in lacresPet)
            {
                if (item.place == place)
                {
                    return item;
                }
            }
        }
        return null;
    }
}
