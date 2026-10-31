namespace SqlDataProvider.Data;

public class ItemTemplateInfo : DataObject
{
	public int TemplateID { get; set; }

	public string Name { get; set; }

	public int CategoryID { get; set; }

	public string Description { get; set; }

	public int Attack { get; set; }

	public int Defence { get; set; }

	public int Luck { get; set; }

	public int Agility { get; set; }

	public int Level { get; set; }

	public string Pic { get; set; }

	public string AddTime { get; set; }

	public int Quality { get; set; }

	public int MaxCount { get; set; }

	public string Data { get; set; }

	public int Property1 { get; set; }

	public int Property2 { get; set; }

	public int Property3 { get; set; }

	public int Property4 { get; set; }

	public int Property5 { get; set; }

	public int Property6 { get; set; }

	public int Property7 { get; set; }

	public int Property8 { get; set; }

	public int NeedSex { get; set; }

	public int NeedLevel { get; set; }

	public bool CanDrop { get; set; }

	public bool CanDelete { get; set; }

	public bool CanEquip { get; set; }

	public bool CanUse { get; set; }

	public string Script { get; set; }

	public string Colors { get; set; }

	public bool CanStrengthen { get; set; }

	public bool CanCompose { get; set; }

	public int BindType { get; set; }

	public int FusionType { get; set; }

	public int FusionRate { get; set; }

	public int FusionNeedRate { get; set; }

	public int RefineryType { get; set; }

	public string Hole { get; set; }

	public int RefineryLevel { get; set; }

	public int ReclaimValue { get; set; }

	public int ReclaimType { get; set; }

	public int CanRecycle { get; set; }

	public int FloorPrice { get; set; }

	public int SuitId { get; set; }

	public int CanTransfer { get; set; }

	public eBageType BagType
	{
		get
		{
			switch (CategoryID)
			{
			case 10:
			case 11:
			case 12:
			case 18:
			case 20:
			case 24:
			case 34:
			case 35:
			case 43:
			case 53:
			case 62:
			case 68:
			case 72:
			case 78:
			case 80:
			case 85:
			case 86:
			case 88:
			case 180:
				return (Property1 != 31 || CategoryID != 11) ? eBageType.PropBag : eBageType.BeadBag;
			case 32:
				return eBageType.FarmBag;
			case 61:
				return eBageType.MagicStoneBag;
			case 69:
				return eBageType.HorseAmuletBag;
			case 81:
				return eBageType.MineBag;
			case 73:
				return eBageType.RoomBordenBag;
			case 74:
				return eBageType.MarkBag;
			default:
				return eBageType.EquipBag;
			}
		}
	}

	public bool IsAmulet => CategoryID == 70;
}
