using System.Collections.Generic;
using System.Linq;

namespace Bussiness;

public static class EnumerableExtensions
{
	public static T Random<T>(this IEnumerable<T> enumerable)
	{
		ThreadSafeRandom threadSafeRandom = new ThreadSafeRandom();
		IList<T> source = (enumerable as IList<T>) ?? enumerable.ToList();
		return source.ElementAt(threadSafeRandom.Next(0, source.Count()));
	}
}
