public class AMFClassDefinition
{
	private string _className;

	private AMFClassMember[] _members;

	private bool _externalizable;

	private bool _dynamic;

	public static AMFClassMember[] EmptyClassMembers = new AMFClassMember[0];

	public string ClassName => _className;

	public int MemberCount
	{
		get
		{
			if (_members == null)
			{
				return 0;
			}
			return _members.Length;
		}
	}

	public AMFClassMember[] Members => _members;

	public bool IsExternalizable => _externalizable;

	public bool IsDynamic => _dynamic;

	public bool IsTypedObject => _className != null && _className != string.Empty;

	public AMFClassDefinition(string className, AMFClassMember[] members, bool externalizable, bool dynamic)
	{
		_className = className;
		_members = members;
		_externalizable = externalizable;
		_dynamic = dynamic;
	}
}
