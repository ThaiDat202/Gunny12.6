using System.ServiceModel;

namespace Center.Server
{
	[ServiceContract]
	public interface ICenterService
	{
		[OperationContract]
		ServerData[] GetServerList(int zoneId);

		[OperationContract]
		string GetServerListByStr(int zoneID);

		[OperationContract]
		string GetAllChat();

		[OperationContract]
		int GetPlayerState(int playerID, int zoneId);

		[OperationContract]
		bool ChargeMoney(int userID, int zoneId, string chargeID);

		[OperationContract]
		bool SystemNotice(string msg);

		[OperationContract]
		bool KitoffUser(int playerID, int zoneId, string msg);

		[OperationContract]
		bool ReLoadServerList();

		[OperationContract]
		bool MailNotice(int playerID, int zoneId);

		[OperationContract]
		bool ActivePlayer(bool isActive);

		[OperationContract]
		bool CreatePlayer(int id, string name, string password, int zoneId, bool isFirst);

		[OperationContract]
		bool ValidateLoginAndGetID(string name, string password, int zoneId, ref int userID, ref bool isFirst);

		[OperationContract]
		bool AASUpdateState(bool state);

		[OperationContract]
		int AASGetState();

		[OperationContract]
		int ExperienceRateUpdate(int serverId);

		[OperationContract]
		int NoticeServerUpdate(int serverId, int type);

		[OperationContract]
		bool UpdateConfigState(int type, bool state);

		[OperationContract]
		int GetConfigState(int type);

		[OperationContract]
		bool Reload(string type);

		[OperationContract]
		bool Shutdown(string type);

		[OperationContract]
		bool LoginDevice(int zoneId, string content);
	}
}
