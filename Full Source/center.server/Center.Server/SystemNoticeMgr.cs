using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using log4net;

namespace Center.Server
{
	public class SystemNoticeMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static List<string> NotceList = new List<string>();

		private static object _syncStop = new object();

		private static string SystemNoticeFile => AppDomain.CurrentDomain.BaseDirectory + "SystemNotice.xml";

		public static bool Start()
		{
			try
			{
				return LoadNotice("");
			}
			catch (Exception arg)
			{
				log.ErrorFormat("Load server list from db failed:{0}", arg);
				return false;
			}
		}

		public static bool LoadNotice(string path)
		{
			string text = path + SystemNoticeFile;
			if (!File.Exists(text))
			{
				log.Error("SystemNotice file : " + text + " not found !");
			}
			else
			{
				try
				{
					foreach (XElement item in XDocument.Load(text).Root.Nodes())
					{
						try
						{
							int.Parse(item.Attribute("id").Value);
							string value = item.Attribute("notice").Value;
							NotceList.Add(value);
						}
						catch (Exception exception)
						{
							log.Error("BattleMgr setup error:", exception);
						}
					}
				}
				catch (Exception exception2)
				{
					log.Error("BattleMgr setup error:", exception2);
				}
			}
			log.InfoFormat("Total {0} syterm notice loaded.", NotceList.Count);
			return true;
		}
	}
}
