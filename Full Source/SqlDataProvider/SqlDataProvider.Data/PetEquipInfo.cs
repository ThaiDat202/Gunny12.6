using System;

namespace SqlDataProvider.Data;

public class PetEquipInfo
{
	private ItemTemplateInfo _template;

	public ItemTemplateInfo Template => _template;

	public int eqType { get; set; }

	public int eqTemplateID { get; set; }

	public DateTime startTime { get; set; }

	public int ValidDate { get; set; }

	public int itemID { get; set; }

	public int AttackCompose { get; set; }

	public int DefendCompose { get; set; }

	public int LuckCompose { get; set; }

	public int AgilityCompose { get; set; }

	public bool canRenew { get; set; }

	public bool canTransfer { get; set; }

	public bool IsAwaken { get; set; }

	public PetEquipInfo(ItemTemplateInfo temp)
	{
		_template = temp;
	}

	public bool IsValidItem()
	{
		if (ValidDate != 0)
		{
			return DateTime.Compare(startTime.AddDays(ValidDate), DateTime.Now) > 0;
		}
		return true;
	}
}
