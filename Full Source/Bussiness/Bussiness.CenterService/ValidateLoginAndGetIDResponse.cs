using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;

namespace Bussiness.CenterService;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[MessageContract(WrapperName = "ValidateLoginAndGetIDResponse", WrapperNamespace = "http://tempuri.org/", IsWrapped = true)]
public class ValidateLoginAndGetIDResponse
{
	[MessageBodyMember(Namespace = "http://tempuri.org/", Order = 0)]
	public bool ValidateLoginAndGetIDResult;

	[MessageBodyMember(Namespace = "http://tempuri.org/", Order = 1)]
	public int userID;

	[MessageBodyMember(Namespace = "http://tempuri.org/", Order = 2)]
	public bool isFirst;

	public ValidateLoginAndGetIDResponse()
	{
	}

	public ValidateLoginAndGetIDResponse(bool ValidateLoginAndGetIDResult, int userID, bool isFirst)
	{
		this.ValidateLoginAndGetIDResult = ValidateLoginAndGetIDResult;
		this.userID = userID;
		this.isFirst = isFirst;
	}
}
