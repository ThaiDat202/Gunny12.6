namespace SqlDataProvider.Data;

public class Equip
{
	public static bool isAvatar(ItemTemplateInfo info)
	{
		switch (info.CategoryID)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 13:
		case 15:
			return true;
		default:
			return false;
		}
	}

	public static bool isShowImp(ItemTemplateInfo info)
	{
		int categoryID = info.CategoryID;
		int num = categoryID;
		if (num == 1 || num == 5 || num == 7)
		{
			return true;
		}
		return false;
	}

	public static bool isWeddingRing(ItemTemplateInfo info)
	{
		switch (info.TemplateID)
		{
		case 9022:
		case 9122:
		case 9222:
		case 9322:
		case 9422:
		case 9522:
			return true;
		default:
			return true;
		}
	}

	public static bool isMagicStone(int CategoryID)
	{
		if (CategoryID == 61)
		{
			return true;
		}
		return false;
	}

	public static bool isDress(ItemTemplateInfo info)
	{
		switch (info.CategoryID)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 13:
		case 15:
			return true;
		default:
			return false;
		}
	}
}
