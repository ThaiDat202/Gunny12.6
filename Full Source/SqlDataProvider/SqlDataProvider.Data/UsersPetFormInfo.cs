namespace SqlDataProvider.Data;

public class UsersPetFormInfo : DataObject
{
	private int _iD;

	private int _userID;

	private int _petsID;

	private string _activePets;

	public int ID
	{
		get
		{
			return _iD;
		}
		set
		{
			_iD = value;
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

	public int PetsID
	{
		get
		{
			return _petsID;
		}
		set
		{
			_petsID = value;
			_isDirty = true;
		}
	}

	public string ActivePets
	{
		get
		{
			return _activePets;
		}
		set
		{
			_activePets = value;
			_isDirty = true;
		}
	}
}
