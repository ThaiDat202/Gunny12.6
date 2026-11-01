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
using Game.Server.Packets.Client;
using Game.Server.Team;
using Game.Service;
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

    private GroupBox groupBox17;

    private Button button30;
    private Label label8;
    private Label label9;
    private Label label10;
    private Label label11;
    private TabPage tabPage4;
    private GroupBox groupBox8;
    private GroupBox groupBox21;
    private Button button18;
    private Button button19;
    private GroupBox groupBox22;
    private Button button16;
    private Button button17;
    private GroupBox groupBox23;
    private Button button12;
    private Button button13;
    private GroupBox groupBox24;
    private Button button14;
    private Button button15;
    private GroupBox groupBox25;
    private Button button24;
    private Button button23;
    private GroupBox groupBox30;
    private GroupBox groupBox34;
    private Button button40;
    private GroupBox groupBox33;
    private Button button37;
    private Button button38;
    private GroupBox groupBox32;
    private Button button34;
    private GroupBox groupBox31;
    private Button button31;
    private GroupBox groupBox35;
    private GroupBox groupBox36;
    private Button button33;
    private GroupBox groupBox39;
    private Button button36;
    private Button button39;
    private GroupBox groupBox38;
    private Button button35;
    private GroupBox groupBox37;
    private Button button32;
    private GroupBox groupBox40;
    private Button button41;
    private GroupBox groupBox41;
    private Button button42;
    private GroupBox groupBox42;
    private Button button43;
    private TabPage tabPage5;
    private GroupBox groupBox43;
    private GroupBox groupBox27;
    private Button button22;
    private Button button20;
    private Button button21;
    private GroupBox groupBox26;
    private Button button25;
    private Button button28;
    private GroupBox groupBox28;
    private GroupBox groupBox44;
    private Button button6;
    private Button button11;
    private Button button26;
    private GroupBox groupBox29;
    private Button button5;
    private GroupBox groupBox48;
    private Button button46;
    private Button button47;
    private GroupBox groupBox47;
    private Button button45;
    private GroupBox groupBox46;
    private GroupBox groupBox45;
    private Button button27;
    private Button button44;
    private GroupBox groupBox49;
    private GroupBox groupBox51;
    private GroupBox groupBox52;
    private GroupBox groupBox53;
    private Button button52;
    private GroupBox groupBox54;
    private Button button53;
    private GroupBox groupBox55;
    private Button button55;
    private GroupBox groupBox56;
    private Button button57;
    private GroupBox groupBox57;
    private Button button59;
    private Button button60;
    private Button button51;
    private Button button49;
    private Button button50;
    private GroupBox groupBox50;
    private GroupBox groupBox60;
    private Button button48;
    private Button button58;
    private GroupBox groupBox61;
    private Button button64;
    private GroupBox groupBox62;
    private Button button65;
    private GroupBox groupBox63;
    private Button button66;
    private GroupBox groupBox58;
    private GroupBox groupBox59;
    private Button button61;
    private GroupBox groupBox64;
    private Button button69;
    private Button button63;
    private GroupBox groupBox65;
    private Button button67;
    private GroupBox groupBox66;
    private Button button68;
    private Button button54;
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
        write($"Xuất {filename} thành công -- {DateTime.Now.ToString()}", Color.Green);
    }

    private void write(string text, Color color)
    {

        richTextBox1.Select(richTextBox1.Text.Length, 0);
        richTextBox1.Font = new Font("Times New Roman", 8f, FontStyle.Regular);
        richTextBox1.ForeColor = color;
        richTextBox1.SelectedText = text;
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
        label45.Text = $"{allPlayers.Length} người";
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
                playerBussiness.SendMailAndItemByNickName(LanguageMgr.GetTranslation("Game.Service.helper.DDTankName"), "Phần thưởng GM", gamePlayer.PlayerCharacter.NickName, 1001, templateId, count1, 0, 0, 0, 0, 0, 0, 0, 0, isBinds: true);
            }
        }
        SendTime.Text = "泡点执行成功 " + (num + 1) + " hạng hai";
    }

    private void SendAnnouncement_Click(object sender, EventArgs e)
    {
        if (richTextBox1.Text == "")
        {
            MessageBox.Show("Vui lòng nhập nội dung cần gửi!", "Thông tin hệ thống");
            return;
        }
        int type = 0;
        switch (ListType.Text)
        {
            case "Màn hình nhỏ":
                type = 0;
                break;
            case "Pop-up":
                type = 4;
                break;
            case "Hiện tại":
                type = 2;
                break;
            case "Chat riêng":
                type = 8;
                break;
        }
        ShowMessage(type, richTextBox1.Text);
        MessageBox.Show("Gửi thông báo thành công");
    }

    private void Kick_Click(object sender, EventArgs e)
    {
        GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(playerBox.Text.Split('/')[0]);
        if (clientByPlayerNickName == null)
        {
            MessageBox.Show("Offline");
            return;
        }
        try
        {
            clientByPlayerNickName.Out.SendKitoff("Bạn đã kick ra khỏi game");
            using (ManageBussiness manageBussiness = new ManageBussiness())
            {
                manageBussiness.KitoffUserByNickName(clientByPlayerNickName.PlayerCharacter.NickName, 1001, "Bạn đã bị kick ra khỏi game");
            }
            MessageBox.Show("Bạn đã bị kick ra khỏi game");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Kick không thành công");
            Console.WriteLine("Kick không thành công, lỗi:" + ex);
        }
    }

    private void Ban_Click(object sender, EventArgs e)
    {
        GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(playerBox.Text.Split('/')[0]);
        if (clientByPlayerNickName == null)
        {
            MessageBox.Show("Offline");
            return;
        }
        try
        {
            clientByPlayerNickName.Out.SendKitoff("Bạn đã bị ban ra khỏi game");
            using (ManageBussiness manageBussiness = new ManageBussiness())
            {
                manageBussiness.ForbidPlayerByNickName(clientByPlayerNickName.PlayerCharacter.NickName, 1001, dateTimePicker1.Value, isExist: false);
            }
            MessageBox.Show("Bạn đã bị cấm đăng nhập");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Bạn không thành công");
            Console.WriteLine("Kick không thành công, lỗi:" + ex);
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
            label47.Text = "Sẵn Sàng";
            label47.ForeColor = Color.Red;
            label48.Visible = true;
            label48.Text = $"Đóng sau：{_minutes} phút";
            _timer = new System.Threading.Timer(ShutDownCallBack, null, 0, 60000);
            ShutDown.Text = "Hủy bỏ";
            return;
        }
        _timer.Dispose();
        _timer = null;
        label47.Text = "Online";
        label47.ForeColor = Color.Green;
        label48.Visible = false;
        ShutDown.Text = "Đóng";
        GameClient[] allClients = GameServer.Instance.GetAllClients();
        GameClient[] array = allClients;
        foreach (GameClient gameClient in array)
        {
            if (gameClient.Out != null)
            {
                gameClient.Out.SendMessage(eMessageType.Normal, "Tạm dừng bảo trì. Chúc ae chơi game vui vẻ");
            }
        }
    }

    private static void ShutDownCallBack(object state)
    {
        Console.WriteLine($"Trò chơi sẽ{_minutes}đóng trong vài phút");
        form.label48.Text = $"Đóng：{_minutes} phút";
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
            form.label47.Text = "Đóng";
            form.label48.Visible = false;
            form.ShutDown.Enabled = false;
            _timer.Dispose();
            _timer = null;
            GameServer.Instance.Stop();
            Console.WriteLine("Đóng Server Thành Công.");
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
                MessageBox.Show("Offline");
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
            MessageBox.Show("Hãy nhập tình trạng", "Khóa or Không Khóa");
            return;
        }
        if (IsBind.Text == "Khóa")
        {
            isBinds = true;
        }
        else if (IsBind.Text == "Không Khóa")
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
        playerBussiness.SendMailAndItemByNickName(LanguageMgr.GetTranslation("Game.Service.helper.DDTankName") + "", "Phần thưởng GM", nickName, 1001, templateID, count, validDate, gold, money, strengthenLevel, attackCompose, defendCompose, agilityCompose, luckCompose, isBinds);
        MessageBox.Show("Gửi thành công");
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
            MessageBox.Show("Vui lòng nhập tên items bạn cần tìm!");
        }
    }

    private void ChangePassword_Click(object sender, EventArgs e)
    {
        SqlDataReader sqlDataReader = null;
        int num = 0;
        if (UserName_Text.Text == "")
        {
            MessageBox.Show("Tài khoản trống!");
            return;
        }
        if (Password.Text == "")
        {
            MessageBox.Show("Mật khẩu trống!");
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
                MessageBox.Show("Đổi mật khẩu không thành công, tài khoản không tồn tại!");
            }
            else if (db.ExecuteNonQuery($"update Mem_UserInfo set Password='{GetMD5(Password.Text)}' where UserId={num}"))
            {
                MessageBox.Show("Thành công");
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
                    Online_Text.Text = "Offline";
                }
                else
                {
                    Online_Text.ForeColor = Color.Green;
                    Online_Text.Text = "Online";
                }
                sqlDataReader.Close();
            }
            else
            {
                UserName_Text.Text = "Người chơi không được tìm thấy";
            }
        }
        else
        {
            UserName_Text.Text = "Vui lòng nhập tên người chơi ở mục Player";
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
                MessageBox.Show("Thành Công");
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
        Online_Text.Text = "Offline";
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
                MessageBox.Show("Thành Công");
                return;
            }
            empty = string.Format("UPDATE Sys_Users_Detail SET Grade = '{0}', GP = (select GP from Db_Tank_All.dbo.LevelInfo where Grade = '{0}') WHERE UserID = '{1}'", Level_Text.Text, UserID_Lab.Text);
            db = new DataBaseHelper("conString");
            if (db.ExecuteNonQuery(empty))
            {
                MessageBox.Show("Thành Công");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
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
        CleanAcitveNum_Btn.Text = "Event đã được thiết lập";
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
            write($"Xóa Code Event -- {DateTime.Now.ToString()} \r\n", Color.Green);
            PlayerLogs.WriteLine($"[Xóa Code Event] - [{DateTime.Now.ToString()}].", 2);
        }
        GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
        GamePlayer[] array = allPlayers;
        foreach (GamePlayer gamePlayer in array)
        {
            if (gamePlayer.ResetTimeBox())
            {
                PlayerLogs.WriteLine($"[Reset Event] - [Player:{gamePlayer.PlayerCharacter.NickName}] - [{DateTime.Now.ToString()}].", 2);
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
                write($"Xóa Code Event -- {DateTime.Now.ToString()} \r\n", Color.Green);
                PlayerLogs.WriteLine($"[Xóa Code Event] - [{DateTime.Now.ToString()}].", 2);
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
        if (Common_Text.Text != "Tối đa" && CommonAddWound_Text.Text != "Tối đa" && CommonMultiBall_Text.Text != "Tối đa" && Special_Text.Text != "Tối đa")
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
                    MessageBox.Show("Thêm thành công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng thay đổi vũ khí và thử lại!");
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
        if (!link.Equals("") && GetWebStatusCode(link, 2000) != "Lỗi: (404) không tìm thấy.")
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
            if (!link.Equals("") && GetWebStatusCode(link, 2000) != "Lỗi: (404) không tìm thấy.")
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
            Common_Text.Text = "Tối đa";
            CommonAddWound_Text.Text = "Tối đa";
            CommonMultiBall_Text.Text = "Tối đa";
            Special_Text.Text = "Tối đa";
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
            if (!link.Equals("") && GetWebStatusCode(link, 2000) != "Lỗi: (404) không tìm thấy.")
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
        CreateAllCeleb.Text = "Chủ đề được thiết lập";
    }

    private void timer3_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string url = RequestUrl + "CreateAllCeleb.ashx";
            Console.WriteLine(BaseInterface.RequestContent(url));
            Console.ForegroundColor = ConsoleColor.White;
            write($"Đồng bộ -- {DateTime.Now.ToString()} \r\n", Color.Green);
        }
    }

    private void Reload_Click(object sender, EventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Đang bắt đầu cập nhật hệ thống trò chơi.. Hãy kiên nhẫn.....");
        GameProperties.Refresh();
        Console.WriteLine("ServerConfig:True");
        Console.WriteLine("Language:" + LanguageMgr.Reload(""));
        Console.WriteLine("LotteryShow:" + LotteryShowTemplateMgr.Init());
        Console.WriteLine("Map:" + MapMgr.Init());
        Console.WriteLine("Item:" + ItemMgr.ReLoad());
        Console.WriteLine("ItemBox:" + ItemBoxMgr.Init());
        Console.WriteLine("Ball:" + BallMgr.Init());
        Console.WriteLine("Exercise:" + ExerciseMgr.Init());
        Console.WriteLine("Level:" + LevelMgr.Init());
        Console.WriteLine("BallConfig:" + BallConfigMgr.Init());
        Console.WriteLine("Fusion:" + FusionMgr.Init());
        Console.WriteLine("Award:" + AwardMgr.Init());
        Console.WriteLine("Achievement:" + AchievementMgr.Init());
        Console.WriteLine("NPCInfo:" + NPCInfoMgr.Init());
        Console.WriteLine("MissionInfo:" + MissionInfoMgr.Init());
        Console.WriteLine("MissionEnergy:" + MissionEnergyMgr.Init());
        Console.WriteLine("PveInfo:" + PveInfoMgr.Init());
        Console.WriteLine("Drop:" + DropMgr.Init());
        Console.WriteLine("FightRate:" + FightRateMgr.Init());
        Console.WriteLine("Refinery:" + RefineryMgr.Init());
        Console.WriteLine("Strengthen:" + StrengthenMgr.Init());
        Console.WriteLine("PropItem:" + PropItemMgr.Init());
        Console.WriteLine("Shop:" + ShopMgr.Init());
        Console.WriteLine("Quest:" + QuestMgr.Init());
        Console.WriteLine("Consortia:" + ConsortiaMgr.Init());
        Console.WriteLine("Card:" + CardMgr.Init());
        Console.WriteLine("Pet:" + PetMgr.Init());
        Console.WriteLine("Mount:" + MountMgr.Init());
        Console.WriteLine("GoldEquip:" + GoldEquipMgr.Init());
        Console.WriteLine("ActiveSystem:" + ActiveSystemMgr.Init());
        Console.WriteLine("LightriddleQuest:" + LightriddleQuestMgr.Init());
        Console.WriteLine("WorldEvent:" + WorldEventMgr.Init());
        Console.WriteLine("SubActive:" + SubActiveMgr.Init());
        Console.WriteLine("DiceLevel:" + DiceLevelAwardMgr.Init());
        Console.WriteLine("EventAward:" + EventAwardMgr.Init());
        Console.WriteLine("Rank:" + RankMgr.Init());
        Console.WriteLine("PetMoeProperty:" + PetMoePropertyMgr.Init());
        Console.WriteLine("Pet:" + PetMgr.Init());
        Console.WriteLine("MagicStone:" + MagicStoneMgr.Init());
        Console.WriteLine("BombKing:" + BombKingMgr.Init());
        Console.WriteLine("VipIntegration:" + VipIntegrationMgr.Init());
        Console.WriteLine("GypsyShop:" + GypsyShopMgr.Init());
        Console.WriteLine("MagicFusionData:" + MagicFusionDataMgr.Init());
        Console.WriteLine("Engrave:" + EngraveMgr.Init());
        Console.WriteLine("AvatarColection:" + AvatarColectionMgr.Init());
        Console.WriteLine("Rune:" + RuneMgr.ReLoad());
        Console.WriteLine("RuneAdvanceTemplate:" + RuneAdvanceTemplateMgr.ReLoad());
        Console.WriteLine("DevilTurn:" + DevilTurnMgr.Init());
        Console.WriteLine("NecklaceStrength:" + NecklaceStrengthMgr.Init());
        Console.WriteLine("Emblem:" + EmblemMgr.Init());
        Console.WriteLine("Elf:" + ElfMgr.Init());
        Console.WriteLine("PetSpecialSkill:" + PetSpecialSkillMgr.Init());
        Console.WriteLine("HomeTemple:" + HomeTempleMgr.Init());
        string url = RequestUrl + "CreateAllXml.ashx";
        Console.WriteLine(BaseInterface.RequestContent(url));
        Console.ForegroundColor = ConsoleColor.White;
        write($"Cập nhật Request -- {DateTime.Now.ToString()} \r\n", Color.Green);
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
            MessageBox.Show("Mã hóa thành công");
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
            MessageBox.Show("Giải mã thành công");
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
            MessageBox.Show("Mã hóa thành công");
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
            MessageBox.Show("Giải mã thành công");
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
            MessageBox.Show("Khôi phục thành công");
        }
        else
        {
            MessageBox.Show("Vui lòng chọn người chơi để khôi phục");
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        string sqlstr = "UPDATE Sys_Users_Detail SET STATE = 0";
        db = new DataBaseHelper("conString");
        if (db.ExecuteNonQuery(sqlstr))
        {
            MessageBox.Show("Reset thành công");
        }
    }

    private void button4_Click(object sender, EventArgs e)
    {
        timer4 = new System.Timers.Timer();
        timer4.Interval = 1000.0;
        timer4.Elapsed += timer4_Elapsed;
        timer4.Enabled = true;
        button4.Enabled = false;
        button4.Text = "Chủ đề";
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
            write($"Xóa điểm BOSS thế giới thành công -- {DateTime.Now.ToString()} \r\n", Color.Green);
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

            int[] array = ((text2 == "Thô") ? new int[42]
            {
                406111, 406121, 406131, 406141, 406151, 406161, 406211, 406221, 406231, 406241,
                406251, 406261, 406311, 406321, 406331, 406341, 406351, 406361, 406411, 406421,
                406431, 406441, 406451, 406461, 406511, 406516, 406521, 406526, 406531, 406536,
                406541, 406546, 406551, 406556, 406561, 406566, 406571, 406576, 406581, 406586,
                406591, 406596
            } : ((!(text2 == "Thường")) ? new int[1] : new int[42]
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
            MessageBox.Show("Xóa thành công");
        }
        else
        {
            MessageBox.Show("Vui lòng chọn người chơi muốn xóa");
        }
    }

    private void button5_Click_1(object sender, EventArgs e)
    {
        //string empty = string.Empty;
        //if (Level_Text.Text != "" && UserID_Lab.Text != "0")
        //{
        //	GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
        //	if (playerById != null)
        //	{
        //		playerById.Horse.Info.curLevel = 89;
        //		playerById.Horse.Info.curExp = 14733460;
        //		playerById.PlayerCharacter.MountLv = 89;
        //		playerById.Out.SendUpHorse(isHasLevelUp: true, playerById.Horse.Info.curLevel, playerById.Horse.Info.curExp);
        //		int[] array = new int[12]
        //		{
        //			10801, 10101, 10701, 10901, 10201, 11001, 10401, 10601, 11101, 11301,
        //			11201, 10301
        //		};
        //		int[] array2 = array;
        //		foreach (int num in array2)
        //		{
        //			MountSkillDataInfo mountSkillDataInfo = new MountSkillDataInfo();
        //			mountSkillDataInfo.SKillID = num;
        //			mountSkillDataInfo.Exp = 0;
        //			mountSkillDataInfo.IsUse = 0;
        //			if (!playerById.Horse.curHasSkill.ContainsKey(num))
        //			{
        //				playerById.Horse.curHasSkill.Add(num, mountSkillDataInfo);
        //			}
        //		}
        //		playerById.UpdateProperties();
        //		MessageBox.Show("Thành Công");
        //		return;
        //	}
        //	int[] array3 = new int[12]
        //	{
        //		10801, 10101, 10701, 10901, 10201, 11001, 10401, 10601, 11101, 11301,
        //		11201, 10301
        //	};
        //	Dictionary<int, MountSkillDataInfo> dictionary = new Dictionary<int, MountSkillDataInfo>();
        //	int[] array4 = array3;
        //	foreach (int num2 in array4)
        //	{
        //		MountSkillDataInfo mountSkillDataInfo2 = new MountSkillDataInfo();
        //		mountSkillDataInfo2.SKillID = num2;
        //		mountSkillDataInfo2.Exp = 0;
        //		mountSkillDataInfo2.IsUse = 0;
        //		if (!dictionary.ContainsKey(num2))
        //		{
        //			dictionary.Add(num2, mountSkillDataInfo2);
        //		}
        //	}
        //	empty = $"UPDATE Mount_Data SET curLevel = {89}, curExp = {14733460}, curHasSkill = {JsonConvert.SerializeObject(dictionary)} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
        //	db = new DataBaseHelper("conString");
        //	if (db.ExecuteNonQuery(empty))
        //	{
        //		MessageBox.Show("Thành Công");
        //	}
        //}
        //else
        //{
        //	MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        //}
    }

    // lên cấp và kỹ năng thú
    private void button6_Click(object sender, EventArgs e)
    {
        if (Online_Text.Text == "Offline")
        {


            string empty = string.Empty;
            if (Level_Text.Text != "" && UserID_Lab.Text != "0")
            {
                // Update Max Level Thú Cưỡi
                GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
                if (playerById != null)
                {
                    playerById.Horse.Info.curLevel = 89;
                    playerById.Horse.Info.curExp = 14733460;
                    playerById.PlayerCharacter.MountLv = 89;
                    playerById.Out.SendUpHorse(isHasLevelUp: true, playerById.Horse.Info.curLevel, playerById.Horse.Info.curExp);
                    playerById.UpdateProperties();
                }
                else
                {
                    empty = $"UPDATE Mount_Data SET curLevel = {89}, curExp = {14733460} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                    db = new DataBaseHelper("conString");
                    db.ExecuteNonQuery(empty);
                }
                // Update Max Kỹ Năng Thú Cưỡi
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
                    MessageBox.Show("Thành Công");
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
                    MessageBox.Show("Thành Công");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng thoát khỏi game trước khi sử dụng tính năng này !");
        }
    }

    private void button11_Click(object sender, EventArgs e)
    {
        //string empty = string.Empty;
        //if (Level_Text.Text != "" && UserID_Lab.Text != "0")
        //{
        //	GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
        //	if (playerById != null)
        //	{
        //		playerById.Horse.Info.curLevel = 89;
        //		playerById.Horse.Info.curExp = 14733460;
        //		playerById.PlayerCharacter.MountLv = 89;
        //		playerById.Out.SendUpHorse(isHasLevelUp: true, playerById.Horse.Info.curLevel, playerById.Horse.Info.curExp);
        //		playerById.UpdateProperties();
        //		MessageBox.Show("Thành Công");
        //	}
        //	else
        //	{
        //		empty = $"UPDATE Mount_Data SET curLevel = {89}, curExp = {14733460} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
        //		db = new DataBaseHelper("conString");
        //		if (db.ExecuteNonQuery(empty))
        //		{
        //			MessageBox.Show("Thành Công");
        //		}
        //	}
        //}
        //else
        //{
        //	MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        //}
    }

    //Lên cấp vật tổ
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
                MessageBox.Show("Thành Công");
                return;
            }
            empty = $"UPDATE Sys_Users_Detail SET totemId = {10350} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
            db = new DataBaseHelper("conString");
            if (db.ExecuteNonQuery(empty))
            {
                MessageBox.Show("Thành Công");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //tăng bậc vật tổ
    private void button13_Click(object sender, EventArgs e)
    {
        if (Online_Text.Text == "Offline")
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
                    MessageBox.Show("Thành Công");
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
                        MessageBox.Show("Thành Công");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng thoát khỏi game trước khi sử dụng tính năng này !");
        }
    }

    //chúc phúc miếu thần
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = $"UPDATE Sys_Active_System_Data SET CurrentSelectIndex = {6}, CurrentLevel = {60}, HomeTempleExp = {54940724} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //thăng hoa miếu thần
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = $"UPDATE Sys_Active_System_Data SET promoteLevel = {60} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //lên cấp thẻ bài
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
                playerById.PlayerCharacter.CardInfo.Attack = 240;
                playerById.PlayerCharacter.CardInfo.Defence = 240;
                playerById.PlayerCharacter.CardInfo.Agility = 240;
                playerById.PlayerCharacter.CardInfo.Lucky = 240;
                CardNewSystemMgr.Out.SendCardInfo(playerById);
                playerById.UpdateProperties();
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = string.Format("UPDATE Sys_Users_CardInfo SET CardLevel = {0}, Point = {1}, Armor = {2}, Crits = {2}, Trick = {2}, Viole = {2}, Attack = {3}, Defence = {3}, Agility = {3}, Lucky = {3} WHERE UserID = {4}", 90, 209, 0, 240, int.Parse(UserID_Lab.Text));
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //đột phá thẻ bài
    private void button17_Click(object sender, EventArgs e)
    {
        string empty = string.Empty;
        if (Level_Text.Text != "" && UserID_Lab.Text != "0")
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
            if (playerById != null)
            {
                //int MaxTemplateID = 314101;
                //int Count = 1;

                //while (Count <= 83)
                //{
                //    if (MaxTemplateID != 314150)
                //    {
                //        playerById.PlayerCharacter.CardSoul.TemplateID = MaxTemplateID;
                //    }
                //}
                //MaxTemplateID++;
                //Count++;
                playerById.PlayerCharacter.CardInfo.SuitLevel = 31;
                CardNewSystemMgr.Out.SendCardInfo(playerById);
                playerById.UpdateProperties();
                MessageBox.Show("Thành Công");
                return;
            }
            empty = $"UPDATE Sys_Users_CardInfo SET SuitLevel = {31} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
            db = new DataBaseHelper("conString");
            if (db.ExecuteNonQuery(empty))
            {
                MessageBox.Show("Thành Công");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //tu luyện cơ bản
    //private void button18_Click(object sender, EventArgs e)
    //{
    //    string empty = string.Empty;
    //    if (Level_Text.Text != "" && UserID_Lab.Text != "0")
    //    {
    //        GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
    //        if (playerById != null)
    //        {
    //            playerById.PlayerCharacter.Texp.attTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.defTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.lukTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.spdTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.hpTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.magicAtkTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.magicDefTexpExp = 56655500;
    //            playerById.UpdateProperties();
    //            playerById.SendUpdatePublicPlayer();
    //            MessageBox.Show("Thành Công");
    //        }
    //        else
    //        {
    //            empty = string.Format("UPDATE Sys_Users_Texp SET attTexpExp = {0}, defTexpExp = {0}, lukTexpExp = {0}, spdTexpExp = {0}, hpTexpExp = {0}, magicAtkTexpExp = {0}, magicDefTexpExp = {0}, WHERE UserID = {1}", 1665550, int.Parse(UserID_Lab.Text));
    //            db = new DataBaseHelper("conString");
    //            if (db.ExecuteNonQuery(empty))
    //            {
    //                MessageBox.Show("Thành Công");
    //            }
    //        }
    //    }
    //    else
    //    {
    //        MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
    //    }
    //}

    //tu luyện cấp 2
    //private void button19_Click(object sender, EventArgs e)
    //{
    //    string empty = string.Empty;
    //    if (Level_Text.Text != "" && UserID_Lab.Text != "0")
    //    {
    //        GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
    //        if (playerById != null)
    //        {
    //            playerById.PlayerCharacter.Texp.TrainCritTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.TrainSunderTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.TrainCritDmgTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.TrainSpeedTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.TrainTricKillTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.TrainDmgTexpExp = 56655500;
    //            playerById.PlayerCharacter.Texp.TrainArmorTexpExp = 56655500;
    //            playerById.UpdateProperties();
    //            playerById.SendUpdatePublicPlayer();
    //            MessageBox.Show("Thành Công");
    //        }
    //        else
    //        {
    //            empty = string.Format("UPDATE Sys_Users_Texp SET TrainCritTexpExp = {0}, TrainSunderTexpExp = {0}, TrainCritDmgTexpExp = {0}, TrainSpeedTexpExp = {0}, TrainTricKillTexpExp = {0}, TrainDmgTexpExp = {0}, TrainArmorTexpExp = {0}, WHERE UserID = {1}", 1665550, int.Parse(UserID_Lab.Text));
    //            db = new DataBaseHelper("conString");
    //            if (db.ExecuteNonQuery(empty))
    //            {
    //                MessageBox.Show("Thành Công");
    //            }
    //        }
    //    }
    //    else
    //    {
    //        MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
    //    }
    //}

    //tăng sao nón
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                MessageBox.Show("Offline");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //tăng sao quần áo
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                MessageBox.Show("Offline");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //tăng sao vũ khí
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                MessageBox.Show("Offline");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //luyện 
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
                MessageBox.Show("Thành Công");
                return;
            }
            empty = $"UPDATE Sys_Users_Detail SET necklaceExp = {630000} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
            db = new DataBaseHelper("conString");
            if (db.ExecuteNonQuery(empty))
            {
                MessageBox.Show("Thành Công");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //đúc 
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = $"UPDATE Sys_Users_Detail SET necklaceCastLevel = {40} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //chế tạo
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
                MessageBox.Show("Thành Công");
                return;
            }
            empty = $"UPDATE Sys_Users_Detail SET fineSuitExp = {345860} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
            db = new DataBaseHelper("conString");
            if (db.ExecuteNonQuery(empty))
            {
                MessageBox.Show("Thành Công");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //tiến hóa pet
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = $"UPDATE Sys_Users_Detail SET evolutionGrade = {50}, evolutionGrade = {809030} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //chiến hồn
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
                    gemStone.FigSpiritIdValue = "6,0,0|6,0,1|6,0,2";
                    playerBussiness.UpdateGemStoneInfo(gemStone);
                    playerById.UpdateGemStone(place, gemStone);
                    playerById.Out.SendPlayerFigSpiritUp(playerById.PlayerCharacter.ID, gemStone, isUp: true, isMaxLevel: true, isFall: true, 0, 0);
                }
                playerById.UpdateProperties();
                MessageBox.Show("Thành Công");
            }
            else
            {
                MessageBox.Show("Offline");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    //manh hóa pet
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = string.Format("UPDATE Sys_Eat_Pets SET clothesLevel = {0}, weaponLevel = {0}, hatLevel = {0}, clothesExp = {1}, weaponExp = {1}, hatExp = {1} WHERE UserID = {2}", 30, 30000, int.Parse(UserID_Lab.Text));
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
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
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Stop = new System.Windows.Forms.Button();
            this.Start = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label51 = new System.Windows.Forms.Label();
            this.time1 = new System.Windows.Forms.TextBox();
            this.tempid1 = new System.Windows.Forms.TextBox();
            this.SendTime = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.time = new System.Windows.Forms.TextBox();
            this.ShutDown = new System.Windows.Forms.Button();
            this.ReStart = new System.Windows.Forms.Button();
            this.label45 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
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
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.button30 = new System.Windows.Forms.Button();
            this.button29 = new System.Windows.Forms.Button();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.ChangePassword = new System.Windows.Forms.Button();
            this.Password = new System.Windows.Forms.TextBox();
            this.label66 = new System.Windows.Forms.Label();
            this.NewPlyaerFightPowerRank = new System.Windows.Forms.DataGridView();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.CleanAcitveNum_Btn = new System.Windows.Forms.Button();
            this.ListActiveID_CBox = new System.Windows.Forms.ComboBox();
            this.label42 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.FindUser_Btn = new System.Windows.Forms.Button();
            this.UserName_Text = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
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
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.ItemArmor_Text = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
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
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox43 = new System.Windows.Forms.GroupBox();
            this.groupBox48 = new System.Windows.Forms.GroupBox();
            this.button46 = new System.Windows.Forms.Button();
            this.button47 = new System.Windows.Forms.Button();
            this.groupBox47 = new System.Windows.Forms.GroupBox();
            this.button45 = new System.Windows.Forms.Button();
            this.groupBox46 = new System.Windows.Forms.GroupBox();
            this.button25 = new System.Windows.Forms.Button();
            this.groupBox45 = new System.Windows.Forms.GroupBox();
            this.button27 = new System.Windows.Forms.Button();
            this.button44 = new System.Windows.Forms.Button();
            this.groupBox44 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button26 = new System.Windows.Forms.Button();
            this.groupBox29 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox27 = new System.Windows.Forms.GroupBox();
            this.button22 = new System.Windows.Forms.Button();
            this.button20 = new System.Windows.Forms.Button();
            this.button21 = new System.Windows.Forms.Button();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.button28 = new System.Windows.Forms.Button();
            this.groupBox35 = new System.Windows.Forms.GroupBox();
            this.groupBox42 = new System.Windows.Forms.GroupBox();
            this.button43 = new System.Windows.Forms.Button();
            this.groupBox41 = new System.Windows.Forms.GroupBox();
            this.button42 = new System.Windows.Forms.Button();
            this.groupBox40 = new System.Windows.Forms.GroupBox();
            this.button41 = new System.Windows.Forms.Button();
            this.groupBox39 = new System.Windows.Forms.GroupBox();
            this.button36 = new System.Windows.Forms.Button();
            this.button39 = new System.Windows.Forms.Button();
            this.groupBox38 = new System.Windows.Forms.GroupBox();
            this.button35 = new System.Windows.Forms.Button();
            this.groupBox37 = new System.Windows.Forms.GroupBox();
            this.button32 = new System.Windows.Forms.Button();
            this.groupBox36 = new System.Windows.Forms.GroupBox();
            this.button33 = new System.Windows.Forms.Button();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.button24 = new System.Windows.Forms.Button();
            this.button23 = new System.Windows.Forms.Button();
            this.groupBox30 = new System.Windows.Forms.GroupBox();
            this.groupBox34 = new System.Windows.Forms.GroupBox();
            this.button40 = new System.Windows.Forms.Button();
            this.groupBox33 = new System.Windows.Forms.GroupBox();
            this.button37 = new System.Windows.Forms.Button();
            this.button38 = new System.Windows.Forms.Button();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.button34 = new System.Windows.Forms.Button();
            this.groupBox31 = new System.Windows.Forms.GroupBox();
            this.button31 = new System.Windows.Forms.Button();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.button54 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox28 = new System.Windows.Forms.GroupBox();
            this.groupBox58 = new System.Windows.Forms.GroupBox();
            this.groupBox59 = new System.Windows.Forms.GroupBox();
            this.button61 = new System.Windows.Forms.Button();
            this.groupBox64 = new System.Windows.Forms.GroupBox();
            this.button69 = new System.Windows.Forms.Button();
            this.button63 = new System.Windows.Forms.Button();
            this.groupBox65 = new System.Windows.Forms.GroupBox();
            this.button67 = new System.Windows.Forms.Button();
            this.groupBox66 = new System.Windows.Forms.GroupBox();
            this.button68 = new System.Windows.Forms.Button();
            this.groupBox50 = new System.Windows.Forms.GroupBox();
            this.groupBox60 = new System.Windows.Forms.GroupBox();
            this.button48 = new System.Windows.Forms.Button();
            this.button58 = new System.Windows.Forms.Button();
            this.groupBox61 = new System.Windows.Forms.GroupBox();
            this.button64 = new System.Windows.Forms.Button();
            this.groupBox62 = new System.Windows.Forms.GroupBox();
            this.button65 = new System.Windows.Forms.Button();
            this.groupBox63 = new System.Windows.Forms.GroupBox();
            this.button66 = new System.Windows.Forms.Button();
            this.groupBox49 = new System.Windows.Forms.GroupBox();
            this.groupBox51 = new System.Windows.Forms.GroupBox();
            this.button49 = new System.Windows.Forms.Button();
            this.groupBox52 = new System.Windows.Forms.GroupBox();
            this.button51 = new System.Windows.Forms.Button();
            this.groupBox53 = new System.Windows.Forms.GroupBox();
            this.button52 = new System.Windows.Forms.Button();
            this.groupBox54 = new System.Windows.Forms.GroupBox();
            this.button53 = new System.Windows.Forms.Button();
            this.groupBox55 = new System.Windows.Forms.GroupBox();
            this.button55 = new System.Windows.Forms.Button();
            this.groupBox56 = new System.Windows.Forms.GroupBox();
            this.button57 = new System.Windows.Forms.Button();
            this.groupBox57 = new System.Windows.Forms.GroupBox();
            this.button50 = new System.Windows.Forms.Button();
            this.button59 = new System.Windows.Forms.Button();
            this.button60 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mainWorker = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox15.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.playerBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.groupBox19.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NewPlyaerFightPowerRank)).BeginInit();
            this.groupBox13.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox43.SuspendLayout();
            this.groupBox48.SuspendLayout();
            this.groupBox47.SuspendLayout();
            this.groupBox46.SuspendLayout();
            this.groupBox45.SuspendLayout();
            this.groupBox44.SuspendLayout();
            this.groupBox29.SuspendLayout();
            this.groupBox27.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.groupBox35.SuspendLayout();
            this.groupBox42.SuspendLayout();
            this.groupBox41.SuspendLayout();
            this.groupBox40.SuspendLayout();
            this.groupBox39.SuspendLayout();
            this.groupBox38.SuspendLayout();
            this.groupBox37.SuspendLayout();
            this.groupBox36.SuspendLayout();
            this.groupBox25.SuspendLayout();
            this.groupBox30.SuspendLayout();
            this.groupBox34.SuspendLayout();
            this.groupBox33.SuspendLayout();
            this.groupBox32.SuspendLayout();
            this.groupBox31.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox28.SuspendLayout();
            this.groupBox58.SuspendLayout();
            this.groupBox59.SuspendLayout();
            this.groupBox64.SuspendLayout();
            this.groupBox65.SuspendLayout();
            this.groupBox66.SuspendLayout();
            this.groupBox50.SuspendLayout();
            this.groupBox60.SuspendLayout();
            this.groupBox61.SuspendLayout();
            this.groupBox62.SuspendLayout();
            this.groupBox63.SuspendLayout();
            this.groupBox49.SuspendLayout();
            this.groupBox51.SuspendLayout();
            this.groupBox52.SuspendLayout();
            this.groupBox53.SuspendLayout();
            this.groupBox54.SuspendLayout();
            this.groupBox55.SuspendLayout();
            this.groupBox56.SuspendLayout();
            this.groupBox57.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1924, 1055);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button10);
            this.tabPage1.Controls.Add(this.button9);
            this.tabPage1.Controls.Add(this.button8);
            this.tabPage1.Controls.Add(this.button7);
            this.tabPage1.Controls.Add(this.groupBox9);
            this.tabPage1.Controls.Add(this.groupBox15);
            this.tabPage1.Controls.Add(this.groupBox16);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.groupBox14);
            this.tabPage1.Controls.Add(this.playerBox);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 35);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1916, 1016);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button10.Location = new System.Drawing.Point(1747, 125);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(143, 60);
            this.button10.TabIndex = 25;
            this.button10.Text = "Giải Mã .PNG";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button9
            // 
            this.button9.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button9.Location = new System.Drawing.Point(1567, 125);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(143, 60);
            this.button9.TabIndex = 24;
            this.button9.Text = "Mã Hóa .PNG";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button8
            // 
            this.button8.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.Location = new System.Drawing.Point(1747, 206);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(143, 61);
            this.button8.TabIndex = 23;
            this.button8.Text = "Giải Mã .XML";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(1567, 206);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(143, 61);
            this.button7.TabIndex = 22;
            this.button7.Text = "Mã Hóa .XML";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.TemplateID_CBox);
            this.groupBox9.Controls.Add(this.pictureBox1);
            this.groupBox9.Controls.Add(this.label7);
            this.groupBox9.Controls.Add(this.FindItem_Btn);
            this.groupBox9.Controls.Add(this.ItemName_Text);
            this.groupBox9.Controls.Add(this.label18);
            this.groupBox9.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox9.Location = new System.Drawing.Point(828, 95);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(711, 186);
            this.groupBox9.TabIndex = 21;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Tìm Kiếm Nhanh";
            // 
            // TemplateID_CBox
            // 
            this.TemplateID_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TemplateID_CBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TemplateID_CBox.FormattingEnabled = true;
            this.TemplateID_CBox.Location = new System.Drawing.Point(279, 116);
            this.TemplateID_CBox.Name = "TemplateID_CBox";
            this.TemplateID_CBox.Size = new System.Drawing.Size(257, 34);
            this.TemplateID_CBox.TabIndex = 27;
            this.TemplateID_CBox.SelectedIndexChanged += new System.EventHandler(this.TemplateID_CBox_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(577, 53);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(110, 113);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(31, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(207, 26);
            this.label7.TabIndex = 4;
            this.label7.Text = "Items được tìm thấy：";
            // 
            // FindItem_Btn
            // 
            this.FindItem_Btn.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FindItem_Btn.Location = new System.Drawing.Point(365, 56);
            this.FindItem_Btn.Name = "FindItem_Btn";
            this.FindItem_Btn.Size = new System.Drawing.Size(171, 34);
            this.FindItem_Btn.TabIndex = 2;
            this.FindItem_Btn.Text = "Tìm Kiếm";
            this.FindItem_Btn.UseVisualStyleBackColor = true;
            this.FindItem_Btn.Click += new System.EventHandler(this.FindItem_Btn_Click);
            // 
            // ItemName_Text
            // 
            this.ItemName_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemName_Text.Location = new System.Drawing.Point(162, 56);
            this.ItemName_Text.Name = "ItemName_Text";
            this.ItemName_Text.Size = new System.Drawing.Size(152, 34);
            this.ItemName_Text.TabIndex = 1;
            this.ItemName_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(31, 59);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(115, 26);
            this.label18.TabIndex = 0;
            this.label18.Text = "Nhập Tên：";
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.label8);
            this.groupBox15.Controls.Add(this.Stop);
            this.groupBox15.Controls.Add(this.Start);
            this.groupBox15.Controls.Add(this.textBox1);
            this.groupBox15.Controls.Add(this.label51);
            this.groupBox15.Controls.Add(this.time1);
            this.groupBox15.Controls.Add(this.tempid1);
            this.groupBox15.Controls.Add(this.SendTime);
            this.groupBox15.Controls.Add(this.label50);
            this.groupBox15.Controls.Add(this.label49);
            this.groupBox15.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox15.Location = new System.Drawing.Point(356, 288);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(447, 258);
            this.groupBox15.TabIndex = 19;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Bong Bóng";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(290, 94);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 26);
            this.label8.TabIndex = 21;
            this.label8.Text = "phút";
            // 
            // Stop
            // 
            this.Stop.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Stop.Location = new System.Drawing.Point(265, 191);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(109, 48);
            this.Stop.TabIndex = 20;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // Start
            // 
            this.Start.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Start.Location = new System.Drawing.Point(81, 191);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(109, 48);
            this.Start.TabIndex = 19;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(374, 33);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(39, 34);
            this.textBox1.TabIndex = 18;
            this.textBox1.Text = "1";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label51.ForeColor = System.Drawing.Color.Black;
            this.label51.Location = new System.Drawing.Point(290, 36);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(58, 26);
            this.label51.TabIndex = 17;
            this.label51.Text = "STT:";
            // 
            // time1
            // 
            this.time1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.time1.Location = new System.Drawing.Point(155, 91);
            this.time1.Name = "time1";
            this.time1.Size = new System.Drawing.Size(104, 34);
            this.time1.TabIndex = 16;
            this.time1.Text = "5";
            // 
            // tempid1
            // 
            this.tempid1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tempid1.Location = new System.Drawing.Point(155, 33);
            this.tempid1.Name = "tempid1";
            this.tempid1.Size = new System.Drawing.Size(104, 34);
            this.tempid1.TabIndex = 15;
            // 
            // SendTime
            // 
            this.SendTime.BackColor = System.Drawing.Color.White;
            this.SendTime.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendTime.ForeColor = System.Drawing.Color.Violet;
            this.SendTime.Location = new System.Drawing.Point(46, 149);
            this.SendTime.Name = "SendTime";
            this.SendTime.Size = new System.Drawing.Size(367, 24);
            this.SendTime.TabIndex = 14;
            this.SendTime.Text = "Điểm bong bóng thành công 0 lần";
            this.SendTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.SendTime.Visible = false;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.ForeColor = System.Drawing.Color.Black;
            this.label50.Location = new System.Drawing.Point(27, 97);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(107, 26);
            this.label50.TabIndex = 3;
            this.label50.Text = "Thời gian:";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label49.ForeColor = System.Drawing.Color.Black;
            this.label49.Location = new System.Drawing.Point(36, 36);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(98, 26);
            this.label49.TabIndex = 1;
            this.label49.Text = "Nhập ID:";
            // 
            // groupBox16
            // 
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
            this.groupBox16.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox16.ForeColor = System.Drawing.Color.Black;
            this.groupBox16.Location = new System.Drawing.Point(828, 287);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(1062, 404);
            this.groupBox16.TabIndex = 20;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Thông Tin Gửi";
            // 
            // SendItemToPlayer
            // 
            this.SendItemToPlayer.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendItemToPlayer.Location = new System.Drawing.Point(479, 333);
            this.SendItemToPlayer.Name = "SendItemToPlayer";
            this.SendItemToPlayer.Size = new System.Drawing.Size(163, 55);
            this.SendItemToPlayer.TabIndex = 28;
            this.SendItemToPlayer.Text = "Gửi";
            this.SendItemToPlayer.UseVisualStyleBackColor = true;
            this.SendItemToPlayer.Click += new System.EventHandler(this.SendItemToPlayer_Click);
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label52.Location = new System.Drawing.Point(682, 311);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(115, 26);
            this.label52.TabIndex = 27;
            this.label52.Text = "Tình trạng:";
            // 
            // IsBind
            // 
            this.IsBind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IsBind.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IsBind.FormattingEnabled = true;
            this.IsBind.Items.AddRange(new object[] {
            "Khóa",
            "Không Khóa"});
            this.IsBind.Location = new System.Drawing.Point(848, 308);
            this.IsBind.Name = "IsBind";
            this.IsBind.Size = new System.Drawing.Size(142, 34);
            this.IsBind.TabIndex = 26;
            // 
            // textBox5
            // 
            this.textBox5.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.Location = new System.Drawing.Point(300, 308);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(137, 34);
            this.textBox5.TabIndex = 10;
            this.textBox5.Text = "0";
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox12
            // 
            this.textBox12.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox12.Location = new System.Drawing.Point(848, 254);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(142, 34);
            this.textBox12.TabIndex = 24;
            this.textBox12.Text = "50";
            this.textBox12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label53.ForeColor = System.Drawing.Color.Black;
            this.label53.Location = new System.Drawing.Point(693, 257);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(104, 26);
            this.label53.TabIndex = 23;
            this.label53.Text = "May mắn:";
            // 
            // textBox11
            // 
            this.textBox11.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox11.Location = new System.Drawing.Point(300, 254);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(137, 34);
            this.textBox11.TabIndex = 22;
            this.textBox11.Text = "50";
            this.textBox11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label54.ForeColor = System.Drawing.Color.Black;
            this.label54.Location = new System.Drawing.Point(125, 257);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(132, 26);
            this.label54.TabIndex = 21;
            this.label54.Text = "Nhanh nhẹn:";
            // 
            // textBox10
            // 
            this.textBox10.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox10.Location = new System.Drawing.Point(848, 198);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(142, 34);
            this.textBox10.TabIndex = 20;
            this.textBox10.Text = "50";
            this.textBox10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label55.ForeColor = System.Drawing.Color.Black;
            this.label55.Location = new System.Drawing.Point(683, 201);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(114, 26);
            this.label55.TabIndex = 19;
            this.label55.Text = "Phòng thủ:";
            // 
            // textBox9
            // 
            this.textBox9.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox9.Location = new System.Drawing.Point(300, 198);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(137, 34);
            this.textBox9.TabIndex = 18;
            this.textBox9.Text = "50";
            this.textBox9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label56.ForeColor = System.Drawing.Color.Black;
            this.label56.Location = new System.Drawing.Point(152, 201);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(105, 26);
            this.label56.TabIndex = 17;
            this.label56.Text = "Tấn công:";
            // 
            // textBox8
            // 
            this.textBox8.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox8.Location = new System.Drawing.Point(848, 87);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(142, 34);
            this.textBox8.TabIndex = 16;
            this.textBox8.Text = "0";
            this.textBox8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label57.ForeColor = System.Drawing.Color.Black;
            this.label57.Location = new System.Drawing.Point(712, 90);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(85, 26);
            this.label57.TabIndex = 15;
            this.label57.Text = "Cấp độ:";
            // 
            // textBox7
            // 
            this.textBox7.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox7.Location = new System.Drawing.Point(848, 142);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(142, 34);
            this.textBox7.TabIndex = 14;
            this.textBox7.Text = "0";
            this.textBox7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label58.ForeColor = System.Drawing.Color.Black;
            this.label58.Location = new System.Drawing.Point(751, 145);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(46, 26);
            this.label58.TabIndex = 13;
            this.label58.Text = "Xu:";
            // 
            // textBox6
            // 
            this.textBox6.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.Location = new System.Drawing.Point(300, 142);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(137, 34);
            this.textBox6.TabIndex = 12;
            this.textBox6.Text = "0";
            this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label59.ForeColor = System.Drawing.Color.Black;
            this.label59.Location = new System.Drawing.Point(190, 145);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(67, 26);
            this.label59.TabIndex = 11;
            this.label59.Text = "Vàng:";
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label60.ForeColor = System.Drawing.Color.Black;
            this.label60.Location = new System.Drawing.Point(162, 311);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(95, 26);
            this.label60.TabIndex = 9;
            this.label60.Text = "Hiệu lực:";
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(300, 87);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(137, 34);
            this.textBox3.TabIndex = 8;
            this.textBox3.Text = "1";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label61.ForeColor = System.Drawing.Color.Black;
            this.label61.Location = new System.Drawing.Point(155, 90);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(102, 26);
            this.label61.TabIndex = 7;
            this.label61.Text = "Số lượng:";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(848, 33);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(142, 34);
            this.textBox2.TabIndex = 6;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label62.ForeColor = System.Drawing.Color.Black;
            this.label62.Location = new System.Drawing.Point(755, 36);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(42, 26);
            this.label62.TabIndex = 5;
            this.label62.Text = "ID:";
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label63.ForeColor = System.Drawing.Color.Black;
            this.label63.Location = new System.Drawing.Point(89, 36);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(168, 26);
            this.label63.TabIndex = 0;
            this.label63.Text = "Tên Người Chơi:";
            // 
            // playernickname
            // 
            this.playernickname.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playernickname.Location = new System.Drawing.Point(300, 33);
            this.playernickname.Name = "playernickname";
            this.playernickname.Size = new System.Drawing.Size(137, 34);
            this.playernickname.TabIndex = 1;
            this.playernickname.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label48);
            this.panel1.Controls.Add(this.label47);
            this.panel1.Controls.Add(this.label46);
            this.panel1.Controls.Add(this.time);
            this.panel1.Controls.Add(this.ShutDown);
            this.panel1.Controls.Add(this.ReStart);
            this.panel1.Controls.Add(this.label45);
            this.panel1.Controls.Add(this.label44);
            this.panel1.Location = new System.Drawing.Point(24, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1866, 68);
            this.panel1.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(527, 14);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(226, 33);
            this.label10.TabIndex = 19;
            this.label10.Text = "Thời Gian Bảo Trì:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(928, 14);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 33);
            this.label9.TabIndex = 18;
            this.label9.Text = "Phút";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label48.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label48.Location = new System.Drawing.Point(1519, 31);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(178, 25);
            this.label48.TabIndex = 17;
            this.label48.Text = "Bảo Trì: 10 phút";
            this.label48.Visible = false;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.ForeColor = System.Drawing.Color.Green;
            this.label47.Location = new System.Drawing.Point(1750, 6);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(77, 25);
            this.label47.TabIndex = 16;
            this.label47.Text = "Online";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label46.Location = new System.Drawing.Point(1519, 6);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(206, 25);
            this.label46.TabIndex = 15;
            this.label46.Text = "Trạng Thái Server：";
            // 
            // time
            // 
            this.time.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.time.Location = new System.Drawing.Point(815, 14);
            this.time.Name = "time";
            this.time.Size = new System.Drawing.Size(94, 36);
            this.time.TabIndex = 14;
            this.time.Text = "10";
            this.time.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ShutDown
            // 
            this.ShutDown.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShutDown.Location = new System.Drawing.Point(1083, 13);
            this.ShutDown.Name = "ShutDown";
            this.ShutDown.Size = new System.Drawing.Size(140, 34);
            this.ShutDown.TabIndex = 3;
            this.ShutDown.Text = "Bảo Trì";
            this.ShutDown.UseVisualStyleBackColor = true;
            this.ShutDown.Click += new System.EventHandler(this.ShutDown_Click);
            // 
            // ReStart
            // 
            this.ReStart.Enabled = false;
            this.ReStart.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReStart.Location = new System.Drawing.Point(1272, 13);
            this.ReStart.Name = "ReStart";
            this.ReStart.Size = new System.Drawing.Size(140, 34);
            this.ReStart.TabIndex = 2;
            this.ReStart.Text = "Restart";
            this.ReStart.UseVisualStyleBackColor = true;
            this.ReStart.Click += new System.EventHandler(this.ReStart_Click);
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.Location = new System.Drawing.Point(320, 13);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(106, 32);
            this.label45.TabIndex = 1;
            this.label45.Text = "0 người";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.Location = new System.Drawing.Point(3, 13);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(262, 32);
            this.label44.TabIndex = 0;
            this.label44.Text = "Người dùng Online：";
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.label11);
            this.groupBox14.Controls.Add(this.SendAnnouncement);
            this.groupBox14.Controls.Add(this.richTextBox1);
            this.groupBox14.Controls.Add(this.ListType);
            this.groupBox14.Controls.Add(this.label43);
            this.groupBox14.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox14.Location = new System.Drawing.Point(24, 697);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(779, 241);
            this.groupBox14.TabIndex = 17;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Thông Báo";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(65, 114);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(109, 26);
            this.label11.TabIndex = 12;
            this.label11.Text = "Nội Dung:";
            // 
            // SendAnnouncement
            // 
            this.SendAnnouncement.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendAnnouncement.Location = new System.Drawing.Point(201, 175);
            this.SendAnnouncement.Name = "SendAnnouncement";
            this.SendAnnouncement.Size = new System.Drawing.Size(225, 49);
            this.SendAnnouncement.TabIndex = 11;
            this.SendAnnouncement.Text = "Gửi Thông Báo";
            this.SendAnnouncement.UseVisualStyleBackColor = true;
            this.SendAnnouncement.Click += new System.EventHandler(this.SendAnnouncement_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(201, 111);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(544, 42);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "[Admin] : ";
            // 
            // ListType
            // 
            this.ListType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListType.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListType.FormattingEnabled = true;
            this.ListType.Items.AddRange(new object[] {
            "Màn hình nhỏ",
            "Pop-up",
            "Hiện Tại",
            "Chat Riêng"});
            this.ListType.Location = new System.Drawing.Point(201, 38);
            this.ListType.Name = "ListType";
            this.ListType.Size = new System.Drawing.Size(225, 34);
            this.ListType.TabIndex = 9;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(73, 41);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(101, 26);
            this.label43.TabIndex = 0;
            this.label43.Text = "Thể Loại:";
            // 
            // playerBox
            // 
            this.playerBox.Controls.Add(this.button1);
            this.playerBox.Controls.Add(this.Ban);
            this.playerBox.Controls.Add(this.Kick);
            this.playerBox.Controls.Add(this.dateTimePicker1);
            this.playerBox.Controls.Add(this.label1);
            this.playerBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerBox.Location = new System.Drawing.Point(356, 95);
            this.playerBox.Name = "playerBox";
            this.playerBox.Size = new System.Drawing.Size(447, 187);
            this.playerBox.TabIndex = 16;
            this.playerBox.TabStop = false;
            this.playerBox.Text = "Player";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(324, 132);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 36);
            this.button1.TabIndex = 5;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Ban
            // 
            this.Ban.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ban.Location = new System.Drawing.Point(179, 132);
            this.Ban.Name = "Ban";
            this.Ban.Size = new System.Drawing.Size(92, 36);
            this.Ban.TabIndex = 4;
            this.Ban.Text = "Ban";
            this.Ban.UseVisualStyleBackColor = true;
            this.Ban.Click += new System.EventHandler(this.Ban_Click);
            // 
            // Kick
            // 
            this.Kick.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Kick.Location = new System.Drawing.Point(32, 132);
            this.Kick.Name = "Kick";
            this.Kick.Size = new System.Drawing.Size(91, 36);
            this.Kick.TabIndex = 3;
            this.Kick.Text = "Kick";
            this.Kick.UseVisualStyleBackColor = true;
            this.Kick.Click += new System.EventHandler(this.Kick_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.Location = new System.Drawing.Point(32, 72);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(389, 34);
            this.dateTimePicker1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ngày/Tháng/Năm:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.UpdateLogs);
            this.groupBox2.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(356, 551);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(447, 140);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Nhật Kí Cập Nhật";
            // 
            // UpdateLogs
            // 
            this.UpdateLogs.BackColor = System.Drawing.SystemColors.Window;
            this.UpdateLogs.Enabled = false;
            this.UpdateLogs.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateLogs.Location = new System.Drawing.Point(58, 44);
            this.UpdateLogs.Multiline = true;
            this.UpdateLogs.Name = "UpdateLogs";
            this.UpdateLogs.ReadOnly = true;
            this.UpdateLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.UpdateLogs.Size = new System.Drawing.Size(290, 66);
            this.UpdateLogs.TabIndex = 0;
            this.UpdateLogs.Text = "1.Cập nhật Tool GM mới\r\n2.Thuật toán tối ưu";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ListOnlinePlayers);
            this.groupBox1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(24, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 597);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Danh Sách Player Online";
            // 
            // ListOnlinePlayers
            // 
            this.ListOnlinePlayers.BackColor = System.Drawing.SystemColors.Window;
            this.ListOnlinePlayers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ListOnlinePlayers.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListOnlinePlayers.FormattingEnabled = true;
            this.ListOnlinePlayers.ItemHeight = 31;
            this.ListOnlinePlayers.Items.AddRange(new object[] {
            "Player1",
            "Player2",
            "Player3",
            "Player4",
            "Player5",
            "Player6",
            "Player7",
            "Player8",
            "Player9",
            "Player10"});
            this.ListOnlinePlayers.Location = new System.Drawing.Point(6, 46);
            this.ListOnlinePlayers.Name = "ListOnlinePlayers";
            this.ListOnlinePlayers.Size = new System.Drawing.Size(302, 434);
            this.ListOnlinePlayers.TabIndex = 0;
            this.ListOnlinePlayers.SelectedIndexChanged += new System.EventHandler(this.ListOnlinePlayers_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox17);
            this.tabPage2.Controls.Add(this.groupBox19);
            this.tabPage2.Controls.Add(this.NewPlyaerFightPowerRank);
            this.tabPage2.Controls.Add(this.groupBox13);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Location = new System.Drawing.Point(4, 35);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1916, 1016);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Player";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.button30);
            this.groupBox17.Controls.Add(this.button29);
            this.groupBox17.Location = new System.Drawing.Point(743, 354);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(626, 137);
            this.groupBox17.TabIndex = 29;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "Đấu Đội";
            // 
            // button30
            // 
            this.button30.Location = new System.Drawing.Point(88, 78);
            this.button30.Name = "button30";
            this.button30.Size = new System.Drawing.Size(472, 38);
            this.button30.TabIndex = 29;
            this.button30.Text = "Phần thưởng xếp hạng cho mùa giải này";
            this.button30.UseVisualStyleBackColor = true;
            this.button30.Click += new System.EventHandler(this.button30_Click);
            // 
            // button29
            // 
            this.button29.Location = new System.Drawing.Point(88, 31);
            this.button29.Name = "button29";
            this.button29.Size = new System.Drawing.Size(472, 35);
            this.button29.TabIndex = 28;
            this.button29.Text = "Phần thưởng xếp hạng đấu đội tuần này";
            this.button29.UseVisualStyleBackColor = true;
            this.button29.Click += new System.EventHandler(this.button29_Click);
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.ChangePassword);
            this.groupBox19.Controls.Add(this.Password);
            this.groupBox19.Controls.Add(this.label66);
            this.groupBox19.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox19.Location = new System.Drawing.Point(743, 126);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Size = new System.Drawing.Size(626, 96);
            this.groupBox19.TabIndex = 26;
            this.groupBox19.TabStop = false;
            this.groupBox19.Text = "Mật Khẩu";
            // 
            // ChangePassword
            // 
            this.ChangePassword.Location = new System.Drawing.Point(392, 33);
            this.ChangePassword.Name = "ChangePassword";
            this.ChangePassword.Size = new System.Drawing.Size(190, 41);
            this.ChangePassword.TabIndex = 3;
            this.ChangePassword.Text = "Đổi mật khẩu";
            this.ChangePassword.UseVisualStyleBackColor = true;
            this.ChangePassword.Click += new System.EventHandler(this.ChangePassword_Click);
            // 
            // Password
            // 
            this.Password.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Password.Location = new System.Drawing.Point(216, 33);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(122, 34);
            this.Password.TabIndex = 1;
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label66.Location = new System.Drawing.Point(25, 40);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(152, 26);
            this.label66.TabIndex = 0;
            this.label66.Text = "Mật Khẩu Mới:";
            // 
            // NewPlyaerFightPowerRank
            // 
            this.NewPlyaerFightPowerRank.AllowUserToAddRows = false;
            this.NewPlyaerFightPowerRank.AllowUserToDeleteRows = false;
            this.NewPlyaerFightPowerRank.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.NewPlyaerFightPowerRank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.NewPlyaerFightPowerRank.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column2,
            this.Column3,
            this.Column4});
            this.NewPlyaerFightPowerRank.Location = new System.Drawing.Point(743, 530);
            this.NewPlyaerFightPowerRank.Name = "NewPlyaerFightPowerRank";
            this.NewPlyaerFightPowerRank.ReadOnly = true;
            this.NewPlyaerFightPowerRank.RowHeadersWidth = 51;
            this.NewPlyaerFightPowerRank.RowTemplate.Height = 23;
            this.NewPlyaerFightPowerRank.Size = new System.Drawing.Size(1165, 204);
            this.NewPlyaerFightPowerRank.TabIndex = 13;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "UserName";
            this.Column2.HeaderText = "Tên Tài Khoản";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "NickName";
            this.Column3.HeaderText = "Tên Người Chơi";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "FightPower";
            this.Column4.HeaderText = "Lực Chiến";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.CleanAcitveNum_Btn);
            this.groupBox13.Controls.Add(this.ListActiveID_CBox);
            this.groupBox13.Controls.Add(this.label42);
            this.groupBox13.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox13.Location = new System.Drawing.Point(743, 238);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(626, 95);
            this.groupBox13.TabIndex = 25;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Xóa Giờ Event(Mục Sự Kiện)";
            // 
            // CleanAcitveNum_Btn
            // 
            this.CleanAcitveNum_Btn.Location = new System.Drawing.Point(392, 33);
            this.CleanAcitveNum_Btn.Name = "CleanAcitveNum_Btn";
            this.CleanAcitveNum_Btn.Size = new System.Drawing.Size(190, 45);
            this.CleanAcitveNum_Btn.TabIndex = 2;
            this.CleanAcitveNum_Btn.Text = "Cleaning";
            this.CleanAcitveNum_Btn.UseVisualStyleBackColor = true;
            this.CleanAcitveNum_Btn.Click += new System.EventHandler(this.CleanAcitveNum_Btn_Click);
            // 
            // ListActiveID_CBox
            // 
            this.ListActiveID_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListActiveID_CBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListActiveID_CBox.FormattingEnabled = true;
            this.ListActiveID_CBox.Location = new System.Drawing.Point(216, 39);
            this.ListActiveID_CBox.Name = "ListActiveID_CBox";
            this.ListActiveID_CBox.Size = new System.Drawing.Size(122, 34);
            this.ListActiveID_CBox.TabIndex = 1;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(52, 41);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(125, 26);
            this.label42.TabIndex = 0;
            this.label42.Text = "Hoạt Động: ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.FindUser_Btn);
            this.groupBox4.Controls.Add(this.UserName_Text);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(7, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(715, 111);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Player";
            // 
            // FindUser_Btn
            // 
            this.FindUser_Btn.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FindUser_Btn.Location = new System.Drawing.Point(483, 38);
            this.FindUser_Btn.Name = "FindUser_Btn";
            this.FindUser_Btn.Size = new System.Drawing.Size(190, 41);
            this.FindUser_Btn.TabIndex = 2;
            this.FindUser_Btn.Text = "Đăng Nhập";
            this.FindUser_Btn.UseVisualStyleBackColor = true;
            this.FindUser_Btn.Click += new System.EventHandler(this.FindUser_Btn_Click);
            // 
            // UserName_Text
            // 
            this.UserName_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserName_Text.Location = new System.Drawing.Point(242, 42);
            this.UserName_Text.Name = "UserName_Text";
            this.UserName_Text.Size = new System.Drawing.Size(183, 34);
            this.UserName_Text.TabIndex = 1;
            this.UserName_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(35, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(170, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "Nhập UserName:";
            // 
            // groupBox5
            // 
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
            this.groupBox5.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(8, 126);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(714, 365);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Thông Tin Player";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(360, 164);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(115, 43);
            this.button3.TabIndex = 27;
            this.button3.Text = "Xóa";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Thô",
            "Thường"});
            this.comboBox1.Location = new System.Drawing.Point(204, 169);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(126, 34);
            this.comboBox1.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(53, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 26);
            this.label4.TabIndex = 27;
            this.label4.Text = "Ấn cần xóa:";
            // 
            // RecoverMessage
            // 
            this.RecoverMessage.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RecoverMessage.Location = new System.Drawing.Point(115, 228);
            this.RecoverMessage.Name = "RecoverMessage";
            this.RecoverMessage.Size = new System.Drawing.Size(492, 44);
            this.RecoverMessage.TabIndex = 29;
            this.RecoverMessage.Text = "Khôi phục Email cho người chơi nếu lỡ xóa";
            this.RecoverMessage.UseVisualStyleBackColor = true;
            this.RecoverMessage.Click += new System.EventHandler(this.RecoverMessage_Click);
            // 
            // PasswordTwo_Text
            // 
            this.PasswordTwo_Text.Enabled = false;
            this.PasswordTwo_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordTwo_Text.Location = new System.Drawing.Point(572, 31);
            this.PasswordTwo_Text.Name = "PasswordTwo_Text";
            this.PasswordTwo_Text.Size = new System.Drawing.Size(120, 34);
            this.PasswordTwo_Text.TabIndex = 9;
            this.PasswordTwo_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(355, 34);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(177, 26);
            this.label15.TabIndex = 8;
            this.label15.Text = "Mật khẩu cấp hai:";
            // 
            // CleanUser_Text
            // 
            this.CleanUser_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CleanUser_Text.Location = new System.Drawing.Point(403, 295);
            this.CleanUser_Text.Name = "CleanUser_Text";
            this.CleanUser_Text.Size = new System.Drawing.Size(147, 49);
            this.CleanUser_Text.TabIndex = 7;
            this.CleanUser_Text.Text = "Đăng Xuất";
            this.CleanUser_Text.UseVisualStyleBackColor = true;
            this.CleanUser_Text.Click += new System.EventHandler(this.CleanUser_Text_Click);
            // 
            // UpdateLevel_Btn
            // 
            this.UpdateLevel_Btn.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateLevel_Btn.Location = new System.Drawing.Point(360, 93);
            this.UpdateLevel_Btn.Name = "UpdateLevel_Btn";
            this.UpdateLevel_Btn.Size = new System.Drawing.Size(190, 47);
            this.UpdateLevel_Btn.TabIndex = 6;
            this.UpdateLevel_Btn.Text = "Update Level";
            this.UpdateLevel_Btn.UseVisualStyleBackColor = true;
            this.UpdateLevel_Btn.Click += new System.EventHandler(this.UpdateLevel_Btn_Click);
            // 
            // Level_Text
            // 
            this.Level_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Level_Text.Location = new System.Drawing.Point(203, 100);
            this.Level_Text.Name = "Level_Text";
            this.Level_Text.Size = new System.Drawing.Size(127, 34);
            this.Level_Text.TabIndex = 5;
            this.Level_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // NickName_Text
            // 
            this.NickName_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NickName_Text.Location = new System.Drawing.Point(204, 31);
            this.NickName_Text.Name = "NickName_Text";
            this.NickName_Text.Size = new System.Drawing.Size(126, 34);
            this.NickName_Text.TabIndex = 4;
            this.NickName_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // UpUserData_Btn
            // 
            this.UpUserData_Btn.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpUserData_Btn.Location = new System.Drawing.Point(183, 295);
            this.UpUserData_Btn.Name = "UpUserData_Btn";
            this.UpUserData_Btn.Size = new System.Drawing.Size(147, 49);
            this.UpUserData_Btn.TabIndex = 3;
            this.UpUserData_Btn.Text = "Cập Nhật";
            this.UpUserData_Btn.UseVisualStyleBackColor = true;
            this.UpUserData_Btn.Click += new System.EventHandler(this.UpUserData_Btn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(110, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 26);
            this.label3.TabIndex = 1;
            this.label3.Text = "Level:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(173, 26);
            this.label5.TabIndex = 0;
            this.label5.Text = "Tên Người Chơi：";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.FindRank_Btn);
            this.groupBox6.Controls.Add(this.EndTime_TimeDate);
            this.groupBox6.Controls.Add(this.StartTime_TimeDate);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(8, 515);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(714, 219);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Bảng Xếp Hạng Người Chơi Mới";
            // 
            // FindRank_Btn
            // 
            this.FindRank_Btn.Location = new System.Drawing.Point(256, 152);
            this.FindRank_Btn.Name = "FindRank_Btn";
            this.FindRank_Btn.Size = new System.Drawing.Size(180, 47);
            this.FindRank_Btn.TabIndex = 4;
            this.FindRank_Btn.Text = "Tìm Kiếm";
            this.FindRank_Btn.UseVisualStyleBackColor = true;
            this.FindRank_Btn.Click += new System.EventHandler(this.FindRank_Btn_Click);
            // 
            // EndTime_TimeDate
            // 
            this.EndTime_TimeDate.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndTime_TimeDate.Location = new System.Drawing.Point(256, 100);
            this.EndTime_TimeDate.Name = "EndTime_TimeDate";
            this.EndTime_TimeDate.Size = new System.Drawing.Size(436, 34);
            this.EndTime_TimeDate.TabIndex = 3;
            // 
            // StartTime_TimeDate
            // 
            this.StartTime_TimeDate.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartTime_TimeDate.Location = new System.Drawing.Point(256, 39);
            this.StartTime_TimeDate.Name = "StartTime_TimeDate";
            this.StartTime_TimeDate.Size = new System.Drawing.Size(436, 34);
            this.StartTime_TimeDate.TabIndex = 2;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(12, 106);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(204, 26);
            this.label17.TabIndex = 1;
            this.label17.Text = "Thời Gian Kết Thúc:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(23, 45);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(193, 26);
            this.label16.TabIndex = 0;
            this.label16.Text = "Thời Gian Bắt Đầu:";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.UserID_Lab);
            this.groupBox7.Controls.Add(this.Label_d);
            this.groupBox7.Controls.Add(this.Online_Text);
            this.groupBox7.Controls.Add(this.label6);
            this.groupBox7.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox7.Location = new System.Drawing.Point(743, 6);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(626, 111);
            this.groupBox7.TabIndex = 9;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Trạng Thái Player";
            // 
            // UserID_Lab
            // 
            this.UserID_Lab.AutoSize = true;
            this.UserID_Lab.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserID_Lab.Location = new System.Drawing.Point(154, 46);
            this.UserID_Lab.Name = "UserID_Lab";
            this.UserID_Lab.Size = new System.Drawing.Size(23, 26);
            this.UserID_Lab.TabIndex = 3;
            this.UserID_Lab.Text = "0";
            // 
            // Label_d
            // 
            this.Label_d.AutoSize = true;
            this.Label_d.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_d.Location = new System.Drawing.Point(42, 46);
            this.Label_d.Name = "Label_d";
            this.Label_d.Size = new System.Drawing.Size(89, 26);
            this.Label_d.TabIndex = 2;
            this.Label_d.Text = "UserID：";
            // 
            // Online_Text
            // 
            this.Online_Text.AutoSize = true;
            this.Online_Text.ForeColor = System.Drawing.Color.Red;
            this.Online_Text.Location = new System.Drawing.Point(502, 45);
            this.Online_Text.Name = "Online_Text";
            this.Online_Text.Size = new System.Drawing.Size(80, 25);
            this.Online_Text.TabIndex = 1;
            this.Online_Text.Text = "Offline";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(341, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 26);
            this.label6.TabIndex = 0;
            this.label6.Text = "Trạng Thái:";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox20);
            this.tabPage3.Controls.Add(this.groupBox18);
            this.tabPage3.Controls.Add(this.groupBox10);
            this.tabPage3.Controls.Add(this.groupBox11);
            this.tabPage3.Controls.Add(this.groupBox12);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 35);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1916, 1016);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Editor";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.button4);
            this.groupBox20.Controls.Add(this.CreateAllCeleb);
            this.groupBox20.Controls.Add(this.Reload);
            this.groupBox20.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox20.Location = new System.Drawing.Point(1540, 28);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Size = new System.Drawing.Size(355, 195);
            this.groupBox20.TabIndex = 32;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "Cập Nhật";
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(55, 86);
            this.button4.Name = "button4";
            this.button4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button4.Size = new System.Drawing.Size(250, 39);
            this.button4.TabIndex = 2;
            this.button4.Text = "BOSS Thế Giới";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // CreateAllCeleb
            // 
            this.CreateAllCeleb.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateAllCeleb.Location = new System.Drawing.Point(100, 137);
            this.CreateAllCeleb.Name = "CreateAllCeleb";
            this.CreateAllCeleb.Size = new System.Drawing.Size(168, 39);
            this.CreateAllCeleb.TabIndex = 1;
            this.CreateAllCeleb.Text = "Chủ Đề";
            this.CreateAllCeleb.UseVisualStyleBackColor = true;
            this.CreateAllCeleb.Click += new System.EventHandler(this.CreateAllCeleb_Click);
            // 
            // Reload
            // 
            this.Reload.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reload.Location = new System.Drawing.Point(100, 30);
            this.Reload.Name = "Reload";
            this.Reload.Size = new System.Drawing.Size(168, 39);
            this.Reload.TabIndex = 0;
            this.Reload.Text = "Đồng Bộ";
            this.Reload.UseVisualStyleBackColor = true;
            this.Reload.Click += new System.EventHandler(this.Reload_Click);
            // 
            // groupBox18
            // 
            this.groupBox18.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox18.Location = new System.Drawing.Point(1540, 252);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Size = new System.Drawing.Size(355, 678);
            this.groupBox18.TabIndex = 31;
            this.groupBox18.TabStop = false;
            this.groupBox18.Text = "Nhật Ký";
            // 
            // groupBox10
            // 
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
            this.groupBox10.Controls.Add(this.pictureBox3);
            this.groupBox10.Controls.Add(this.ItemArmor_Text);
            this.groupBox10.Controls.Add(this.label40);
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
            this.groupBox10.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox10.Location = new System.Drawing.Point(780, 28);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(723, 771);
            this.groupBox10.TabIndex = 28;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Tùy Chọn Trang Phục";
            // 
            // NewItemID_TextBox
            // 
            this.NewItemID_TextBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemID_TextBox.Location = new System.Drawing.Point(209, 684);
            this.NewItemID_TextBox.Name = "NewItemID_TextBox";
            this.NewItemID_TextBox.Size = new System.Drawing.Size(268, 34);
            this.NewItemID_TextBox.TabIndex = 34;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.Location = new System.Drawing.Point(69, 687);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(89, 26);
            this.label41.TabIndex = 33;
            this.label41.Text = "Tạo ID：";
            // 
            // ItemQuality_CBox
            // 
            this.ItemQuality_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ItemQuality_CBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemQuality_CBox.FormattingEnabled = true;
            this.ItemQuality_CBox.Items.AddRange(new object[] {
            "Thô",
            "Thường",
            "Ưu Tú",
            "Hoàn Mỹ",
            "Siêu Việt",
            "Truyền Thuyết",
            "Hiện Vật"});
            this.ItemQuality_CBox.Location = new System.Drawing.Point(209, 306);
            this.ItemQuality_CBox.Name = "ItemQuality_CBox";
            this.ItemQuality_CBox.Size = new System.Drawing.Size(174, 34);
            this.ItemQuality_CBox.TabIndex = 30;
            // 
            // InsertItem_Btn
            // 
            this.InsertItem_Btn.Location = new System.Drawing.Point(528, 679);
            this.InsertItem_Btn.Name = "InsertItem_Btn";
            this.InsertItem_Btn.Size = new System.Drawing.Size(173, 41);
            this.InsertItem_Btn.TabIndex = 29;
            this.InsertItem_Btn.Text = "Random ID";
            this.InsertItem_Btn.UseVisualStyleBackColor = true;
            this.InsertItem_Btn.Click += new System.EventHandler(this.InsertItem_Btn_Click);
            // 
            // ItemLuck_TextBox
            // 
            this.ItemLuck_TextBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemLuck_TextBox.Location = new System.Drawing.Point(565, 604);
            this.ItemLuck_TextBox.Name = "ItemLuck_TextBox";
            this.ItemLuck_TextBox.Size = new System.Drawing.Size(136, 34);
            this.ItemLuck_TextBox.TabIndex = 28;
            this.ItemLuck_TextBox.Text = "2147483648";
            this.ItemLuck_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ItemAgi_TextBox
            // 
            this.ItemAgi_TextBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemAgi_TextBox.Location = new System.Drawing.Point(209, 604);
            this.ItemAgi_TextBox.Name = "ItemAgi_TextBox";
            this.ItemAgi_TextBox.Size = new System.Drawing.Size(141, 34);
            this.ItemAgi_TextBox.TabIndex = 27;
            this.ItemAgi_TextBox.Text = "2147483648";
            this.ItemAgi_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ItemDef_TextBox
            // 
            this.ItemDef_TextBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemDef_TextBox.Location = new System.Drawing.Point(565, 528);
            this.ItemDef_TextBox.Name = "ItemDef_TextBox";
            this.ItemDef_TextBox.Size = new System.Drawing.Size(136, 34);
            this.ItemDef_TextBox.TabIndex = 26;
            this.ItemDef_TextBox.Text = "2147483648";
            this.ItemDef_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ItemAtt_TextBox
            // 
            this.ItemAtt_TextBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemAtt_TextBox.Location = new System.Drawing.Point(209, 528);
            this.ItemAtt_TextBox.Name = "ItemAtt_TextBox";
            this.ItemAtt_TextBox.Size = new System.Drawing.Size(141, 34);
            this.ItemAtt_TextBox.TabIndex = 25;
            this.ItemAtt_TextBox.Text = "2147483648";
            this.ItemAtt_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ItemDesc_TextBox
            // 
            this.ItemDesc_TextBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemDesc_TextBox.Location = new System.Drawing.Point(209, 450);
            this.ItemDesc_TextBox.Name = "ItemDesc_TextBox";
            this.ItemDesc_TextBox.Size = new System.Drawing.Size(487, 34);
            this.ItemDesc_TextBox.TabIndex = 24;
            this.ItemDesc_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(38, 309);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(120, 26);
            this.label39.TabIndex = 23;
            this.label39.Text = "Phẩm Chất:";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(209, 155);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(174, 124);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 20;
            this.pictureBox3.TabStop = false;
            // 
            // ItemArmor_Text
            // 
            this.ItemArmor_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemArmor_Text.Location = new System.Drawing.Point(565, 166);
            this.ItemArmor_Text.Name = "ItemArmor_Text";
            this.ItemArmor_Text.Size = new System.Drawing.Size(131, 34);
            this.ItemArmor_Text.TabIndex = 22;
            this.ItemArmor_Text.Text = "2147483648";
            this.ItemArmor_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.Location = new System.Drawing.Point(448, 169);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(95, 26);
            this.label40.TabIndex = 21;
            this.label40.Text = "Hộ Giáp:";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(49, 166);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(109, 26);
            this.label38.TabIndex = 13;
            this.label38.Text = "Hình Ảnh:";
            // 
            // ItemName_TextBox
            // 
            this.ItemName_TextBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemName_TextBox.Location = new System.Drawing.Point(209, 368);
            this.ItemName_TextBox.Name = "ItemName_TextBox";
            this.ItemName_TextBox.Size = new System.Drawing.Size(487, 34);
            this.ItemName_TextBox.TabIndex = 12;
            this.ItemName_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.Location = new System.Drawing.Point(404, 609);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(109, 26);
            this.label37.TabIndex = 11;
            this.label37.Text = "May mắn：";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.Location = new System.Drawing.Point(21, 607);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(137, 26);
            this.label36.TabIndex = 10;
            this.label36.Text = "Nhanh nhẹn：";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(394, 536);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(119, 26);
            this.label35.TabIndex = 9;
            this.label35.Text = "Phòng thủ：";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(48, 536);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(110, 26);
            this.label34.TabIndex = 8;
            this.label34.Text = "Tấn công：";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(79, 450);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(79, 26);
            this.label33.TabIndex = 7;
            this.label33.Text = "Mô Tả:";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(104, 371);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(54, 26);
            this.label32.TabIndex = 6;
            this.label32.Text = "Tên:";
            // 
            // ListItem_CBox
            // 
            this.ListItem_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListItem_CBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListItem_CBox.FormattingEnabled = true;
            this.ListItem_CBox.Location = new System.Drawing.Point(209, 95);
            this.ListItem_CBox.Name = "ListItem_CBox";
            this.ListItem_CBox.Size = new System.Drawing.Size(487, 34);
            this.ListItem_CBox.TabIndex = 5;
            this.ListItem_CBox.SelectedIndexChanged += new System.EventHandler(this.ListItem_CBox_SelectedIndexChanged);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(59, 98);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(99, 26);
            this.label31.TabIndex = 4;
            this.label31.Text = "ID Items:";
            // 
            // ListCategoryID_CBox
            // 
            this.ListCategoryID_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListCategoryID_CBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListCategoryID_CBox.FormattingEnabled = true;
            this.ListCategoryID_CBox.Items.AddRange(new object[] {
            "1 -- Nón",
            "2 -- Kính",
            "3 -- Tóc",
            "4 -- Mặt",
            "5 -- Quần Áo",
            "6 -- Mắt",
            "8 -- Vòng Tay",
            "9 -- Nhẫn",
            "13 -- Set Quần Áo",
            "14 -- Dây Chuyền",
            "15 -- Cánh",
            "16 -- Bong Bóng"});
            this.ListCategoryID_CBox.Location = new System.Drawing.Point(209, 33);
            this.ListCategoryID_CBox.Name = "ListCategoryID_CBox";
            this.ListCategoryID_CBox.Size = new System.Drawing.Size(268, 34);
            this.ListCategoryID_CBox.TabIndex = 3;
            this.ListCategoryID_CBox.SelectedIndexChanged += new System.EventHandler(this.ListCategoryID_CBox_SelectedIndexChanged);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(57, 35);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(101, 26);
            this.label30.TabIndex = 0;
            this.label30.Text = "Thể Loại:";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.NewTemplateID_Text);
            this.groupBox11.Controls.Add(this.label29);
            this.groupBox11.Controls.Add(this.Special_Text);
            this.groupBox11.Controls.Add(this.CommonAddWound_Text);
            this.groupBox11.Controls.Add(this.CommonMultiBall_Text);
            this.groupBox11.Controls.Add(this.Common_Text);
            this.groupBox11.Controls.Add(this.InsertArm_Btn);
            this.groupBox11.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox11.Location = new System.Drawing.Point(19, 564);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(723, 235);
            this.groupBox11.TabIndex = 27;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Cấu Hình Hiệu Ứng";
            // 
            // NewTemplateID_Text
            // 
            this.NewTemplateID_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewTemplateID_Text.Location = new System.Drawing.Point(170, 159);
            this.NewTemplateID_Text.Name = "NewTemplateID_Text";
            this.NewTemplateID_Text.Size = new System.Drawing.Size(286, 34);
            this.NewTemplateID_Text.TabIndex = 32;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(46, 162);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(84, 26);
            this.label29.TabIndex = 31;
            this.label29.Text = "Tạo ID:";
            // 
            // Special_Text
            // 
            this.Special_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Special_Text.Location = new System.Drawing.Point(549, 73);
            this.Special_Text.Name = "Special_Text";
            this.Special_Text.Size = new System.Drawing.Size(114, 34);
            this.Special_Text.TabIndex = 30;
            this.Special_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CommonAddWound_Text
            // 
            this.CommonAddWound_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommonAddWound_Text.Location = new System.Drawing.Point(224, 73);
            this.CommonAddWound_Text.Name = "CommonAddWound_Text";
            this.CommonAddWound_Text.Size = new System.Drawing.Size(117, 34);
            this.CommonAddWound_Text.TabIndex = 29;
            this.CommonAddWound_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CommonMultiBall_Text
            // 
            this.CommonMultiBall_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommonMultiBall_Text.Location = new System.Drawing.Point(391, 73);
            this.CommonMultiBall_Text.Name = "CommonMultiBall_Text";
            this.CommonMultiBall_Text.Size = new System.Drawing.Size(116, 34);
            this.CommonMultiBall_Text.TabIndex = 28;
            this.CommonMultiBall_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Common_Text
            // 
            this.Common_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Common_Text.Location = new System.Drawing.Point(67, 73);
            this.Common_Text.Name = "Common_Text";
            this.Common_Text.Size = new System.Drawing.Size(117, 34);
            this.Common_Text.TabIndex = 27;
            this.Common_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // InsertArm_Btn
            // 
            this.InsertArm_Btn.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InsertArm_Btn.Location = new System.Drawing.Point(511, 152);
            this.InsertArm_Btn.Name = "InsertArm_Btn";
            this.InsertArm_Btn.Size = new System.Drawing.Size(177, 45);
            this.InsertArm_Btn.TabIndex = 26;
            this.InsertArm_Btn.Text = "Random ID";
            this.InsertArm_Btn.UseVisualStyleBackColor = true;
            this.InsertArm_Btn.Click += new System.EventHandler(this.InsertArm_Btn_Click);
            // 
            // groupBox12
            // 
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
            this.groupBox12.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox12.Location = new System.Drawing.Point(19, 28);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(723, 517);
            this.groupBox12.TabIndex = 25;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Tùy Chọn Vũ Khí";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(51, 235);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(120, 26);
            this.label28.TabIndex = 20;
            this.label28.Text = "Phẩm Chất:";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(194, 98);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(174, 124);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            // 
            // NewItemQuality_CBox
            // 
            this.NewItemQuality_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NewItemQuality_CBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemQuality_CBox.FormattingEnabled = true;
            this.NewItemQuality_CBox.Items.AddRange(new object[] {
            "Thô",
            "Thường",
            "Ưu Tú",
            "Hoàn Mỹ",
            "Siêu Việt",
            "Truyền Thuyết",
            "Hiện Vật"});
            this.NewItemQuality_CBox.Location = new System.Drawing.Point(194, 232);
            this.NewItemQuality_CBox.Name = "NewItemQuality_CBox";
            this.NewItemQuality_CBox.Size = new System.Drawing.Size(174, 34);
            this.NewItemQuality_CBox.TabIndex = 26;
            // 
            // NewItemDamage_Text
            // 
            this.NewItemDamage_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemDamage_Text.Location = new System.Drawing.Point(549, 89);
            this.NewItemDamage_Text.Name = "NewItemDamage_Text";
            this.NewItemDamage_Text.Size = new System.Drawing.Size(139, 34);
            this.NewItemDamage_Text.TabIndex = 18;
            this.NewItemDamage_Text.Text = "2147483648";
            this.NewItemDamage_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // NewItemAgi_Text
            // 
            this.NewItemAgi_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemAgi_Text.Location = new System.Drawing.Point(194, 455);
            this.NewItemAgi_Text.Name = "NewItemAgi_Text";
            this.NewItemAgi_Text.Size = new System.Drawing.Size(139, 34);
            this.NewItemAgi_Text.TabIndex = 16;
            this.NewItemAgi_Text.Text = "2147483648";
            this.NewItemAgi_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // NewItemLuck_Text
            // 
            this.NewItemLuck_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemLuck_Text.Location = new System.Drawing.Point(549, 455);
            this.NewItemLuck_Text.Name = "NewItemLuck_Text";
            this.NewItemLuck_Text.Size = new System.Drawing.Size(139, 34);
            this.NewItemLuck_Text.TabIndex = 15;
            this.NewItemLuck_Text.Text = "2147483648";
            this.NewItemLuck_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // NewItemDef_Text
            // 
            this.NewItemDef_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemDef_Text.Location = new System.Drawing.Point(549, 399);
            this.NewItemDef_Text.Name = "NewItemDef_Text";
            this.NewItemDef_Text.Size = new System.Drawing.Size(139, 34);
            this.NewItemDef_Text.TabIndex = 14;
            this.NewItemDef_Text.Text = "2147483648";
            this.NewItemDef_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // NewItemAtt_Text
            // 
            this.NewItemAtt_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemAtt_Text.Location = new System.Drawing.Point(194, 399);
            this.NewItemAtt_Text.Name = "NewItemAtt_Text";
            this.NewItemAtt_Text.Size = new System.Drawing.Size(139, 34);
            this.NewItemAtt_Text.TabIndex = 13;
            this.NewItemAtt_Text.Text = "2147483648";
            this.NewItemAtt_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // NewItemDesc_Text
            // 
            this.NewItemDesc_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemDesc_Text.Location = new System.Drawing.Point(194, 340);
            this.NewItemDesc_Text.Name = "NewItemDesc_Text";
            this.NewItemDesc_Text.Size = new System.Drawing.Size(494, 34);
            this.NewItemDesc_Text.TabIndex = 12;
            this.NewItemDesc_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // NewItemName_Text
            // 
            this.NewItemName_Text.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemName_Text.Location = new System.Drawing.Point(194, 281);
            this.NewItemName_Text.Name = "NewItemName_Text";
            this.NewItemName_Text.Size = new System.Drawing.Size(494, 34);
            this.NewItemName_Text.TabIndex = 11;
            this.NewItemName_Text.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(386, 92);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(126, 26);
            this.label27.TabIndex = 10;
            this.label27.Text = "Sát Thương:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(62, 92);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(109, 26);
            this.label26.TabIndex = 9;
            this.label26.Text = "Hình Ảnh:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(29, 458);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(137, 26);
            this.label25.TabIndex = 8;
            this.label25.Text = "Nhanh nhẹn：";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(401, 458);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(109, 26);
            this.label24.TabIndex = 7;
            this.label24.Text = "May mắn：";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(393, 402);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(119, 26);
            this.label23.TabIndex = 6;
            this.label23.Text = "Phòng thủ：";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(46, 402);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(110, 26);
            this.label22.TabIndex = 5;
            this.label22.Text = "Tấn công：";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(77, 343);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(79, 26);
            this.label21.TabIndex = 4;
            this.label21.Text = "Mô Tả:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(102, 284);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(54, 26);
            this.label20.TabIndex = 3;
            this.label20.Text = "Tên:";
            // 
            // ListItemName_CBox
            // 
            this.ListItemName_CBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListItemName_CBox.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListItemName_CBox.FormattingEnabled = true;
            this.ListItemName_CBox.Location = new System.Drawing.Point(194, 38);
            this.ListItemName_CBox.Name = "ListItemName_CBox";
            this.ListItemName_CBox.Size = new System.Drawing.Size(494, 34);
            this.ListItemName_CBox.TabIndex = 2;
            this.ListItemName_CBox.SelectedIndexChanged += new System.EventHandler(this.ListItemName_CBox_SelectedIndexChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(29, 41);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(143, 26);
            this.label19.TabIndex = 0;
            this.label19.Text = "Chọn Vũ Khí:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.textBox13);
            this.groupBox3.Controls.Add(this.textBox14);
            this.groupBox3.Controls.Add(this.textBox15);
            this.groupBox3.Controls.Add(this.label64);
            this.groupBox3.Controls.Add(this.label65);
            this.groupBox3.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(19, 821);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1484, 109);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tool Đồng Bộ XML (Request)";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.ForeColor = System.Drawing.Color.Black;
            this.checkBox1.Location = new System.Drawing.Point(1149, 52);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(106, 30);
            this.checkBox1.TabIndex = 18;
            this.checkBox1.Text = "Mã Hóa";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(1317, 41);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 50);
            this.button2.TabIndex = 17;
            this.button2.Text = "Tạo";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox13
            // 
            this.textBox13.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox13.Location = new System.Drawing.Point(395, 50);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(61, 34);
            this.textBox13.TabIndex = 16;
            this.textBox13.Text = "Item";
            this.textBox13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox14
            // 
            this.textBox14.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox14.Location = new System.Drawing.Point(224, 50);
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new System.Drawing.Size(144, 34);
            this.textBox14.TabIndex = 12;
            this.textBox14.Text = "Shop_Goods";
            this.textBox14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox15
            // 
            this.textBox15.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox15.Location = new System.Drawing.Point(848, 50);
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new System.Drawing.Size(226, 34);
            this.textBox15.TabIndex = 13;
            this.textBox15.Text = "TemplateAllList";
            this.textBox15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label64.ForeColor = System.Drawing.Color.Black;
            this.label64.Location = new System.Drawing.Point(658, 53);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(142, 26);
            this.label64.TabIndex = 14;
            this.label64.Text = "Tên tệp XML:";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label65.ForeColor = System.Drawing.Color.Black;
            this.label65.Location = new System.Drawing.Point(77, 53);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(110, 26);
            this.label65.TabIndex = 15;
            this.label65.Text = "Tên Table:";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox8);
            this.tabPage4.Location = new System.Drawing.Point(4, 35);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1916, 1016);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Level Up 1";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.groupBox43);
            this.groupBox8.Controls.Add(this.groupBox35);
            this.groupBox8.Controls.Add(this.groupBox30);
            this.groupBox8.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox8.Location = new System.Drawing.Point(18, 17);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(1880, 940);
            this.groupBox8.TabIndex = 28;
            this.groupBox8.TabStop = false;
            // 
            // groupBox43
            // 
            this.groupBox43.Controls.Add(this.groupBox48);
            this.groupBox43.Controls.Add(this.groupBox47);
            this.groupBox43.Controls.Add(this.groupBox46);
            this.groupBox43.Controls.Add(this.groupBox45);
            this.groupBox43.Controls.Add(this.groupBox44);
            this.groupBox43.Controls.Add(this.groupBox29);
            this.groupBox43.Controls.Add(this.groupBox27);
            this.groupBox43.Controls.Add(this.groupBox26);
            this.groupBox43.Location = new System.Drawing.Point(1289, 33);
            this.groupBox43.Name = "groupBox43";
            this.groupBox43.Size = new System.Drawing.Size(528, 879);
            this.groupBox43.TabIndex = 39;
            this.groupBox43.TabStop = false;
            this.groupBox43.Text = "Tiệm Rèn";
            // 
            // groupBox48
            // 
            this.groupBox48.Controls.Add(this.button46);
            this.groupBox48.Controls.Add(this.button47);
            this.groupBox48.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox48.Location = new System.Drawing.Point(35, 747);
            this.groupBox48.Name = "groupBox48";
            this.groupBox48.Size = new System.Drawing.Size(457, 96);
            this.groupBox48.TabIndex = 39;
            this.groupBox48.TabStop = false;
            this.groupBox48.Text = "Phong Ấn";
            // 
            // button46
            // 
            this.button46.Location = new System.Drawing.Point(265, 33);
            this.button46.Name = "button46";
            this.button46.Size = new System.Drawing.Size(155, 47);
            this.button46.TabIndex = 16;
            this.button46.Text = "Giải Ấn";
            this.button46.UseVisualStyleBackColor = true;
            // 
            // button47
            // 
            this.button47.Location = new System.Drawing.Point(63, 33);
            this.button47.Name = "button47";
            this.button47.Size = new System.Drawing.Size(154, 47);
            this.button47.TabIndex = 19;
            this.button47.Text = "Phong Ấn";
            this.button47.UseVisualStyleBackColor = true;
            // 
            // groupBox47
            // 
            this.groupBox47.Controls.Add(this.button45);
            this.groupBox47.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox47.Location = new System.Drawing.Point(35, 645);
            this.groupBox47.Name = "groupBox47";
            this.groupBox47.Size = new System.Drawing.Size(457, 96);
            this.groupBox47.TabIndex = 38;
            this.groupBox47.TabStop = false;
            this.groupBox47.Text = "Tiến Hóa VK Phụ";
            // 
            // button45
            // 
            this.button45.Location = new System.Drawing.Point(130, 31);
            this.button45.Name = "button45";
            this.button45.Size = new System.Drawing.Size(214, 47);
            this.button45.TabIndex = 16;
            this.button45.Text = "Tiến Hóa";
            this.button45.UseVisualStyleBackColor = true;
            // 
            // groupBox46
            // 
            this.groupBox46.Controls.Add(this.button25);
            this.groupBox46.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox46.Location = new System.Drawing.Point(35, 441);
            this.groupBox46.Name = "groupBox46";
            this.groupBox46.Size = new System.Drawing.Size(457, 96);
            this.groupBox46.TabIndex = 37;
            this.groupBox46.TabStop = false;
            this.groupBox46.Text = "Chế Tạo";
            // 
            // button25
            // 
            this.button25.Location = new System.Drawing.Point(130, 32);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(214, 47);
            this.button25.TabIndex = 16;
            this.button25.Text = "Tạo";
            this.button25.UseVisualStyleBackColor = true;
            this.button25.Click += new System.EventHandler(this.button25_Click_1);
            // 
            // groupBox45
            // 
            this.groupBox45.Controls.Add(this.button27);
            this.groupBox45.Controls.Add(this.button44);
            this.groupBox45.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox45.Location = new System.Drawing.Point(35, 339);
            this.groupBox45.Name = "groupBox45";
            this.groupBox45.Size = new System.Drawing.Size(457, 96);
            this.groupBox45.TabIndex = 36;
            this.groupBox45.TabStop = false;
            this.groupBox45.Text = "Phụ Ma";
            // 
            // button27
            // 
            this.button27.Location = new System.Drawing.Point(265, 33);
            this.button27.Name = "button27";
            this.button27.Size = new System.Drawing.Size(155, 47);
            this.button27.TabIndex = 16;
            this.button27.Text = "Đúc Hồn";
            this.button27.UseVisualStyleBackColor = true;
            // 
            // button44
            // 
            this.button44.Location = new System.Drawing.Point(63, 33);
            this.button44.Name = "button44";
            this.button44.Size = new System.Drawing.Size(154, 47);
            this.button44.TabIndex = 19;
            this.button44.Text = "Phụ Ma";
            this.button44.UseVisualStyleBackColor = true;
            // 
            // groupBox44
            // 
            this.groupBox44.Controls.Add(this.button6);
            this.groupBox44.Controls.Add(this.button11);
            this.groupBox44.Controls.Add(this.button26);
            this.groupBox44.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox44.Location = new System.Drawing.Point(35, 135);
            this.groupBox44.Name = "groupBox44";
            this.groupBox44.Size = new System.Drawing.Size(457, 96);
            this.groupBox44.TabIndex = 35;
            this.groupBox44.TabStop = false;
            this.groupBox44.Text = "Dát Vàng";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(20, 36);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(124, 46);
            this.button6.TabIndex = 13;
            this.button6.Text = "Vũ Khí";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(167, 36);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(124, 46);
            this.button11.TabIndex = 11;
            this.button11.Text = "Nón";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button26
            // 
            this.button26.Location = new System.Drawing.Point(315, 36);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(124, 46);
            this.button26.TabIndex = 12;
            this.button26.Text = "Quần Áo";
            this.button26.UseVisualStyleBackColor = true;
            // 
            // groupBox29
            // 
            this.groupBox29.Controls.Add(this.button5);
            this.groupBox29.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox29.Location = new System.Drawing.Point(35, 33);
            this.groupBox29.Name = "groupBox29";
            this.groupBox29.Size = new System.Drawing.Size(457, 96);
            this.groupBox29.TabIndex = 34;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "Tiềm Năng";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(130, 33);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(214, 45);
            this.button5.TabIndex = 14;
            this.button5.Text = "Kích Hoạt";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // groupBox27
            // 
            this.groupBox27.Controls.Add(this.button22);
            this.groupBox27.Controls.Add(this.button20);
            this.groupBox27.Controls.Add(this.button21);
            this.groupBox27.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox27.Location = new System.Drawing.Point(35, 543);
            this.groupBox27.Name = "groupBox27";
            this.groupBox27.Size = new System.Drawing.Size(457, 96);
            this.groupBox27.TabIndex = 33;
            this.groupBox27.TabStop = false;
            this.groupBox27.Text = "Tăng Sao";
            // 
            // button22
            // 
            this.button22.Location = new System.Drawing.Point(20, 36);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(124, 46);
            this.button22.TabIndex = 13;
            this.button22.Text = "Vũ Khí";
            this.button22.UseVisualStyleBackColor = true;
            this.button22.Click += new System.EventHandler(this.button22_Click_1);
            // 
            // button20
            // 
            this.button20.Location = new System.Drawing.Point(167, 38);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(124, 43);
            this.button20.TabIndex = 11;
            this.button20.Text = "Nón";
            this.button20.UseVisualStyleBackColor = true;
            this.button20.Click += new System.EventHandler(this.button20_Click_1);
            // 
            // button21
            // 
            this.button21.Location = new System.Drawing.Point(315, 36);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(124, 46);
            this.button21.TabIndex = 12;
            this.button21.Text = "Quần Áo";
            this.button21.UseVisualStyleBackColor = true;
            this.button21.Click += new System.EventHandler(this.button21_Click_1);
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.button28);
            this.groupBox26.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox26.Location = new System.Drawing.Point(35, 237);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(457, 96);
            this.groupBox26.TabIndex = 32;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Chiến Hồn";
            // 
            // button28
            // 
            this.button28.Location = new System.Drawing.Point(130, 31);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(214, 47);
            this.button28.TabIndex = 19;
            this.button28.Text = "Chiến Hồn";
            this.button28.UseVisualStyleBackColor = true;
            this.button28.Click += new System.EventHandler(this.button28_Click_1);
            // 
            // groupBox35
            // 
            this.groupBox35.Controls.Add(this.groupBox42);
            this.groupBox35.Controls.Add(this.groupBox41);
            this.groupBox35.Controls.Add(this.groupBox40);
            this.groupBox35.Controls.Add(this.groupBox39);
            this.groupBox35.Controls.Add(this.groupBox38);
            this.groupBox35.Controls.Add(this.groupBox37);
            this.groupBox35.Controls.Add(this.groupBox36);
            this.groupBox35.Controls.Add(this.groupBox25);
            this.groupBox35.Location = new System.Drawing.Point(666, 33);
            this.groupBox35.Name = "groupBox35";
            this.groupBox35.Size = new System.Drawing.Size(528, 879);
            this.groupBox35.TabIndex = 36;
            this.groupBox35.TabStop = false;
            this.groupBox35.Text = "Cá Nhân";
            // 
            // groupBox42
            // 
            this.groupBox42.Controls.Add(this.button43);
            this.groupBox42.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox42.Location = new System.Drawing.Point(34, 747);
            this.groupBox42.Name = "groupBox42";
            this.groupBox42.Size = new System.Drawing.Size(457, 96);
            this.groupBox42.TabIndex = 38;
            this.groupBox42.TabStop = false;
            this.groupBox42.Text = "Nhẫn Cầu Hôn";
            // 
            // button43
            // 
            this.button43.Location = new System.Drawing.Point(130, 33);
            this.button43.Name = "button43";
            this.button43.Size = new System.Drawing.Size(214, 45);
            this.button43.TabIndex = 14;
            this.button43.Text = "Lên Cấp";
            this.button43.UseVisualStyleBackColor = true;
            // 
            // groupBox41
            // 
            this.groupBox41.Controls.Add(this.button42);
            this.groupBox41.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox41.Location = new System.Drawing.Point(34, 645);
            this.groupBox41.Name = "groupBox41";
            this.groupBox41.Size = new System.Drawing.Size(457, 96);
            this.groupBox41.TabIndex = 37;
            this.groupBox41.TabStop = false;
            this.groupBox41.Text = "Danh Hiệu";
            // 
            // button42
            // 
            this.button42.Location = new System.Drawing.Point(130, 33);
            this.button42.Name = "button42";
            this.button42.Size = new System.Drawing.Size(214, 45);
            this.button42.TabIndex = 14;
            this.button42.Text = "Lên Cấp";
            this.button42.UseVisualStyleBackColor = true;
            // 
            // groupBox40
            // 
            this.groupBox40.Controls.Add(this.button41);
            this.groupBox40.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox40.Location = new System.Drawing.Point(34, 543);
            this.groupBox40.Name = "groupBox40";
            this.groupBox40.Size = new System.Drawing.Size(457, 96);
            this.groupBox40.TabIndex = 36;
            this.groupBox40.TabStop = false;
            this.groupBox40.Text = "Vinh Dự Gunner";
            // 
            // button41
            // 
            this.button41.Location = new System.Drawing.Point(130, 33);
            this.button41.Name = "button41";
            this.button41.Size = new System.Drawing.Size(214, 45);
            this.button41.TabIndex = 14;
            this.button41.Text = "Lên Cấp";
            this.button41.UseVisualStyleBackColor = true;
            // 
            // groupBox39
            // 
            this.groupBox39.Controls.Add(this.button36);
            this.groupBox39.Controls.Add(this.button39);
            this.groupBox39.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox39.Location = new System.Drawing.Point(34, 441);
            this.groupBox39.Name = "groupBox39";
            this.groupBox39.Size = new System.Drawing.Size(457, 96);
            this.groupBox39.TabIndex = 35;
            this.groupBox39.TabStop = false;
            this.groupBox39.Text = "Hỗ Trợ";
            // 
            // button36
            // 
            this.button36.Location = new System.Drawing.Point(270, 33);
            this.button36.Name = "button36";
            this.button36.Size = new System.Drawing.Size(153, 45);
            this.button36.TabIndex = 15;
            this.button36.Text = "Tẩy Luyện";
            this.button36.UseVisualStyleBackColor = true;
            // 
            // button39
            // 
            this.button39.Location = new System.Drawing.Point(38, 33);
            this.button39.Name = "button39";
            this.button39.Size = new System.Drawing.Size(151, 45);
            this.button39.TabIndex = 14;
            this.button39.Text = "Lên Cấp";
            this.button39.UseVisualStyleBackColor = true;
            // 
            // groupBox38
            // 
            this.groupBox38.Controls.Add(this.button35);
            this.groupBox38.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox38.Location = new System.Drawing.Point(34, 237);
            this.groupBox38.Name = "groupBox38";
            this.groupBox38.Size = new System.Drawing.Size(457, 96);
            this.groupBox38.TabIndex = 34;
            this.groupBox38.TabStop = false;
            this.groupBox38.Text = "Sổ Tay Thám Hiểm";
            // 
            // button35
            // 
            this.button35.Location = new System.Drawing.Point(130, 33);
            this.button35.Name = "button35";
            this.button35.Size = new System.Drawing.Size(214, 45);
            this.button35.TabIndex = 14;
            this.button35.Text = "Tiến Hóa";
            this.button35.UseVisualStyleBackColor = true;
            // 
            // groupBox37
            // 
            this.groupBox37.Controls.Add(this.button32);
            this.groupBox37.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox37.Location = new System.Drawing.Point(34, 135);
            this.groupBox37.Name = "groupBox37";
            this.groupBox37.Size = new System.Drawing.Size(457, 96);
            this.groupBox37.TabIndex = 33;
            this.groupBox37.TabStop = false;
            this.groupBox37.Text = "Bảo Vệ";
            // 
            // button32
            // 
            this.button32.Location = new System.Drawing.Point(130, 33);
            this.button32.Name = "button32";
            this.button32.Size = new System.Drawing.Size(214, 45);
            this.button32.TabIndex = 14;
            this.button32.Text = "Bảo Vệ";
            this.button32.UseVisualStyleBackColor = true;
            // 
            // groupBox36
            // 
            this.groupBox36.Controls.Add(this.button33);
            this.groupBox36.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox36.Location = new System.Drawing.Point(34, 33);
            this.groupBox36.Name = "groupBox36";
            this.groupBox36.Size = new System.Drawing.Size(457, 96);
            this.groupBox36.TabIndex = 32;
            this.groupBox36.TabStop = false;
            this.groupBox36.Text = "Cấp Vua Gà";
            // 
            // button33
            // 
            this.button33.Location = new System.Drawing.Point(130, 33);
            this.button33.Name = "button33";
            this.button33.Size = new System.Drawing.Size(214, 45);
            this.button33.TabIndex = 14;
            this.button33.Text = "Đột Phá";
            this.button33.UseVisualStyleBackColor = true;
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.button24);
            this.groupBox25.Controls.Add(this.button23);
            this.groupBox25.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox25.Location = new System.Drawing.Point(34, 339);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(457, 96);
            this.groupBox25.TabIndex = 31;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "Dây Chuyền";
            // 
            // button24
            // 
            this.button24.Location = new System.Drawing.Point(270, 33);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(153, 45);
            this.button24.TabIndex = 15;
            this.button24.Text = "Đúc";
            this.button24.UseVisualStyleBackColor = true;
            this.button24.Click += new System.EventHandler(this.button24_Click_1);
            // 
            // button23
            // 
            this.button23.Location = new System.Drawing.Point(38, 33);
            this.button23.Name = "button23";
            this.button23.Size = new System.Drawing.Size(151, 45);
            this.button23.TabIndex = 14;
            this.button23.Text = "Luyện";
            this.button23.UseVisualStyleBackColor = true;
            this.button23.Click += new System.EventHandler(this.button23_Click_1);
            // 
            // groupBox30
            // 
            this.groupBox30.Controls.Add(this.groupBox34);
            this.groupBox30.Controls.Add(this.groupBox33);
            this.groupBox30.Controls.Add(this.groupBox32);
            this.groupBox30.Controls.Add(this.groupBox31);
            this.groupBox30.Controls.Add(this.groupBox23);
            this.groupBox30.Controls.Add(this.groupBox21);
            this.groupBox30.Controls.Add(this.groupBox22);
            this.groupBox30.Controls.Add(this.groupBox24);
            this.groupBox30.Location = new System.Drawing.Point(51, 33);
            this.groupBox30.Name = "groupBox30";
            this.groupBox30.Size = new System.Drawing.Size(528, 879);
            this.groupBox30.TabIndex = 35;
            this.groupBox30.TabStop = false;
            this.groupBox30.Text = "Túi";
            // 
            // groupBox34
            // 
            this.groupBox34.Controls.Add(this.button40);
            this.groupBox34.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox34.Location = new System.Drawing.Point(32, 747);
            this.groupBox34.Name = "groupBox34";
            this.groupBox34.Size = new System.Drawing.Size(457, 96);
            this.groupBox34.TabIndex = 37;
            this.groupBox34.TabStop = false;
            this.groupBox34.Text = "Ấn";
            // 
            // button40
            // 
            this.button40.Location = new System.Drawing.Point(128, 33);
            this.button40.Name = "button40";
            this.button40.Size = new System.Drawing.Size(214, 46);
            this.button40.TabIndex = 13;
            this.button40.Text = "Ấn";
            this.button40.UseVisualStyleBackColor = true;
            // 
            // groupBox33
            // 
            this.groupBox33.Controls.Add(this.button37);
            this.groupBox33.Controls.Add(this.button38);
            this.groupBox33.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox33.Location = new System.Drawing.Point(32, 543);
            this.groupBox33.Name = "groupBox33";
            this.groupBox33.Size = new System.Drawing.Size(457, 96);
            this.groupBox33.TabIndex = 36;
            this.groupBox33.TabStop = false;
            this.groupBox33.Text = "Ma Thạch";
            // 
            // button37
            // 
            this.button37.Location = new System.Drawing.Point(39, 36);
            this.button37.Name = "button37";
            this.button37.Size = new System.Drawing.Size(153, 46);
            this.button37.TabIndex = 13;
            this.button37.Text = "Ma Thạch";
            this.button37.UseVisualStyleBackColor = true;
            // 
            // button38
            // 
            this.button38.Location = new System.Drawing.Point(265, 39);
            this.button38.Name = "button38";
            this.button38.Size = new System.Drawing.Size(153, 43);
            this.button38.TabIndex = 11;
            this.button38.Text = "Ma Ngọc";
            this.button38.UseVisualStyleBackColor = true;
            // 
            // groupBox32
            // 
            this.groupBox32.Controls.Add(this.button34);
            this.groupBox32.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox32.Location = new System.Drawing.Point(32, 441);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Size = new System.Drawing.Size(457, 96);
            this.groupBox32.TabIndex = 35;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "Phụ Kiện";
            // 
            // button34
            // 
            this.button34.Location = new System.Drawing.Point(128, 33);
            this.button34.Name = "button34";
            this.button34.Size = new System.Drawing.Size(214, 46);
            this.button34.TabIndex = 13;
            this.button34.Text = "Bộ Sưu Tập";
            this.button34.UseVisualStyleBackColor = true;
            // 
            // groupBox31
            // 
            this.groupBox31.Controls.Add(this.button31);
            this.groupBox31.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox31.Location = new System.Drawing.Point(32, 339);
            this.groupBox31.Name = "groupBox31";
            this.groupBox31.Size = new System.Drawing.Size(457, 96);
            this.groupBox31.TabIndex = 34;
            this.groupBox31.TabStop = false;
            this.groupBox31.Text = "Châu Báu";
            // 
            // button31
            // 
            this.button31.Location = new System.Drawing.Point(128, 33);
            this.button31.Name = "button31";
            this.button31.Size = new System.Drawing.Size(214, 46);
            this.button31.TabIndex = 13;
            this.button31.Text = "Mở Mũi Khoan";
            this.button31.UseVisualStyleBackColor = true;
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.button12);
            this.groupBox23.Controls.Add(this.button13);
            this.groupBox23.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox23.Location = new System.Drawing.Point(32, 237);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Size = new System.Drawing.Size(457, 96);
            this.groupBox23.TabIndex = 29;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Vật Tổ";
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(39, 33);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(153, 46);
            this.button12.TabIndex = 3;
            this.button12.Text = "Lên Cấp";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click_1);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(265, 33);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(153, 46);
            this.button13.TabIndex = 4;
            this.button13.Text = "Tăng Bậc";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click_1);
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.button54);
            this.groupBox21.Controls.Add(this.button18);
            this.groupBox21.Controls.Add(this.button19);
            this.groupBox21.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox21.Location = new System.Drawing.Point(32, 33);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(457, 96);
            this.groupBox21.TabIndex = 27;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Tu Luyện";
            // 
            // button54
            // 
            this.button54.Location = new System.Drawing.Point(295, 31);
            this.button54.Name = "button54";
            this.button54.Size = new System.Drawing.Size(144, 48);
            this.button54.TabIndex = 12;
            this.button54.Text = "Nguyên Tố";
            this.button54.UseVisualStyleBackColor = true;
            this.button54.Click += new System.EventHandler(this.button54_Click);
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(15, 31);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(109, 48);
            this.button18.TabIndex = 9;
            this.button18.Text = "Cơ Bản";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click_1);
            // 
            // button19
            // 
            this.button19.Location = new System.Drawing.Point(142, 31);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(133, 48);
            this.button19.TabIndex = 10;
            this.button19.Text = "TT Cấp 2";
            this.button19.UseVisualStyleBackColor = true;
            this.button19.Click += new System.EventHandler(this.button19_Click_1);
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.button16);
            this.groupBox22.Controls.Add(this.button17);
            this.groupBox22.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox22.Location = new System.Drawing.Point(32, 135);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(457, 96);
            this.groupBox22.TabIndex = 28;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Thẻ Bài";
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(39, 33);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(153, 49);
            this.button16.TabIndex = 7;
            this.button16.Text = "Lên Cấp";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click_1);
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(265, 33);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(153, 49);
            this.button17.TabIndex = 8;
            this.button17.Text = "Đột Phá";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click_1);
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.button14);
            this.groupBox24.Controls.Add(this.button15);
            this.groupBox24.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox24.Location = new System.Drawing.Point(32, 645);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(457, 96);
            this.groupBox24.TabIndex = 30;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "Miếu Thần";
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(39, 33);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(153, 45);
            this.button14.TabIndex = 5;
            this.button14.Text = "Chúc Phúc";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click_1);
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(265, 32);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(153, 46);
            this.button15.TabIndex = 6;
            this.button15.Text = "Thăng Hoa";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click_2);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox28);
            this.tabPage5.Location = new System.Drawing.Point(4, 35);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1916, 1016);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Level Up 2";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox28
            // 
            this.groupBox28.Controls.Add(this.groupBox58);
            this.groupBox28.Controls.Add(this.groupBox50);
            this.groupBox28.Controls.Add(this.groupBox49);
            this.groupBox28.Location = new System.Drawing.Point(18, 17);
            this.groupBox28.Name = "groupBox28";
            this.groupBox28.Size = new System.Drawing.Size(1870, 940);
            this.groupBox28.TabIndex = 0;
            this.groupBox28.TabStop = false;
            // 
            // groupBox58
            // 
            this.groupBox58.Controls.Add(this.groupBox59);
            this.groupBox58.Controls.Add(this.groupBox64);
            this.groupBox58.Controls.Add(this.groupBox65);
            this.groupBox58.Controls.Add(this.groupBox66);
            this.groupBox58.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox58.Location = new System.Drawing.Point(1288, 33);
            this.groupBox58.Name = "groupBox58";
            this.groupBox58.Size = new System.Drawing.Size(528, 879);
            this.groupBox58.TabIndex = 38;
            this.groupBox58.TabStop = false;
            this.groupBox58.Text = "Thần Hộ Mệnh";
            // 
            // groupBox59
            // 
            this.groupBox59.Controls.Add(this.button61);
            this.groupBox59.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox59.Location = new System.Drawing.Point(32, 339);
            this.groupBox59.Name = "groupBox59";
            this.groupBox59.Size = new System.Drawing.Size(457, 96);
            this.groupBox59.TabIndex = 34;
            this.groupBox59.TabStop = false;
            this.groupBox59.Text = "Linh Bảo";
            // 
            // button61
            // 
            this.button61.Location = new System.Drawing.Point(128, 32);
            this.button61.Name = "button61";
            this.button61.Size = new System.Drawing.Size(214, 49);
            this.button61.TabIndex = 9;
            this.button61.Text = "Trang Bị";
            this.button61.UseVisualStyleBackColor = true;
            // 
            // groupBox64
            // 
            this.groupBox64.Controls.Add(this.button69);
            this.groupBox64.Controls.Add(this.button63);
            this.groupBox64.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox64.Location = new System.Drawing.Point(32, 237);
            this.groupBox64.Name = "groupBox64";
            this.groupBox64.Size = new System.Drawing.Size(457, 96);
            this.groupBox64.TabIndex = 29;
            this.groupBox64.TabStop = false;
            this.groupBox64.Text = "Sách Kỹ Năng";
            // 
            // button69
            // 
            this.button69.Location = new System.Drawing.Point(267, 33);
            this.button69.Name = "button69";
            this.button69.Size = new System.Drawing.Size(153, 49);
            this.button69.TabIndex = 11;
            this.button69.Text = "Dung Hợp";
            this.button69.UseVisualStyleBackColor = true;
            // 
            // button63
            // 
            this.button63.Location = new System.Drawing.Point(43, 30);
            this.button63.Name = "button63";
            this.button63.Size = new System.Drawing.Size(153, 49);
            this.button63.TabIndex = 3;
            this.button63.Text = "Học";
            this.button63.UseVisualStyleBackColor = true;
            // 
            // groupBox65
            // 
            this.groupBox65.Controls.Add(this.button67);
            this.groupBox65.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox65.Location = new System.Drawing.Point(32, 33);
            this.groupBox65.Name = "groupBox65";
            this.groupBox65.Size = new System.Drawing.Size(457, 96);
            this.groupBox65.TabIndex = 27;
            this.groupBox65.TabStop = false;
            this.groupBox65.Text = "Nuôi Thần Hộ Mệnh";
            // 
            // button67
            // 
            this.button67.Location = new System.Drawing.Point(128, 33);
            this.button67.Name = "button67";
            this.button67.Size = new System.Drawing.Size(214, 46);
            this.button67.TabIndex = 9;
            this.button67.Text = "Nuôi";
            this.button67.UseVisualStyleBackColor = true;
            // 
            // groupBox66
            // 
            this.groupBox66.Controls.Add(this.button68);
            this.groupBox66.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox66.Location = new System.Drawing.Point(32, 135);
            this.groupBox66.Name = "groupBox66";
            this.groupBox66.Size = new System.Drawing.Size(457, 96);
            this.groupBox66.TabIndex = 28;
            this.groupBox66.TabStop = false;
            this.groupBox66.Text = "Luyện Thần Hộ Mệnh";
            // 
            // button68
            // 
            this.button68.Location = new System.Drawing.Point(128, 36);
            this.button68.Name = "button68";
            this.button68.Size = new System.Drawing.Size(214, 46);
            this.button68.TabIndex = 10;
            this.button68.Text = "Luyện";
            this.button68.UseVisualStyleBackColor = true;
            // 
            // groupBox50
            // 
            this.groupBox50.Controls.Add(this.groupBox60);
            this.groupBox50.Controls.Add(this.groupBox61);
            this.groupBox50.Controls.Add(this.groupBox62);
            this.groupBox50.Controls.Add(this.groupBox63);
            this.groupBox50.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox50.Location = new System.Drawing.Point(666, 33);
            this.groupBox50.Name = "groupBox50";
            this.groupBox50.Size = new System.Drawing.Size(528, 879);
            this.groupBox50.TabIndex = 37;
            this.groupBox50.TabStop = false;
            this.groupBox50.Text = "Thú Cưỡi";
            // 
            // groupBox60
            // 
            this.groupBox60.Controls.Add(this.button48);
            this.groupBox60.Controls.Add(this.button58);
            this.groupBox60.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox60.Location = new System.Drawing.Point(32, 339);
            this.groupBox60.Name = "groupBox60";
            this.groupBox60.Size = new System.Drawing.Size(457, 96);
            this.groupBox60.TabIndex = 34;
            this.groupBox60.TabStop = false;
            this.groupBox60.Text = "Bùa Thú";
            // 
            // button48
            // 
            this.button48.Location = new System.Drawing.Point(43, 32);
            this.button48.Name = "button48";
            this.button48.Size = new System.Drawing.Size(153, 49);
            this.button48.TabIndex = 9;
            this.button48.Text = "Dùng Bùa";
            this.button48.UseVisualStyleBackColor = true;
            // 
            // button58
            // 
            this.button58.Location = new System.Drawing.Point(267, 32);
            this.button58.Name = "button58";
            this.button58.Size = new System.Drawing.Size(153, 49);
            this.button58.TabIndex = 10;
            this.button58.Text = "Tẩy Bùa";
            this.button58.UseVisualStyleBackColor = true;
            // 
            // groupBox61
            // 
            this.groupBox61.Controls.Add(this.button64);
            this.groupBox61.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox61.Location = new System.Drawing.Point(32, 237);
            this.groupBox61.Name = "groupBox61";
            this.groupBox61.Size = new System.Drawing.Size(457, 96);
            this.groupBox61.TabIndex = 29;
            this.groupBox61.TabStop = false;
            this.groupBox61.Text = "Biến Hình Thú";
            // 
            // button64
            // 
            this.button64.Location = new System.Drawing.Point(128, 33);
            this.button64.Name = "button64";
            this.button64.Size = new System.Drawing.Size(214, 46);
            this.button64.TabIndex = 3;
            this.button64.Text = "Biến Hình";
            this.button64.UseVisualStyleBackColor = true;
            // 
            // groupBox62
            // 
            this.groupBox62.Controls.Add(this.button65);
            this.groupBox62.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox62.Location = new System.Drawing.Point(32, 33);
            this.groupBox62.Name = "groupBox62";
            this.groupBox62.Size = new System.Drawing.Size(457, 96);
            this.groupBox62.TabIndex = 27;
            this.groupBox62.TabStop = false;
            this.groupBox62.Text = "Tăng Cấp Thú";
            // 
            // button65
            // 
            this.button65.Location = new System.Drawing.Point(128, 33);
            this.button65.Name = "button65";
            this.button65.Size = new System.Drawing.Size(214, 46);
            this.button65.TabIndex = 9;
            this.button65.Text = "Lên Cấp";
            this.button65.UseVisualStyleBackColor = true;
            this.button65.Click += new System.EventHandler(this.button65_Click);
            // 
            // groupBox63
            // 
            this.groupBox63.Controls.Add(this.button66);
            this.groupBox63.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox63.Location = new System.Drawing.Point(32, 135);
            this.groupBox63.Name = "groupBox63";
            this.groupBox63.Size = new System.Drawing.Size(457, 96);
            this.groupBox63.TabIndex = 28;
            this.groupBox63.TabStop = false;
            this.groupBox63.Text = "Kỹ Năng Thú";
            // 
            // button66
            // 
            this.button66.Location = new System.Drawing.Point(128, 36);
            this.button66.Name = "button66";
            this.button66.Size = new System.Drawing.Size(214, 46);
            this.button66.TabIndex = 10;
            this.button66.Text = "Lên Cấp";
            this.button66.UseVisualStyleBackColor = true;
            this.button66.Click += new System.EventHandler(this.button66_Click);
            // 
            // groupBox49
            // 
            this.groupBox49.Controls.Add(this.groupBox51);
            this.groupBox49.Controls.Add(this.groupBox52);
            this.groupBox49.Controls.Add(this.groupBox53);
            this.groupBox49.Controls.Add(this.groupBox54);
            this.groupBox49.Controls.Add(this.groupBox55);
            this.groupBox49.Controls.Add(this.groupBox56);
            this.groupBox49.Controls.Add(this.groupBox57);
            this.groupBox49.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox49.Location = new System.Drawing.Point(51, 33);
            this.groupBox49.Name = "groupBox49";
            this.groupBox49.Size = new System.Drawing.Size(528, 879);
            this.groupBox49.TabIndex = 36;
            this.groupBox49.TabStop = false;
            this.groupBox49.Text = "Thú Cưng";
            // 
            // groupBox51
            // 
            this.groupBox51.Controls.Add(this.button49);
            this.groupBox51.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox51.Location = new System.Drawing.Point(32, 543);
            this.groupBox51.Name = "groupBox51";
            this.groupBox51.Size = new System.Drawing.Size(457, 96);
            this.groupBox51.TabIndex = 36;
            this.groupBox51.TabStop = false;
            this.groupBox51.Text = "Đột Phá PET";
            // 
            // button49
            // 
            this.button49.Location = new System.Drawing.Point(128, 33);
            this.button49.Name = "button49";
            this.button49.Size = new System.Drawing.Size(214, 46);
            this.button49.TabIndex = 13;
            this.button49.Text = "Hiến Tế";
            this.button49.UseVisualStyleBackColor = true;
            // 
            // groupBox52
            // 
            this.groupBox52.Controls.Add(this.button51);
            this.groupBox52.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox52.Location = new System.Drawing.Point(32, 441);
            this.groupBox52.Name = "groupBox52";
            this.groupBox52.Size = new System.Drawing.Size(457, 96);
            this.groupBox52.TabIndex = 35;
            this.groupBox52.TabStop = false;
            this.groupBox52.Text = "Manh Hóa";
            // 
            // button51
            // 
            this.button51.Location = new System.Drawing.Point(128, 33);
            this.button51.Name = "button51";
            this.button51.Size = new System.Drawing.Size(214, 46);
            this.button51.TabIndex = 14;
            this.button51.Text = "Manh Hóa";
            this.button51.UseVisualStyleBackColor = true;
            this.button51.Click += new System.EventHandler(this.button51_Click);
            // 
            // groupBox53
            // 
            this.groupBox53.Controls.Add(this.button52);
            this.groupBox53.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox53.Location = new System.Drawing.Point(32, 339);
            this.groupBox53.Name = "groupBox53";
            this.groupBox53.Size = new System.Drawing.Size(457, 96);
            this.groupBox53.TabIndex = 34;
            this.groupBox53.TabStop = false;
            this.groupBox53.Text = "Biến Hình PET";
            // 
            // button52
            // 
            this.button52.Location = new System.Drawing.Point(128, 33);
            this.button52.Name = "button52";
            this.button52.Size = new System.Drawing.Size(214, 46);
            this.button52.TabIndex = 13;
            this.button52.Text = "Thức Tỉnh";
            this.button52.UseVisualStyleBackColor = true;
            // 
            // groupBox54
            // 
            this.groupBox54.Controls.Add(this.button53);
            this.groupBox54.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox54.Location = new System.Drawing.Point(32, 237);
            this.groupBox54.Name = "groupBox54";
            this.groupBox54.Size = new System.Drawing.Size(457, 96);
            this.groupBox54.TabIndex = 29;
            this.groupBox54.TabStop = false;
            this.groupBox54.Text = "Tiến Hóa PET";
            // 
            // button53
            // 
            this.button53.Location = new System.Drawing.Point(128, 33);
            this.button53.Name = "button53";
            this.button53.Size = new System.Drawing.Size(214, 46);
            this.button53.TabIndex = 3;
            this.button53.Text = "Tiến Hóa";
            this.button53.UseVisualStyleBackColor = true;
            this.button53.Click += new System.EventHandler(this.button53_Click);
            // 
            // groupBox55
            // 
            this.groupBox55.Controls.Add(this.button55);
            this.groupBox55.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox55.Location = new System.Drawing.Point(32, 33);
            this.groupBox55.Name = "groupBox55";
            this.groupBox55.Size = new System.Drawing.Size(457, 96);
            this.groupBox55.TabIndex = 27;
            this.groupBox55.TabStop = false;
            this.groupBox55.Text = "Trùng Sinh PET";
            // 
            // button55
            // 
            this.button55.Location = new System.Drawing.Point(128, 33);
            this.button55.Name = "button55";
            this.button55.Size = new System.Drawing.Size(214, 46);
            this.button55.TabIndex = 9;
            this.button55.Text = "Trùng Sinh";
            this.button55.UseVisualStyleBackColor = true;
            // 
            // groupBox56
            // 
            this.groupBox56.Controls.Add(this.button57);
            this.groupBox56.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox56.Location = new System.Drawing.Point(32, 135);
            this.groupBox56.Name = "groupBox56";
            this.groupBox56.Size = new System.Drawing.Size(457, 96);
            this.groupBox56.TabIndex = 28;
            this.groupBox56.TabStop = false;
            this.groupBox56.Text = "Tăng Sao PET";
            // 
            // button57
            // 
            this.button57.Location = new System.Drawing.Point(128, 33);
            this.button57.Name = "button57";
            this.button57.Size = new System.Drawing.Size(214, 49);
            this.button57.TabIndex = 7;
            this.button57.Text = "Tăng Sao";
            this.button57.UseVisualStyleBackColor = true;
            // 
            // groupBox57
            // 
            this.groupBox57.Controls.Add(this.button50);
            this.groupBox57.Controls.Add(this.button59);
            this.groupBox57.Controls.Add(this.button60);
            this.groupBox57.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox57.Location = new System.Drawing.Point(32, 645);
            this.groupBox57.Name = "groupBox57";
            this.groupBox57.Size = new System.Drawing.Size(457, 96);
            this.groupBox57.TabIndex = 30;
            this.groupBox57.TabStop = false;
            this.groupBox57.Text = "Thức Tỉnh PET";
            // 
            // button50
            // 
            this.button50.Location = new System.Drawing.Point(311, 34);
            this.button50.Name = "button50";
            this.button50.Size = new System.Drawing.Size(138, 45);
            this.button50.TabIndex = 17;
            this.button50.Text = "Dung Hợp";
            this.button50.UseVisualStyleBackColor = true;
            // 
            // button59
            // 
            this.button59.Location = new System.Drawing.Point(6, 34);
            this.button59.Name = "button59";
            this.button59.Size = new System.Drawing.Size(141, 44);
            this.button59.TabIndex = 5;
            this.button59.Text = "Thường";
            this.button59.UseVisualStyleBackColor = true;
            // 
            // button60
            // 
            this.button60.Location = new System.Drawing.Point(165, 34);
            this.button60.Name = "button60";
            this.button60.Size = new System.Drawing.Size(126, 44);
            this.button60.TabIndex = 6;
            this.button60.Text = "Siêu";
            this.button60.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // mainWorker
            // 
            this.mainWorker.Enabled = true;
            this.mainWorker.Interval = 10000;
            this.mainWorker.Tick += new System.EventHandler(this.mainWorker_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1924, 1055);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
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
            this.groupBox17.ResumeLayout(false);
            this.groupBox19.ResumeLayout(false);
            this.groupBox19.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NewPlyaerFightPowerRank)).EndInit();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox20.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox43.ResumeLayout(false);
            this.groupBox48.ResumeLayout(false);
            this.groupBox47.ResumeLayout(false);
            this.groupBox46.ResumeLayout(false);
            this.groupBox45.ResumeLayout(false);
            this.groupBox44.ResumeLayout(false);
            this.groupBox29.ResumeLayout(false);
            this.groupBox27.ResumeLayout(false);
            this.groupBox26.ResumeLayout(false);
            this.groupBox35.ResumeLayout(false);
            this.groupBox42.ResumeLayout(false);
            this.groupBox41.ResumeLayout(false);
            this.groupBox40.ResumeLayout(false);
            this.groupBox39.ResumeLayout(false);
            this.groupBox38.ResumeLayout(false);
            this.groupBox37.ResumeLayout(false);
            this.groupBox36.ResumeLayout(false);
            this.groupBox25.ResumeLayout(false);
            this.groupBox30.ResumeLayout(false);
            this.groupBox34.ResumeLayout(false);
            this.groupBox33.ResumeLayout(false);
            this.groupBox32.ResumeLayout(false);
            this.groupBox31.ResumeLayout(false);
            this.groupBox23.ResumeLayout(false);
            this.groupBox21.ResumeLayout(false);
            this.groupBox22.ResumeLayout(false);
            this.groupBox24.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.groupBox28.ResumeLayout(false);
            this.groupBox58.ResumeLayout(false);
            this.groupBox59.ResumeLayout(false);
            this.groupBox64.ResumeLayout(false);
            this.groupBox65.ResumeLayout(false);
            this.groupBox66.ResumeLayout(false);
            this.groupBox50.ResumeLayout(false);
            this.groupBox60.ResumeLayout(false);
            this.groupBox61.ResumeLayout(false);
            this.groupBox62.ResumeLayout(false);
            this.groupBox63.ResumeLayout(false);
            this.groupBox49.ResumeLayout(false);
            this.groupBox51.ResumeLayout(false);
            this.groupBox52.ResumeLayout(false);
            this.groupBox53.ResumeLayout(false);
            this.groupBox54.ResumeLayout(false);
            this.groupBox55.ResumeLayout(false);
            this.groupBox56.ResumeLayout(false);
            this.groupBox57.ResumeLayout(false);
            this.ResumeLayout(false);
        this.pictureBox1.Image = Road.Service.Properties.Resources._404;
        this.pictureBox2.Image = Road.Service.Properties.Resources._404;
        this.pictureBox3.Image = Road.Service.Properties.Resources._404;
    }

    
    private void button18_Click_1(object sender, EventArgs e)
    {
        if (Online_Text.Text == "Offline")
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
                    MessageBox.Show("Thành Công");
                }
                else
                {
                    empty = string.Format("UPDATE Sys_Users_Texp SET attTexpExp = {0}, defTexpExp = {0}, lukTexpExp = {0}, spdTexpExp = {0}, hpTexpExp = {0}, magicAtkTexpExp = {0}, magicDefTexpExp = {0} WHERE UserID = {1}", 56655500, int.Parse(UserID_Lab.Text));
                    db = new DataBaseHelper("conString");
                    if (db.ExecuteNonQuery(empty))
                    {
                        MessageBox.Show("Thành Công");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng thoát khỏi game để sử dụng tính năng này!");
        }
        
    }

    //tu luyện
    private void button19_Click_1(object sender, EventArgs e)
    {
        if (Online_Text.Text == "Offline")
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
                    MessageBox.Show("Thành Công");
                }
                else
                {
                    empty = string.Format("UPDATE Sys_Users_Texp SET TrainCritTexpExp = {0}, TrainSunderTexpExp = {0}, TrainCritDmgTexpExp = {0}, TrainSpeedTexpExp = {0}, TrainTricKillTexpExp = {0}, TrainDmgTexpExp = {0}, TrainArmorTexpExp = {0} WHERE UserID = {1}", 56655500, int.Parse(UserID_Lab.Text));
                    db = new DataBaseHelper("conString");
                    if (db.ExecuteNonQuery(empty))
                    {
                        MessageBox.Show("Thành Công");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng thoát khỏi game để sử dụng tính năng này!");
        }
        
    }
    //nguyên tố
    private void button54_Click(object sender, EventArgs e)
    {
        if (Online_Text.Text == "Offline")
        {
            string empty = string.Empty;
            if (Level_Text.Text != "" && UserID_Lab.Text != "0")
            {
                GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
                if (playerById != null)
                {
                    playerById.PlayerCharacter.Texp.TrainAvoidDmgTextExp = 56655500;
                    playerById.PlayerCharacter.Texp.TrainTricDefTexpExp = 56655500;
                    playerById.PlayerCharacter.Texp.TrainTenacTexpExp = 56655500;
                    playerById.PlayerCharacter.Texp.TrainGuardTexpExp = 56655500;
                    playerById.PlayerCharacter.Texp.TrainFireDefTexpExp = 56655500;
                    playerById.PlayerCharacter.Texp.TrainWaterDefTexpExp = 56655500;
                    playerById.PlayerCharacter.Texp.TrainWindDefTexpExp = 56655500;
                    playerById.PlayerCharacter.Texp.TrainLandDefTexpExp = 56655500;
                    playerById.UpdateProperties();
                    playerById.SendUpdatePublicPlayer();
                    MessageBox.Show("Thành Công");
                }
                else
                {
                    empty = string.Format("UPDATE Sys_Users_Texp SET TrainAvoidDmgTextExp = {0}, TrainTricDefTexpExp = {0}, TrainTenacTexpExp = {0}, TrainGuardTexpExp = {0}, TrainFireDefTexpExp = {0}, TrainWaterDefTexpExp = {0}, TrainWindDefTexpExp = {0},TrainLandDefTexpExp = {0} WHERE UserID = {1}", 56655500, int.Parse(UserID_Lab.Text));
                    db = new DataBaseHelper("conString");
                    if (db.ExecuteNonQuery(empty))
                    {
                        MessageBox.Show("Thành Công");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng thoát khỏi game để sử dụng tính năng này!");
        }
        
    }
    //thẻ bài
    private void button16_Click_1(object sender, EventArgs e)
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
                playerById.PlayerCharacter.CardInfo.Attack = 240;
                playerById.PlayerCharacter.CardInfo.Defence = 240;
                playerById.PlayerCharacter.CardInfo.Agility = 240;
                playerById.PlayerCharacter.CardInfo.Lucky = 240;
                CardNewSystemMgr.Out.SendCardInfo(playerById);
                playerById.UpdateProperties();
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = string.Format("UPDATE Sys_Users_CardInfo SET CardLevel = {0}, Point = {1}, Armor = {2}, Crits = {2}, Trick = {2}, Viole = {2}, Attack = {3}, Defence = {3}, Agility = {3}, Lucky = {3} WHERE UserID = {4}", 90, 209, 0, 240, int.Parse(UserID_Lab.Text));
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button17_Click_1(object sender, EventArgs e)
    {
        string empty = string.Empty;
        if (Level_Text.Text != "" && UserID_Lab.Text != "0")
        {
            //Đột phá
            GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
            if (playerById != null)
            {
                //int MaxTemplateID = 314101;
                //int Count = 1;

                //while (Count <= 83)
                //{
                //    if (MaxTemplateID != 314150)
                //    {
                //        playerById.PlayerCharacter..TemplateID = MaxTemplateID;
                //    }
                //}
                //MaxTemplateID++;
                //Count++;
                playerById.PlayerCharacter.CardInfo.SuitLevel = 31;
                CardNewSystemMgr.Out.SendCardInfo(playerById);
                playerById.UpdateProperties();
                MessageBox.Show("Thành Công");
                return;
            }
            empty = $"UPDATE Sys_Users_CardInfo SET SuitLevel = {31} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
            db = new DataBaseHelper("conString");
            if (db.ExecuteNonQuery(empty))
            {
                MessageBox.Show("Thành Công");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button12_Click_1(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
                return;
            }
            empty = $"UPDATE Sys_Users_Detail SET totemId = {10350} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
            db = new DataBaseHelper("conString");
            if (db.ExecuteNonQuery(empty))
            {
                MessageBox.Show("Thành Công");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button13_Click_1(object sender, EventArgs e)
    {
        if (Online_Text.Text == "Offline")
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
                    MessageBox.Show("Thành Công");
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
                        MessageBox.Show("Thành Công");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng thoát khỏi game trước khi sử dụng tính năng này !");
        }
    }

    private void button14_Click_1(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = $"UPDATE Sys_Active_System_Data SET CurrentSelectIndex = {6}, CurrentLevel = {60}, HomeTempleExp = {54940724} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button15_Click_2(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = $"UPDATE Sys_Active_System_Data SET promoteLevel = {60} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button20_Click_1(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                MessageBox.Show("Offline");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button21_Click_1(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                MessageBox.Show("Offline");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button22_Click_1(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                MessageBox.Show("Offline");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button23_Click_1(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
                return;
            }
            empty = $"UPDATE Sys_Users_Detail SET necklaceExp = {630000} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
            db = new DataBaseHelper("conString");
            if (db.ExecuteNonQuery(empty))
            {
                MessageBox.Show("Thành Công");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button24_Click_1(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = $"UPDATE Sys_Users_Detail SET necklaceCastLevel = {40} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button25_Click_1(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
                return;
            }
            empty = $"UPDATE Sys_Users_Detail SET fineSuitExp = {345860} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
            db = new DataBaseHelper("conString");
            if (db.ExecuteNonQuery(empty))
            {
                MessageBox.Show("Thành Công");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button53_Click(object sender, EventArgs e)
    {
        string empty = string.Empty;
        if (Level_Text.Text != "" && UserID_Lab.Text != "0")
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
            if (playerById != null)
            {
                playerById.PlayerCharacter.evolutionGrade = 50;
                playerById.PlayerCharacter.evolutionExp = 809026;
                playerById.UpdateProperties();
                playerById.SendUpdatePublicPlayer();
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = $"UPDATE Sys_Users_Detail SET evolutionGrade = {50}, evolutionGrade = {809026} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button28_Click_1(object sender, EventArgs e)
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
                    gemStone.FigSpiritIdValue = "6,0,0|6,0,1|6,0,2";
                    playerBussiness.UpdateGemStoneInfo(gemStone);
                    playerById.UpdateGemStone(place, gemStone);
                    playerById.Out.SendPlayerFigSpiritUp(playerById.PlayerCharacter.ID, gemStone, isUp: true, isMaxLevel: true, isFall: true, 0, 0);
                }
                playerById.UpdateProperties();
                MessageBox.Show("Thành Công");
            }
            else
            {
                MessageBox.Show("Offline");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button51_Click(object sender, EventArgs e)
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
                MessageBox.Show("Thành Công");
            }
            else
            {
                empty = string.Format("UPDATE Sys_Eat_Pets SET clothesLevel = {0}, weaponLevel = {0}, hatLevel = {0}, clothesExp = {1}, weaponExp = {1}, hatExp = {1} WHERE UserID = {2}", 30, 30000, int.Parse(UserID_Lab.Text));
                db = new DataBaseHelper("conString");
                if (db.ExecuteNonQuery(empty))
                {
                    MessageBox.Show("Thành Công");
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn Player ở mục Player để sửa");
        }
    }

    private void button65_Click(object sender, EventArgs e)
    {
        if (Online_Text.Text == "Offline")
        {


            string empty = string.Empty;
            if (Level_Text.Text != "" && UserID_Lab.Text != "0")
            {
                // Update Max Level Thú Cưỡi
                GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
                if (playerById != null)
                {
                    playerById.Horse.Info.curLevel = 89;
                    playerById.Horse.Info.curExp = 14733460;
                    playerById.PlayerCharacter.MountLv = 89;
                    playerById.Out.SendUpHorse(isHasLevelUp: true, playerById.Horse.Info.curLevel, playerById.Horse.Info.curExp);
                    playerById.UpdateProperties();
                }
                else
                {
                    empty = $"UPDATE Mount_Data SET curLevel = {89}, curExp = {14733460} WHERE UserID = {int.Parse(UserID_Lab.Text)}";
                    db = new DataBaseHelper("conString");
                    if (db.ExecuteNonQuery(empty))
                    {
                        MessageBox.Show("Thành Công");
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Player để sửa");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng thoát khỏi game trước khi sử dụng tính năng này !");
        }
    }

    private void button66_Click(object sender, EventArgs e)
    {
        if (Online_Text.Text == "Offline")
        {


            string empty = string.Empty;
            if (Level_Text.Text != "" && UserID_Lab.Text != "0")
            {
                
                GamePlayer playerById = WorldMgr.GetPlayerById(int.Parse(UserID_Lab.Text));
                // Update Max Kỹ Năng Thú Cưỡi
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
                    MessageBox.Show("Thành Công");
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
                    MessageBox.Show("Thành Công");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn Player để sửa");
            }
        }
        else
        {
            MessageBox.Show("Vui lòng thoát khỏi game trước khi sử dụng tính năng này !");
        }
    }

    
}
