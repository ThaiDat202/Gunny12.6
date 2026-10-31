using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using Game.Base;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class StockMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, StockInfo> _stockList;

	private static Dictionary<int, StockNoticeInfo> _stockNoticeList;

	private static BackgroundWorker m_worker;

	private static readonly int m_perMin = 1;

	private static DateTime m_lastTimeChange = DateTime.Now;

	public static void Init()
	{
		_stockList = new Dictionary<int, StockInfo>();
		_stockNoticeList = new Dictionary<int, StockNoticeInfo>();
		m_worker = new BackgroundWorker();
		m_worker.DoWork += M_worker_DoWork;
		m_worker.RunWorkerAsync();
		ReLoad();
	}

	private static void M_worker_DoWork(object sender, DoWorkEventArgs e)
	{
		if (!m_worker.IsBusy && m_lastTimeChange.AddMinutes(m_perMin) < DateTime.Now)
		{
			Console.WriteLine("Stock try scaning...");
			Console.WriteLine("Stock scaning completed!");
			m_lastTimeChange = DateTime.Now;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, StockNoticeInfo> dictionary = new Dictionary<int, StockNoticeInfo>();
			if (LoadStockNotice(dictionary))
			{
				try
				{
					_stockNoticeList = dictionary;
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
				log.Error("StockMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadStockNotice(Dictionary<int, StockNoticeInfo> datas)
	{
		try
		{
			XmlDocument xmlDocument = Marshal.LoadXMLData("StockNoticeList", isEncrypt: true);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("Result/child::node()");
			foreach (XmlNode item in xmlNodeList)
			{
				int num = int.Parse(item.Attributes["NewsId"].Value);
				if (!datas.ContainsKey(num))
				{
					StockNoticeInfo stockNoticeInfo = new StockNoticeInfo();
					stockNoticeInfo.NewsId = num;
					stockNoticeInfo.NewsContent = item.Attributes["NewsContent"].Value;
					datas.Add(num, stockNoticeInfo);
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
}
