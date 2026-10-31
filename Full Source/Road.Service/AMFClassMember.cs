public class AMFClassMember
{
	private string _name;

	private int _bindingFlags;

	private int _memberType;

	private object[] _customAttributes;

	public string Name => _name;

	public int BindingFlags => _bindingFlags;

	public int MemberType => _memberType;

	public object[] CustomAttributes => _customAttributes;

	public AMFClassMember(string name, int bindingFlags, int memberType, object[] customAttributes)
	{
		_name = name;
		_bindingFlags = bindingFlags;
		_memberType = memberType;
		_customAttributes = customAttributes;
	}
}
