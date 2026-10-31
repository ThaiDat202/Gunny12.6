using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using Bussiness;
using Bussiness.Interface;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server;
using Game.Server.BombKing;
using Game.Server.CardNewSystem;
using Game.Server.DevilTurn;
using Game.Server.Elf;
using Game.Server.Emblem;
using Game.Server.GameObjects;
using Game.Server.GypsyShop;
using Game.Server.HomeTemple;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Team;
using Game.Service.Helper;
using Newtonsoft.Json;
using Road.Service.Properties;
using SqlDataProvider.Data;
using Utils.Dlg;
using zlib;

namespace Road.Service.Helper.Forms;

public class Form1 : Form
{
	private static Form1 form;

	private DataBaseHelper db;

	private static int templateId;

	private static int Packtime;

	private static int count1;

	private static System.Threading.Timer _timer;

	private static int _minutes;

	private string NewItemPic = string.Empty;

	private string Pic = string.Empty;

	private int CategoryID;

	private int NeedSex;

	private int ActiveID;

	private System.Timers.Timer timer3;

	private System.Timers.Timer timer4;

	private IContainer components = null;

	private TextBox NewItemID_TextBox;

	private Label label41;

	private ComboBox ItemQuality_CBox;

	private Button InsertItem_Btn;

	private TextBox ItemLuck_TextBox;

	private TextBox ItemAgi_TextBox;

	private TextBox ItemDef_TextBox;

	private TextBox ItemAtt_TextBox;

	private TextBox ItemDesc_TextBox;

	private Label label39;

	private TextBox ItemArmor_Text;

	private GroupBox groupBox10;

	private Label label40;

	private PictureBox pictureBox3;

	private Label label38;

	private TextBox ItemName_TextBox;

	private Label label37;

	private Label label36;

	private Label label35;

	private Label label34;

	private Label label33;

	private Label label32;

	private ComboBox ListItem_CBox;

	private Label label31;

	private ComboBox ListCategoryID_CBox;

	private Label label30;

	private DataGridViewTextBoxColumn Column3;

	private RichTextBox richTextBox2;

	private DataGridViewTextBoxColumn Column2;

	private DataGridViewTextBoxColumn Column4;

	private GroupBox groupBox5;

	private Button RecoverMessage;

	private TextBox PasswordTwo_Text;

	private Label label15;

	private Button CleanUser_Text;

	private Button UpdateLevel_Btn;

	private TextBox Level_Text;

	private TextBox NickName_Text;

	private Button UpUserData_Btn;

	private Label label3;

	private Label label5;

	private System.Windows.Forms.Timer timer1;

	private GroupBox groupBox6;

	private Button FindRank_Btn;

	private DateTimePicker EndTime_TimeDate;

	private DateTimePicker StartTime_TimeDate;

	private Label label17;

	private Label label16;

	private GroupBox groupBox18;

	private Label UserID_Lab;

	private Label Label_d;

	private Label Online_Text;

	private Label label6;

	private TabPage tabPage3;

	private GroupBox groupBox20;

	private Button button4;

	private Button CreateAllCeleb;

	private Button Reload;

	private GroupBox groupBox11;

	private TextBox NewTemplateID_Text;

	private Label label29;

	private TextBox Special_Text;

	private TextBox CommonAddWound_Text;

	private TextBox CommonMultiBall_Text;

	private TextBox Common_Text;

	private Button InsertArm_Btn;

	private GroupBox groupBox12;

	private Label label28;

	private PictureBox pictureBox2;

	private ComboBox NewItemQuality_CBox;

	private TextBox NewItemDamage_Text;

	private TextBox NewItemAgi_Text;

	private TextBox NewItemLuck_Text;

	private TextBox NewItemDef_Text;

	private TextBox NewItemAtt_Text;

	private TextBox NewItemDesc_Text;

	private TextBox NewItemName_Text;

	private Label label27;

	private Label label26;

	private Label label25;

	private Label label24;

	private Label label23;

	private Label label22;

	private Label label21;

	private Label label20;

	private ComboBox ListItemName_CBox;

	private Label label19;

	private GroupBox groupBox7;

	private System.Windows.Forms.Timer mainWorker;

	private Button SendItemToPlayer;

	private Label label52;

	private ComboBox IsBind;

	private TextBox textBox5;

	private TextBox textBox12;

	private Label label53;

	private TextBox textBox11;

	private Label label54;

	private TextBox textBox10;

	private GroupBox groupBox15;

	private Button Stop;

	private Button Start;

	private TextBox textBox1;

	private Label label51;

	private TextBox time1;

	private TextBox tempid1;

	private Label SendTime;

	private Label label50;

	private Label label49;

	private Label label55;

	private TabControl tabControl1;

	private TabPage tabPage1;

	private Button button10;

	private Button button9;

	private Button button8;

	private Button button7;

	private GroupBox groupBox9;

	private ComboBox TemplateID_CBox;

	private PictureBox pictureBox1;

	private Label label7;

	private Button FindItem_Btn;

	private TextBox ItemName_Text;

	private Label label18;

	private GroupBox groupBox16;

	private TextBox textBox9;

	private Label label56;

	private TextBox textBox8;

	private Label label57;

	private TextBox textBox7;

	private Label label58;

	private TextBox textBox6;

	private Label label59;

	private Label label60;

	private TextBox textBox3;

	private Label label61;

	private TextBox textBox2;

	private Label label62;

	private Label label63;

	private TextBox playernickname;

	private Panel panel1;

	private Label label48;

	private Label label47;

	private Label label46;

	private TextBox time;

	private Button ShutDown;

	private Button ReStart;

	private Label label45;

	private Label label44;

	private GroupBox groupBox14;

	private Button SendAnnouncement;

	private TextBox richTextBox1;

	private ComboBox ListType;

	private Label label43;

	private GroupBox playerBox;

	private Button button1;

	private Button Ban;

	private Button Kick;

	private DateTimePicker dateTimePicker1;

	private Label label1;

	private GroupBox groupBox2;

	private TextBox UpdateLogs;

	private GroupBox groupBox1;

	private ListBox ListOnlinePlayers;

	private TabPage tabPage2;

	private GroupBox groupBox19;

	private Button ChangePassword;

	private TextBox Password;

	private Label label66;

	private GroupBox groupBox13;

	private Button CleanAcitveNum_Btn;

	private ComboBox ListActiveID_CBox;

	private Label label42;

	private GroupBox groupBox4;

	private Button FindUser_Btn;

	private TextBox UserName_Text;

	private Label label2;

	private DataGridView NewPlyaerFightPowerRank;

	private GroupBox groupBox3;

	private CheckBox checkBox1;

	private Button button2;

	private TextBox textBox13;

	private TextBox textBox14;

	private TextBox textBox15;

	private Label label64;

	private Label label65;

	private Button button3;

	private ComboBox comboBox1;

	private Label label4;

	private GroupBox groupBox8;

	private Button button6;

	private Button button5;

	private Button button11;

	private Button button12;

	private Button button13;

	private Button button14;

	private Button button15;

	private Button button16;

	private Button button17;

	private Button button19;

	private Button button18;

	private Button button20;

	private Button button21;

	private Button button22;

	private Button button24;

	private Button button23;

	private Button button25;

	private Button button26;

	private Button button27;

	private Button button28;

	private GroupBox groupBox17;

	private Button button30;

	private Button button29;

	private static string Resource => ConfigurationManager.AppSettings["Resource"];

	private static string Request => ConfigurationManager.AppSettings["Request"];

	private static string RequestUrl => ConfigurationManager.AppSettings["RequestUrl"];

	public Form1()
	{
		InitializeComponent();
		form = this;
		Control.CheckForIllegalCrossThreadCalls = false;
	}

	private void Form1_Load(object sender, EventArgs e)
	{
		Init();
	}

	private void Init()
	{
		timer1.Enabled = false;
		base.MaximizeBox = false;
		Start.Enabled = true;
		Stop.Enabled = false;
		ListOnlinePlayers.Items.Clear();
		SqlDataReader sqlDataReader = null;
		StartTime_TimeDate.Value = DateTime.Now.AddDays(-7.0);
		string sqlstr = "SELECT ActiveID, Title FROM Active";
		db = new DataBaseHelper("crosszoneString");
		db.ExecuteReader(sqlstr, ref sqlDataReader);
		while (sqlDataReader.Read())
		{
			ListActiveID_CBox.Items.Add(sqlDataReader["ActiveID"].ToString() + " -- " + sqlDataReader["Title"].ToString());
		}
		sqlDataReader.Close();
		sqlstr = "SELECT Name ,TemplateID FROM Shop_Goods WHERE CategoryID = 7";
		db.ExecuteReader(sqlstr, ref sqlDataReader);
		while (sqlDataReader.Read())
		{
			ListItemName_CBox.Items.Add(sqlDataReader["TemplateID"].ToString() + " -- " + sqlDataReader["Name"].ToString());
		}
		sqlDataReader.Close();
		if (ListActiveID_CBox.Items.Count != 0)
		{
			ListActiveID_CBox.SelectedIndex = 0;
		}
		if (ListItemName_CBox.Items.Count != 0)
		{
			ListItemName_CBox.SelectedIndex = 0;
		}
		if (ListCategoryID_CBox.Items.Count != 0)
		{
			ListCategoryID_CBox.SelectedIndex = 0;
		}
		if (ListType.Items.Count != 0)
		{
			ListType.SelectedIndex = 0;
		}
		if (IsBind.Items.Count != 0)
		{
			IsBind.SelectedIndex = 0;
		}
	}

	public void ShowMessage(int type, string msg)
	{
		GameClient[] allClients = GameServer.Instance.GetAllClients();
		GameClient[] array = allClients;
		foreach (GameClient gameClient in array)
		{
			if (gameClient.Out != null)
			{
				gameClient.Out.SendMessage((eMessageType)type, msg);
			}
		}
	}

	public static string GetLink(string pic, int categoryId, int needSex)
	{
		string text = "";
		pic = pic.ToLower();
		string text2 = Resource;
		string text3 = ((needSex == 1) ? "m" : "f");
		switch (categoryId)
		{
		case 1:
			text = text + "image/equip/" + text3 + "/head/" + pic + "/icon_1.png";
			break;
		case 2:
			text = text + "image/equip/" + text3 + "/glass/" + pic + "/icon_1.png";
			break;
		case 3:
			text = text + "image/equip/" + text3 + "/hair/" + pic + "/icon_1.png";
			break;
		case 4:
			text = text + "image/equip/" + text3 + "/eff/" + pic + "/icon_1.png";
			break;
		case 5:
			text = text + "image/equip/" + text3 + "/cloth/" + pic + "/icon_1.png";
			break;
		case 6:
			text = text + "image/equip/" + text3 + "/face/" + pic + "/icon_1.png";
			break;
		case 7:
		case 27:
		case 64:
			text = text + "image/arm/" + pic + "/1/icon.png";
			break;
		case 8:
		case 28:
			text = text + "image/equip/armlet/" + pic + "/icon.png";
			break;
		case 9:
		case 29:
			text = text + "image/equip/ring/" + pic + "/icon.png";
			break;
		case 32:
		case 36:
			text = text + "image/farm/Crops/" + pic + "/seed.png";
			break;
		case 33:
			text = text + "image/farm/fertilizer/" + pic + "/icon.png";
			break;
		case 11:
		case 20:
		case 24:
		case 30:
		case 34:
		case 35:
		case 37:
		case 40:
		case 53:
		case 60:
		case 61:
		case 62:
		case 68:
		case 69:
		case 71:
		case 72:
		case 78:
		case 79:
		case 82:
		case 83:
		case 84:
		case 85:
		case 86:
			text = text + "image/unfrightprop/" + pic + "/icon.png";
			break;
		case 12:
			text = text + "image/task/" + pic + "/icon.png";
			break;
		case 13:
			text = text + "image/equip/" + text3 + "/suits/" + pic + "/icon_1.png";
			break;
		case 14:
			text = text + "image/equip/necklace/" + pic + "/icon.png";
			break;
		case 15:
			text = text + "image/equip/wing/" + pic + "/icon.png";
			break;
		case 16:
			text = text + "image/specialprop/chatball/" + pic + "/icon.png";
			break;
		case 17:
		case 31:
			text = text + "image/equip/offhand/" + pic + "/icon.png";
			break;
		case 18:
		case 66:
			text = text + "image/cardbox/" + pic + "/icon.png";
			break;
		case 19:
			text = text + "image/equip/recover/" + pic + "/icon.png";
			break;
		case 26:
			text = text + "image/card/" + pic + "/icon.jpg";
			break;
		case 25:
			text = text + "image/gift/" + pic + "/icon.png";
			break;
		case 50:
			text = text + "image/petequip/arm/" + pic + "/icon.png";
			break;
		case 51:
			text = text + "image/petequip/hat/" + pic + "/icon.png";
			break;
		case 52:
			text = text + "image/petequip/cloth/" + pic + "/icon.png";
			break;
		case 70:
			text = text + "image/equip/amulet/" + pic + "/icon.png";
			break;
		default:
			text2 = null;
			break;
		}
		return text2 + text;
	}

	public static string GetWebStatusCode(string url, int timeout)
	{
		HttpWebRequest httpWebRequest = null;
		try
		{
			httpWebRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
			httpWebRequest.Method = "HEAD";
			httpWebRequest.Timeout = timeout;
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			return Convert.ToInt32(httpWebResponse.StatusCode).ToString();
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
		finally
		{
			if (httpWebRequest != null)
			{
				httpWebRequest.Abort();
				httpWebRequest = null;
			}
		}
	}

	public static int[] GetSqlstrVariable(int CategoryID)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		switch (CategoryID)
		{
		case 1:
			num = 1;
			num2 = 1;
			num9 = 12;
			break;
		case 2:
		case 13:
			num = 0;
			num2 = 1;
			num9 = 12;
			break;
		case 3:
		case 4:
			num = 0;
			num2 = 0;
			num9 = 12;
			break;
		case 5:
			num = 1;
			num2 = 1;
			num9 = 123;
			break;
		case 6:
			num = 0;
			num2 = 0;
			num9 = 123;
			break;
		case 8:
		case 9:
			num = 0;
			num2 = 1;
			num9 = 1;
			break;
		case 14:
		case 15:
		case 16:
			num = 0;
			num2 = 0;
			num9 = 1;
			break;
		}
		return new int[9] { num, num2, num3, num4, num5, num6, num7, num8, num9 };
	}

	public void OutXML1(string db, string outpath, string filename, bool isCompress, string item)
	{
		DataTable dataTable = new DataTable();
		using (SqlConnection sqlConnection = new SqlConnection())
		{
			sqlConnection.ConnectionString = ConfigurationManager.AppSettings["crosszoneString"];
			using SqlCommand sqlCommand = sqlConnection.CreateCommand();
			sqlCommand.CommandText = "select * from " + db;
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
			sqlDataAdapter.Fill(dataTable);
		}
		XElement xElement = new XElement("Result");
		for (int i = 0; i < dataTable.Rows.Count; i++)
		{
			XElement xElement2 = new XElement(item);
			for (int j = 0; j < dataTable.Columns.Count; j++)
			{
				xElement2.Add(new XAttribute(dataTable.Columns[j].ColumnName, (dataTable.Rows[i][j] == null) ? string.Empty : dataTable.Rows[i][j]));
			}
			xElement.Add(xElement2);
		}
		xElement.Add(new XAttribute("value", true));
		xElement.Add(new XAttribute("message", "Success!"));
		using FileStream fileStream = new FileStream(outpath + "\\" + filename, FileMode.Create);
		if (isCompress)
		{
			using BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(Compress(xElement.ToString(check: false)));
		}
		else
		{
			using StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(xElement.ToString(check: false));
		}
		write($"生成 {filename} 成功,时间 -- {DateTime.Now.ToString()}", Color.Green);
	}

	private void write(string text, Color color)
	{
		richTextBox2.Select(richTextBox2.Text.Length, 0);
		richTextBox2.SelectionFont = new Font("微软雅黑", 8f, FontStyle.Regular);
		richTextBox2.SelectionColor = color;
		richTextBox2.SelectedText = text;
	}

	private static string GetIDByChar(string CBoxText)
	{
		string[] array = CBoxText.Split(new string[1] { " -- " }, StringSplitOptions.None);
		return array[0];
	}

	private static int GetIDBySplit(string CBoxText)
	{
		string[] array = CBoxText.Split(new string[1] { " -- " }, StringSplitOptions.None);
		return Convert.ToInt32(array[0]);
	}

	private string GetMD5(string input)
	{
		return Md5(input);
	}

