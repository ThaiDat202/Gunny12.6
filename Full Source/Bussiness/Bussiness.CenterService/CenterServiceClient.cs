using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Bussiness.CenterService;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class CenterServiceClient : ClientBase<ICenterService>, ICenterService
{
	public CenterServiceClient()
	{
	}

	public CenterServiceClient(string endpointConfigurationName)
		: base(endpointConfigurationName)
	{
	}

	public CenterServiceClient(string endpointConfigurationName, string remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public CenterServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public CenterServiceClient(Binding binding, EndpointAddress remoteAddress)
		: base(binding, remoteAddress)
	{
	}

	public ServerData[] GetServerList(int zoneId)
	{
		return base.Channel.GetServerList(zoneId);
	}

	public Task<ServerData[]> GetServerListAsync(int zoneId)
	{
		return base.Channel.GetServerListAsync(zoneId);
	}

	public string GetServerListByStr(int zoneID)
	{
		return base.Channel.GetServerListByStr(zoneID);
	}

	public Task<string> GetServerListByStrAsync(int zoneID)
	{
		return base.Channel.GetServerListByStrAsync(zoneID);
	}

	public string GetAllChat()
	{
		return base.Channel.GetAllChat();
	}

	public Task<string> GetAllChatAsync()
	{
		return base.Channel.GetAllChatAsync();
	}

	public int GetPlayerState(int playerID, int zoneId)
	{
		return base.Channel.GetPlayerState(playerID, zoneId);
	}

	public Task<int> GetPlayerStateAsync(int playerID, int zoneId)
	{
		return base.Channel.GetPlayerStateAsync(playerID, zoneId);
	}

	public bool ChargeMoney(int userID, int zoneId, string chargeID)
	{
		return base.Channel.ChargeMoney(userID, zoneId, chargeID);
	}

	public Task<bool> ChargeMoneyAsync(int userID, int zoneId, string chargeID)
	{
		return base.Channel.ChargeMoneyAsync(userID, zoneId, chargeID);
	}

	public bool SystemNotice(string msg)
	{
		return base.Channel.SystemNotice(msg);
	}

	public Task<bool> SystemNoticeAsync(string msg)
	{
		return base.Channel.SystemNoticeAsync(msg);
	}

	public bool KitoffUser(int playerID, int zoneId, string msg)
	{
		return base.Channel.KitoffUser(playerID, zoneId, msg);
	}

	public Task<bool> KitoffUserAsync(int playerID, int zoneId, string msg)
	{
		return base.Channel.KitoffUserAsync(playerID, zoneId, msg);
	}

	public bool ReLoadServerList()
	{
		return base.Channel.ReLoadServerList();
	}

	public Task<bool> ReLoadServerListAsync()
	{
		return base.Channel.ReLoadServerListAsync();
	}

	public bool MailNotice(int playerID, int zoneId)
	{
		return base.Channel.MailNotice(playerID, zoneId);
	}

	public Task<bool> MailNoticeAsync(int playerID, int zoneId)
	{
		return base.Channel.MailNoticeAsync(playerID, zoneId);
	}

	public bool ActivePlayer(bool isActive)
	{
		return base.Channel.ActivePlayer(isActive);
	}

	public Task<bool> ActivePlayerAsync(bool isActive)
	{
		return base.Channel.ActivePlayerAsync(isActive);
	}

	public bool CreatePlayer(int id, string name, string password, int zoneId, bool isFirst)
	{
		return base.Channel.CreatePlayer(id, name, password, zoneId, isFirst);
	}

	public Task<bool> CreatePlayerAsync(int id, string name, string password, int zoneId, bool isFirst)
	{
		return base.Channel.CreatePlayerAsync(id, name, password, zoneId, isFirst);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	ValidateLoginAndGetIDResponse ICenterService.ValidateLoginAndGetID(ValidateLoginAndGetIDRequest request)
	{
		return base.Channel.ValidateLoginAndGetID(request);
	}

	public bool ValidateLoginAndGetID(string name, string password, int zoneId, ref int userID, ref bool isFirst)
	{
		ValidateLoginAndGetIDRequest validateLoginAndGetIDRequest = new ValidateLoginAndGetIDRequest();
		validateLoginAndGetIDRequest.name = name;
		validateLoginAndGetIDRequest.password = password;
		validateLoginAndGetIDRequest.zoneId = zoneId;
		validateLoginAndGetIDRequest.userID = userID;
		validateLoginAndGetIDRequest.isFirst = isFirst;
		ValidateLoginAndGetIDResponse validateLoginAndGetIDResponse = ((ICenterService)this).ValidateLoginAndGetID(validateLoginAndGetIDRequest);
		userID = validateLoginAndGetIDResponse.userID;
		isFirst = validateLoginAndGetIDResponse.isFirst;
		return validateLoginAndGetIDResponse.ValidateLoginAndGetIDResult;
	}

	public Task<ValidateLoginAndGetIDResponse> ValidateLoginAndGetIDAsync(ValidateLoginAndGetIDRequest request)
	{
		return base.Channel.ValidateLoginAndGetIDAsync(request);
	}

	public bool AASUpdateState(bool state)
	{
		return base.Channel.AASUpdateState(state);
	}

	public Task<bool> AASUpdateStateAsync(bool state)
	{
		return base.Channel.AASUpdateStateAsync(state);
	}

	public int AASGetState()
	{
		return base.Channel.AASGetState();
	}

	public Task<int> AASGetStateAsync()
	{
		return base.Channel.AASGetStateAsync();
	}

	public int ExperienceRateUpdate(int serverId)
	{
		return base.Channel.ExperienceRateUpdate(serverId);
	}

	public Task<int> ExperienceRateUpdateAsync(int serverId)
	{
		return base.Channel.ExperienceRateUpdateAsync(serverId);
	}

	public int NoticeServerUpdate(int serverId, int type)
	{
		return base.Channel.NoticeServerUpdate(serverId, type);
	}

	public Task<int> NoticeServerUpdateAsync(int serverId, int type)
	{
		return base.Channel.NoticeServerUpdateAsync(serverId, type);
	}

	public bool UpdateConfigState(int type, bool state)
	{
		return base.Channel.UpdateConfigState(type, state);
	}

	public Task<bool> UpdateConfigStateAsync(int type, bool state)
	{
		return base.Channel.UpdateConfigStateAsync(type, state);
	}

	public int GetConfigState(int type)
	{
		return base.Channel.GetConfigState(type);
	}

	public Task<int> GetConfigStateAsync(int type)
	{
		return base.Channel.GetConfigStateAsync(type);
	}

	public bool Reload(string type)
	{
		return base.Channel.Reload(type);
	}

	public Task<bool> ReloadAsync(string type)
	{
		return base.Channel.ReloadAsync(type);
	}

	public bool Shutdown(string type)
	{
		return base.Channel.Shutdown(type);
	}

	public Task<bool> ShutdownAsync(string type)
	{
		return base.Channel.ShutdownAsync(type);
	}
}
