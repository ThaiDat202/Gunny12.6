using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;

namespace Bussiness.CenterService;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[MessageContract(WrapperName = "ValidateLoginAndGetID", WrapperNamespace = "http://tempuri.org/", IsWrapped = true)]
public class ValidateLoginAndGetIDRequest
{
	[MessageBodyMember(Namespace = "http://tempuri.org/", Order = 0)]
	public string name;

	[MessageBodyMember(Namespace = "http://tempuri.org/", Order = 1)]
	public string password;

	[MessageBodyMember(Namespace = "http://tempuri.org/", Order = 2)]
	public int zoneId;

	[MessageBodyMember(Namespace = "http://tempuri.org/", Order = 3)]
	public int userID;

	[MessageBodyMember(Namespace = "http://tempuri.org/", Order = 4)]
	public bool isFirst;

	public ValidateLoginAndGetIDRequest()
	{
	}

	public ValidateLoginAndGetIDRequest(string name, string password, int zoneId, int userID, bool isFirst)
	{
		this.name = name;
		this.password = password;
		this.zoneId = zoneId;
		this.userID = userID;
		this.isFirst = isFirst;
	}
}
