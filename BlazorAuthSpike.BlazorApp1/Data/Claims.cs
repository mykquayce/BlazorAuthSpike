namespace System.Security.Claims;

public class Claims : List<Claim>
{
	public void Add(string type, string value)
	{
		var claim = new Claim(type, value);
		base.Add(claim);
	}
}
