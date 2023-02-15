namespace System.Security.Claims;

public class Claims : Dictionary<string, string>, IEnumerable<Claim>
{
	IEnumerator<Claim> IEnumerable<Claim>.GetEnumerator()
	{
		var query = from type in this.Keys
					let value = this[type]
					let claim = new Claim(type, value)
					select claim;
		return query.GetEnumerator();
	}
}
