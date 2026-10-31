using System.ServiceModel;

namespace Game.Service.GMService;

[ServiceContract]
public interface IService
{
	[OperationContract]
	void ShowMessage(int type, string msg);
}
