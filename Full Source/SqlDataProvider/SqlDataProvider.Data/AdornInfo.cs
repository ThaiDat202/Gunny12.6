namespace SqlDataProvider.Data;

public class AdornInfo : DataObject
{
	private int _ID;

	private int _userID;

	private int _templateId;

	private int _category;

	public int ID
	{
		get
		{
			return _ID;
		}
		set
		{
			_ID = value;
			_isDirty = true;
		}
	}

	public int UserID
	{
		get
		{
			return _userID;
		}
		set
		{
			_userID = value;
			_isDirty = true;
		}
	}

	public int TemplateId
	{
		get
		{
			return _templateId;
		}
		set
		{
			_templateId = value;
			_isDirty = true;
		}
	}

	public int Category
	{
		get
		{
			return _category;
		}
		set
		{
			_category = value;
			_isDirty = true;
		}
	}
}
