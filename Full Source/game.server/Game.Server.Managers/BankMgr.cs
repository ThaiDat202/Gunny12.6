using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Game.Base;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class BankMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, BankTemplateInfo> _bankTemplate;

	public static bool Init()
	{
		try
		{
			_bankTemplate = new Dictionary<int, BankTemplateInfo>();
			return ReLoad();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("LevelMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, BankTemplateInfo> dictionary = new Dictionary<int, BankTemplateInfo>();
			if (LoadBankTemplate(dictionary))
			{
				try
				{
					_bankTemplate = dictionary;
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
				log.Error("LevelMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadBankTemplate(Dictionary<int, BankTemplateInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("banktemplateinfo", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				int num = int.Parse(item.Attributes["ID"].Value);
				if (!datas.ContainsKey(num))
				{
					BankTemplateInfo bankTemplateInfo = new BankTemplateInfo();
					bankTemplateInfo.ID = num;
					bankTemplateInfo.Name = item.Attributes["Name"].Value;
					bankTemplateInfo.InterestRate = int.Parse(item.Attributes["InterestRate"].Value);
					bankTemplateInfo.MinAmount = int.Parse(item.Attributes["MinAmount"].Value);
					bankTemplateInfo.Multiple = int.Parse(item.Attributes["Multiple"].Value);
					bankTemplateInfo.Consume = int.Parse(item.Attributes["Consume"].Value);
					bankTemplateInfo.DeadLine = int.Parse(item.Attributes["DeadLine"].Value);
					datas.Add(num, bankTemplateInfo);
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

	public static BankTemplateInfo GetBankTemplate(int id)
	{
		if (_bankTemplate.ContainsKey(id))
		{
			return _bankTemplate[id];
		}
		return null;
	}

	public static int GetProfitMoney(UserBankInfo bankInfo, bool isAll, int realMoney, bool isAchieve)
	{
		int result = 0;
		int num = (isAll ? bankInfo.Amount : realMoney);
		BankTemplateInfo bankTemplate = GetBankTemplate(bankInfo.TempID);
		if (bankTemplate != null)
		{
			int num2 = (DateTime.Now - bankInfo.BeginTime).Days;
			if (num2 == 0 && isAll)
			{
				num2 = 1;
			}
			result = ((bankTemplate.DeadLine == 0) ? (num * bankTemplate.InterestRate * num2 / 10000) : ((!isAchieve) ? (num * bankTemplate.InterestRate * num2 / 10000) : (num * bankTemplate.InterestRate * bankTemplate.DeadLine * 30 / 10000)));
		}
		return result;
	}
}
