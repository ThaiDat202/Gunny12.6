using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Bussiness.Protocol;
using Game.Base;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class EngraveMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, EngraveSetInfo> _engraveSetList;

	private static Dictionary<int, EngraveDebrisInfo> _engraveDebrisList;

	private static Dictionary<int, List<DebrisPropertyConfigInfo>> _debrisPropertyConfigs;

	private static List<EngraveRefineryConfigInfo> _engraveRefineryConfigs;

	private static List<EngraveTemperConfigInfo> _engraveTemperConfigs;

	private static Dictionary<int, EngraveSetElementInfo> _engraveSetElements;

	private static ThreadSafeRandom m_random;

	public static bool Init()
	{
		try
		{
			_engraveSetList = new Dictionary<int, EngraveSetInfo>();
			_engraveDebrisList = new Dictionary<int, EngraveDebrisInfo>();
			_debrisPropertyConfigs = new Dictionary<int, List<DebrisPropertyConfigInfo>>();
			_engraveRefineryConfigs = new List<EngraveRefineryConfigInfo>();
			_engraveSetElements = new Dictionary<int, EngraveSetElementInfo>();
			m_random = new ThreadSafeRandom();
			return ReLoad();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("EngraveMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, EngraveSetInfo> dictionary = new Dictionary<int, EngraveSetInfo>();
			Dictionary<int, List<DebrisPropertyConfigInfo>> dictionary2 = new Dictionary<int, List<DebrisPropertyConfigInfo>>();
			Dictionary<int, EngraveDebrisInfo> dictionary3 = new Dictionary<int, EngraveDebrisInfo>();
			List<EngraveRefineryConfigInfo> list = new List<EngraveRefineryConfigInfo>();
			List<EngraveTemperConfigInfo> list2 = new List<EngraveTemperConfigInfo>();
			Dictionary<int, EngraveSetElementInfo> dictionary4 = new Dictionary<int, EngraveSetElementInfo>();
			if (LoadEngraveSet(dictionary) && LoadDebrisPropertyConfig(dictionary2) && LoadEngraveDebrisInfo(dictionary3) && EngraveRefineryConfig(list) && EngraveTemperConfig(list2) && EngraveSetElement(dictionary4))
			{
				try
				{
					_engraveSetList = dictionary;
					_debrisPropertyConfigs = dictionary2;
					_engraveDebrisList = dictionary3;
					_engraveRefineryConfigs = list;
					_engraveTemperConfigs = list2;
					_engraveSetElements = dictionary4;
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
				log.Error("EngraveMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadEngraveSet(Dictionary<int, EngraveSetInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("EngraveSetInfo", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				int num = int.Parse(item.Attributes["SetId"].Value);
				if (!datas.ContainsKey(num))
				{
					EngraveSetInfo engraveSetInfo = new EngraveSetInfo();
					engraveSetInfo.SetId = num;
					engraveSetInfo.Name = item.Attributes["Name"].Value;
					engraveSetInfo.HelpExplain = item.Attributes["HelpExplain"].Value;
					datas.Add(num, engraveSetInfo);
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

	private static bool LoadDebrisPropertyConfig(Dictionary<int, List<DebrisPropertyConfigInfo>> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("DebrisPropertyConfig", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				int num = int.Parse(item.Attributes["ID"].Value);
				DebrisPropertyConfigInfo debrisPropertyConfigInfo = new DebrisPropertyConfigInfo();
				debrisPropertyConfigInfo.ID = num;
				debrisPropertyConfigInfo.AttributeType = int.Parse(item.Attributes["AttributeType"].Value);
				debrisPropertyConfigInfo.MinValue = int.Parse(item.Attributes["MinValue"].Value);
				debrisPropertyConfigInfo.MaxValue = int.Parse(item.Attributes["MaxValue"].Value);
				if (!datas.ContainsKey(num))
				{
					List<DebrisPropertyConfigInfo> list = new List<DebrisPropertyConfigInfo>();
					list.Add(debrisPropertyConfigInfo);
					datas.Add(num, list);
				}
				else
				{
					datas[num].Add(debrisPropertyConfigInfo);
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

	private static bool LoadEngraveDebrisInfo(Dictionary<int, EngraveDebrisInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("EngraveDebrisInfo", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				int num = int.Parse(item.Attributes["Id"].Value);
				if (!datas.ContainsKey(num))
				{
					EngraveDebrisInfo engraveDebrisInfo = new EngraveDebrisInfo();
					engraveDebrisInfo.Id = num;
					engraveDebrisInfo.Place = int.Parse(item.Attributes["Place"].Value);
					engraveDebrisInfo.Character = int.Parse(item.Attributes["Character"].Value);
					engraveDebrisInfo.SetID = int.Parse(item.Attributes["SetID"].Value);
					engraveDebrisInfo.AttributeAdd1 = int.Parse(item.Attributes["AttributeAdd1"].Value);
					engraveDebrisInfo.AttributeAdd2 = int.Parse(item.Attributes["AttributeAdd2"].Value);
					engraveDebrisInfo.AttributeAdd3 = int.Parse(item.Attributes["AttributeAdd3"].Value);
					engraveDebrisInfo.AttributeAdd4 = int.Parse(item.Attributes["AttributeAdd4"].Value);
					engraveDebrisInfo.AttributeAdd5 = int.Parse(item.Attributes["AttributeAdd5"].Value);
					engraveDebrisInfo.AttributeAdd6 = int.Parse(item.Attributes["AttributeAdd6"].Value);
					engraveDebrisInfo.AttributeAdd7 = int.Parse(item.Attributes["AttributeAdd7"].Value);
					engraveDebrisInfo.AttributeAdd8 = int.Parse(item.Attributes["AttributeAdd8"].Value);
					engraveDebrisInfo.AttributeAdd9 = int.Parse(item.Attributes["AttributeAdd9"].Value);
					engraveDebrisInfo.AttributeAdd10 = int.Parse(item.Attributes["AttributeAdd10"].Value);
					engraveDebrisInfo.AttributeAdd11 = int.Parse(item.Attributes["AttributeAdd11"].Value);
					engraveDebrisInfo.AttributeAdd12 = int.Parse(item.Attributes["AttributeAdd12"].Value);
					engraveDebrisInfo.AttributeAdd13 = int.Parse(item.Attributes["AttributeAdd13"].Value);
					engraveDebrisInfo.AttributeAdd14 = int.Parse(item.Attributes["AttributeAdd14"].Value);
					datas.Add(num, engraveDebrisInfo);
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

	private static bool EngraveRefineryConfig(List<EngraveRefineryConfigInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("EngraveRefineryConfigInfo", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				EngraveRefineryConfigInfo engraveRefineryConfigInfo = new EngraveRefineryConfigInfo();
				engraveRefineryConfigInfo.Grade = int.Parse(item.Attributes["Grade"].Value);
				engraveRefineryConfigInfo.Character = int.Parse(item.Attributes["Character"].Value);
				engraveRefineryConfigInfo.Currency = int.Parse(item.Attributes["Currency"].Value);
				engraveRefineryConfigInfo.Expend = int.Parse(item.Attributes["Expend"].Value);
				engraveRefineryConfigInfo.Material = int.Parse(item.Attributes["Material"].Value);
				engraveRefineryConfigInfo.NeedMaterial = int.Parse(item.Attributes["NeedMaterial"].Value);
				datas.Add(engraveRefineryConfigInfo);
			}
			return true;
		}
		catch (Exception message)
		{
			log.Error(message);
		}
		return false;
	}

	private static bool EngraveTemperConfig(List<EngraveTemperConfigInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("engravetemperconfiginfo", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				EngraveTemperConfigInfo engraveTemperConfigInfo = new EngraveTemperConfigInfo();
				engraveTemperConfigInfo.Level = int.Parse(item.Attributes["Level"].Value);
				engraveTemperConfigInfo.Character = int.Parse(item.Attributes["Character"].Value);
				engraveTemperConfigInfo.Currency = int.Parse(item.Attributes["Currency"].Value);
				engraveTemperConfigInfo.Expend = int.Parse(item.Attributes["Expend"].Value);
				engraveTemperConfigInfo.SuccessRate = int.Parse(item.Attributes["SuccessRate"].Value);
				datas.Add(engraveTemperConfigInfo);
			}
			return true;
		}
		catch (Exception message)
		{
			log.Error(message);
		}
		return false;
	}

	private static bool EngraveSetElement(Dictionary<int, EngraveSetElementInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("EngraveSetElementInfo", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				EngraveSetElementInfo engraveSetElementInfo = new EngraveSetElementInfo();
				engraveSetElementInfo.Id = int.Parse(item.Attributes["Id"].Value);
				if (!datas.ContainsKey(engraveSetElementInfo.Id))
				{
					engraveSetElementInfo.Name = item.Attributes["Name"].Value;
					engraveSetElementInfo.SetId = int.Parse(item.Attributes["SetId"].Value);
					engraveSetElementInfo.SkillId = int.Parse(item.Attributes["SkillId"].Value);
					engraveSetElementInfo.Explain = item.Attributes["Explain"].Value;
					engraveSetElementInfo.Demand = int.Parse(item.Attributes["Demand"].Value);
					engraveSetElementInfo.Attribute = item.Attributes["Attribute"].Value;
					datas.Add(engraveSetElementInfo.Id, engraveSetElementInfo);
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

	public static EngraveSetElementInfo GetEngraveSetElement(int Id)
	{
		if (_engraveSetElements.ContainsKey(Id))
		{
			return _engraveSetElements[Id];
		}
		return null;
	}

	public static List<EngraveSetElementInfo> GetAllEngraveSetElement()
	{
		return _engraveSetElements.Values.ToList();
	}

	public static EngraveTemperConfigInfo GetTemperConfig(int level, int character)
	{
		level = Math.Min(level + 1, GetTemperTopLevel(character));
		EngraveTemperConfigInfo engraveTemperConfigInfo = null;
		foreach (EngraveTemperConfigInfo engraveTemperConfig in _engraveTemperConfigs)
		{
			engraveTemperConfigInfo = engraveTemperConfig;
			if (engraveTemperConfigInfo.Character == character && engraveTemperConfigInfo.Level == level)
			{
				return engraveTemperConfigInfo;
			}
		}
		return engraveTemperConfigInfo;
	}

	public static int GetTemperTopLevel(int character)
	{
		int num = 0;
		foreach (EngraveTemperConfigInfo engraveTemperConfig in _engraveTemperConfigs)
		{
			if (engraveTemperConfig.Character == character && engraveTemperConfig.Level >= num)
			{
				num = engraveTemperConfig.Level;
			}
		}
		return num;
	}

	public static EngraveRefineryConfigInfo GetRefineryConfig(int character, int grade)
	{
		return _engraveRefineryConfigs.SingleOrDefault((EngraveRefineryConfigInfo a) => a.Character == character && a.Grade == grade);
	}

	public static List<DebrisPropertyConfigInfo> GetDebrisProperty(int templateId, List<MarkProDataInfo> extec = null)
	{
		List<DebrisPropertyConfigInfo> list = new List<DebrisPropertyConfigInfo>();
		if (_debrisPropertyConfigs.ContainsKey(templateId))
		{
			list.AddRange(_debrisPropertyConfigs[templateId]);
		}
		if (extec != null)
		{
			foreach (MarkProDataInfo ex in extec)
			{
				if (ex.type != 0)
				{
					list.RemoveAll((DebrisPropertyConfigInfo a) => a.AttributeType == ex.type);
				}
			}
		}
		return list;
	}

	public static DebrisPropertyConfigInfo GetDebrisProperty(int templateId, int type)
	{
		if (_debrisPropertyConfigs.ContainsKey(templateId))
		{
			return _debrisPropertyConfigs[templateId].SingleOrDefault((DebrisPropertyConfigInfo a) => a.AttributeType == type);
		}
		return null;
	}

	public static EngraveDebrisInfo GetSingleEngraveDebris(int templateId)
	{
		if (_engraveDebrisList.ContainsKey(templateId))
		{
			return _engraveDebrisList[templateId];
		}
		return null;
	}

	public static List<MarkProDataInfo> GetAvalibleEngraveDebrisProp(EngraveDebrisInfo info)
	{
		List<MarkProDataInfo> list = new List<MarkProDataInfo>();
		if (info.AttributeAdd1 > 0)
		{
			list.Add(new MarkProDataInfo(31, info.AttributeAdd1));
		}
		if (info.AttributeAdd2 > 0)
		{
			list.Add(new MarkProDataInfo(32, info.AttributeAdd2));
		}
		if (info.AttributeAdd3 > 0)
		{
			list.Add(new MarkProDataInfo(33, info.AttributeAdd3));
		}
		if (info.AttributeAdd4 > 0)
		{
			list.Add(new MarkProDataInfo(34, info.AttributeAdd4));
		}
		if (info.AttributeAdd5 > 0)
		{
			list.Add(new MarkProDataInfo(37, info.AttributeAdd5));
		}
		if (info.AttributeAdd6 > 0)
		{
			list.Add(new MarkProDataInfo(36, info.AttributeAdd6));
		}
		if (info.AttributeAdd7 > 0)
		{
			list.Add(new MarkProDataInfo(35, info.AttributeAdd7));
		}
		if (info.AttributeAdd8 > 0)
		{
			list.Add(new MarkProDataInfo(101, info.AttributeAdd8));
		}
		if (info.AttributeAdd9 > 0)
		{
			list.Add(new MarkProDataInfo(102, info.AttributeAdd9));
		}
		if (info.AttributeAdd10 > 0)
		{
			list.Add(new MarkProDataInfo(1, info.AttributeAdd10));
		}
		if (info.AttributeAdd11 > 0)
		{
			list.Add(new MarkProDataInfo(3, info.AttributeAdd11));
		}
		if (info.AttributeAdd12 > 0)
		{
			list.Add(new MarkProDataInfo(7, info.AttributeAdd12));
		}
		if (info.AttributeAdd13 > 0)
		{
			list.Add(new MarkProDataInfo(9, info.AttributeAdd13));
		}
		if (info.AttributeAdd14 > 0)
		{
			list.Add(new MarkProDataInfo(5, info.AttributeAdd14));
		}
		return list;
	}

	public static int GetAttributeAdd(EngraveDebrisInfo info, int hLv, eMarkProType type)
	{
		EngraveDebrisInfo singleEngraveDebris = GetSingleEngraveDebris(info.Id);
		int num = 0;
		if (singleEngraveDebris != null)
		{
			switch (type)
			{
			case eMarkProType.ATTACK:
				num = singleEngraveDebris.AttributeAdd1;
				break;
			case eMarkProType.DEFENCE:
				num = singleEngraveDebris.AttributeAdd2;
				break;
			case eMarkProType.AGILITY:
				num = singleEngraveDebris.AttributeAdd3;
				break;
			case eMarkProType.LUCK:
				num = singleEngraveDebris.AttributeAdd4;
				break;
			case eMarkProType.BOOLD:
				num = singleEngraveDebris.AttributeAdd5;
				break;
			case eMarkProType.DAMAGE:
				num = singleEngraveDebris.AttributeAdd6;
				break;
			case eMarkProType.GUARD:
				num = singleEngraveDebris.AttributeAdd7;
				break;
			case eMarkProType.MAGICATTACK:
				num = singleEngraveDebris.AttributeAdd8;
				break;
			case eMarkProType.MAGICDEFENCE:
				num = singleEngraveDebris.AttributeAdd9;
				break;
			case eMarkProType.CIRCTAL:
				num = singleEngraveDebris.AttributeAdd10;
				break;
			case eMarkProType.SUNDERARMOR:
				num = singleEngraveDebris.AttributeAdd11;
				break;
			case eMarkProType.VIOLENCEINJURY:
				num = singleEngraveDebris.AttributeAdd12;
				break;
			case eMarkProType.SPEED:
				num = singleEngraveDebris.AttributeAdd13;
				break;
			case eMarkProType.KILL:
				num = singleEngraveDebris.AttributeAdd14;
				break;
			}
			num = (int)((double)num * 0.1 * (double)hLv);
		}
		return num;
	}

	public static List<EngraveSetInfo> GetAllEngraveSet()
	{
		return _engraveSetList.Values.ToList();
	}
}
