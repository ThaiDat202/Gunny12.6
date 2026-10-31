using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Game.Base;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class SpiritSkillMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static List<SpiritInfo> _spiritList;

	public static int MAX_LEVEL = 0;

	public static bool Init()
	{
		try
		{
			_spiritList = new List<SpiritInfo>();
			return ReLoad();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SpiritInfoMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			List<SpiritInfo> list = new List<SpiritInfo>();
			if (LoadSpirits(list))
			{
				try
				{
					_spiritList = list;
					return true;
				}
				catch
				{
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SpiritInfoMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadSpirits(List<SpiritInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("spiritinfolist", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				int num = int.Parse(item.Attributes["Level"].Value);
				SpiritInfo spiritInfo = new SpiritInfo();
				spiritInfo.Level = num;
				spiritInfo.MustGetTimes = int.Parse(item.Attributes["MustGetTimes"].Value);
				spiritInfo.BaseSuccessPro = int.Parse(item.Attributes["BaseSuccessPro"].Value);
				spiritInfo.RefrenceValue = int.Parse(item.Attributes["RefrenceValue"].Value);
				spiritInfo.SkillId = int.Parse(item.Attributes["SkillId"].Value);
				spiritInfo.AttackAdd = int.Parse(item.Attributes["AttackAdd"].Value);
				spiritInfo.LuckAdd = int.Parse(item.Attributes["LuckAdd"].Value);
				spiritInfo.DefendAdd = int.Parse(item.Attributes["DefendAdd"].Value);
				spiritInfo.AgilityAdd = int.Parse(item.Attributes["AgilityAdd"].Value);
				spiritInfo.BagType = int.Parse(item.Attributes["BagType"].Value);
				spiritInfo.BagPlace = int.Parse(item.Attributes["BagPlace"].Value);
				spiritInfo.CategoryId = int.Parse(item.Attributes["CategoryId"].Value);
				datas.Add(spiritInfo);
				if (MAX_LEVEL < num)
				{
					MAX_LEVEL = num;
				}
			}
			return true;
		}
		catch (Exception message)
		{
			log.Error(message);
		}
		return false;
	}

	public static int GetSpiritSkill(int bagType, int place, int level)
	{
		lock (_spiritList)
		{
			foreach (SpiritInfo spirit in _spiritList)
			{
				if (spirit.BagType == bagType && spirit.BagPlace == place && spirit.Level == level)
				{
					return spirit.SkillId;
				}
			}
		}
		return 0;
	}
}
