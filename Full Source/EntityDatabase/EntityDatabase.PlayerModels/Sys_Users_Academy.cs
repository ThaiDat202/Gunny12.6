using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_Academy
{
	public string m_masterOrApprentices;

	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int ApprenticeshipState { get; set; }

	public int MasterID { get; set; }

	public int GraduatesCount { get; set; }

	public string HonourOfMaster { get; set; }

	public DateTime FreezesDate { get; set; }

	public bool IsFirst { get; set; }

	public bool Get100 { get; set; }

	public bool Get300 { get; set; }

	public bool Get500 { get; set; }

	public bool Get1000 { get; set; }

	public bool IsMaster { get; set; }

	public string MasterOrApprentices
	{
		get
		{
			return m_masterOrApprentices;
		}
		set
		{
			m_masterOrApprentices = value;
			updateMasterOrApprenticesArr(value);
		}
	}

	public Dictionary<int, string> MasterOrApprenticesArr => _masterOrApprenticesArr;

	private Dictionary<int, string> _masterOrApprenticesArr { get; set; }

	public bool AddMasterOrApprentices(int id, string nickname)
	{
		if (!MasterOrApprenticesArr.ContainsKey(id))
		{
			MasterOrApprenticesArr.Add(id, nickname);
			ConvertMasterOrApprentices();
			return true;
		}
		return false;
	}

	public bool RemoveMasterOrApprentices(int id)
	{
		if (MasterOrApprenticesArr.ContainsKey(id))
		{
			MasterOrApprenticesArr.Remove(id);
			ConvertMasterOrApprentices();
			return true;
		}
		return false;
	}

	public void updateMasterOrApprenticesArr(string val)
	{
		if (_masterOrApprenticesArr == null)
		{
			_masterOrApprenticesArr = new Dictionary<int, string>();
		}
		lock (_masterOrApprenticesArr)
		{
			_masterOrApprenticesArr.Clear();
			if (val != null && val != "")
			{
				string[] array = val.Split(',');
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split('|');
					_masterOrApprenticesArr.Add(int.Parse(array3[0]), array3[1]);
				}
			}
		}
	}

	public void ConvertMasterOrApprentices()
	{
		List<string> list = new List<string>();
		lock (_masterOrApprenticesArr)
		{
			if (_masterOrApprenticesArr.Count == 0)
			{
				ApprenticeshipState = 0;
			}
			else if (_masterOrApprenticesArr.Count >= 3)
			{
				ApprenticeshipState = 3;
			}
			else if (_masterOrApprenticesArr.Count > 0 && MasterID != 0)
			{
				ApprenticeshipState = 1;
			}
			else if (_masterOrApprenticesArr.Count > 0 && MasterID == 0)
			{
				ApprenticeshipState = 2;
			}
			foreach (KeyValuePair<int, string> item in _masterOrApprenticesArr)
			{
				list.Add(item.Key + "|" + item.Value);
			}
		}
		m_masterOrApprentices = string.Join(",", list);
	}
}