	public static string Md5(string s)
	{
		using MD5 mD = MD5.Create();
		byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(s));
		string text = BitConverter.ToString(array);
		return text.Replace("-", "").ToUpper();
	}

	private void mainWorker_Tick(object sender, EventArgs e)
	{
		if (!GameServer.KeepRunning)
		{
			return;
		}
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		label45.Text = $"{allPlayers.Length} 人";
		ListOnlinePlayers.Items.Clear();
		if (allPlayers != null)
		{
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				ListOnlinePlayers.Items.Add(gamePlayer.PlayerCharacter.NickName);
			}
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		int num = 0;
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		if (allPlayers != null)
		{
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				PlayerBussiness playerBussiness = new PlayerBussiness();
				playerBussiness.SendMailAndItemByNickName(LanguageMgr.GetTranslation("Game.Service.helper.DDTankName"), "恭喜您获得了泡点奖励", gamePlayer.PlayerCharacter.NickName, 1001, templateId, count1, 0, 0, 0, 0, 0, 0, 0, 0, isBinds: true);
			}
		}
		SendTime.Text = "泡点执行成功 " + (num + 1) + " 次";
	}

	private void SendAnnouncement_Click(object sender, EventArgs e)
	{
		if (richTextBox1.Text == "")
		{
			MessageBox.Show("请输入需要发送的内容！", "系统消息");
			return;
		}
		int type = 0;
		switch (ListType.Text)
		{
		case "屏幕提示":
			type = 0;
			break;
		case "弹出框":
			type = 4;
			break;
		case "黄色":
			type = 2;
			break;
		case "紫色":
			type = 8;
			break;
		}
		ShowMessage(type, richTextBox1.Text);
		MessageBox.Show("发送消息成功");
	}

	private void Kick_Click(object sender, EventArgs e)
	{
		GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(playerBox.Text.Split('/')[0]);
		if (clientByPlayerNickName == null)
		{
			MessageBox.Show("玩家已离线");
			return;
		}
		try
		{
			clientByPlayerNickName.Out.SendKitoff("您已被GM踢出了游戏");
			using (ManageBussiness manageBussiness = new ManageBussiness())
			{
				manageBussiness.KitoffUserByNickName(clientByPlayerNickName.PlayerCharacter.NickName, 1001, "您已被GM踢出了游戏");
			}
			MessageBox.Show("玩家已被踢出游戏");
		}
		catch (Exception ex)
		{
			MessageBox.Show("踢出失败");
			Console.WriteLine("踢出玩家失败,错误:" + ex);
		}
	}

	private void Ban_Click(object sender, EventArgs e)
	{
		GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(playerBox.Text.Split('/')[0]);
		if (clientByPlayerNickName == null)
		{
			MessageBox.Show("玩家已离线");
			return;
		}
		try
		{
			clientByPlayerNickName.Out.SendKitoff("您已被GM封号");
			using (ManageBussiness manageBussiness = new ManageBussiness())
			{
				manageBussiness.ForbidPlayerByNickName(clientByPlayerNickName.PlayerCharacter.NickName, 1001, dateTimePicker1.Value, isExist: false);
			}
			MessageBox.Show("玩家已被禁止登陆");
		}
		catch (Exception ex)
		{
			MessageBox.Show("封号失败");
			Console.WriteLine("封号失败,错误:" + ex);
		}
	}

	private void ReStart_Click(object sender, EventArgs e)
	{
	}

	private void ShutDown_Click(object sender, EventArgs e)
	{
		if (!int.TryParse(time.Text, out _minutes))
		{
			return;
		}
		if (_timer == null)
		{
			label47.Text = "准备关闭";
			label47.ForeColor = Color.Red;
			label48.Visible = true;
			label48.Text = $"距离关闭还有：{_minutes} 分钟";
			_timer = new System.Threading.Timer(ShutDownCallBack, null, 0, 60000);
			ShutDown.Text = "取消";
			return;
		}
		_timer.Dispose();
		_timer = null;
		label47.Text = "工作中";
		label47.ForeColor = Color.Green;
		label48.Visible = false;
		ShutDown.Text = "关闭";
		GameClient[] allClients = GameServer.Instance.GetAllClients();
		GameClient[] array = allClients;
		foreach (GameClient gameClient in array)
		{
			if (gameClient.Out != null)
			{
				gameClient.Out.SendMessage(eMessageType.Normal, "临时公告,本次维护取消,具体请看群公告!");
			}
		}
	}

	private static void ShutDownCallBack(object state)
	{
		Console.WriteLine($"游戏将在{_minutes}分钟后关闭!");
		form.label48.Text = $"距离关闭还有：{_minutes} 分钟";
		GameClient[] allClients = GameServer.Instance.GetAllClients();
		GameClient[] array = allClients;
		foreach (GameClient gameClient in array)
		{
			if (gameClient.Out != null && gameClient.Out != null)
			{
				gameClient.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1"), _minutes, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2")));
			}
		}
		if (_minutes <= 0)
		{
			form.label47.Text = "已关闭";
			form.label48.Visible = false;
			form.ShutDown.Enabled = false;
			_timer.Dispose();
			_timer = null;
			GameServer.Instance.Stop();
			Console.WriteLine("游戏关闭成功!");
		}
		_minutes--;
	}

	private void Start_Click(object sender, EventArgs e)
	{
		if (time1.Text.Length == 0 || tempid1.Text.Length == 0)
		{
			MessageBox.Show("泡点时间或者物品不输入,你泡个鸡儿的点呀?", "错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			return;
		}
		Packtime = Convert.ToInt32(time1.Text);
		timer1.Enabled = true;
		SendTime.Visible = true;
		templateId = Convert.ToInt32(tempid1.Text);
		count1 = Convert.ToInt32(textBox1.Text);
		double totalMilliseconds = new TimeSpan(0, Packtime, 0).TotalMilliseconds;
		int interval = (int)totalMilliseconds;
		timer1.Interval = interval;
		Start.Enabled = false;
		Stop.Enabled = true;
	}

	private void Stop_Click(object sender, EventArgs e)
	{
		Start.Enabled = true;
		Stop.Enabled = false;
		timer1.Enabled = false;
	}

	private void ListOnlinePlayers_SelectedIndexChanged(object sender, EventArgs e)
	{
		ListBox listBox = (ListBox)sender;
		if (listBox.SelectedItem != null)
		{
			GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(listBox.SelectedItem.ToString());
			if (clientByPlayerNickName == null)
			{
				MessageBox.Show("玩家已经离线");
				return;
			}
			playerBox.Text = $"{clientByPlayerNickName.PlayerCharacter.NickName}/{clientByPlayerNickName.PlayerCharacter.UserName}";
			playernickname.Text = clientByPlayerNickName.PlayerCharacter.NickName;
		}
	}

	private void SendItemToPlayer_Click(object sender, EventArgs e)
	{
		bool isBinds = true;
		if (playernickname.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox5.Text == "" || textBox6.Text == "" || textBox7.Text == "" || textBox8.Text == "" || textBox9.Text == "" || textBox10.Text == "" || textBox11.Text == "" || textBox12.Text == "")
		{
			MessageBox.Show("哇,我这么辛苦写判定,尼玛你连数据都不输入齐,你怎么不去撞墙?", "操你妈的大傻逼");
			return;
		}
		if (IsBind.Text == "绑定")
		{
			isBinds = true;
		}
		else if (IsBind.Text == "未绑定")
		{
			isBinds = false;
		}
		string nickName = playernickname.Text.ToString();
		int templateID = Convert.ToInt32(textBox2.Text);
		int count = Convert.ToInt32(textBox3.Text);
		int validDate = Convert.ToInt32(textBox5.Text);
		int gold = Convert.ToInt32(textBox6.Text);
		int money = Convert.ToInt32(textBox7.Text);
		int strengthenLevel = Convert.ToInt32(textBox8.Text);
		int attackCompose = Convert.ToInt32(textBox9.Text);
		int defendCompose = Convert.ToInt32(textBox10.Text);
		int agilityCompose = Convert.ToInt32(textBox11.Text);
		int luckCompose = Convert.ToInt32(textBox12.Text);
		PlayerBussiness playerBussiness = new PlayerBussiness();
		playerBussiness.SendMailAndItemByNickName(LanguageMgr.GetTranslation("Game.Service.helper.DDTankName") + "祝您游戏愉快", "恭喜您获得了最帅气可爱的GM发送的奖励", nickName, 1001, templateID, count, validDate, gold, money, strengthenLevel, attackCompose, defendCompose, agilityCompose, luckCompose, isBinds);
		MessageBox.Show("发送邮件成功");
	}

	private void FindItem_Btn_Click(object sender, EventArgs e)
	{
		TemplateID_CBox.Items.Clear();
		string empty = string.Empty;
		SqlDataReader sqlDataReader = null;
		if (ItemName_Text.Text != "")
		{
			empty = $"SELECT TemplateID, Name FROM Shop_Goods WHERE Name LIKE '%{ItemName_Text.Text}%'";
			db = new DataBaseHelper("crosszoneString");
			db.ExecuteReader(empty, ref sqlDataReader);
			while (sqlDataReader.Read())
			{
				TemplateID_CBox.Items.Add(sqlDataReader["TemplateID"].ToString() + " -- " + sqlDataReader["Name"].ToString());
				TemplateID_CBox.SelectedIndex = 0;
			}
			sqlDataReader.Close();
		}
		else
		{
			MessageBox.Show("请输入你需要查询的道具名称!");
		}
	}

	private void ChangePassword_Click(object sender, EventArgs e)
	{
		SqlDataReader sqlDataReader = null;
		int num = 0;
		if (UserName_Text.Text == "")
		{
			MessageBox.Show("玩家账号为空");
			return;
		}
		if (Password.Text == "")
		{
			MessageBox.Show("玩家密码为空");
			return;
		}
		db = new DataBaseHelper("membershipDb");
		db.ExecuteReader($"select * from Mem_Users where UserName='{UserName_Text.Text}' ", ref sqlDataReader);
		if (sqlDataReader.Read())
		{
			num = int.Parse(sqlDataReader["UserId"].ToString());
			sqlDataReader.Close();
			if (num == 0)
			{
				MessageBox.Show("修改密码失败,账号不存在!");
			}
			else if (db.ExecuteNonQuery($"update Mem_UserInfo set Password='{GetMD5(Password.Text)}' where UserId={num}"))
			{
				MessageBox.Show("修改密码成功");
			}
		}
	}

	private void FindUser_Btn_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		SqlDataReader sqlDataReader = null;
		if (UserName_Text.Text != "")
		{
			empty = $"SELECT UserID, NickName, Grade, State, PasswordTwo FROM Sys_Users_Detail WHERE UserName = '{UserName_Text.Text}'";
			db = new DataBaseHelper("conString");
			db.ExecuteReader(empty, ref sqlDataReader);
			if (sqlDataReader.Read())
			{
				UserID_Lab.Text = sqlDataReader["UserID"].ToString();
				NickName_Text.Text = sqlDataReader["NickName"].ToString();
				Level_Text.Text = sqlDataReader["Grade"].ToString();
				PasswordTwo_Text.Text = sqlDataReader["PasswordTwo"].ToString();
				if ((int)sqlDataReader["State"] == 0)
				{
					Online_Text.ForeColor = Color.Red;
					Online_Text.Text = "离线";
				}
				else
				{
					Online_Text.ForeColor = Color.Green;
					Online_Text.Text = "在线";
				}
				sqlDataReader.Close();
			}
			else
			{
				UserName_Text.Text = "未找到该用户";
			}
		}
		else
		{
			UserName_Text.Text = "请输入用户名";
		}
	}

	private void UpUserData_Btn_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (PasswordTwo_Text.Text != "")
		{
			empty = $"UPDATE Sys_Users_Detail SET PasswordTwo = '{PasswordTwo_Text.Text}' WHERE UserID = '{UserID_Lab.Text}'";
			db = new DataBaseHelper("conString");
			if (!db.ExecuteNonQuery(empty))
			{
			}
		}
		if (NickName_Text.Text != "" && UserID_Lab.Text != "0")
		{
			empty = $"UPDATE Sys_Users_Detail SET NickName = '{NickName_Text.Text}' WHERE UserID = '{UserID_Lab.Text}'";
			db = new DataBaseHelper("conString");
			if (db.ExecuteNonQuery(empty))
			{
				MessageBox.Show("修改成功");
			}
		}
	}

	private void CleanUser_Text_Click(object sender, EventArgs e)
	{
		NickName_Text.Clear();
		UserName_Text.Clear();
		Level_Text.Clear();
		UserID_Lab.Text = "0";
		Online_Text.ForeColor = Color.Red;
		Online_Text.Text = "离线";
	}

	private void UpdateLevel_Btn_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				LevelInfo levelInfo = LevelMgr.FindLevel(int.Parse(Level_Text.Text));
				playerById.PlayerCharacter.GP = levelInfo.GP;
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
				return;
			}
			empty = string.Format("UPDATE Sys_Users_Detail SET Grade = '{0}', GP = (select GP from Db_Tank_All.dbo.LevelInfo where Grade = '{0}') WHERE UserID = '{1}'", Level_Text.Text, UserID_Lab.Text);
			db = new DataBaseHelper("conString");
			if (db.ExecuteNonQuery(empty))
			{
				MessageBox.Show("修改成功");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void FindRank_Btn_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		empty = string.Format("SELECT TOP 20 UserName, NickName, FightPower From Sys_Users_Detail where Date > '{0}' and Date < '{1}' ORDER BY FightPower DESC", StartTime_TimeDate.Value.ToString("yyyy-MM-dd"), EndTime_TimeDate.Value.ToString("yyyy-MM-dd"));
		db = new DataBaseHelper("conString");
		DataTable dataTable = new DataTable();
		DataColumn dataColumn = new DataColumn();
		dataColumn.ColumnName = "ID";
		dataColumn.AutoIncrement = true;
		dataColumn.AutoIncrementSeed = 1L;
		dataColumn.AutoIncrementStep = 1L;
		dataTable.Columns.Add(dataColumn);
		dataTable.Merge(db.GetDateSet(empty).Tables[0]);
		NewPlyaerFightPowerRank.DataSource = dataTable;
		NewPlyaerFightPowerRank.Columns["ID"].DisplayIndex = 0;
		NewPlyaerFightPowerRank.Columns["ID"].Width = 30;
	}

	private void CleanAcitveNum_Btn_Click(object sender, EventArgs e)
	{
		SetTaskAtFixedTime();
		CleanAcitveNum_Btn.Enabled = false;
		CleanAcitveNum_Btn.Text = "线程已设定";
		ActiveID = GetIDBySplit(ListActiveID_CBox.Text);
	}

	private void SetTaskAtFixedTime()
	{
		DateTime now = DateTime.Now;
		DateTime dateTime = DateTime.Today;
		if (now > dateTime)
		{
			dateTime = dateTime.AddDays(1.0);
		}
		int dueTime = (int)(dateTime - now).TotalMilliseconds;
		System.Threading.Timer timer = new System.Threading.Timer(doAt0AM);
		timer.Change(dueTime, -1);
	}

	private void doAt0AM(object state)
	{
		string sqlstr = $"DELETE Active_Number WHERE ActiveID = {ActiveID}";
		db = new DataBaseHelper("conString");
		if (db.ExecuteNonQuery(sqlstr))
		{
			write($"定时删除激活码 -- {DateTime.Now.ToString()} \r\n", Color.Green);
			PlayerLogs.WriteLine($"[定时删除激活码] - [{DateTime.Now.ToString()}].", 2);
		}
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.ResetTimeBox())
			{
				PlayerLogs.WriteLine($"[定时重置活动] - [在线玩家:{gamePlayer.PlayerCharacter.NickName}] - [{DateTime.Now.ToString()}].", 2);
			}
		}
		SetTaskAtFixedTime();
	}

	private void timer2_Elapsed(object sender, ElapsedEventArgs e)
	{
		if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
		{
			string sqlstr = $"DELETE Active_Number WHERE ActiveID = {ActiveID}";
			db = new DataBaseHelper("conString");
			if (db.ExecuteNonQuery(sqlstr))
			{
				write($"定时删除激活码 -- {DateTime.Now.ToString()} \r\n", Color.Green);
				PlayerLogs.WriteLine($"[定时删除激活码] - [{DateTime.Now.ToString()}].", 2);
			}
		}
	}

	private void InsertItem_Btn_Click(object sender, EventArgs e)
	{
		SqlDataReader sqlDataReader = null;
		int[] sqlstrVariable = GetSqlstrVariable(CategoryID);
		int num = sqlstrVariable[0];
		int num2 = sqlstrVariable[1];
		int num3 = sqlstrVariable[2];
		int num4 = sqlstrVariable[3];
		int num5 = sqlstrVariable[4];
		int num6 = sqlstrVariable[5];
		int num7 = sqlstrVariable[6];
		int num8 = sqlstrVariable[7];
		int num9 = sqlstrVariable[8];
		string sqlstr = $"select max(TemplateID)TemplateID from Shop_Goods ";
		db = new DataBaseHelper("crosszoneString");
		db.ExecuteReader(sqlstr, ref sqlDataReader);
		if (sqlDataReader.Read())
		{
			int num10 = (int)sqlDataReader["TemplateID"] + 1;
			sqlDataReader.Close();
			sqlstr = $"INSERT INTO Shop_Goods VALUES ({num10}, '{ItemName_TextBox.Text}', '', {CategoryID}, '{ItemDesc_TextBox.Text}', {ItemAtt_TextBox.Text}, {ItemDef_TextBox.Text}, {ItemAgi_TextBox.Text}, {ItemLuck_TextBox.Text}, 3, {ItemQuality_CBox.SelectedIndex + 1}, '{Pic}', 1, {NeedSex}, 0, {num}, {num2}, 0, 1, 1, 1, '', '', NULL, {num3}, {num4}, {num5}, {num6}, {num7}, {num8}, {ItemArmor_Text.Text}, {num9}, 0, 1, '{DateTime.Now.ToString()}', 2, 0, 0, 0, '0,-1|0,-1|0,-1|0,-1|0,-1|0,-1', 0, 0, 200, 2, 0, 0, 0)";
			if (db.ExecuteNonQuery(sqlstr))
			{
				NewItemID_TextBox.Text = num10.ToString();
				sqlDataReader.Close();
			}
		}
	}

	private void InsertArm_Btn_Click(object sender, EventArgs e)
	{
		if (Common_Text.Text != "未添加" && CommonAddWound_Text.Text != "未添加" && CommonMultiBall_Text.Text != "未添加" && Special_Text.Text != "未添加")
		{
			SqlDataReader sqlDataReader = null;
			string sqlstr = $"SELECT MAX(TemplateID)TemplateID FROM Shop_Goods ";
			db = new DataBaseHelper("crosszoneString");
			db.ExecuteReader(sqlstr, ref sqlDataReader);
			if (!sqlDataReader.Read())
			{
				return;
			}
			int num = (int)sqlDataReader["TemplateID"] + 1;
			sqlDataReader.Close();
			sqlstr = $"INSERT INTO Shop_Goods VALUES ({num}, '{NewItemName_Text.Text}', '', 7, '{NewItemDesc_Text.Text}', {NewItemAtt_Text.Text}, {NewItemDef_Text.Text}, {NewItemAgi_Text.Text}, {NewItemLuck_Text.Text}, 3, {NewItemQuality_CBox.SelectedIndex + 1}, '{NewItemPic}', 1, 0, 1, 1, 1, 0, 1, 1, 1, '', '', NULL, 0, 0, 0, 0, 15, 65, {NewItemDamage_Text.Text}, 0, 0, 1, '{DateTime.Now.ToString()}', 2, 0, 0, 0, '3,1|6,3|9,3|12,3|0,3|0,3', 0, 0, 0, 0, 0, 0, 0)";
			if (!db.ExecuteNonQuery(sqlstr))
			{
				return;
			}
			NewTemplateID_Text.Text = num.ToString();
			sqlstr = $"INSERT INTO BallConfig VALUES ({num}, {Common_Text.Text}, {CommonAddWound_Text.Text}, {CommonMultiBall_Text.Text}, {Special_Text.Text})";
			if (!db.ExecuteNonQuery(sqlstr))
			{
				return;
			}
			int num2 = int.Parse(NewItemAtt_Text.Text);
			int num3 = int.Parse(NewItemDef_Text.Text);
			int num4 = int.Parse(NewItemAgi_Text.Text);
			int num5 = int.Parse(NewItemLuck_Text.Text);
			sqlstr = $"SELECT MAX(ID)ID FROM GoldEquipTemplateLoad ";
			db.ExecuteReader(sqlstr, ref sqlDataReader);
			if (sqlDataReader.Read())
			{
				int num6 = (int)sqlDataReader["ID"] + 1;
				sqlDataReader.Close();
				sqlstr = string.Format("INSERT INTO GoldEquipTemplateLoad VALUES ({0}, {1}, {1}, 7, -1, {2}, {3}, {4}, {5}, -1, -1, 0, 11560, '{6}')", num6, num, (double)num2 * 1.3, (double)num3 * 1.3, (double)num4 * 1.3, (double)num5 * 1.3, NewItemPic);
				if (db.ExecuteNonQuery(sqlstr))
				{
					MessageBox.Show("定制成功");
				}
			}
		}
		else
		{
			MessageBox.Show("原武器未含有特效配置,请更换武器再试！");
		}
	}

	private void ListCategoryID_CBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		SqlDataReader sqlDataReader = null;
		ListItem_CBox.Items.Clear();
		int iDBySplit = GetIDBySplit(ListCategoryID_CBox.Text);
		string sqlstr = $"SELECT Name ,TemplateID FROM Shop_Goods WHERE CategoryID = '{iDBySplit}'";
		db = new DataBaseHelper("crosszoneString");
		db.ExecuteReader(sqlstr, ref sqlDataReader);
		while (sqlDataReader.Read())
		{
			ListItem_CBox.Items.Add(sqlDataReader["TemplateID"].ToString() + " -- " + sqlDataReader["Name"].ToString());
		}
		ListItem_CBox.SelectedIndex = 0;
	}

	private void ListItem_CBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		SqlDataReader sqlDataReader = null;
		string iDByChar = GetIDByChar(ListItem_CBox.Text);
		string sqlstr = $"SELECT Name, Description, Attack, Defence, Agility, Luck, Pic, Property7, CategoryID, NeedSex, Quality FROM Shop_Goods WHERE TemplateID = {iDByChar}";
		db = new DataBaseHelper("crosszoneString");
		db.ExecuteReader(sqlstr, ref sqlDataReader);
		if (!sqlDataReader.Read())
		{
			return;
		}
		ItemName_TextBox.Text = sqlDataReader["Name"].ToString();
		ItemDesc_TextBox.Text = sqlDataReader["Description"].ToString();
		ItemAtt_TextBox.Text = sqlDataReader["Attack"].ToString();
		ItemDef_TextBox.Text = sqlDataReader["Defence"].ToString();
		ItemAgi_TextBox.Text = sqlDataReader["Agility"].ToString();
		ItemLuck_TextBox.Text = sqlDataReader["Luck"].ToString();
		Pic = sqlDataReader["Pic"].ToString();
		CategoryID = (int)sqlDataReader["CategoryID"];
		NeedSex = (int)sqlDataReader["NeedSex"];
		ItemQuality_CBox.SelectedIndex = (int)sqlDataReader["Quality"] - 1;
		string link = GetLink(Pic, CategoryID, NeedSex);
		if (!link.Equals("") && GetWebStatusCode(link, 2000) != "远程服务器返回错误: (404) 未找到。")
		{
			if (!isEncryed(link))
			{
				pictureBox3.Load(link);
			}
		}
		else
		{
			pictureBox3.Image = Resources._404;
		}
		if (sqlDataReader["Property7"].ToString().Length == 1)
		{
			ItemArmor_Text.Enabled = false;
			ItemArmor_Text.Text = "0";
		}
		else
		{
			ItemArmor_Text.Enabled = true;
			ItemArmor_Text.Text = sqlDataReader["Property7"].ToString();
		}
	}

	private void ListItemName_CBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		SqlDataReader sqlDataReader = null;
		string iDByChar = GetIDByChar(ListItemName_CBox.Text);
		string sqlstr = $"SELECT Name, Description, Attack, Defence, Agility, Luck, Pic, Property7, Quality FROM Shop_Goods WHERE TemplateID = {iDByChar}";
		db = new DataBaseHelper("crosszoneString");
		db.ExecuteReader(sqlstr, ref sqlDataReader);
		if (sqlDataReader.Read())
		{
			NewItemName_Text.Text = sqlDataReader["Name"].ToString();
			NewItemDesc_Text.Text = sqlDataReader["Description"].ToString();
			NewItemAtt_Text.Text = sqlDataReader["Attack"].ToString();
			NewItemDef_Text.Text = sqlDataReader["Defence"].ToString();
			NewItemLuck_Text.Text = sqlDataReader["Luck"].ToString();
			NewItemAgi_Text.Text = sqlDataReader["Agility"].ToString();
			NewItemDamage_Text.Text = sqlDataReader["Property7"].ToString();
			NewItemQuality_CBox.SelectedIndex = (int)sqlDataReader["Quality"] - 1;
			NewItemPic = sqlDataReader["Pic"].ToString();
			string link = GetLink(NewItemPic, 7, 0);
			if (!link.Equals("") && GetWebStatusCode(link, 2000) != "远程服务器返回错误: (404) 未找到。")
			{
				if (!isEncryed(link))
				{
					pictureBox2.Load(link);
				}
			}
			else
			{
				pictureBox2.Image = Resources._404;
			}
		}
		sqlDataReader.Close();
		sqlstr = $"SELECT Common, CommonAddWound, CommonMultiBall, Special FROM BallConfig WHERE TemplateID = '{iDByChar}'";
		db.ExecuteReader(sqlstr, ref sqlDataReader);
		if (sqlDataReader.Read())
		{
			Common_Text.Text = sqlDataReader["Common"].ToString();
			CommonAddWound_Text.Text = sqlDataReader["CommonAddWound"].ToString();
			CommonMultiBall_Text.Text = sqlDataReader["CommonMultiBall"].ToString();
			Special_Text.Text = sqlDataReader["Special"].ToString();
		}
		else
		{
			Common_Text.Text = "未添加";
			CommonAddWound_Text.Text = "未添加";
			CommonMultiBall_Text.Text = "未添加";
			Special_Text.Text = "未添加";
		}
		sqlDataReader.Close();
	}

	private void TemplateID_CBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		SqlDataReader sqlDataReader = null;
		string iDByChar = GetIDByChar(TemplateID_CBox.Text);
		textBox2.Text = iDByChar;
		string sqlstr = $"SELECT CateGoryID, Pic, NeedSex,MaxCount FROM Shop_Goods WHERE TemplateID = {iDByChar}";
		db = new DataBaseHelper("crosszoneString");
		db.ExecuteReader(sqlstr, ref sqlDataReader);
		if (sqlDataReader.Read())
		{
			textBox3.Text = sqlDataReader["MaxCount"].ToString();
			string link = GetLink(sqlDataReader["Pic"].ToString(), (int)sqlDataReader["CateGoryID"], (int)sqlDataReader["NeedSex"]);
			if (!link.Equals("") && GetWebStatusCode(link, 2000) != "远程服务器返回错误: (404) 未找到。")
			{
				if (!isEncryed(link))
				{
					pictureBox1.Load(link);
				}
			}
			else
			{
				pictureBox1.Image = Resources._404;
			}
		}
		else
		{
			pictureBox1.Image = Resources._404;
		}
		sqlDataReader.Close();
	}

	private void CreateAllCeleb_Click(object sender, EventArgs e)
	{
		timer3 = new System.Timers.Timer();
		timer3.Interval = 40000.0;
		timer3.Elapsed += timer3_Elapsed;
		timer3.Enabled = true;
		CreateAllCeleb.Enabled = false;
		CreateAllCeleb.Text = "线程已设定";
	}

	private void timer3_Elapsed(object sender, ElapsedEventArgs e)
	{
		if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string url = RequestUrl + "CreateAllCeleb.ashx";
			Console.WriteLine(BaseInterface.RequestContent(url));
			Console.ForegroundColor = ConsoleColor.White;
			write($"定时同步名人堂 -- {DateTime.Now.ToString()} \r\n", Color.Green);
		}
	}

	private void Reload_Click(object sender, EventArgs e)
	{
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("开始更新游戏系统..请耐心等待.....");
		GameProperties.Refresh();
		Console.WriteLine("更新ServerConfig:True");
		Console.WriteLine("更新语言包:" + LanguageMgr.Reload(""));
		Console.WriteLine("更新天使魔罐:" + LotteryShowTemplateMgr.Init());
		Console.WriteLine("更新地图:" + MapMgr.Init());
		Console.WriteLine("更新道具:" + ItemMgr.ReLoad());
		Console.WriteLine("更新道具盒:" + ItemBoxMgr.Init());
		Console.WriteLine("更新武器特效:" + BallMgr.Init());
		Console.WriteLine("更新修炼属性:" + ExerciseMgr.Init());
		Console.WriteLine("更新玩家等级:" + LevelMgr.Init());
		Console.WriteLine("更新武器特效配置:" + BallConfigMgr.Init());
		Console.WriteLine("更新熔炼系统:" + FusionMgr.Init());
		Console.WriteLine("更新活动奖励:" + AwardMgr.Init());
		Console.WriteLine("更新游戏成就系统:" + AchievementMgr.Init());
		Console.WriteLine("更新NPC怪物属性:" + NPCInfoMgr.Init());
		Console.WriteLine("更新副本关卡配置:" + MissionInfoMgr.Init());
		Console.WriteLine("更新副本关卡消耗活力:" + MissionEnergyMgr.Init());
		Console.WriteLine("更新副本:" + PveInfoMgr.Init());
		Console.WriteLine("更新副本掉落:" + DropMgr.Init());
		Console.WriteLine("更新竞技胜率:" + FightRateMgr.Init());
		Console.WriteLine("更新炼金系统:" + RefineryMgr.Init());
		Console.WriteLine("更新强化系统:" + StrengthenMgr.Init());
		Console.WriteLine("更新副本掉落物品:" + PropItemMgr.Init());
		Console.WriteLine("更新游戏商城:" + ShopMgr.Init());
		Console.WriteLine("更新任务系统:" + QuestMgr.Init());
		Console.WriteLine("更新公会系统:" + ConsortiaMgr.Init());
		Console.WriteLine("更新卡牌系统:" + CardMgr.Init());
		Console.WriteLine("更新宠物系统:" + PetMgr.Init());
		Console.WriteLine("更新坐骑系统:" + MountMgr.Init());
		Console.WriteLine("更新铸金系统:" + GoldEquipMgr.Init());
		Console.WriteLine("更新活动系统:" + ActiveSystemMgr.Init());
		Console.WriteLine("更新灯谜游戏:" + LightriddleQuestMgr.Init());
		Console.WriteLine("更新活动事件:" + WorldEventMgr.Init());
		Console.WriteLine("更新套装系统:" + SubActiveMgr.Init());
		Console.WriteLine("更新大富翁活动等级奖励:" + DiceLevelAwardMgr.Init());
		Console.WriteLine("更新活动事件奖励:" + EventAwardMgr.Init());
		Console.WriteLine("更新称号卡牌系统:" + RankMgr.Init());
		Console.WriteLine("更新宠物萌化属性:" + PetMoePropertyMgr.Init());
		Console.WriteLine("更新宠物系统:" + PetMgr.Init());
		Console.WriteLine("更新魔石系统:" + MagicStoneMgr.Init());
		Console.WriteLine("更新弹王等级:" + BombKingMgr.Init());
		Console.WriteLine("更新vip商城:" + VipIntegrationMgr.Init());
		Console.WriteLine("更新吉普赛商城:" + GypsyShopMgr.Init());
		Console.WriteLine("更新魔法屋熔炼:" + MagicFusionDataMgr.Init());
		Console.WriteLine("更新刻印系统:" + EngraveMgr.Init());
		Console.WriteLine("更新图鉴系统:" + AvatarColectionMgr.Init());
		Console.WriteLine("更新宝珠系统:" + RuneMgr.ReLoad());
		Console.WriteLine("更新宝珠进阶:" + RuneAdvanceTemplateMgr.ReLoad());
		Console.WriteLine("更新恶魔宝藏:" + DevilTurnMgr.Init());
		Console.WriteLine("更新项链精铸:" + NecklaceStrengthMgr.Init());
		Console.WriteLine("更新纹章系统:" + EmblemMgr.Init());
		Console.WriteLine("更新圣灵系统:" + ElfMgr.Init());
		Console.WriteLine("更新宠物技能:" + PetSpecialSkillMgr.Init());
		Console.WriteLine("更新神庙系统:" + HomeTempleMgr.Init());
		string url = RequestUrl + "CreateAllXml.ashx";
		Console.WriteLine(BaseInterface.RequestContent(url));
		Console.ForegroundColor = ConsoleColor.White;
		write($"同步游戏数据 -- {DateTime.Now.ToString()} \r\n", Color.Green);
	}

	public static byte[] Compress(string str)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		return Compress(bytes);
	}

	public static byte[] Compress(byte[] src)
	{
		MemoryStream memoryStream = new MemoryStream();
		Stream stream = new ZOutputStream(memoryStream, 9);
		stream.Write(src, 0, src.Length);
		stream.Close();
		return memoryStream.ToArray();
	}

	public static byte[] Uncompress(byte[] src)
	{
		MemoryStream memoryStream = new MemoryStream();
		Stream stream = new ZOutputStream(memoryStream);
		stream.Write(src, 0, src.Length);
		stream.Close();
		return memoryStream.ToArray();
	}

	private void button7_Click(object sender, EventArgs e)
	{
		CFileOpenDlgThreadApartmentSafe cFileOpenDlgThreadApartmentSafe = new CFileOpenDlgThreadApartmentSafe();
		cFileOpenDlgThreadApartmentSafe.Filter = "XML(*.xml)|*.xml";
		if (cFileOpenDlgThreadApartmentSafe.ShowDialog() == DialogResult.OK)
		{
			string filePath = cFileOpenDlgThreadApartmentSafe.FilePath;
			FileStream fileStream = File.OpenRead(filePath);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
			fileStream.Close();
			FileStream fileStream2 = File.Create(filePath);
			byte[] array2 = Compress(array);
			fileStream2.Write(array2, 0, array2.Length);
			fileStream2.Flush();
			fileStream2.Close();
			MessageBox.Show("加密XML成功");
		}
	}

	private void button8_Click(object sender, EventArgs e)
	{
		CFileOpenDlgThreadApartmentSafe cFileOpenDlgThreadApartmentSafe = new CFileOpenDlgThreadApartmentSafe();
		cFileOpenDlgThreadApartmentSafe.Filter = "XML(*.xml)|*.xml|BA(*.ba)|*.ba";
		if (cFileOpenDlgThreadApartmentSafe.ShowDialog() == DialogResult.OK)
		{
			string filePath = cFileOpenDlgThreadApartmentSafe.FilePath;
			FileStream fileStream = File.OpenRead(filePath);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
			fileStream.Close();
			FileStream fileStream2 = File.Create(filePath);
			byte[] array2 = Uncompress(array);
			fileStream2.Write(array2, 0, array2.Length);
			fileStream2.Flush();
			fileStream2.Close();
			MessageBox.Show("解密XML成功");
		}
	}

	private void button9_Click(object sender, EventArgs e)
	{
		CFileOpenDlgThreadApartmentSafe cFileOpenDlgThreadApartmentSafe = new CFileOpenDlgThreadApartmentSafe();
		cFileOpenDlgThreadApartmentSafe.Filter = "PNG(*.png)|*.png|SWF(*.swf)|*.swf";
		if (cFileOpenDlgThreadApartmentSafe.ShowDialog() == DialogResult.OK)
		{
			string filePath = cFileOpenDlgThreadApartmentSafe.FilePath;
			FileStream fileStream = File.OpenRead(filePath);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
			fileStream.Close();
			byte[] buffer = new byte[21];
			new Random().NextBytes(buffer);
			FileStream fileStream2 = File.Create(filePath);
			fileStream2.Write(buffer, 0, 21);
			fileStream2.Write(array, 124, array.Length - 124);
			fileStream2.Write(array, 3, 121);
			fileStream2.Flush();
			fileStream2.Close();
			MessageBox.Show("加密PNG成功");
		}
	}

	private void button10_Click(object sender, EventArgs e)
	{
		CFileOpenDlgThreadApartmentSafe cFileOpenDlgThreadApartmentSafe = new CFileOpenDlgThreadApartmentSafe();
		cFileOpenDlgThreadApartmentSafe.Filter = "PNG(*.png)|*.png|SWF(*.swf)|*.swf";
		if (cFileOpenDlgThreadApartmentSafe.ShowDialog() == DialogResult.OK)
		{
			string filePath = cFileOpenDlgThreadApartmentSafe.FilePath;
			FileStream fileStream = File.OpenRead(filePath);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
			string @string = Encoding.UTF8.GetString(array);
			if (@string.Contains("^_^"))
			{
				fileStream.Close();
				ByteArray byteArray = new ByteArray(array);
				uint position = byteArray.Position;
				byteArray.Position = 0u;
				byteArray.ReadUTF();
				ByteArray byteArray2 = new ByteArray();
				byteArray.ReadBytes(byteArray2, 0u, 16u);
				int num = byteArray.ReadByte();
				num = ~num;
				ByteArray byteArray3 = new ByteArray();
				byteArray.ReadBytes(byteArray3, 0u, byteArray3.BytesAvailable);
				FileStream fileStream2 = File.OpenWrite(filePath);
				byte[] buffer = byteArray2.ToArray();
				byte[] array2 = byteArray3.ToArray();
				fileStream2.Write(buffer, 0, 16);
				fileStream2.WriteByte((byte)num);
				fileStream2.Write(array2, 0, array2.Length);
				byteArray.Position = position;
				fileStream2.Flush();
				fileStream2.Close();
			}
			else
			{
				fileStream.Close();
				FileStream fileStream3 = File.Create(filePath);
				fileStream3.WriteByte(67);
				fileStream3.WriteByte(87);
				fileStream3.WriteByte(83);
				Stream stream = fileStream3;
				byte[] array3 = array;
				stream.Write(array3, array3.Length - 121, 121);
				fileStream3.Write(array, 21, array.Length - 121);
				fileStream3.Flush();
				fileStream3.Close();
			}
			MessageBox.Show("解密PNG成功");
		}
	}

	private void button2_Click(object sender, EventArgs e)
	{
		OutXML1(textBox14.Text, Request, textBox15.Text + ".xml", checkBox1.Checked, textBox13.Text);
	}

	private void button5_Click(object sender, EventArgs e)
	{
		OutXML1("Item_Strengthen_Goods", Request, "itemstrengthengoodsinfo.xml", isCompress: true, "Item");
	}

	private void button15_Click(object sender, EventArgs e)
	{
		OutXML1("GoldEquipTemplateLoad", Request, "goldequiptemplateload.xml", isCompress: true, "item");
	}

	private void RecoverMessage_Click(object sender, EventArgs e)
	{
		if (UserID_Lab.Text != "0")
		{
			string sqlstr = $"update User_Messages set IsExist = 1 where  IsExist = 0  and ReceiverID = '{UserID_Lab.Text}' and(Annex1 != '' or Annex2 != '' or Annex3 != '' or Annex4 != ''  or Annex5 != '')";
			db = new DataBaseHelper("conString");
			if (!db.ExecuteNonQuery(sqlstr))
			{
				return;
			}
			GameClient[] allClients = GameServer.Instance.GetAllClients();
			GameClient[] array = allClients;
			foreach (GameClient gameClient in array)
			{
				if (gameClient.Out != null)
				{
					gameClient.Out.SendMailResponse(int.Parse(UserID_Lab.Text), eMailRespose.Receiver);
				}
			}
			MessageBox.Show("恢复成功");
		}
		else
		{
			MessageBox.Show("请选择需要恢复的玩家.");
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		string sqlstr = "UPDATE Sys_Users_Detail SET STATE = 0";
		db = new DataBaseHelper("conString");
		if (db.ExecuteNonQuery(sqlstr))
		{
			MessageBox.Show("在线状态重置成功");
		}
	}

	private void button4_Click(object sender, EventArgs e)
	{
		timer4 = new System.Timers.Timer();
		timer4.Interval = 1000.0;
		timer4.Elapsed += timer4_Elapsed;
		timer4.Enabled = true;
		button4.Enabled = false;
		button4.Text = "线程已设定";
	}

	private void timer4_Elapsed(object sender, ElapsedEventArgs e)
	{
		if ((DateTime.Now.Hour == 14 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0) || (DateTime.Now.Hour == 19 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0))
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string sqlstr = "update Sys_Users_WorldBoss_Reward set ScorePerTime = 0, HonorPerTime = 0";
			db = new DataBaseHelper("conString");
			db.ExecuteNonQuery(sqlstr);
			Console.ForegroundColor = ConsoleColor.White;
			write($"定时清理世界BOSS积分 -- {DateTime.Now.ToString()} \r\n", Color.Green);
		}
	}

	public static bool isEncryed(string url)
	{
		WebClient webClient = new WebClient();
		string @string = Encoding.UTF8.GetString(webClient.DownloadData(url));
		if (@string.Contains("^_^"))
		{
			return true;
		}
		return false;
	}

	private void button3_Click(object sender, EventArgs e)
	{
		if (UserID_Lab.Text != "0")
		{
			string text = comboBox1.Text;
			string text2 = text;
			int[] array = ((text2 == "粗糙品质") ? new int[42]
			{
				406111, 406121, 406131, 406141, 406151, 406161, 406211, 406221, 406231, 406241,
				406251, 406261, 406311, 406321, 406331, 406341, 406351, 406361, 406411, 406421,
				406431, 406441, 406451, 406461, 406511, 406516, 406521, 406526, 406531, 406536,
				406541, 406546, 406551, 406556, 406561, 406566, 406571, 406576, 406581, 406586,
				406591, 406596
			} : ((!(text2 == "普通品质")) ? new int[1] : new int[42]
			{
				406112, 406122, 406132, 406142, 406152, 406162, 406212, 406222, 406232, 406242,
				406252, 406262, 406312, 406322, 406332, 406342, 406352, 406362, 406412, 406422,
				406432, 406442, 406452, 406462, 406512, 406517, 406522, 406527, 406532, 406537,
				406542, 406547, 406552, 406557, 406562, 406567, 406572, 406577, 406582, 406587,
				406592, 406597
			}));
			for (int i = 0; i < array.Length; i++)
			{
				string sqlstr = $"DELETE Sys_Users_Mark WHERE UserID = {UserID_Lab.Text} AND TemplateID = {array[i]}";
				db = new DataBaseHelper("conString");
				db.ExecuteNonQuery(sqlstr);
			}
			MessageBox.Show("清理成功");
		}
		else
		{
			MessageBox.Show("请选择需要清理的玩家.");
		}
	}

	private void button5_Click_1(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.Horse.Info.curLevel = 89;
				playerById.Horse.Info.curExp = 14733500;
				playerById.PlayerCharacter.MountLv = 89;
				playerById.Out.SendUpHorse(isHasLevelUp: true, playerById.Horse.Info.curLevel, playerById.Horse.Info.curExp);
				int[] array = new int[12]
				{
					10801, 10101, 10701, 10901, 10201, 11001, 10401, 10601, 11101, 11301,
					11201, 10301
				};
				int[] array2 = array;
				foreach (int num in array2)
				{
					MountSkillDataInfo mountSkillDataInfo = new MountSkillDataInfo();
					mountSkillDataInfo.SKillID = num;
					mountSkillDataInfo.Exp = 0;
					mountSkillDataInfo.IsUse = 0;
					if (!playerById.Horse.curHasSkill.ContainsKey(num))
					{
						playerById.Horse.curHasSkill.Add(num, mountSkillDataInfo);
					}
				}
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
				return;
			}
			int[] array3 = new int[12]
			{
				10801, 10101, 10701, 10901, 10201, 11001, 10401, 10601, 11101, 11301,
				11201, 10301
			};
			Dictionary<int, MountSkillDataInfo> dictionary = new Dictionary<int, MountSkillDataInfo>();
			int[] array4 = array3;
			foreach (int num2 in array4)
			{
				MountSkillDataInfo mountSkillDataInfo2 = new MountSkillDataInfo();
				mountSkillDataInfo2.SKillID = num2;
				mountSkillDataInfo2.Exp = 0;
				mountSkillDataInfo2.IsUse = 0;
				if (!dictionary.ContainsKey(num2))
				{
					dictionary.Add(num2, mountSkillDataInfo2);
				}
			}
			empty = $"UPDATE Mount_Data SET curLevel = {89}, curExp = {14733500}, curHasSkill = {JsonConvert.SerializeObject(dictionary)} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
			db = new DataBaseHelper("conString");
			if (db.ExecuteNonQuery(empty))
			{
				MessageBox.Show("修改成功");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button6_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				int[] array = new int[12]
				{
					10110, 10210, 10310, 10405, 10610, 10705, 10805, 10905, 11005, 11110,
					11205, 11310
				};
				int[] array2 = new int[12]
				{
					47000, 47000, 87000, 45500, 57000, 45500, 45500, 45500, 45500, 87000,
					45500, 87000
				};
				int num = 0;
				foreach (KeyValuePair<int, MountSkillDataInfo> item in playerById.Horse.curHasSkill.ToList())
				{
					playerById.Horse.curHasSkill[item.Key] = null;
				}
				int[] array3 = array;
				foreach (int num2 in array3)
				{
					MountSkillDataInfo mountSkillDataInfo = new MountSkillDataInfo();
					mountSkillDataInfo.SKillID = num2;
					mountSkillDataInfo.Exp = array2[num];
					mountSkillDataInfo.IsUse = 0;
					if (!playerById.Horse.curHasSkill.ContainsKey(num2))
					{
						playerById.Horse.curHasSkill.Add(num2, mountSkillDataInfo);
					}
					num++;
				}
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
				return;
			}
			int[] array4 = new int[12]
			{
				10110, 10210, 10310, 10405, 10610, 10705, 10805, 10905, 11005, 11110,
				11205, 11310
			};
			int[] array5 = new int[12]
			{
				47000, 47000, 87000, 45500, 57000, 45500, 45500, 45500, 45500, 87000,
				45500, 87000
			};
			int num3 = 0;
			Dictionary<int, MountSkillDataInfo> dictionary = new Dictionary<int, MountSkillDataInfo>();
			int[] array6 = array4;
			foreach (int num4 in array6)
			{
				MountSkillDataInfo mountSkillDataInfo2 = new MountSkillDataInfo();
				mountSkillDataInfo2.SKillID = num4;
				mountSkillDataInfo2.Exp = array5[num3];
				mountSkillDataInfo2.IsUse = 0;
				if (!dictionary.ContainsKey(num4))
				{
					dictionary.Add(num4, mountSkillDataInfo2);
				}
				num3++;
			}
			empty = $"UPDATE Mount_Data SET curHasSkill = '{JsonConvert.SerializeObject(dictionary)}' WHERE UserID = {int.Parse(UserID_Lab.Text)}";
			db = new DataBaseHelper("conString");
			if (db.ExecuteNonQuery(empty))
			{
				MessageBox.Show("修改成功");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button11_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.Horse.Info.curLevel = 89;
				playerById.Horse.Info.curExp = 14733500;
				playerById.PlayerCharacter.MountLv = 89;
				playerById.Out.SendUpHorse(isHasLevelUp: true, playerById.Horse.Info.curLevel, playerById.Horse.Info.curExp);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				empty = $"UPDATE Mount_Data SET curLevel = {89}, curExp = {14733500} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button12_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PlayerCharacter.totemId = 10350;
				playerById.Out.SendPlayerRefreshTotem(playerById.PlayerCharacter);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
				return;
			}
			empty = $"UPDATE Sys_Users_Detail SET totemId = {10350} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
			db = new DataBaseHelper("conString");
			if (db.ExecuteNonQuery(empty))
			{
				MessageBox.Show("修改成功");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button13_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				Dictionary<int, int> totemUpgrade = playerById.PlayerCharacter.GetTotemUpgrade();
				if (totemUpgrade.ContainsKey(1))
				{
					totemUpgrade[1] = 20;
				}
				else
				{
					totemUpgrade.Add(1, 20);
				}
				if (totemUpgrade.ContainsKey(2))
				{
					totemUpgrade[2] = 20;
				}
				else
				{
					totemUpgrade.Add(2, 20);
				}
				if (totemUpgrade.ContainsKey(3))
				{
					totemUpgrade[3] = 20;
				}
				else
				{
					totemUpgrade.Add(3, 20);
				}
				if (totemUpgrade.ContainsKey(4))
				{
					totemUpgrade[4] = 20;
				}
				else
				{
					totemUpgrade.Add(4, 20);
				}
				if (totemUpgrade.ContainsKey(5))
				{
					totemUpgrade[5] = 20;
				}
				else
				{
					totemUpgrade.Add(5, 20);
				}
				playerById.PlayerCharacter.SetTotemUpgrade(totemUpgrade);
				playerById.Out.SendTotemUpgradeInfo(playerById.PlayerCharacter.GetTotemUpgrade());
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				dictionary.Add(1, 20);
				dictionary.Add(2, 20);
				dictionary.Add(3, 20);
				dictionary.Add(4, 20);
				dictionary.Add(5, 20);
				empty = $"UPDATE Sys_Users_Detail SET TotemUpgrade = '{JsonConvert.SerializeObject(dictionary)}' WHERE UserID = {int.Parse(UserID_Lab.Text)}";
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button14_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.Actives.Info.CurrentSelectIndex = 6;
				playerById.Actives.Info.CurrentLevel = 60;
				playerById.Actives.Info.HomeTempleExp = 54940724;
				GSPacketIn gSPacketIn = new GSPacketIn(297);
				gSPacketIn.WriteByte(0);
				gSPacketIn.WriteInt(playerById.Actives.Info.CurrentSelectIndex);
				gSPacketIn.WriteInt(playerById.Actives.Info.CurrentLevel);
				gSPacketIn.WriteInt(playerById.Actives.Info.HomeTempleExp);
				gSPacketIn.WriteInt(playerById.Actives.Info.promoteLevel);
				playerById.SendTCP(gSPacketIn);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				empty = $"UPDATE Sys_Active_System_Data SET CurrentSelectIndex = {6}, CurrentLevel = {60}, HomeTempleExp = {54940724} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button15_Click_1(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.Actives.Info.promoteLevel = 60;
				GSPacketIn gSPacketIn = new GSPacketIn(297);
				gSPacketIn.WriteByte(0);
				gSPacketIn.WriteInt(playerById.Actives.Info.CurrentSelectIndex);
				gSPacketIn.WriteInt(playerById.Actives.Info.CurrentLevel);
				gSPacketIn.WriteInt(playerById.Actives.Info.HomeTempleExp);
				gSPacketIn.WriteInt(playerById.Actives.Info.promoteLevel);
				playerById.SendTCP(gSPacketIn);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				empty = $"UPDATE Sys_Active_System_Data SET promoteLevel = {60} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button16_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PlayerCharacter.CardInfo.CardLevel = 90;
				playerById.PlayerCharacter.CardInfo.Point = 209;
				playerById.PlayerCharacter.CardInfo.Armor = 0;
				playerById.PlayerCharacter.CardInfo.Crits = 0;
				playerById.PlayerCharacter.CardInfo.Trick = 0;
				playerById.PlayerCharacter.CardInfo.Viole = 0;
				CardNewSystemMgr.Out.SendCardInfo(playerById);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				empty = string.Format("UPDATE Sys_Users_CardInfo SET CardLevel = {0}, Point = {1}, Armor = {2}, Crits = {2}, Trick = {2}, Viole = {2} WHERE UserID = {3}", 90, 209, 0, int.Parse(UserID_Lab.Text));
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button17_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PlayerCharacter.CardInfo.SuitLevel = 30;
				CardNewSystemMgr.Out.SendCardInfo(playerById);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
				return;
			}
			empty = $"UPDATE Sys_Users_CardInfo SET SuitLevel = {30} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
			db = new DataBaseHelper("conString");
			if (db.ExecuteNonQuery(empty))
			{
				MessageBox.Show("修改成功");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button18_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PlayerCharacter.Texp.attTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.defTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.lukTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.spdTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.hpTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.magicAtkTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.magicDefTexpExp = 56655500;
				playerById.UpdateProperties();
				playerById.SendUpdatePublicPlayer();
				MessageBox.Show("修改成功");
			}
			else
			{
				empty = string.Format("UPDATE Sys_Users_Texp SET attTexpExp = {0}, defTexpExp = {0}, lukTexpExp = {0}, spdTexpExp = {0}, hpTexpExp = {0}, magicAtkTexpExp = {0}, magicDefTexpExp = {0}, WHERE UserID = {1}", 56655500, int.Parse(UserID_Lab.Text));
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button19_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PlayerCharacter.Texp.TrainCritTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.TrainSunderTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.TrainCritDmgTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.TrainSpeedTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.TrainTricKillTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.TrainDmgTexpExp = 56655500;
				playerById.PlayerCharacter.Texp.TrainArmorTexpExp = 56655500;
				playerById.UpdateProperties();
				playerById.SendUpdatePublicPlayer();
				MessageBox.Show("修改成功");
			}
			else
			{
				empty = string.Format("UPDATE Sys_Users_Texp SET TrainCritTexpExp = {0}, TrainSunderTexpExp = {0}, TrainCritDmgTexpExp = {0}, TrainSpeedTexpExp = {0}, TrainTricKillTexpExp = {0}, TrainDmgTexpExp = {0}, TrainArmorTexpExp = {0}, WHERE UserID = {1}", 56655500, int.Parse(UserID_Lab.Text));
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button20_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				List<SpiritInfo> spirit = SpiritInfoMgr.GetSpirit(1);
				UserEquipGhostInfo ghostEquip = playerById.GetGhostEquip(spirit[0].BagType, spirit[0].BagPlace);
				if (ghostEquip == null)
				{
					ghostEquip = new UserEquipGhostInfo();
					ghostEquip.UserID = playerById.PlayerId;
					ghostEquip.BagType = spirit[0].BagType;
					ghostEquip.Place = spirit[0].BagPlace;
					ghostEquip.Level = 10;
					ghostEquip.TotalGhost = 0;
					playerById.AddEquipGhost(ghostEquip);
				}
				else
				{
					ghostEquip.Level = 10;
				}
				playerById.Out.SendUserSyncEquipGhost(playerById);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				MessageBox.Show("玩家不在线..");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button21_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				List<SpiritInfo> spirit = SpiritInfoMgr.GetSpirit(5);
				UserEquipGhostInfo ghostEquip = playerById.GetGhostEquip(spirit[0].BagType, spirit[0].BagPlace);
				if (ghostEquip == null)
				{
					ghostEquip = new UserEquipGhostInfo();
					ghostEquip.UserID = playerById.PlayerId;
					ghostEquip.BagType = spirit[0].BagType;
					ghostEquip.Place = spirit[0].BagPlace;
					ghostEquip.Level = 10;
					ghostEquip.TotalGhost = 0;
					playerById.AddEquipGhost(ghostEquip);
				}
				else
				{
					ghostEquip.Level = 10;
				}
				playerById.Out.SendUserSyncEquipGhost(playerById);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				MessageBox.Show("玩家不在线..");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button22_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				List<SpiritInfo> spirit = SpiritInfoMgr.GetSpirit(7);
				UserEquipGhostInfo ghostEquip = playerById.GetGhostEquip(spirit[0].BagType, spirit[0].BagPlace);
				if (ghostEquip == null)
				{
					ghostEquip = new UserEquipGhostInfo();
					ghostEquip.UserID = playerById.PlayerId;
					ghostEquip.BagType = spirit[0].BagType;
					ghostEquip.Place = spirit[0].BagPlace;
					ghostEquip.Level = 10;
					ghostEquip.TotalGhost = 0;
					playerById.AddEquipGhost(ghostEquip);
				}
				else
				{
					ghostEquip.Level = 10;
				}
				playerById.Out.SendUserSyncEquipGhost(playerById);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				MessageBox.Show("玩家不在线..");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button23_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PlayerCharacter.necklaceExp = 630000;
				NecklaceStrengthMgr.UpdateNecklaceInfo(playerById);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
				return;
			}
			empty = $"UPDATE Sys_Users_Detail SET necklaceExp = {630000} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
			db = new DataBaseHelper("conString");
			if (db.ExecuteNonQuery(empty))
			{
				MessageBox.Show("修改成功");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button24_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PlayerCharacter.necklaceCastLevel = 40;
				NecklaceStrengthMgr.SendNecklaceCastInfo(playerById);
				NecklaceStrengthMgr.UpdateNecklaceInfo(playerById);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				empty = $"UPDATE Sys_Users_Detail SET necklaceCastLevel = {40} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button25_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PlayerCharacter.fineSuitExp = 345860;
				playerById.UpdateProperties();
				playerById.SendUpdatePublicPlayer();
				MessageBox.Show("修改成功");
				return;
			}
			empty = $"UPDATE Sys_Users_Detail SET fineSuitExp = {345860} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
			db = new DataBaseHelper("conString");
			if (db.ExecuteNonQuery(empty))
			{
				MessageBox.Show("修改成功");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button26_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PlayerCharacter.evolutionGrade = 50;
				playerById.PlayerCharacter.evolutionExp = 809030;
				playerById.UpdateProperties();
				playerById.SendUpdatePublicPlayer();
				MessageBox.Show("修改成功");
			}
			else
			{
				empty = $"UPDATE Sys_Users_Detail SET evolutionGrade = {50}, evolutionGrade = {809030} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button28_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			PlayerBussiness playerBussiness = new PlayerBussiness();
			if (playerById != null)
			{
				List<int> list = new List<int> { 11, 5, 2, 3, 13 };
				for (int i = 0; i < list.Count; i++)
				{
					int place = list[i];
					UserGemStone gemStone = playerById.GetGemStone(place);
					gemStone.FigSpiritIdValue = "12,0,2|12,0,1|12,0,0";
					playerBussiness.UpdateGemStoneInfo(gemStone);
					playerById.UpdateGemStone(place, gemStone);
					playerById.Out.SendPlayerFigSpiritUp(playerById.PlayerCharacter.ID, gemStone, isUp: true, isMaxLevel: true, isFall: true, 0, 0);
				}
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				MessageBox.Show("玩家不在线");
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button27_Click(object sender, EventArgs e)
	{
		string empty = string.Empty;
		if (Level_Text.Text != "" && UserID_Lab.Text != "0")
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
			if (playerById != null)
			{
				playerById.PetBag.EatPets.clothesLevel = 30;
				playerById.PetBag.EatPets.clothesExp = 30000;
				playerById.PetBag.EatPets.weaponLevel = 30;
				playerById.PetBag.EatPets.weaponExp = 30000;
				playerById.PetBag.EatPets.hatLevel = 30;
				playerById.PetBag.EatPets.hatExp = 30000;
				playerById.Out.SendEatPetsInfo(playerById.PetBag.EatPets);
				playerById.UpdateProperties();
				MessageBox.Show("修改成功");
			}
			else
			{
				empty = string.Format("UPDATE Sys_Eat_Pets SET clothesLevel = {0}, weaponLevel = {0}, hatLevel = {0}, clothesExp = {1}, weaponExp = {1}, hatExp = {1} WHERE UserID = {2}", 30, 30000, int.Parse(UserID_Lab.Text));
				db = new DataBaseHelper("conString");
				if (db.ExecuteNonQuery(empty))
				{
					MessageBox.Show("修改成功");
				}
			}
		}
		else
		{
			MessageBox.Show("请选择需要修改的玩家");
		}
	}

	private void button29_Click(object sender, EventArgs e)
	{
		string Mes = string.Empty;
		TeamMgr.SendTeamWeekReward(ref Mes);
		MessageBox.Show(Mes);
	}

	private void button30_Click(object sender, EventArgs e)
	{
		string Mes = string.Empty;
		TeamMgr.SendTeamSeasonReward(ref Mes);
		MessageBox.Show(Mes);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.button10 = new System.Windows.Forms.Button();
		this.button9 = new System.Windows.Forms.Button();
		this.button8 = new System.Windows.Forms.Button();
		this.button7 = new System.Windows.Forms.Button();
		this.groupBox9 = new System.Windows.Forms.GroupBox();
		this.TemplateID_CBox = new System.Windows.Forms.ComboBox();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.label7 = new System.Windows.Forms.Label();
		this.FindItem_Btn = new System.Windows.Forms.Button();
		this.ItemName_Text = new System.Windows.Forms.TextBox();
		this.label18 = new System.Windows.Forms.Label();
		this.groupBox16 = new System.Windows.Forms.GroupBox();
		this.SendItemToPlayer = new System.Windows.Forms.Button();
		this.label52 = new System.Windows.Forms.Label();
		this.IsBind = new System.Windows.Forms.ComboBox();
		this.textBox5 = new System.Windows.Forms.TextBox();
		this.textBox12 = new System.Windows.Forms.TextBox();
		this.label53 = new System.Windows.Forms.Label();
		this.textBox11 = new System.Windows.Forms.TextBox();
		this.label54 = new System.Windows.Forms.Label();
		this.textBox10 = new System.Windows.Forms.TextBox();
		this.label55 = new System.Windows.Forms.Label();
		this.textBox9 = new System.Windows.Forms.TextBox();
		this.label56 = new System.Windows.Forms.Label();
		this.textBox8 = new System.Windows.Forms.TextBox();
		this.label57 = new System.Windows.Forms.Label();
		this.textBox7 = new System.Windows.Forms.TextBox();
		this.label58 = new System.Windows.Forms.Label();
		this.textBox6 = new System.Windows.Forms.TextBox();
		this.label59 = new System.Windows.Forms.Label();
		this.label60 = new System.Windows.Forms.Label();
		this.textBox3 = new System.Windows.Forms.TextBox();
		this.label61 = new System.Windows.Forms.Label();
		this.textBox2 = new System.Windows.Forms.TextBox();
		this.label62 = new System.Windows.Forms.Label();
		this.label63 = new System.Windows.Forms.Label();
		this.playernickname = new System.Windows.Forms.TextBox();
		this.groupBox15 = new System.Windows.Forms.GroupBox();
		this.Stop = new System.Windows.Forms.Button();
		this.Start = new System.Windows.Forms.Button();
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.label51 = new System.Windows.Forms.Label();
		this.time1 = new System.Windows.Forms.TextBox();
		this.tempid1 = new System.Windows.Forms.TextBox();
		this.SendTime = new System.Windows.Forms.Label();
		this.label50 = new System.Windows.Forms.Label();
		this.label49 = new System.Windows.Forms.Label();
		this.panel1 = new System.Windows.Forms.Panel();
		this.label48 = new System.Windows.Forms.Label();
		this.label47 = new System.Windows.Forms.Label();
		this.label46 = new System.Windows.Forms.Label();
		this.time = new System.Windows.Forms.TextBox();
		this.ShutDown = new System.Windows.Forms.Button();
		this.ReStart = new System.Windows.Forms.Button();
		this.label45 = new System.Windows.Forms.Label();
		this.label44 = new System.Windows.Forms.Label();
		this.groupBox14 = new System.Windows.Forms.GroupBox();
		this.SendAnnouncement = new System.Windows.Forms.Button();
		this.richTextBox1 = new System.Windows.Forms.TextBox();
		this.ListType = new System.Windows.Forms.ComboBox();
		this.label43 = new System.Windows.Forms.Label();
		this.playerBox = new System.Windows.Forms.GroupBox();
		this.button1 = new System.Windows.Forms.Button();
		this.Ban = new System.Windows.Forms.Button();
		this.Kick = new System.Windows.Forms.Button();
		this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
		this.label1 = new System.Windows.Forms.Label();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.UpdateLogs = new System.Windows.Forms.TextBox();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.ListOnlinePlayers = new System.Windows.Forms.ListBox();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.groupBox8 = new System.Windows.Forms.GroupBox();
		this.button28 = new System.Windows.Forms.Button();
		this.button27 = new System.Windows.Forms.Button();
		this.button26 = new System.Windows.Forms.Button();
		this.button25 = new System.Windows.Forms.Button();
		this.button24 = new System.Windows.Forms.Button();
		this.button23 = new System.Windows.Forms.Button();
		this.button22 = new System.Windows.Forms.Button();
		this.button21 = new System.Windows.Forms.Button();
		this.button20 = new System.Windows.Forms.Button();
		this.button19 = new System.Windows.Forms.Button();
		this.button18 = new System.Windows.Forms.Button();
		this.button17 = new System.Windows.Forms.Button();
		this.button16 = new System.Windows.Forms.Button();
		this.button15 = new System.Windows.Forms.Button();
		this.button14 = new System.Windows.Forms.Button();
		this.button13 = new System.Windows.Forms.Button();
		this.button12 = new System.Windows.Forms.Button();
		this.button11 = new System.Windows.Forms.Button();
		this.button6 = new System.Windows.Forms.Button();
		this.button5 = new System.Windows.Forms.Button();
		this.groupBox19 = new System.Windows.Forms.GroupBox();
		this.ChangePassword = new System.Windows.Forms.Button();
		this.Password = new System.Windows.Forms.TextBox();
		this.label66 = new System.Windows.Forms.Label();
		this.groupBox13 = new System.Windows.Forms.GroupBox();
		this.CleanAcitveNum_Btn = new System.Windows.Forms.Button();
		this.ListActiveID_CBox = new System.Windows.Forms.ComboBox();
		this.label42 = new System.Windows.Forms.Label();
		this.groupBox4 = new System.Windows.Forms.GroupBox();
		this.FindUser_Btn = new System.Windows.Forms.Button();
		this.UserName_Text = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.NewPlyaerFightPowerRank = new System.Windows.Forms.DataGridView();
		this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.groupBox5 = new System.Windows.Forms.GroupBox();
		this.button3 = new System.Windows.Forms.Button();
		this.comboBox1 = new System.Windows.Forms.ComboBox();
		this.label4 = new System.Windows.Forms.Label();
		this.RecoverMessage = new System.Windows.Forms.Button();
		this.PasswordTwo_Text = new System.Windows.Forms.TextBox();
		this.label15 = new System.Windows.Forms.Label();
		this.CleanUser_Text = new System.Windows.Forms.Button();
		this.UpdateLevel_Btn = new System.Windows.Forms.Button();
		this.Level_Text = new System.Windows.Forms.TextBox();
		this.NickName_Text = new System.Windows.Forms.TextBox();
		this.UpUserData_Btn = new System.Windows.Forms.Button();
		this.label3 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.groupBox6 = new System.Windows.Forms.GroupBox();
		this.FindRank_Btn = new System.Windows.Forms.Button();
		this.EndTime_TimeDate = new System.Windows.Forms.DateTimePicker();
		this.StartTime_TimeDate = new System.Windows.Forms.DateTimePicker();
		this.label17 = new System.Windows.Forms.Label();
		this.label16 = new System.Windows.Forms.Label();
		this.groupBox7 = new System.Windows.Forms.GroupBox();
		this.UserID_Lab = new System.Windows.Forms.Label();
		this.Label_d = new System.Windows.Forms.Label();
		this.Online_Text = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.tabPage3 = new System.Windows.Forms.TabPage();
		this.groupBox20 = new System.Windows.Forms.GroupBox();
		this.button4 = new System.Windows.Forms.Button();
		this.CreateAllCeleb = new System.Windows.Forms.Button();
		this.Reload = new System.Windows.Forms.Button();
		this.groupBox18 = new System.Windows.Forms.GroupBox();
		this.richTextBox2 = new System.Windows.Forms.RichTextBox();
		this.groupBox10 = new System.Windows.Forms.GroupBox();
		this.NewItemID_TextBox = new System.Windows.Forms.TextBox();
		this.label41 = new System.Windows.Forms.Label();
		this.ItemQuality_CBox = new System.Windows.Forms.ComboBox();
		this.InsertItem_Btn = new System.Windows.Forms.Button();
		this.ItemLuck_TextBox = new System.Windows.Forms.TextBox();
		this.ItemAgi_TextBox = new System.Windows.Forms.TextBox();
		this.ItemDef_TextBox = new System.Windows.Forms.TextBox();
		this.ItemAtt_TextBox = new System.Windows.Forms.TextBox();
		this.ItemDesc_TextBox = new System.Windows.Forms.TextBox();
		this.label39 = new System.Windows.Forms.Label();
		this.ItemArmor_Text = new System.Windows.Forms.TextBox();
		this.label40 = new System.Windows.Forms.Label();
		this.pictureBox3 = new System.Windows.Forms.PictureBox();
		this.label38 = new System.Windows.Forms.Label();
		this.ItemName_TextBox = new System.Windows.Forms.TextBox();
		this.label37 = new System.Windows.Forms.Label();
		this.label36 = new System.Windows.Forms.Label();
		this.label35 = new System.Windows.Forms.Label();
		this.label34 = new System.Windows.Forms.Label();
		this.label33 = new System.Windows.Forms.Label();
		this.label32 = new System.Windows.Forms.Label();
		this.ListItem_CBox = new System.Windows.Forms.ComboBox();
		this.label31 = new System.Windows.Forms.Label();
		this.ListCategoryID_CBox = new System.Windows.Forms.ComboBox();
		this.label30 = new System.Windows.Forms.Label();
		this.groupBox11 = new System.Windows.Forms.GroupBox();
		this.NewTemplateID_Text = new System.Windows.Forms.TextBox();
		this.label29 = new System.Windows.Forms.Label();
		this.Special_Text = new System.Windows.Forms.TextBox();
		this.CommonAddWound_Text = new System.Windows.Forms.TextBox();
		this.CommonMultiBall_Text = new System.Windows.Forms.TextBox();
		this.Common_Text = new System.Windows.Forms.TextBox();
		this.InsertArm_Btn = new System.Windows.Forms.Button();
		this.groupBox12 = new System.Windows.Forms.GroupBox();
		this.label28 = new System.Windows.Forms.Label();
		this.pictureBox2 = new System.Windows.Forms.PictureBox();
		this.NewItemQuality_CBox = new System.Windows.Forms.ComboBox();
		this.NewItemDamage_Text = new System.Windows.Forms.TextBox();
		this.NewItemAgi_Text = new System.Windows.Forms.TextBox();
		this.NewItemLuck_Text = new System.Windows.Forms.TextBox();
		this.NewItemDef_Text = new System.Windows.Forms.TextBox();
		this.NewItemAtt_Text = new System.Windows.Forms.TextBox();
		this.NewItemDesc_Text = new System.Windows.Forms.TextBox();
		this.NewItemName_Text = new System.Windows.Forms.TextBox();
		this.label27 = new System.Windows.Forms.Label();
		this.label26 = new System.Windows.Forms.Label();
		this.label25 = new System.Windows.Forms.Label();
		this.label24 = new System.Windows.Forms.Label();
		this.label23 = new System.Windows.Forms.Label();
		this.label22 = new System.Windows.Forms.Label();
		this.label21 = new System.Windows.Forms.Label();
		this.label20 = new System.Windows.Forms.Label();
		this.ListItemName_CBox = new System.Windows.Forms.ComboBox();
		this.label19 = new System.Windows.Forms.Label();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.button2 = new System.Windows.Forms.Button();
		this.textBox13 = new System.Windows.Forms.TextBox();
		this.textBox14 = new System.Windows.Forms.TextBox();
		this.textBox15 = new System.Windows.Forms.TextBox();
		this.label64 = new System.Windows.Forms.Label();
		this.label65 = new System.Windows.Forms.Label();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.mainWorker = new System.Windows.Forms.Timer(this.components);
		this.button29 = new System.Windows.Forms.Button();
		this.groupBox17 = new System.Windows.Forms.GroupBox();
		this.button30 = new System.Windows.Forms.Button();
		this.tabControl1.SuspendLayout();
		this.tabPage1.SuspendLayout();
		this.groupBox9.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		this.groupBox16.SuspendLayout();
		this.groupBox15.SuspendLayout();
		this.panel1.SuspendLayout();
		this.groupBox14.SuspendLayout();
		this.playerBox.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.groupBox1.SuspendLayout();
		this.tabPage2.SuspendLayout();
		this.groupBox8.SuspendLayout();
		this.groupBox19.SuspendLayout();
		this.groupBox13.SuspendLayout();
		this.groupBox4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.NewPlyaerFightPowerRank).BeginInit();
		this.groupBox5.SuspendLayout();
		this.groupBox6.SuspendLayout();
		this.groupBox7.SuspendLayout();
		this.tabPage3.SuspendLayout();
		this.groupBox20.SuspendLayout();
		this.groupBox18.SuspendLayout();
		this.groupBox10.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox3).BeginInit();
		this.groupBox11.SuspendLayout();
		this.groupBox12.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox2).BeginInit();
		this.groupBox3.SuspendLayout();
		this.groupBox17.SuspendLayout();
		base.SuspendLayout();
		this.tabControl1.Controls.Add(this.tabPage1);
		this.tabControl1.Controls.Add(this.tabPage2);
		this.tabControl1.Controls.Add(this.tabPage3);
		this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.tabControl1.Location = new System.Drawing.Point(4, 5);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(787, 465);
		this.tabControl1.TabIndex = 0;
		this.tabPage1.Controls.Add(this.button10);
		this.tabPage1.Controls.Add(this.button9);
		this.tabPage1.Controls.Add(this.button8);
		this.tabPage1.Controls.Add(this.button7);
		this.tabPage1.Controls.Add(this.groupBox9);
		this.tabPage1.Controls.Add(this.groupBox16);
		this.tabPage1.Controls.Add(this.groupBox15);
		this.tabPage1.Controls.Add(this.panel1);
		this.tabPage1.Controls.Add(this.groupBox14);
		this.tabPage1.Controls.Add(this.playerBox);
		this.tabPage1.Controls.Add(this.groupBox2);
		this.tabPage1.Controls.Add(this.groupBox1);
		this.tabPage1.Location = new System.Drawing.Point(4, 26);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(779, 435);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "分组1";
		this.tabPage1.UseVisualStyleBackColor = true;
		this.button10.Location = new System.Drawing.Point(698, 32);
		this.button10.Name = "button10";
		this.button10.Size = new System.Drawing.Size(75, 23);
		this.button10.TabIndex = 25;
		this.button10.Text = "解密PNG";
		this.button10.UseVisualStyleBackColor = true;
		this.button10.Click += new System.EventHandler(button10_Click);
		this.button9.Location = new System.Drawing.Point(698, 6);
		this.button9.Name = "button9";
		this.button9.Size = new System.Drawing.Size(75, 23);
		this.button9.TabIndex = 24;
		this.button9.Text = "加密PNG";
		this.button9.UseVisualStyleBackColor = true;
		this.button9.Click += new System.EventHandler(button9_Click);
		this.button8.Location = new System.Drawing.Point(615, 32);
		this.button8.Name = "button8";
		this.button8.Size = new System.Drawing.Size(75, 23);
		this.button8.TabIndex = 23;
		this.button8.Text = "解密XML";
		this.button8.UseVisualStyleBackColor = true;
		this.button8.Click += new System.EventHandler(button8_Click);
		this.button7.Location = new System.Drawing.Point(615, 6);
		this.button7.Name = "button7";
		this.button7.Size = new System.Drawing.Size(75, 23);
		this.button7.TabIndex = 22;
		this.button7.Text = "加密XML";
		this.button7.UseVisualStyleBackColor = true;
		this.button7.Click += new System.EventHandler(button7_Click);
		this.groupBox9.Controls.Add(this.TemplateID_CBox);
		this.groupBox9.Controls.Add(this.pictureBox1);
		this.groupBox9.Controls.Add(this.label7);
		this.groupBox9.Controls.Add(this.FindItem_Btn);
		this.groupBox9.Controls.Add(this.ItemName_Text);
		this.groupBox9.Controls.Add(this.label18);
		this.groupBox9.Location = new System.Drawing.Point(484, 59);
		this.groupBox9.Name = "groupBox9";
		this.groupBox9.Size = new System.Drawing.Size(289, 128);
		this.groupBox9.TabIndex = 21;
		this.groupBox9.TabStop = false;
		this.groupBox9.Text = "快速查找";
		this.TemplateID_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.TemplateID_CBox.FormattingEnabled = true;
		this.TemplateID_CBox.Location = new System.Drawing.Point(12, 90);
		this.TemplateID_CBox.Name = "TemplateID_CBox";
		this.TemplateID_CBox.Size = new System.Drawing.Size(157, 25);
		this.TemplateID_CBox.TabIndex = 27;
		this.TemplateID_CBox.SelectedIndexChanged += new System.EventHandler(TemplateID_CBox_SelectedIndexChanged);
		this.pictureBox1.Image = Road.Service.Properties.Resources._404;
		this.pictureBox1.Location = new System.Drawing.Point(185, 19);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(90, 90);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 5;
		this.pictureBox1.TabStop = false;
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(9, 70);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(92, 17);
		this.label7.TabIndex = 4;
		this.label7.Text = "查找到的道具：";
		this.FindItem_Btn.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.FindItem_Btn.Location = new System.Drawing.Point(12, 39);
		this.FindItem_Btn.Name = "FindItem_Btn";
		this.FindItem_Btn.Size = new System.Drawing.Size(156, 28);
		this.FindItem_Btn.TabIndex = 2;
		this.FindItem_Btn.Text = "查 找 游 戏 道 具";
		this.FindItem_Btn.UseVisualStyleBackColor = true;
		this.FindItem_Btn.Click += new System.EventHandler(FindItem_Btn_Click);
		this.ItemName_Text.Location = new System.Drawing.Point(79, 13);
		this.ItemName_Text.Name = "ItemName_Text";
		this.ItemName_Text.Size = new System.Drawing.Size(89, 23);
		this.ItemName_Text.TabIndex = 1;
		this.ItemName_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label18.AutoSize = true;
		this.label18.Location = new System.Drawing.Point(9, 19);
		this.label18.Name = "label18";
		this.label18.Size = new System.Drawing.Size(68, 17);
		this.label18.TabIndex = 0;
		this.label18.Text = "道具名称：";
		this.groupBox16.Controls.Add(this.SendItemToPlayer);
		this.groupBox16.Controls.Add(this.label52);
		this.groupBox16.Controls.Add(this.IsBind);
		this.groupBox16.Controls.Add(this.textBox5);
		this.groupBox16.Controls.Add(this.textBox12);
		this.groupBox16.Controls.Add(this.label53);
		this.groupBox16.Controls.Add(this.textBox11);
		this.groupBox16.Controls.Add(this.label54);
		this.groupBox16.Controls.Add(this.textBox10);
		this.groupBox16.Controls.Add(this.label55);
		this.groupBox16.Controls.Add(this.textBox9);
		this.groupBox16.Controls.Add(this.label56);
		this.groupBox16.Controls.Add(this.textBox8);
		this.groupBox16.Controls.Add(this.label57);
		this.groupBox16.Controls.Add(this.textBox7);
		this.groupBox16.Controls.Add(this.label58);
		this.groupBox16.Controls.Add(this.textBox6);
		this.groupBox16.Controls.Add(this.label59);
		this.groupBox16.Controls.Add(this.label60);
		this.groupBox16.Controls.Add(this.textBox3);
		this.groupBox16.Controls.Add(this.label61);
		this.groupBox16.Controls.Add(this.textBox2);
		this.groupBox16.Controls.Add(this.label62);
		this.groupBox16.Controls.Add(this.label63);
		this.groupBox16.Controls.Add(this.playernickname);
		this.groupBox16.ForeColor = System.Drawing.Color.Black;
		this.groupBox16.Location = new System.Drawing.Point(484, 193);
		this.groupBox16.Name = "groupBox16";
		this.groupBox16.Size = new System.Drawing.Size(289, 238);
		this.groupBox16.TabIndex = 20;
		this.groupBox16.TabStop = false;
		this.groupBox16.Text = "发送物品";
		this.SendItemToPlayer.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.SendItemToPlayer.Location = new System.Drawing.Point(79, 201);
		this.SendItemToPlayer.Name = "SendItemToPlayer";
		this.SendItemToPlayer.Size = new System.Drawing.Size(127, 28);
		this.SendItemToPlayer.TabIndex = 28;
		this.SendItemToPlayer.Text = "发 送 道 具";
		this.SendItemToPlayer.UseVisualStyleBackColor = true;
		this.SendItemToPlayer.Click += new System.EventHandler(SendItemToPlayer_Click);
		this.label52.AutoSize = true;
		this.label52.Location = new System.Drawing.Point(147, 170);
		this.label52.Name = "label52";
		this.label52.Size = new System.Drawing.Size(32, 17);
		this.label52.TabIndex = 27;
		this.label52.Text = "绑定";
		this.IsBind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.IsBind.FormattingEnabled = true;
		this.IsBind.Items.AddRange(new object[2] { "绑定", "未绑定" });
		this.IsBind.Location = new System.Drawing.Point(185, 165);
		this.IsBind.Name = "IsBind";
		this.IsBind.Size = new System.Drawing.Size(86, 25);
		this.IsBind.TabIndex = 26;
		this.textBox5.Location = new System.Drawing.Point(55, 167);
		this.textBox5.Name = "textBox5";
		this.textBox5.Size = new System.Drawing.Size(86, 23);
		this.textBox5.TabIndex = 10;
		this.textBox5.Text = "0";
		this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.textBox12.Location = new System.Drawing.Point(185, 138);
		this.textBox12.Name = "textBox12";
		this.textBox12.Size = new System.Drawing.Size(86, 23);
		this.textBox12.TabIndex = 24;
		this.textBox12.Text = "0";
		this.textBox12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label53.AutoSize = true;
		this.label53.ForeColor = System.Drawing.Color.Black;
		this.label53.Location = new System.Drawing.Point(147, 141);
		this.label53.Name = "label53";
		this.label53.Size = new System.Drawing.Size(32, 17);
		this.label53.TabIndex = 23;
		this.label53.Text = "幸运";
		this.textBox11.Location = new System.Drawing.Point(185, 109);
		this.textBox11.Name = "textBox11";
		this.textBox11.Size = new System.Drawing.Size(86, 23);
		this.textBox11.TabIndex = 22;
		this.textBox11.Text = "0";
		this.textBox11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label54.AutoSize = true;
		this.label54.ForeColor = System.Drawing.Color.Black;
		this.label54.Location = new System.Drawing.Point(147, 112);
		this.label54.Name = "label54";
		this.label54.Size = new System.Drawing.Size(32, 17);
		this.label54.TabIndex = 21;
		this.label54.Text = "敏捷";
		this.textBox10.Location = new System.Drawing.Point(55, 138);
		this.textBox10.Name = "textBox10";
		this.textBox10.Size = new System.Drawing.Size(86, 23);
		this.textBox10.TabIndex = 20;
		this.textBox10.Text = "0";
		this.textBox10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label55.AutoSize = true;
		this.label55.ForeColor = System.Drawing.Color.Black;
		this.label55.Location = new System.Drawing.Point(17, 144);
		this.label55.Name = "label55";
		this.label55.Size = new System.Drawing.Size(32, 17);
		this.label55.TabIndex = 19;
		this.label55.Text = "防御";
		this.textBox9.Location = new System.Drawing.Point(55, 109);
		this.textBox9.Name = "textBox9";
		this.textBox9.Size = new System.Drawing.Size(86, 23);
		this.textBox9.TabIndex = 18;
		this.textBox9.Text = "0";
		this.textBox9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label56.AutoSize = true;
		this.label56.ForeColor = System.Drawing.Color.Black;
		this.label56.Location = new System.Drawing.Point(17, 112);
		this.label56.Name = "label56";
		this.label56.Size = new System.Drawing.Size(32, 17);
		this.label56.TabIndex = 17;
		this.label56.Text = "攻击";
		this.textBox8.Location = new System.Drawing.Point(185, 51);
		this.textBox8.Name = "textBox8";
		this.textBox8.Size = new System.Drawing.Size(86, 23);
		this.textBox8.TabIndex = 16;
		this.textBox8.Text = "0";
		this.textBox8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label57.AutoSize = true;
		this.label57.ForeColor = System.Drawing.Color.Black;
		this.label57.Location = new System.Drawing.Point(147, 54);
		this.label57.Name = "label57";
		this.label57.Size = new System.Drawing.Size(32, 17);
		this.label57.TabIndex = 15;
		this.label57.Text = "强化";
		this.textBox7.Location = new System.Drawing.Point(185, 80);
		this.textBox7.Name = "textBox7";
		this.textBox7.Size = new System.Drawing.Size(86, 23);
		this.textBox7.TabIndex = 14;
		this.textBox7.Text = "0";
		this.textBox7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label58.AutoSize = true;
		this.label58.ForeColor = System.Drawing.Color.Black;
		this.label58.Location = new System.Drawing.Point(147, 83);
		this.label58.Name = "label58";
		this.label58.Size = new System.Drawing.Size(32, 17);
		this.label58.TabIndex = 13;
		this.label58.Text = "点券";
		this.textBox6.Location = new System.Drawing.Point(55, 80);
		this.textBox6.Name = "textBox6";
		this.textBox6.Size = new System.Drawing.Size(86, 23);
		this.textBox6.TabIndex = 12;
		this.textBox6.Text = "0";
		this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label59.AutoSize = true;
		this.label59.ForeColor = System.Drawing.Color.Black;
		this.label59.Location = new System.Drawing.Point(17, 83);
		this.label59.Name = "label59";
		this.label59.Size = new System.Drawing.Size(32, 17);
		this.label59.TabIndex = 11;
		this.label59.Text = "金币";
		this.label60.AutoSize = true;
		this.label60.ForeColor = System.Drawing.Color.Black;
		this.label60.Location = new System.Drawing.Point(5, 170);
		this.label60.Name = "label60";
		this.label60.Size = new System.Drawing.Size(44, 17);
		this.label60.TabIndex = 9;
		this.label60.Text = "有效期";
		this.textBox3.Location = new System.Drawing.Point(55, 51);
		this.textBox3.Name = "textBox3";
		this.textBox3.Size = new System.Drawing.Size(86, 23);
		this.textBox3.TabIndex = 8;
		this.textBox3.Text = "1";
		this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label61.AutoSize = true;
		this.label61.ForeColor = System.Drawing.Color.Black;
		this.label61.Location = new System.Drawing.Point(17, 54);
		this.label61.Name = "label61";
		this.label61.Size = new System.Drawing.Size(32, 17);
		this.label61.TabIndex = 7;
		this.label61.Text = "数量";
		this.textBox2.Location = new System.Drawing.Point(185, 22);
		this.textBox2.Name = "textBox2";
		this.textBox2.Size = new System.Drawing.Size(86, 23);
		this.textBox2.TabIndex = 6;
		this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label62.AutoSize = true;
		this.label62.ForeColor = System.Drawing.Color.Black;
		this.label62.Location = new System.Drawing.Point(147, 25);
		this.label62.Name = "label62";
		this.label62.Size = new System.Drawing.Size(21, 17);
		this.label62.TabIndex = 5;
		this.label62.Text = "ID";
		this.label63.AutoSize = true;
		this.label63.ForeColor = System.Drawing.Color.Black;
		this.label63.Location = new System.Drawing.Point(17, 25);
		this.label63.Name = "label63";
		this.label63.Size = new System.Drawing.Size(32, 17);
		this.label63.TabIndex = 0;
		this.label63.Text = "昵称";
		this.playernickname.Location = new System.Drawing.Point(55, 22);
		this.playernickname.Name = "playernickname";
		this.playernickname.Size = new System.Drawing.Size(86, 23);
		this.playernickname.TabIndex = 1;
		this.playernickname.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.groupBox15.Controls.Add(this.Stop);
		this.groupBox15.Controls.Add(this.Start);
		this.groupBox15.Controls.Add(this.textBox1);
		this.groupBox15.Controls.Add(this.label51);
		this.groupBox15.Controls.Add(this.time1);
		this.groupBox15.Controls.Add(this.tempid1);
		this.groupBox15.Controls.Add(this.SendTime);
		this.groupBox15.Controls.Add(this.label50);
		this.groupBox15.Controls.Add(this.label49);
		this.groupBox15.Location = new System.Drawing.Point(278, 165);
		this.groupBox15.Name = "groupBox15";
		this.groupBox15.Size = new System.Drawing.Size(200, 132);
		this.groupBox15.TabIndex = 19;
		this.groupBox15.TabStop = false;
		this.groupBox15.Text = "泡点功能";
		this.Stop.Location = new System.Drawing.Point(112, 103);
		this.Stop.Name = "Stop";
		this.Stop.Size = new System.Drawing.Size(75, 23);
		this.Stop.TabIndex = 20;
		this.Stop.Text = "停止";
		this.Stop.UseVisualStyleBackColor = true;
		this.Stop.Click += new System.EventHandler(Stop_Click);
		this.Start.Location = new System.Drawing.Point(6, 103);
		this.Start.Name = "Start";
		this.Start.Size = new System.Drawing.Size(75, 23);
		this.Start.TabIndex = 19;
		this.Start.Text = "启动";
		this.Start.UseVisualStyleBackColor = true;
		this.Start.Click += new System.EventHandler(Start_Click);
		this.textBox1.Location = new System.Drawing.Point(140, 45);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(44, 23);
		this.textBox1.TabIndex = 18;
		this.textBox1.Text = "1";
		this.label51.AutoSize = true;
		this.label51.ForeColor = System.Drawing.Color.Black;
		this.label51.Location = new System.Drawing.Point(138, 19);
		this.label51.Name = "label51";
		this.label51.Size = new System.Drawing.Size(56, 17);
		this.label51.TabIndex = 17;
		this.label51.Text = "物品数量";
		this.time1.Location = new System.Drawing.Point(74, 45);
		this.time1.Name = "time1";
		this.time1.Size = new System.Drawing.Size(60, 23);
		this.time1.TabIndex = 16;
		this.time1.Text = "5";
		this.tempid1.Location = new System.Drawing.Point(75, 16);
		this.tempid1.Name = "tempid1";
		this.tempid1.Size = new System.Drawing.Size(59, 23);
		this.tempid1.TabIndex = 15;
		this.SendTime.BackColor = System.Drawing.Color.White;
		this.SendTime.Font = new System.Drawing.Font("微软雅黑", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
		this.SendTime.ForeColor = System.Drawing.Color.Violet;
		this.SendTime.Location = new System.Drawing.Point(12, 71);
		this.SendTime.Name = "SendTime";
		this.SendTime.Size = new System.Drawing.Size(171, 24);
		this.SendTime.TabIndex = 14;
		this.SendTime.Text = "泡点执行成功 0 次";
		this.SendTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
		this.SendTime.Visible = false;
		this.label50.AutoSize = true;
		this.label50.ForeColor = System.Drawing.Color.Black;
		this.label50.Location = new System.Drawing.Point(6, 48);
		this.label50.Name = "label50";
		this.label50.Size = new System.Drawing.Size(64, 17);
		this.label50.TabIndex = 3;
		this.label50.Text = "时间(分钟)";
		this.label49.AutoSize = true;
		this.label49.ForeColor = System.Drawing.Color.Black;
		this.label49.Location = new System.Drawing.Point(6, 19);
		this.label49.Name = "label49";
		this.label49.Size = new System.Drawing.Size(69, 17);
		this.label49.TabIndex = 1;
		this.label49.Text = "泡点物品ID";
		this.panel1.Controls.Add(this.label48);
		this.panel1.Controls.Add(this.label47);
		this.panel1.Controls.Add(this.label46);
		this.panel1.Controls.Add(this.time);
		this.panel1.Controls.Add(this.ShutDown);
		this.panel1.Controls.Add(this.ReStart);
		this.panel1.Controls.Add(this.label45);
		this.panel1.Controls.Add(this.label44);
		this.panel1.Location = new System.Drawing.Point(6, 6);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(603, 47);
		this.panel1.TabIndex = 18;
		this.label48.AutoSize = true;
		this.label48.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
		this.label48.ForeColor = System.Drawing.SystemColors.Highlight;
		this.label48.Location = new System.Drawing.Point(461, 23);
		this.label48.Name = "label48";
		this.label48.Size = new System.Drawing.Size(134, 17);
		this.label48.TabIndex = 17;
		this.label48.Text = "距离关闭还有：10 分钟";
		this.label48.Visible = false;
		this.label47.AutoSize = true;
		this.label47.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
		this.label47.ForeColor = System.Drawing.Color.Green;
		this.label47.Location = new System.Drawing.Point(551, 3);
		this.label47.Name = "label47";
		this.label47.Size = new System.Drawing.Size(44, 17);
		this.label47.TabIndex = 16;
		this.label47.Text = "工作中";
		this.label46.AutoSize = true;
		this.label46.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
		this.label46.Location = new System.Drawing.Point(461, 6);
		this.label46.Name = "label46";
		this.label46.Size = new System.Drawing.Size(80, 17);
		this.label46.TabIndex = 15;
		this.label46.Text = "服务器状态：";
		this.time.Font = new System.Drawing.Font("微软雅黑", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.time.Location = new System.Drawing.Point(408, 6);
		this.time.Name = "time";
		this.time.Size = new System.Drawing.Size(47, 34);
		this.time.TabIndex = 14;
		this.time.Text = "10";
		this.time.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.ShutDown.Font = new System.Drawing.Font("微软雅黑", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
		this.ShutDown.Location = new System.Drawing.Point(327, 6);
		this.ShutDown.Name = "ShutDown";
		this.ShutDown.Size = new System.Drawing.Size(75, 34);
		this.ShutDown.TabIndex = 3;
		this.ShutDown.Text = "关 闭";
		this.ShutDown.UseVisualStyleBackColor = true;
		this.ShutDown.Click += new System.EventHandler(ShutDown_Click);
		this.ReStart.Enabled = false;
		this.ReStart.Font = new System.Drawing.Font("微软雅黑", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
		this.ReStart.Location = new System.Drawing.Point(246, 6);
		this.ReStart.Name = "ReStart";
		this.ReStart.Size = new System.Drawing.Size(75, 34);
		this.ReStart.TabIndex = 2;
		this.ReStart.Text = "重 启";
		this.ReStart.UseVisualStyleBackColor = true;
		this.ReStart.Click += new System.EventHandler(ReStart_Click);
		this.label45.AutoSize = true;
		this.label45.Font = new System.Drawing.Font("微软雅黑", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
		this.label45.Location = new System.Drawing.Point(98, 11);
		this.label45.Name = "label45";
		this.label45.Size = new System.Drawing.Size(49, 26);
		this.label45.TabIndex = 1;
		this.label45.Text = "0 人";
		this.label44.AutoSize = true;
		this.label44.Font = new System.Drawing.Font("微软雅黑", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
		this.label44.Location = new System.Drawing.Point(3, 11);
		this.label44.Name = "label44";
		this.label44.Size = new System.Drawing.Size(107, 26);
		this.label44.TabIndex = 0;
		this.label44.Text = "在线人数：";
		this.groupBox14.Controls.Add(this.SendAnnouncement);
		this.groupBox14.Controls.Add(this.richTextBox1);
		this.groupBox14.Controls.Add(this.ListType);
		this.groupBox14.Controls.Add(this.label43);
		this.groupBox14.Location = new System.Drawing.Point(6, 378);
		this.groupBox14.Name = "groupBox14";
		this.groupBox14.Size = new System.Drawing.Size(472, 53);
		this.groupBox14.TabIndex = 17;
		this.groupBox14.TabStop = false;
		this.groupBox14.Text = "公告";
		this.SendAnnouncement.Location = new System.Drawing.Point(391, 19);
		this.SendAnnouncement.Name = "SendAnnouncement";
		this.SendAnnouncement.Size = new System.Drawing.Size(75, 23);
		this.SendAnnouncement.TabIndex = 11;
		this.SendAnnouncement.Text = "发送公告";
		this.SendAnnouncement.UseVisualStyleBackColor = true;
		this.SendAnnouncement.Click += new System.EventHandler(SendAnnouncement_Click);
		this.richTextBox1.Location = new System.Drawing.Point(136, 19);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.Size = new System.Drawing.Size(249, 23);
		this.richTextBox1.TabIndex = 10;
		this.ListType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.ListType.FormattingEnabled = true;
		this.ListType.Items.AddRange(new object[4] { "屏幕提示", "弹出框", "黄色", "紫色" });
		this.ListType.Location = new System.Drawing.Point(44, 19);
		this.ListType.Name = "ListType";
		this.ListType.Size = new System.Drawing.Size(86, 25);
		this.ListType.TabIndex = 9;
		this.label43.AutoSize = true;
		this.label43.Location = new System.Drawing.Point(6, 22);
		this.label43.Name = "label43";
		this.label43.Size = new System.Drawing.Size(32, 17);
		this.label43.TabIndex = 0;
		this.label43.Text = "类型";
		this.playerBox.Controls.Add(this.button1);
		this.playerBox.Controls.Add(this.Ban);
		this.playerBox.Controls.Add(this.Kick);
		this.playerBox.Controls.Add(this.dateTimePicker1);
		this.playerBox.Controls.Add(this.label1);
		this.playerBox.Location = new System.Drawing.Point(278, 59);
		this.playerBox.Name = "playerBox";
		this.playerBox.Size = new System.Drawing.Size(200, 100);
		this.playerBox.TabIndex = 16;
		this.playerBox.TabStop = false;
		this.playerBox.Text = "选择一名玩家";
		this.button1.Location = new System.Drawing.Point(128, 69);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(59, 23);
		this.button1.TabIndex = 5;
		this.button1.Text = "重置";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.Ban.Location = new System.Drawing.Point(68, 69);
		this.Ban.Name = "Ban";
		this.Ban.Size = new System.Drawing.Size(54, 23);
		this.Ban.TabIndex = 4;
		this.Ban.Text = "封禁";
		this.Ban.UseVisualStyleBackColor = true;
		this.Ban.Click += new System.EventHandler(Ban_Click);
		this.Kick.Location = new System.Drawing.Point(6, 69);
		this.Kick.Name = "Kick";
		this.Kick.Size = new System.Drawing.Size(53, 23);
		this.Kick.TabIndex = 3;
		this.Kick.Text = "踢出";
		this.Kick.UseVisualStyleBackColor = true;
		this.Kick.Click += new System.EventHandler(Kick_Click);
		this.dateTimePicker1.Location = new System.Drawing.Point(6, 40);
		this.dateTimePicker1.Name = "dateTimePicker1";
		this.dateTimePicker1.Size = new System.Drawing.Size(181, 23);
		this.dateTimePicker1.TabIndex = 2;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(3, 19);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(56, 17);
		this.label1.TabIndex = 1;
		this.label1.Text = "封禁日期";
		this.groupBox2.Controls.Add(this.UpdateLogs);
		this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
		this.groupBox2.Location = new System.Drawing.Point(278, 303);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(200, 69);
		this.groupBox2.TabIndex = 15;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "更新日志";
		this.UpdateLogs.BackColor = System.Drawing.SystemColors.Window;
		this.UpdateLogs.Enabled = false;
		this.UpdateLogs.Location = new System.Drawing.Point(9, 20);
		this.UpdateLogs.Multiline = true;
		this.UpdateLogs.Name = "UpdateLogs";
		this.UpdateLogs.ReadOnly = true;
		this.UpdateLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.UpdateLogs.Size = new System.Drawing.Size(185, 43);
		this.UpdateLogs.TabIndex = 0;
		this.UpdateLogs.Text = "1、更新全新GM工具\r\n2、优化算法";
		this.groupBox1.Controls.Add(this.ListOnlinePlayers);
		this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.groupBox1.Location = new System.Drawing.Point(6, 59);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(266, 313);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "在线玩家列表";
		this.ListOnlinePlayers.BackColor = System.Drawing.SystemColors.Window;
		this.ListOnlinePlayers.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.ListOnlinePlayers.Font = new System.Drawing.Font("微软雅黑", 15.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.ListOnlinePlayers.FormattingEnabled = true;
		this.ListOnlinePlayers.ItemHeight = 28;
		this.ListOnlinePlayers.Items.AddRange(new object[10] { "Player1", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7", "Player8", "Player9", "Player10" });
		this.ListOnlinePlayers.Location = new System.Drawing.Point(6, 20);
		this.ListOnlinePlayers.Name = "ListOnlinePlayers";
		this.ListOnlinePlayers.Size = new System.Drawing.Size(250, 280);
		this.ListOnlinePlayers.TabIndex = 0;
		this.ListOnlinePlayers.SelectedIndexChanged += new System.EventHandler(ListOnlinePlayers_SelectedIndexChanged);
		this.tabPage2.Controls.Add(this.groupBox17);
		this.tabPage2.Controls.Add(this.groupBox8);
		this.tabPage2.Controls.Add(this.groupBox19);
		this.tabPage2.Controls.Add(this.groupBox13);
		this.tabPage2.Controls.Add(this.groupBox4);
		this.tabPage2.Controls.Add(this.NewPlyaerFightPowerRank);
		this.tabPage2.Controls.Add(this.groupBox5);
		this.tabPage2.Controls.Add(this.groupBox6);
		this.tabPage2.Controls.Add(this.groupBox7);
		this.tabPage2.Location = new System.Drawing.Point(4, 26);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(779, 435);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "分组2";
		this.tabPage2.UseVisualStyleBackColor = true;
		this.groupBox8.Controls.Add(this.button28);
		this.groupBox8.Controls.Add(this.button27);
		this.groupBox8.Controls.Add(this.button26);
		this.groupBox8.Controls.Add(this.button25);
		this.groupBox8.Controls.Add(this.button24);
		this.groupBox8.Controls.Add(this.button23);
		this.groupBox8.Controls.Add(this.button22);
		this.groupBox8.Controls.Add(this.button21);
		this.groupBox8.Controls.Add(this.button20);
		this.groupBox8.Controls.Add(this.button19);
		this.groupBox8.Controls.Add(this.button18);
		this.groupBox8.Controls.Add(this.button17);
		this.groupBox8.Controls.Add(this.button16);
		this.groupBox8.Controls.Add(this.button15);
		this.groupBox8.Controls.Add(this.button14);
		this.groupBox8.Controls.Add(this.button13);
		this.groupBox8.Controls.Add(this.button12);
		this.groupBox8.Controls.Add(this.button11);
		this.groupBox8.Controls.Add(this.button6);
		this.groupBox8.Controls.Add(this.button5);
		this.groupBox8.Location = new System.Drawing.Point(537, 6);
		this.groupBox8.Name = "groupBox8";
		this.groupBox8.Size = new System.Drawing.Size(236, 230);
		this.groupBox8.TabIndex = 27;
		this.groupBox8.TabStop = false;
		this.groupBox8.Text = "一键满级";
		this.button28.Location = new System.Drawing.Point(158, 167);
		this.button28.Name = "button28";
		this.button28.Size = new System.Drawing.Size(72, 23);
		this.button28.TabIndex = 19;
		this.button28.Text = "战魂满级";
		this.button28.UseVisualStyleBackColor = true;
		this.button28.Click += new System.EventHandler(button28_Click);
		this.button27.Location = new System.Drawing.Point(84, 196);
		this.button27.Name = "button27";
		this.button27.Size = new System.Drawing.Size(72, 23);
		this.button27.TabIndex = 18;
		this.button27.Text = "宠物萌化";
		this.button27.UseVisualStyleBackColor = true;
		this.button27.Click += new System.EventHandler(button27_Click);
		this.button26.Location = new System.Drawing.Point(6, 196);
		this.button26.Name = "button26";
		this.button26.Size = new System.Drawing.Size(72, 23);
		this.button26.TabIndex = 17;
		this.button26.Text = "宠物进化";
		this.button26.UseVisualStyleBackColor = true;
		this.button26.Click += new System.EventHandler(button26_Click);
		this.button25.Location = new System.Drawing.Point(158, 138);
		this.button25.Name = "button25";
		this.button25.Size = new System.Drawing.Size(72, 23);
		this.button25.TabIndex = 16;
		this.button25.Text = "打造满级";
		this.button25.UseVisualStyleBackColor = true;
		this.button25.Click += new System.EventHandler(button25_Click);
		this.button24.Location = new System.Drawing.Point(82, 167);
		this.button24.Name = "button24";
		this.button24.Size = new System.Drawing.Size(72, 23);
		this.button24.TabIndex = 15;
		this.button24.Text = "项链精铸";
		this.button24.UseVisualStyleBackColor = true;
		this.button24.Click += new System.EventHandler(button24_Click);
		this.button23.Location = new System.Drawing.Point(6, 167);
		this.button23.Name = "button23";
		this.button23.Size = new System.Drawing.Size(72, 23);
		this.button23.TabIndex = 14;
		this.button23.Text = "项链炼化";
		this.button23.UseVisualStyleBackColor = true;
		this.button23.Click += new System.EventHandler(button23_Click);
		this.button22.Location = new System.Drawing.Point(158, 109);
		this.button22.Name = "button22";
		this.button22.Size = new System.Drawing.Size(72, 23);
		this.button22.TabIndex = 13;
		this.button22.Text = "启灵.武器";
		this.button22.UseVisualStyleBackColor = true;
		this.button22.Click += new System.EventHandler(button22_Click);
		this.button21.Location = new System.Drawing.Point(158, 80);
		this.button21.Name = "button21";
		this.button21.Size = new System.Drawing.Size(72, 23);
		this.button21.TabIndex = 12;
		this.button21.Text = "启灵.衣服";
		this.button21.UseVisualStyleBackColor = true;
		this.button21.Click += new System.EventHandler(button21_Click);
		this.button20.Location = new System.Drawing.Point(158, 51);
		this.button20.Name = "button20";
		this.button20.Size = new System.Drawing.Size(72, 23);
		this.button20.TabIndex = 11;
		this.button20.Text = "启灵.帽子";
		this.button20.UseVisualStyleBackColor = true;
		this.button20.Click += new System.EventHandler(button20_Click);
		this.button19.Location = new System.Drawing.Point(82, 138);
		this.button19.Name = "button19";
		this.button19.Size = new System.Drawing.Size(72, 23);
		this.button19.TabIndex = 10;
		this.button19.Text = "修炼.二级";
		this.button19.UseVisualStyleBackColor = true;
		this.button19.Click += new System.EventHandler(button19_Click);
		this.button18.Location = new System.Drawing.Point(6, 138);
		this.button18.Name = "button18";
		this.button18.Size = new System.Drawing.Size(72, 23);
		this.button18.TabIndex = 9;
		this.button18.Text = "修炼.普通";
		this.button18.UseVisualStyleBackColor = true;
		this.button18.Click += new System.EventHandler(button18_Click);
		this.button17.Location = new System.Drawing.Point(82, 51);
		this.button17.Name = "button17";
		this.button17.Size = new System.Drawing.Size(72, 23);
		this.button17.TabIndex = 8;
		this.button17.Text = "卡牌图册";
		this.button17.UseVisualStyleBackColor = true;
		this.button17.Click += new System.EventHandler(button17_Click);
		this.button16.Location = new System.Drawing.Point(6, 51);
		this.button16.Name = "button16";
		this.button16.Size = new System.Drawing.Size(72, 23);
		this.button16.TabIndex = 7;
		this.button16.Text = "卡牌满级";
		this.button16.UseVisualStyleBackColor = true;
		this.button16.Click += new System.EventHandler(button16_Click);
		this.button15.Location = new System.Drawing.Point(84, 109);
		this.button15.Name = "button15";
		this.button15.Size = new System.Drawing.Size(72, 23);
		this.button15.TabIndex = 6;
		this.button15.Text = "神庙升华";
		this.button15.UseVisualStyleBackColor = true;
		this.button15.Click += new System.EventHandler(button15_Click_1);
		this.button14.Location = new System.Drawing.Point(6, 109);
		this.button14.Name = "button14";
		this.button14.Size = new System.Drawing.Size(72, 23);
		this.button14.TabIndex = 5;
		this.button14.Text = "神庙祝福";
		this.button14.UseVisualStyleBackColor = true;
		this.button14.Click += new System.EventHandler(button14_Click);
		this.button13.Location = new System.Drawing.Point(84, 80);
		this.button13.Name = "button13";
		this.button13.Size = new System.Drawing.Size(72, 23);
		this.button13.TabIndex = 4;
		this.button13.Text = "图腾升阶";
		this.button13.UseVisualStyleBackColor = true;
		this.button13.Click += new System.EventHandler(button13_Click);
		this.button12.Location = new System.Drawing.Point(6, 80);
		this.button12.Name = "button12";
		this.button12.Size = new System.Drawing.Size(72, 23);
		this.button12.TabIndex = 3;
		this.button12.Text = "图腾满级";
		this.button12.UseVisualStyleBackColor = true;
		this.button12.Click += new System.EventHandler(button12_Click);
		this.button11.Location = new System.Drawing.Point(82, 22);
		this.button11.Name = "button11";
		this.button11.Size = new System.Drawing.Size(72, 23);
		this.button11.TabIndex = 2;
		this.button11.Text = "坐骑(>12)";
		this.button11.UseVisualStyleBackColor = true;
		this.button11.Click += new System.EventHandler(button11_Click);
		this.button6.Location = new System.Drawing.Point(158, 22);
		this.button6.Name = "button6";
		this.button6.Size = new System.Drawing.Size(72, 23);
		this.button6.TabIndex = 1;
		this.button6.Text = "坐骑技能";
		this.button6.UseVisualStyleBackColor = true;
		this.button6.Click += new System.EventHandler(button6_Click);
		this.button5.Location = new System.Drawing.Point(6, 22);
		this.button5.Name = "button5";
		this.button5.Size = new System.Drawing.Size(72, 23);
		this.button5.TabIndex = 0;
		this.button5.Text = "坐骑(<12)";
		this.button5.UseVisualStyleBackColor = true;
		this.button5.Click += new System.EventHandler(button5_Click_1);
		this.groupBox19.Controls.Add(this.ChangePassword);
		this.groupBox19.Controls.Add(this.Password);
		this.groupBox19.Controls.Add(this.label66);
		this.groupBox19.Location = new System.Drawing.Point(279, 48);
		this.groupBox19.Name = "groupBox19";
		this.groupBox19.Size = new System.Drawing.Size(252, 55);
		this.groupBox19.TabIndex = 26;
		this.groupBox19.TabStop = false;
		this.groupBox19.Text = "修改玩家密码";
		this.ChangePassword.Location = new System.Drawing.Point(169, 22);
		this.ChangePassword.Name = "ChangePassword";
		this.ChangePassword.Size = new System.Drawing.Size(76, 23);
		this.ChangePassword.TabIndex = 3;
		this.ChangePassword.Text = "修改密码";
		this.ChangePassword.UseVisualStyleBackColor = true;
		this.ChangePassword.Click += new System.EventHandler(ChangePassword_Click);
		this.Password.Location = new System.Drawing.Point(56, 22);
		this.Password.Name = "Password";
		this.Password.Size = new System.Drawing.Size(107, 23);
		this.Password.TabIndex = 1;
		this.label66.AutoSize = true;
		this.label66.Location = new System.Drawing.Point(9, 25);
		this.label66.Name = "label66";
		this.label66.Size = new System.Drawing.Size(44, 17);
		this.label66.TabIndex = 0;
		this.label66.Text = "密码：";
		this.groupBox13.Controls.Add(this.CleanAcitveNum_Btn);
		this.groupBox13.Controls.Add(this.ListActiveID_CBox);
		this.groupBox13.Controls.Add(this.label42);
		this.groupBox13.Location = new System.Drawing.Point(277, 107);
		this.groupBox13.Name = "groupBox13";
		this.groupBox13.Size = new System.Drawing.Size(253, 55);
		this.groupBox13.TabIndex = 25;
		this.groupBox13.TabStop = false;
		this.groupBox13.Text = "定时清理每日激活码";
		this.CleanAcitveNum_Btn.Location = new System.Drawing.Point(170, 22);
		this.CleanAcitveNum_Btn.Name = "CleanAcitveNum_Btn";
		this.CleanAcitveNum_Btn.Size = new System.Drawing.Size(76, 23);
		this.CleanAcitveNum_Btn.TabIndex = 2;
		this.CleanAcitveNum_Btn.Text = "定时清理";
		this.CleanAcitveNum_Btn.UseVisualStyleBackColor = true;
		this.CleanAcitveNum_Btn.Click += new System.EventHandler(CleanAcitveNum_Btn_Click);
		this.ListActiveID_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.ListActiveID_CBox.FormattingEnabled = true;
		this.ListActiveID_CBox.Location = new System.Drawing.Point(57, 20);
		this.ListActiveID_CBox.Name = "ListActiveID_CBox";
		this.ListActiveID_CBox.Size = new System.Drawing.Size(107, 25);
		this.ListActiveID_CBox.TabIndex = 1;
		this.label42.AutoSize = true;
		this.label42.Location = new System.Drawing.Point(10, 23);
		this.label42.Name = "label42";
		this.label42.Size = new System.Drawing.Size(51, 17);
		this.label42.TabIndex = 0;
		this.label42.Text = "活动1：";
		this.groupBox4.Controls.Add(this.FindUser_Btn);
		this.groupBox4.Controls.Add(this.UserName_Text);
		this.groupBox4.Controls.Add(this.label2);
		this.groupBox4.Location = new System.Drawing.Point(6, 6);
		this.groupBox4.Name = "groupBox4";
		this.groupBox4.Size = new System.Drawing.Size(266, 51);
		this.groupBox4.TabIndex = 7;
		this.groupBox4.TabStop = false;
		this.groupBox4.Text = "玩家查询";
		this.FindUser_Btn.Location = new System.Drawing.Point(183, 19);
		this.FindUser_Btn.Name = "FindUser_Btn";
		this.FindUser_Btn.Size = new System.Drawing.Size(75, 23);
		this.FindUser_Btn.TabIndex = 2;
		this.FindUser_Btn.Text = "查询玩家";
		this.FindUser_Btn.UseVisualStyleBackColor = true;
		this.FindUser_Btn.Click += new System.EventHandler(FindUser_Btn_Click);
		this.UserName_Text.Location = new System.Drawing.Point(77, 20);
		this.UserName_Text.Name = "UserName_Text";
		this.UserName_Text.Size = new System.Drawing.Size(100, 23);
		this.UserName_Text.TabIndex = 1;
		this.UserName_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(6, 26);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(68, 17);
		this.label2.TabIndex = 0;
		this.label2.Text = "输入用户名";
		this.NewPlyaerFightPowerRank.AllowUserToAddRows = false;
		this.NewPlyaerFightPowerRank.AllowUserToDeleteRows = false;
		this.NewPlyaerFightPowerRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.NewPlyaerFightPowerRank.Columns.AddRange(this.Column2, this.Column3, this.Column4);
		this.NewPlyaerFightPowerRank.Location = new System.Drawing.Point(258, 220);
		this.NewPlyaerFightPowerRank.Name = "NewPlyaerFightPowerRank";
		this.NewPlyaerFightPowerRank.ReadOnly = true;
		this.NewPlyaerFightPowerRank.RowHeadersWidth = 51;
		this.NewPlyaerFightPowerRank.RowTemplate.Height = 23;
		this.NewPlyaerFightPowerRank.Size = new System.Drawing.Size(273, 156);
		this.NewPlyaerFightPowerRank.TabIndex = 13;
		this.Column2.DataPropertyName = "UserName";
		this.Column2.HeaderText = "账号";
		this.Column2.MinimumWidth = 6;
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column2.Width = 69;
		this.Column3.DataPropertyName = "NickName";
		this.Column3.HeaderText = "昵称";
		this.Column3.MinimumWidth = 6;
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		this.Column3.Width = 69;
		this.Column4.DataPropertyName = "FightPower";
		this.Column4.HeaderText = "战力";
		this.Column4.MinimumWidth = 6;
		this.Column4.Name = "Column4";
		this.Column4.ReadOnly = true;
		this.Column4.Width = 70;
		this.groupBox5.Controls.Add(this.button3);
		this.groupBox5.Controls.Add(this.comboBox1);
		this.groupBox5.Controls.Add(this.label4);
		this.groupBox5.Controls.Add(this.RecoverMessage);
		this.groupBox5.Controls.Add(this.PasswordTwo_Text);
		this.groupBox5.Controls.Add(this.label15);
		this.groupBox5.Controls.Add(this.CleanUser_Text);
		this.groupBox5.Controls.Add(this.UpdateLevel_Btn);
		this.groupBox5.Controls.Add(this.Level_Text);
		this.groupBox5.Controls.Add(this.NickName_Text);
		this.groupBox5.Controls.Add(this.UpUserData_Btn);
		this.groupBox5.Controls.Add(this.label3);
		this.groupBox5.Controls.Add(this.label5);
		this.groupBox5.Location = new System.Drawing.Point(6, 63);
		this.groupBox5.Name = "groupBox5";
		this.groupBox5.Size = new System.Drawing.Size(266, 151);
		this.groupBox5.TabIndex = 8;
		this.groupBox5.TabStop = false;
		this.groupBox5.Text = "玩家信息";
		this.button3.Location = new System.Drawing.Point(214, 104);
		this.button3.Name = "button3";
		this.button3.Size = new System.Drawing.Size(44, 23);
		this.button3.TabIndex = 27;
		this.button3.Text = "删除";
		this.button3.UseVisualStyleBackColor = true;
		this.button3.Click += new System.EventHandler(button3_Click);
		this.comboBox1.FormattingEnabled = true;
		this.comboBox1.Items.AddRange(new object[2] { "粗糙品质", "普通品质" });
		this.comboBox1.Location = new System.Drawing.Point(126, 104);
		this.comboBox1.Name = "comboBox1";
		this.comboBox1.Size = new System.Drawing.Size(83, 25);
		this.comboBox1.TabIndex = 27;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(6, 107);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(116, 17);
		this.label4.TabIndex = 27;
		this.label4.Text = "需要删除的刻印品质";
		this.RecoverMessage.Location = new System.Drawing.Point(6, 76);
		this.RecoverMessage.Name = "RecoverMessage";
		this.RecoverMessage.Size = new System.Drawing.Size(133, 23);
		this.RecoverMessage.TabIndex = 29;
		this.RecoverMessage.Text = "恢复玩家误删邮件";
		this.RecoverMessage.UseVisualStyleBackColor = true;
		this.RecoverMessage.Click += new System.EventHandler(RecoverMessage_Click);
		this.PasswordTwo_Text.Location = new System.Drawing.Point(188, 20);
		this.PasswordTwo_Text.Name = "PasswordTwo_Text";
		this.PasswordTwo_Text.Size = new System.Drawing.Size(70, 23);
		this.PasswordTwo_Text.TabIndex = 9;
		this.PasswordTwo_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label15.AutoSize = true;
		this.label15.Location = new System.Drawing.Point(123, 23);
		this.label15.Name = "label15";
		this.label15.Size = new System.Drawing.Size(68, 17);
		this.label15.TabIndex = 8;
		this.label15.Text = "二级密码：";
		this.CleanUser_Text.Location = new System.Drawing.Point(64, 47);
		this.CleanUser_Text.Name = "CleanUser_Text";
		this.CleanUser_Text.Size = new System.Drawing.Size(52, 23);
		this.CleanUser_Text.TabIndex = 7;
		this.CleanUser_Text.Text = "清空";
		this.CleanUser_Text.UseVisualStyleBackColor = true;
		this.CleanUser_Text.Click += new System.EventHandler(CleanUser_Text_Click);
		this.UpdateLevel_Btn.Location = new System.Drawing.Point(145, 76);
		this.UpdateLevel_Btn.Name = "UpdateLevel_Btn";
		this.UpdateLevel_Btn.Size = new System.Drawing.Size(113, 23);
		this.UpdateLevel_Btn.TabIndex = 6;
		this.UpdateLevel_Btn.Text = "修改等级";
		this.UpdateLevel_Btn.UseVisualStyleBackColor = true;
		this.UpdateLevel_Btn.Click += new System.EventHandler(UpdateLevel_Btn_Click);
		this.Level_Text.Location = new System.Drawing.Point(188, 47);
		this.Level_Text.Name = "Level_Text";
		this.Level_Text.Size = new System.Drawing.Size(70, 23);
		this.Level_Text.TabIndex = 5;
		this.Level_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.NickName_Text.Location = new System.Drawing.Point(44, 20);
		this.NickName_Text.Name = "NickName_Text";
		this.NickName_Text.Size = new System.Drawing.Size(72, 23);
		this.NickName_Text.TabIndex = 4;
		this.NickName_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.UpUserData_Btn.Location = new System.Drawing.Point(6, 47);
		this.UpUserData_Btn.Name = "UpUserData_Btn";
		this.UpUserData_Btn.Size = new System.Drawing.Size(52, 23);
		this.UpUserData_Btn.TabIndex = 3;
		this.UpUserData_Btn.Text = "修改";
		this.UpUserData_Btn.UseVisualStyleBackColor = true;
		this.UpUserData_Btn.Click += new System.EventHandler(UpUserData_Btn_Click);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(123, 50);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(68, 17);
		this.label3.TabIndex = 1;
		this.label3.Text = "玩家等级：";
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(6, 23);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(44, 17);
		this.label5.TabIndex = 0;
		this.label5.Text = "昵称：";
		this.groupBox6.Controls.Add(this.FindRank_Btn);
		this.groupBox6.Controls.Add(this.EndTime_TimeDate);
		this.groupBox6.Controls.Add(this.StartTime_TimeDate);
		this.groupBox6.Controls.Add(this.label17);
		this.groupBox6.Controls.Add(this.label16);
		this.groupBox6.Location = new System.Drawing.Point(6, 214);
		this.groupBox6.Name = "groupBox6";
		this.groupBox6.Size = new System.Drawing.Size(246, 162);
		this.groupBox6.TabIndex = 12;
		this.groupBox6.TabStop = false;
		this.groupBox6.Text = "新手排名";
		this.FindRank_Btn.Location = new System.Drawing.Point(64, 121);
		this.FindRank_Btn.Name = "FindRank_Btn";
		this.FindRank_Btn.Size = new System.Drawing.Size(75, 23);
		this.FindRank_Btn.TabIndex = 4;
		this.FindRank_Btn.Text = "查询排名";
		this.FindRank_Btn.UseVisualStyleBackColor = true;
		this.FindRank_Btn.Click += new System.EventHandler(FindRank_Btn_Click);
		this.EndTime_TimeDate.Location = new System.Drawing.Point(94, 76);
		this.EndTime_TimeDate.Name = "EndTime_TimeDate";
		this.EndTime_TimeDate.Size = new System.Drawing.Size(115, 23);
		this.EndTime_TimeDate.TabIndex = 3;
		this.StartTime_TimeDate.Location = new System.Drawing.Point(94, 33);
		this.StartTime_TimeDate.Name = "StartTime_TimeDate";
		this.StartTime_TimeDate.Size = new System.Drawing.Size(115, 23);
		this.StartTime_TimeDate.TabIndex = 2;
		this.label17.AutoSize = true;
		this.label17.Location = new System.Drawing.Point(11, 82);
		this.label17.Name = "label17";
		this.label17.Size = new System.Drawing.Size(56, 17);
		this.label17.TabIndex = 1;
		this.label17.Text = "结束时间";
		this.label16.AutoSize = true;
		this.label16.Location = new System.Drawing.Point(11, 39);
		this.label16.Name = "label16";
		this.label16.Size = new System.Drawing.Size(56, 17);
		this.label16.TabIndex = 0;
		this.label16.Text = "注册时间";
		this.groupBox7.Controls.Add(this.UserID_Lab);
		this.groupBox7.Controls.Add(this.Label_d);
		this.groupBox7.Controls.Add(this.Online_Text);
		this.groupBox7.Controls.Add(this.label6);
		this.groupBox7.Location = new System.Drawing.Point(278, 6);
		this.groupBox7.Name = "groupBox7";
		this.groupBox7.Size = new System.Drawing.Size(252, 42);
		this.groupBox7.TabIndex = 9;
		this.groupBox7.TabStop = false;
		this.groupBox7.Text = "玩家状态";
		this.UserID_Lab.AutoSize = true;
		this.UserID_Lab.Location = new System.Drawing.Point(82, 19);
		this.UserID_Lab.Name = "UserID_Lab";
		this.UserID_Lab.Size = new System.Drawing.Size(15, 17);
		this.UserID_Lab.TabIndex = 3;
		this.UserID_Lab.Text = "0";
		this.Label_d.AutoSize = true;
		this.Label_d.Location = new System.Drawing.Point(26, 19);
		this.Label_d.Name = "Label_d";
		this.Label_d.Size = new System.Drawing.Size(57, 17);
		this.Label_d.TabIndex = 2;
		this.Label_d.Text = "玩家ID：";
		this.Online_Text.AutoSize = true;
		this.Online_Text.ForeColor = System.Drawing.Color.Red;
		this.Online_Text.Location = new System.Drawing.Point(200, 19);
		this.Online_Text.Name = "Online_Text";
		this.Online_Text.Size = new System.Drawing.Size(32, 17);
		this.Online_Text.TabIndex = 1;
		this.Online_Text.Text = "离线";
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(129, 19);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(68, 17);
		this.label6.TabIndex = 0;
		this.label6.Text = "在线状态：";
		this.tabPage3.Controls.Add(this.groupBox20);
		this.tabPage3.Controls.Add(this.groupBox18);
		this.tabPage3.Controls.Add(this.groupBox10);
		this.tabPage3.Controls.Add(this.groupBox11);
		this.tabPage3.Controls.Add(this.groupBox12);
		this.tabPage3.Controls.Add(this.groupBox3);
		this.tabPage3.Location = new System.Drawing.Point(4, 26);
		this.tabPage3.Name = "tabPage3";
		this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage3.Size = new System.Drawing.Size(779, 435);
		this.tabPage3.TabIndex = 2;
		this.tabPage3.Text = "分组3";
		this.tabPage3.UseVisualStyleBackColor = true;
		this.groupBox20.Controls.Add(this.button4);
		this.groupBox20.Controls.Add(this.CreateAllCeleb);
		this.groupBox20.Controls.Add(this.Reload);
		this.groupBox20.Location = new System.Drawing.Point(542, 6);
		this.groupBox20.Name = "groupBox20";
		this.groupBox20.Size = new System.Drawing.Size(231, 54);
		this.groupBox20.TabIndex = 32;
		this.groupBox20.TabStop = false;
		this.groupBox20.Text = "热更新";
		this.button4.Location = new System.Drawing.Point(79, 22);
		this.button4.Name = "button4";
		this.button4.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.button4.Size = new System.Drawing.Size(79, 23);
		this.button4.TabIndex = 2;
		this.button4.Text = "世界BOSS";
		this.button4.UseVisualStyleBackColor = true;
		this.button4.Click += new System.EventHandler(button4_Click);
		this.CreateAllCeleb.Location = new System.Drawing.Point(164, 22);
		this.CreateAllCeleb.Name = "CreateAllCeleb";
		this.CreateAllCeleb.Size = new System.Drawing.Size(61, 23);
		this.CreateAllCeleb.TabIndex = 1;
		this.CreateAllCeleb.Text = "名人堂";
		this.CreateAllCeleb.UseVisualStyleBackColor = true;
		this.CreateAllCeleb.Click += new System.EventHandler(CreateAllCeleb_Click);
		this.Reload.Location = new System.Drawing.Point(6, 22);
		this.Reload.Name = "Reload";
		this.Reload.Size = new System.Drawing.Size(67, 23);
		this.Reload.TabIndex = 0;
		this.Reload.Text = "立即同步";
		this.Reload.UseVisualStyleBackColor = true;
		this.Reload.Click += new System.EventHandler(Reload_Click);
		this.groupBox18.Controls.Add(this.richTextBox2);
		this.groupBox18.Location = new System.Drawing.Point(542, 67);
		this.groupBox18.Name = "groupBox18";
		this.groupBox18.Size = new System.Drawing.Size(231, 362);
		this.groupBox18.TabIndex = 31;
		this.groupBox18.TabStop = false;
		this.groupBox18.Text = "运行日志";
		this.richTextBox2.AcceptsTab = true;
		this.richTextBox2.BackColor = System.Drawing.Color.White;
		this.richTextBox2.Location = new System.Drawing.Point(6, 22);
		this.richTextBox2.Name = "richTextBox2";
		this.richTextBox2.ReadOnly = true;
		this.richTextBox2.Size = new System.Drawing.Size(219, 334);
		this.richTextBox2.TabIndex = 14;
		this.richTextBox2.Text = "";
		this.groupBox10.Controls.Add(this.NewItemID_TextBox);
		this.groupBox10.Controls.Add(this.label41);
		this.groupBox10.Controls.Add(this.ItemQuality_CBox);
		this.groupBox10.Controls.Add(this.InsertItem_Btn);
		this.groupBox10.Controls.Add(this.ItemLuck_TextBox);
		this.groupBox10.Controls.Add(this.ItemAgi_TextBox);
		this.groupBox10.Controls.Add(this.ItemDef_TextBox);
		this.groupBox10.Controls.Add(this.ItemAtt_TextBox);
		this.groupBox10.Controls.Add(this.ItemDesc_TextBox);
		this.groupBox10.Controls.Add(this.label39);
		this.groupBox10.Controls.Add(this.ItemArmor_Text);
		this.groupBox10.Controls.Add(this.label40);
		this.groupBox10.Controls.Add(this.pictureBox3);
		this.groupBox10.Controls.Add(this.label38);
		this.groupBox10.Controls.Add(this.ItemName_TextBox);
		this.groupBox10.Controls.Add(this.label37);
		this.groupBox10.Controls.Add(this.label36);
		this.groupBox10.Controls.Add(this.label35);
		this.groupBox10.Controls.Add(this.label34);
		this.groupBox10.Controls.Add(this.label33);
		this.groupBox10.Controls.Add(this.label32);
		this.groupBox10.Controls.Add(this.ListItem_CBox);
		this.groupBox10.Controls.Add(this.label31);
		this.groupBox10.Controls.Add(this.ListCategoryID_CBox);
		this.groupBox10.Controls.Add(this.label30);
		this.groupBox10.Location = new System.Drawing.Point(282, 6);
		this.groupBox10.Name = "groupBox10";
		this.groupBox10.Size = new System.Drawing.Size(254, 356);
		this.groupBox10.TabIndex = 28;
		this.groupBox10.TabStop = false;
		this.groupBox10.Text = "装扮定制";
		this.NewItemID_TextBox.Location = new System.Drawing.Point(6, 324);
		this.NewItemID_TextBox.Name = "NewItemID_TextBox";
		this.NewItemID_TextBox.Size = new System.Drawing.Size(235, 23);
		this.NewItemID_TextBox.TabIndex = 34;
		this.label41.AutoSize = true;
		this.label41.Location = new System.Drawing.Point(7, 304);
		this.label41.Name = "label41";
		this.label41.Size = new System.Drawing.Size(105, 17);
		this.label41.TabIndex = 33;
		this.label41.Text = "生成定制装扮ID：";
		this.ItemQuality_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.ItemQuality_CBox.FormattingEnabled = true;
		this.ItemQuality_CBox.Items.AddRange(new object[7] { "粗糙", "普通", "优秀", "精良", "卓越", "传说", "神器" });
		this.ItemQuality_CBox.Location = new System.Drawing.Point(157, 119);
		this.ItemQuality_CBox.Name = "ItemQuality_CBox";
		this.ItemQuality_CBox.Size = new System.Drawing.Size(87, 25);
		this.ItemQuality_CBox.TabIndex = 30;
		this.InsertItem_Btn.Location = new System.Drawing.Point(10, 266);
		this.InsertItem_Btn.Name = "InsertItem_Btn";
		this.InsertItem_Btn.Size = new System.Drawing.Size(235, 23);
		this.InsertItem_Btn.TabIndex = 29;
		this.InsertItem_Btn.Text = "定制装扮";
		this.InsertItem_Btn.UseVisualStyleBackColor = true;
		this.InsertItem_Btn.Click += new System.EventHandler(InsertItem_Btn_Click);
		this.ItemLuck_TextBox.Location = new System.Drawing.Point(166, 237);
		this.ItemLuck_TextBox.Name = "ItemLuck_TextBox";
		this.ItemLuck_TextBox.Size = new System.Drawing.Size(78, 23);
		this.ItemLuck_TextBox.TabIndex = 28;
		this.ItemLuck_TextBox.Text = "2147483648";
		this.ItemLuck_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.ItemAgi_TextBox.Location = new System.Drawing.Point(48, 237);
		this.ItemAgi_TextBox.Name = "ItemAgi_TextBox";
		this.ItemAgi_TextBox.Size = new System.Drawing.Size(78, 23);
		this.ItemAgi_TextBox.TabIndex = 27;
		this.ItemAgi_TextBox.Text = "2147483648";
		this.ItemAgi_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.ItemDef_TextBox.Location = new System.Drawing.Point(166, 208);
		this.ItemDef_TextBox.Name = "ItemDef_TextBox";
		this.ItemDef_TextBox.Size = new System.Drawing.Size(78, 23);
		this.ItemDef_TextBox.TabIndex = 26;
		this.ItemDef_TextBox.Text = "2147483648";
		this.ItemDef_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.ItemAtt_TextBox.Location = new System.Drawing.Point(48, 208);
		this.ItemAtt_TextBox.Name = "ItemAtt_TextBox";
		this.ItemAtt_TextBox.Size = new System.Drawing.Size(78, 23);
		this.ItemAtt_TextBox.TabIndex = 25;
		this.ItemAtt_TextBox.Text = "2147483648";
		this.ItemAtt_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.ItemDesc_TextBox.Location = new System.Drawing.Point(48, 179);
		this.ItemDesc_TextBox.Name = "ItemDesc_TextBox";
		this.ItemDesc_TextBox.Size = new System.Drawing.Size(196, 23);
		this.ItemDesc_TextBox.TabIndex = 24;
		this.ItemDesc_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label39.AutoSize = true;
		this.label39.Location = new System.Drawing.Point(110, 126);
		this.label39.Name = "label39";
		this.label39.Size = new System.Drawing.Size(44, 17);
		this.label39.TabIndex = 23;
		this.label39.Text = "品质：";
		this.ItemArmor_Text.Location = new System.Drawing.Point(157, 88);
		this.ItemArmor_Text.Name = "ItemArmor_Text";
		this.ItemArmor_Text.Size = new System.Drawing.Size(87, 23);
		this.ItemArmor_Text.TabIndex = 22;
		this.ItemArmor_Text.Text = "2147483648";
		this.ItemArmor_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label40.AutoSize = true;
		this.label40.Location = new System.Drawing.Point(110, 91);
		this.label40.Name = "label40";
		this.label40.Size = new System.Drawing.Size(44, 17);
		this.label40.TabIndex = 21;
		this.label40.Text = "护甲：";
		this.pictureBox3.Image = Road.Service.Properties.Resources._404;
		this.pictureBox3.Location = new System.Drawing.Point(48, 88);
		this.pictureBox3.Name = "pictureBox3";
		this.pictureBox3.Size = new System.Drawing.Size(50, 50);
		this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox3.TabIndex = 20;
		this.pictureBox3.TabStop = false;
		this.label38.AutoSize = true;
		this.label38.Location = new System.Drawing.Point(7, 91);
		this.label38.Name = "label38";
		this.label38.Size = new System.Drawing.Size(44, 17);
		this.label38.TabIndex = 13;
		this.label38.Text = "图片：";
		this.ItemName_TextBox.Location = new System.Drawing.Point(48, 150);
		this.ItemName_TextBox.Name = "ItemName_TextBox";
		this.ItemName_TextBox.Size = new System.Drawing.Size(196, 23);
		this.ItemName_TextBox.TabIndex = 12;
		this.ItemName_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label37.AutoSize = true;
		this.label37.Location = new System.Drawing.Point(129, 242);
		this.label37.Name = "label37";
		this.label37.Size = new System.Drawing.Size(44, 17);
		this.label37.TabIndex = 11;
		this.label37.Text = "幸运：";
		this.label36.AutoSize = true;
		this.label36.Location = new System.Drawing.Point(7, 240);
		this.label36.Name = "label36";
		this.label36.Size = new System.Drawing.Size(44, 17);
		this.label36.TabIndex = 10;
		this.label36.Text = "敏捷：";
		this.label35.AutoSize = true;
		this.label35.Location = new System.Drawing.Point(129, 213);
		this.label35.Name = "label35";
		this.label35.Size = new System.Drawing.Size(44, 17);
		this.label35.TabIndex = 9;
		this.label35.Text = "防御：";
		this.label34.AutoSize = true;
		this.label34.Location = new System.Drawing.Point(7, 211);
		this.label34.Name = "label34";
		this.label34.Size = new System.Drawing.Size(44, 17);
		this.label34.TabIndex = 8;
		this.label34.Text = "攻击：";
		this.label33.AutoSize = true;
		this.label33.Location = new System.Drawing.Point(7, 182);
		this.label33.Name = "label33";
		this.label33.Size = new System.Drawing.Size(44, 17);
		this.label33.TabIndex = 7;
		this.label33.Text = "描述：";
		this.label32.AutoSize = true;
		this.label32.Location = new System.Drawing.Point(7, 153);
		this.label32.Name = "label32";
		this.label32.Size = new System.Drawing.Size(44, 17);
		this.label32.TabIndex = 6;
		this.label32.Text = "名称：";
		this.ListItem_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.ListItem_CBox.FormattingEnabled = true;
		this.ListItem_CBox.Location = new System.Drawing.Point(79, 57);
		this.ListItem_CBox.Name = "ListItem_CBox";
		this.ListItem_CBox.Size = new System.Drawing.Size(166, 25);
		this.ListItem_CBox.TabIndex = 5;
		this.ListItem_CBox.SelectedIndexChanged += new System.EventHandler(ListItem_CBox_SelectedIndexChanged);
		this.label31.AutoSize = true;
		this.label31.Location = new System.Drawing.Point(7, 61);
		this.label31.Name = "label31";
		this.label31.Size = new System.Drawing.Size(80, 17);
		this.label31.TabIndex = 4;
		this.label31.Text = "请选择装扮：";
		this.ListCategoryID_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.ListCategoryID_CBox.FormattingEnabled = true;
		this.ListCategoryID_CBox.Items.AddRange(new object[12]
		{
			"1 -- 帽子", "2 -- 眼镜", "3 -- 头发", "4 -- 脸饰", "5 -- 衣服", "6 -- 眼睛", "8 -- 手镯", "9 -- 戒指", "13 -- 套装", "14 -- 项链",
			"15 -- 装饰", "16 -- 泡泡"
		});
		this.ListCategoryID_CBox.Location = new System.Drawing.Point(78, 26);
		this.ListCategoryID_CBox.Name = "ListCategoryID_CBox";
		this.ListCategoryID_CBox.Size = new System.Drawing.Size(166, 25);
		this.ListCategoryID_CBox.TabIndex = 3;
		this.ListCategoryID_CBox.SelectedIndexChanged += new System.EventHandler(ListCategoryID_CBox_SelectedIndexChanged);
		this.label30.AutoSize = true;
		this.label30.Location = new System.Drawing.Point(7, 30);
		this.label30.Name = "label30";
		this.label30.Size = new System.Drawing.Size(80, 17);
		this.label30.TabIndex = 0;
		this.label30.Text = "请选择类型：";
		this.groupBox11.Controls.Add(this.NewTemplateID_Text);
		this.groupBox11.Controls.Add(this.label29);
		this.groupBox11.Controls.Add(this.Special_Text);
		this.groupBox11.Controls.Add(this.CommonAddWound_Text);
		this.groupBox11.Controls.Add(this.CommonMultiBall_Text);
		this.groupBox11.Controls.Add(this.Common_Text);
		this.groupBox11.Controls.Add(this.InsertArm_Btn);
		this.groupBox11.Location = new System.Drawing.Point(6, 230);
		this.groupBox11.Name = "groupBox11";
		this.groupBox11.Size = new System.Drawing.Size(270, 132);
		this.groupBox11.TabIndex = 27;
		this.groupBox11.TabStop = false;
		this.groupBox11.Text = "特效配置";
		this.NewTemplateID_Text.Location = new System.Drawing.Point(10, 100);
		this.NewTemplateID_Text.Name = "NewTemplateID_Text";
		this.NewTemplateID_Text.Size = new System.Drawing.Size(251, 23);
		this.NewTemplateID_Text.TabIndex = 32;
		this.label29.AutoSize = true;
		this.label29.Location = new System.Drawing.Point(10, 82);
		this.label29.Name = "label29";
		this.label29.Size = new System.Drawing.Size(105, 17);
		this.label29.TabIndex = 31;
		this.label29.Text = "生成定制武器ID：";
		this.Special_Text.Location = new System.Drawing.Point(203, 29);
		this.Special_Text.Name = "Special_Text";
		this.Special_Text.Size = new System.Drawing.Size(58, 23);
		this.Special_Text.TabIndex = 30;
		this.Special_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.CommonAddWound_Text.Location = new System.Drawing.Point(75, 29);
		this.CommonAddWound_Text.Name = "CommonAddWound_Text";
		this.CommonAddWound_Text.Size = new System.Drawing.Size(58, 23);
		this.CommonAddWound_Text.TabIndex = 29;
		this.CommonAddWound_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.CommonMultiBall_Text.Location = new System.Drawing.Point(139, 29);
		this.CommonMultiBall_Text.Name = "CommonMultiBall_Text";
		this.CommonMultiBall_Text.Size = new System.Drawing.Size(58, 23);
		this.CommonMultiBall_Text.TabIndex = 28;
		this.CommonMultiBall_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.Common_Text.Location = new System.Drawing.Point(11, 29);
		this.Common_Text.Name = "Common_Text";
		this.Common_Text.Size = new System.Drawing.Size(58, 23);
		this.Common_Text.TabIndex = 27;
		this.Common_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.InsertArm_Btn.Location = new System.Drawing.Point(11, 56);
		this.InsertArm_Btn.Name = "InsertArm_Btn";
		this.InsertArm_Btn.Size = new System.Drawing.Size(250, 23);
		this.InsertArm_Btn.TabIndex = 26;
		this.InsertArm_Btn.Text = "定制武器";
		this.InsertArm_Btn.UseVisualStyleBackColor = true;
		this.InsertArm_Btn.Click += new System.EventHandler(InsertArm_Btn_Click);
		this.groupBox12.Controls.Add(this.label28);
		this.groupBox12.Controls.Add(this.pictureBox2);
		this.groupBox12.Controls.Add(this.NewItemQuality_CBox);
		this.groupBox12.Controls.Add(this.NewItemDamage_Text);
		this.groupBox12.Controls.Add(this.NewItemAgi_Text);
		this.groupBox12.Controls.Add(this.NewItemLuck_Text);
		this.groupBox12.Controls.Add(this.NewItemDef_Text);
		this.groupBox12.Controls.Add(this.NewItemAtt_Text);
		this.groupBox12.Controls.Add(this.NewItemDesc_Text);
		this.groupBox12.Controls.Add(this.NewItemName_Text);
		this.groupBox12.Controls.Add(this.label27);
		this.groupBox12.Controls.Add(this.label26);
		this.groupBox12.Controls.Add(this.label25);
		this.groupBox12.Controls.Add(this.label24);
		this.groupBox12.Controls.Add(this.label23);
		this.groupBox12.Controls.Add(this.label22);
		this.groupBox12.Controls.Add(this.label21);
		this.groupBox12.Controls.Add(this.label20);
		this.groupBox12.Controls.Add(this.ListItemName_CBox);
		this.groupBox12.Controls.Add(this.label19);
		this.groupBox12.Location = new System.Drawing.Point(6, 6);
		this.groupBox12.Name = "groupBox12";
		this.groupBox12.Size = new System.Drawing.Size(270, 222);
		this.groupBox12.TabIndex = 25;
		this.groupBox12.TabStop = false;
		this.groupBox12.Text = "武器定制";
		this.label28.AutoSize = true;
		this.label28.Location = new System.Drawing.Point(137, 87);
		this.label28.Name = "label28";
		this.label28.Size = new System.Drawing.Size(44, 17);
		this.label28.TabIndex = 20;
		this.label28.Text = "品质：";
		this.pictureBox2.Image = Road.Service.Properties.Resources._404;
		this.pictureBox2.Location = new System.Drawing.Point(57, 49);
		this.pictureBox2.Name = "pictureBox2";
		this.pictureBox2.Size = new System.Drawing.Size(50, 50);
		this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox2.TabIndex = 19;
		this.pictureBox2.TabStop = false;
		this.NewItemQuality_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.NewItemQuality_CBox.FormattingEnabled = true;
		this.NewItemQuality_CBox.Items.AddRange(new object[7] { "粗糙", "普通", "优秀", "精良", "卓越", "传说", "神器" });
		this.NewItemQuality_CBox.Location = new System.Drawing.Point(184, 79);
		this.NewItemQuality_CBox.Name = "NewItemQuality_CBox";
		this.NewItemQuality_CBox.Size = new System.Drawing.Size(78, 25);
		this.NewItemQuality_CBox.TabIndex = 26;
		this.NewItemDamage_Text.Location = new System.Drawing.Point(184, 49);
		this.NewItemDamage_Text.Name = "NewItemDamage_Text";
		this.NewItemDamage_Text.Size = new System.Drawing.Size(78, 23);
		this.NewItemDamage_Text.TabIndex = 18;
		this.NewItemDamage_Text.Text = "2147483648";
		this.NewItemDamage_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.NewItemAgi_Text.Location = new System.Drawing.Point(55, 190);
		this.NewItemAgi_Text.Name = "NewItemAgi_Text";
		this.NewItemAgi_Text.Size = new System.Drawing.Size(76, 23);
		this.NewItemAgi_Text.TabIndex = 16;
		this.NewItemAgi_Text.Text = "2147483648";
		this.NewItemAgi_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.NewItemLuck_Text.Location = new System.Drawing.Point(184, 190);
		this.NewItemLuck_Text.Name = "NewItemLuck_Text";
		this.NewItemLuck_Text.Size = new System.Drawing.Size(78, 23);
		this.NewItemLuck_Text.TabIndex = 15;
		this.NewItemLuck_Text.Text = "2147483648";
		this.NewItemLuck_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.NewItemDef_Text.Location = new System.Drawing.Point(184, 163);
		this.NewItemDef_Text.Name = "NewItemDef_Text";
		this.NewItemDef_Text.Size = new System.Drawing.Size(78, 23);
		this.NewItemDef_Text.TabIndex = 14;
		this.NewItemDef_Text.Text = "2147483648";
		this.NewItemDef_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.NewItemAtt_Text.Location = new System.Drawing.Point(55, 163);
		this.NewItemAtt_Text.Name = "NewItemAtt_Text";
		this.NewItemAtt_Text.Size = new System.Drawing.Size(76, 23);
		this.NewItemAtt_Text.TabIndex = 13;
		this.NewItemAtt_Text.Text = "2147483648";
		this.NewItemAtt_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.NewItemDesc_Text.Location = new System.Drawing.Point(55, 136);
		this.NewItemDesc_Text.Name = "NewItemDesc_Text";
		this.NewItemDesc_Text.Size = new System.Drawing.Size(207, 23);
		this.NewItemDesc_Text.TabIndex = 12;
		this.NewItemDesc_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.NewItemName_Text.Location = new System.Drawing.Point(55, 109);
		this.NewItemName_Text.Name = "NewItemName_Text";
		this.NewItemName_Text.Size = new System.Drawing.Size(207, 23);
		this.NewItemName_Text.TabIndex = 11;
		this.NewItemName_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label27.AutoSize = true;
		this.label27.Location = new System.Drawing.Point(137, 52);
		this.label27.Name = "label27";
		this.label27.Size = new System.Drawing.Size(44, 17);
		this.label27.TabIndex = 10;
		this.label27.Text = "伤害：";
		this.label26.AutoSize = true;
		this.label26.Location = new System.Drawing.Point(10, 49);
		this.label26.Name = "label26";
		this.label26.Size = new System.Drawing.Size(44, 17);
		this.label26.TabIndex = 9;
		this.label26.Text = "图片：";
		this.label25.AutoSize = true;
		this.label25.Location = new System.Drawing.Point(8, 193);
		this.label25.Name = "label25";
		this.label25.Size = new System.Drawing.Size(44, 17);
		this.label25.TabIndex = 8;
		this.label25.Text = "敏捷：";
		this.label24.AutoSize = true;
		this.label24.Location = new System.Drawing.Point(137, 193);
		this.label24.Name = "label24";
		this.label24.Size = new System.Drawing.Size(44, 17);
		this.label24.TabIndex = 7;
		this.label24.Text = "幸运：";
		this.label23.AutoSize = true;
		this.label23.Location = new System.Drawing.Point(137, 166);
		this.label23.Name = "label23";
		this.label23.Size = new System.Drawing.Size(44, 17);
		this.label23.TabIndex = 6;
		this.label23.Text = "防御：";
		this.label22.AutoSize = true;
		this.label22.Location = new System.Drawing.Point(8, 166);
		this.label22.Name = "label22";
		this.label22.Size = new System.Drawing.Size(44, 17);
		this.label22.TabIndex = 5;
		this.label22.Text = "攻击：";
		this.label21.AutoSize = true;
		this.label21.Location = new System.Drawing.Point(8, 139);
		this.label21.Name = "label21";
		this.label21.Size = new System.Drawing.Size(44, 17);
		this.label21.TabIndex = 4;
		this.label21.Text = "描述：";
		this.label20.AutoSize = true;
		this.label20.Location = new System.Drawing.Point(8, 112);
		this.label20.Name = "label20";
		this.label20.Size = new System.Drawing.Size(44, 17);
		this.label20.TabIndex = 3;
		this.label20.Text = "名称：";
		this.ListItemName_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.ListItemName_CBox.FormattingEnabled = true;
		this.ListItemName_CBox.Location = new System.Drawing.Point(92, 18);
		this.ListItemName_CBox.Name = "ListItemName_CBox";
		this.ListItemName_CBox.Size = new System.Drawing.Size(169, 25);
		this.ListItemName_CBox.TabIndex = 2;
		this.ListItemName_CBox.SelectedIndexChanged += new System.EventHandler(ListItemName_CBox_SelectedIndexChanged);
		this.label19.AutoSize = true;
		this.label19.Location = new System.Drawing.Point(9, 21);
		this.label19.Name = "label19";
		this.label19.Size = new System.Drawing.Size(80, 17);
		this.label19.TabIndex = 0;
		this.label19.Text = "请选择武器：";
		this.groupBox3.Controls.Add(this.checkBox1);
		this.groupBox3.Controls.Add(this.button2);
		this.groupBox3.Controls.Add(this.textBox13);
		this.groupBox3.Controls.Add(this.textBox14);
		this.groupBox3.Controls.Add(this.textBox15);
		this.groupBox3.Controls.Add(this.label64);
		this.groupBox3.Controls.Add(this.label65);
		this.groupBox3.Location = new System.Drawing.Point(6, 368);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Size = new System.Drawing.Size(530, 61);
		this.groupBox3.TabIndex = 29;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "XML 自定义同步工具";
		this.checkBox1.AutoSize = true;
		this.checkBox1.ForeColor = System.Drawing.Color.Black;
		this.checkBox1.Location = new System.Drawing.Point(409, 27);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(51, 21);
		this.checkBox1.TabIndex = 18;
		this.checkBox1.Text = "加密";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.button2.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
		this.button2.ForeColor = System.Drawing.Color.Black;
		this.button2.Location = new System.Drawing.Point(459, 22);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(65, 27);
		this.button2.TabIndex = 17;
		this.button2.Text = "生成";
		this.button2.UseVisualStyleBackColor = true;
		this.button2.Click += new System.EventHandler(button2_Click);
		this.textBox13.Location = new System.Drawing.Point(159, 24);
		this.textBox13.Name = "textBox13";
		this.textBox13.Size = new System.Drawing.Size(54, 23);
		this.textBox13.TabIndex = 16;
		this.textBox13.Text = "Item";
		this.textBox13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.textBox14.Location = new System.Drawing.Point(49, 24);
		this.textBox14.Name = "textBox14";
		this.textBox14.Size = new System.Drawing.Size(104, 23);
		this.textBox14.TabIndex = 12;
		this.textBox14.Text = "Shop_Goods";
		this.textBox14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.textBox15.Location = new System.Drawing.Point(296, 24);
		this.textBox15.Name = "textBox15";
		this.textBox15.Size = new System.Drawing.Size(106, 23);
		this.textBox15.TabIndex = 13;
		this.textBox15.Text = "TemplateAllList";
		this.textBox15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.label64.AutoSize = true;
		this.label64.ForeColor = System.Drawing.Color.Black;
		this.label64.Location = new System.Drawing.Point(219, 28);
		this.label64.Name = "label64";
		this.label64.Size = new System.Drawing.Size(82, 17);
		this.label64.TabIndex = 14;
		this.label64.Text = "XML文件名：";
		this.label65.AutoSize = true;
		this.label65.ForeColor = System.Drawing.Color.Black;
		this.label65.Location = new System.Drawing.Point(10, 28);
		this.label65.Name = "label65";
		this.label65.Size = new System.Drawing.Size(44, 17);
		this.label65.TabIndex = 15;
		this.label65.Text = "表名：";
		this.timer1.Enabled = true;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.mainWorker.Enabled = true;
		this.mainWorker.Interval = 10000;
		this.mainWorker.Tick += new System.EventHandler(mainWorker_Tick);
		this.button29.Location = new System.Drawing.Point(9, 18);
		this.button29.Name = "button29";
		this.button29.Size = new System.Drawing.Size(118, 23);
		this.button29.TabIndex = 28;
		this.button29.Text = "本周战队排名奖励";
		this.button29.UseVisualStyleBackColor = true;
		this.button29.Click += new System.EventHandler(button29_Click);
		this.groupBox17.Controls.Add(this.button30);
		this.groupBox17.Controls.Add(this.button29);
		this.groupBox17.Location = new System.Drawing.Point(277, 167);
		this.groupBox17.Name = "groupBox17";
		this.groupBox17.Size = new System.Drawing.Size(253, 47);
		this.groupBox17.TabIndex = 29;
		this.groupBox17.TabStop = false;
		this.groupBox17.Text = "战队赛奖励";
		this.button30.Location = new System.Drawing.Point(133, 18);
		this.button30.Name = "button30";
		this.button30.Size = new System.Drawing.Size(113, 23);
		this.button30.TabIndex = 29;
		this.button30.Text = "本赛季排名奖励";
		this.button30.UseVisualStyleBackColor = true;
		this.button30.Click += new System.EventHandler(button30_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		base.ClientSize = new System.Drawing.Size(793, 474);
		base.Controls.Add(this.tabControl1);
		base.Name = "Form1";
		base.Load += new System.EventHandler(Form1_Load);
		this.tabControl1.ResumeLayout(false);
		this.tabPage1.ResumeLayout(false);
		this.groupBox9.ResumeLayout(false);
		this.groupBox9.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		this.groupBox16.ResumeLayout(false);
		this.groupBox16.PerformLayout();
		this.groupBox15.ResumeLayout(false);
		this.groupBox15.PerformLayout();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.groupBox14.ResumeLayout(false);
		this.groupBox14.PerformLayout();
		this.playerBox.ResumeLayout(false);
		this.playerBox.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		this.groupBox1.ResumeLayout(false);
		this.tabPage2.ResumeLayout(false);
		this.groupBox8.ResumeLayout(false);
		this.groupBox19.ResumeLayout(false);
		this.groupBox19.PerformLayout();
		this.groupBox13.ResumeLayout(false);
		this.groupBox13.PerformLayout();
		this.groupBox4.ResumeLayout(false);
		this.groupBox4.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.NewPlyaerFightPowerRank).EndInit();
		this.groupBox5.ResumeLayout(false);
		this.groupBox5.PerformLayout();
		this.groupBox6.ResumeLayout(false);
		this.groupBox6.PerformLayout();
		this.groupBox7.ResumeLayout(false);
		this.groupBox7.PerformLayout();
		this.tabPage3.ResumeLayout(false);
		this.groupBox20.ResumeLayout(false);
		this.groupBox18.ResumeLayout(false);
		this.groupBox10.ResumeLayout(false);
		this.groupBox10.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox3).EndInit();
		this.groupBox11.ResumeLayout(false);
		this.groupBox11.PerformLayout();
		this.groupBox12.ResumeLayout(false);
		this.groupBox12.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox2).EndInit();
		this.groupBox3.ResumeLayout(false);
		this.groupBox3.PerformLayout();
		this.groupBox17.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
