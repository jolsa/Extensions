//	JoinExtensions: Created 07/12/2014 - Johnny Olsa

using System.Linq;

namespace System.Collections.Generic
{
	public enum OrderByDirection
	{
		None = 0,
		Ascending,
		Descending
	}
	public static class OrderExtensions
	{
		/// <summary>
		///		Performs an ordering of the elements in a sequence in the specified order by using a specified comparer.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="value">An IEnumerable&lt;TElement&gt; that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="comparer">An IComparer&lt;T&gt; to compare keys.</param>
		/// <param name="direction">Ascending or Descending</param>
		/// <returns>An IOrderedEnumerable&lt;TElement&gt; whose elements are sorted according to a key.</returns>
		public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> value, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, OrderByDirection direction)
		{
			//	Ideally, if order is not needed, the method would not be called, but sometimes it's easier to code a "no sort" option.
			//	Testing shows the "OrderByDirection.None" is negligible on large IEnumerables
			if (direction == OrderByDirection.None)
				return value.OrderBy(o => false);
			return direction == OrderByDirection.Descending ? value.OrderByDescending(keySelector, comparer) : value.OrderBy(keySelector, comparer);
		}
		/// <summary>
		///		Performs an ordering of the elements in a sequence in the specified order by using the default comparer.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="value">An IEnumerable&lt;TElement&gt; that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="direction">Ascending or Descending</param>
		/// <returns>An IOrderedEnumerable&lt;TElement&gt; whose elements are sorted according to a key.</returns>
		public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> value, Func<TSource, TKey> keySelector, OrderByDirection direction)
		{
			return value.OrderByWithDirection(keySelector, default(IComparer<TKey>), direction);
		}
		/// <summary>
		///		Performs a subsequent ordering of the elements in a sequence in the specified order by using a specified comparer.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="value">An IOrderedEnumerable&lt;TElement&gt; that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="comparer">An IComparer&lt;T&gt; to compare keys.</param>
		/// <param name="direction">Ascending or Descending</param>
		/// <returns>An IOrderedEnumerable&lt;TElement&gt; whose elements are sorted according to a key.</returns>
		public static IOrderedEnumerable<TSource> ThenByWithDirection<TSource, TKey>(this IOrderedEnumerable<TSource> value, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, OrderByDirection direction)
		{
			//	Ideally, if order is not needed, the method would not be called, but sometimes it's easier to code a "no sort" option.
			//	Testing shows the "OrderByDirection.None" is negligible on large IOrderedEnumerables
			if (direction == OrderByDirection.None)
				return value;
			return direction == OrderByDirection.Descending ? value.ThenByDescending(keySelector, comparer) : value.ThenBy(keySelector, comparer);
		}
		/// <summary>
		///		Performs a subsequent ordering of the elements in a sequence in the specified order by using the default comparer.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="value">An IOrderedEnumerable&lt;TElement&gt; that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="direction">Ascending or Descending</param>
		/// <returns>An IOrderedEnumerable&lt;TElement&gt; whose elements are sorted according to a key.</returns>
		public static IOrderedEnumerable<TSource> ThenByWithDirection<TSource, TKey>(this IOrderedEnumerable<TSource> value, Func<TSource, TKey> keySelector, OrderByDirection direction)
		{
			return value.ThenByWithDirection(keySelector, default(IComparer<TKey>), direction);
		}
	}
	/// <summary>
	/// Join Extensions that .NET should have provided?
	/// </summary>
	public static class JoinExtensions
	{
		/// <summary>
		/// Correlates the elements of two sequences and produces a Cartesian product.
		/// </summary>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="resultSelector">A function to create a result element from the combined elements.</param>
		/// <returns>
		/// An System.Collections.Generic.IEnumerable&lt;T&gt; that has elements of type TResult
		/// that are obtained by combining the two sequences.
		/// </returns>
		public static IEnumerable<TResult> CrossJoin<TOuter, TInner, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TInner, TResult> resultSelector)
		{
			return outer.SelectMany(o => inner.Select(i => resultSelector(o, i)));
		}

		/// <summary>
		/// Correlates the elements of two sequences based on matching keys. A specified
		/// System.Collections.Generic.IEqualityComparer&lt;T&gt; is used to compare keys.
		/// </summary>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from two combined elements.</param>
		/// <param name="comparer">A System.Collections.Generic.IEqualityComparer&lt;T&gt; to hash and compare keys.</param>
		/// <returns>
		/// An System.Collections.Generic.IEnumerable&lt;T&gt; that has elements of type TResult
		/// that are obtained by performing an left outer join on two sequences.
		/// </returns>
		/// <example>
		/// Example:
		/// <code>
		/// class TestClass
		/// {
		///		static int Main()
		///		{
		///			var strings1 = new string[] { "1", "2", "3", "4", "a" };
		///			var strings2 = new string[] { "1", "2", "3", "16", "A" };
		///			
		///			var lj = strings1.LeftJoin(
		///				strings2,
		///				a => a,
		///				b => b,
		///				(a, b) => (a ?? "null") + "-" + (b ?? "null"),
		///				StringComparer.OrdinalIgnoreCase)
		///				.ToList();
		///		}
		///	}
		///	</code>
		/// </example>
		public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
			Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			return outer.GroupJoin(
				inner,
				outerKeySelector,
				innerKeySelector,
				(o, ei) => ei
					.Select(i => resultSelector(o, i))
					.DefaultIfEmpty(resultSelector(o, default(TInner))), comparer)
					.SelectMany(oi => oi);
		}

		/// <summary>
		/// Correlates the elements of two sequences based on matching keys. The default
		/// equality comparer is used to compare keys.
		/// </summary>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from two combined elements.</param>
		/// <returns>
		/// An System.Collections.Generic.IEnumerable&lt;T&gt; that has elements of type TResult
		/// that are obtained by performing an left outer join on two sequences.
		/// </returns>
		/// <example>
		/// Example:
		/// <code>
		/// class TestClass
		/// {
		///		static int Main()
		///		{
		///			var strings1 = new string[] { "1", "2", "3", "4", "a" };
		///			var strings2 = new string[] { "1", "2", "3", "16", "A" };
		///			
		///			var lj = strings1.LeftJoin(
		///				strings2,
		///				a => a,
		///				b => b,
		///				(a, b) => (a ?? "null") + "-" + (b ?? "null"))
		///				.ToList();
		///		}
		///	}
		///	</code>
		/// </example>
		public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
			Func<TOuter, TInner, TResult> resultSelector)
		{
			return outer.LeftJoin(inner, outerKeySelector, innerKeySelector, resultSelector, default(IEqualityComparer<TKey>));
		}

		/// <summary>
		/// Correlates the elements of two sequences based on matching keys. A specified
		/// System.Collections.Generic.IEqualityComparer&lt;T&gt; is used to compare keys.
		/// </summary>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from two combined elements.</param>
		/// <param name="comparer">A System.Collections.Generic.IEqualityComparer&lt;T&gt; to hash and compare keys.</param>
		/// <returns>
		/// An System.Collections.Generic.IEnumerable&lt;T&gt; that has elements of type TResult
		/// that are obtained by performing a right outer join on two sequences.
		/// </returns>
		/// <example>
		/// Example:
		/// <code>
		/// class TestClass
		/// {
		///		static int Main()
		///		{
		///			var strings1 = new string[] { "1", "2", "3", "4", "a" };
		///			var strings2 = new string[] { "1", "2", "3", "16", "A" };
		///			
		///			var lj = strings1.RightJoin(
		///				strings2,
		///				a => a,
		///				b => b,
		///				(a, b) => (a ?? "null") + "-" + (b ?? "null"),
		///				StringComparer.OrdinalIgnoreCase)
		///				.ToList();
		///		}
		///	}
		///	</code>
		/// </example>
		public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
			Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			return inner.LeftJoin(outer, innerKeySelector, outerKeySelector, (o, i) => resultSelector(i, o), comparer);
		}

		/// <summary>
		/// Correlates the elements of two sequences based on matching keys. The default
		/// equality comparer is used to compare keys.
		/// </summary>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from two combined elements.</param>
		/// <returns>
		/// An System.Collections.Generic.IEnumerable&lt;T&gt; that has elements of type TResult
		/// that are obtained by performing a right outer join on two sequences.
		/// </returns>
		/// <example>
		/// Example:
		/// <code>
		/// class TestClass
		/// {
		///		static int Main()
		///		{
		///			var strings1 = new string[] { "1", "2", "3", "4", "a" };
		///			var strings2 = new string[] { "1", "2", "3", "16", "A" };
		///			
		///			var lj = strings1.RightJoin(
		///				strings2,
		///				a => a,
		///				b => b,
		///				(a, b) => (a ?? "null") + "-" + (b ?? "null"))
		///				.ToList();
		///		}
		///	}
		///	</code>
		/// </example>
		public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
			Func<TOuter, TInner, TResult> resultSelector)
		{
			return outer.RightJoin(inner, outerKeySelector, innerKeySelector, resultSelector, default(IEqualityComparer<TKey>));
		}

		/// <summary>
		/// Correlates the elements of two sequences based on matching keys. A specified
		/// System.Collections.Generic.IEqualityComparer&lt;T&gt; is used to compare keys.
		/// </summary>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from two combined elements.</param>
		/// <param name="comparer">A System.Collections.Generic.IEqualityComparer&lt;T&gt; to hash and compare keys.</param>
		/// <returns>
		/// An System.Collections.Generic.IEnumerable&lt;T&gt; that has elements of type TResult
		/// that are obtained by performing a full outer join on two sequences.
		/// </returns>
		/// <example>
		/// Example:
		/// <code>
		/// class TestClass
		/// {
		///		static int Main()
		///		{
		///			var strings1 = new string[] { "1", "2", "3", "4", "a" };
		///			var strings2 = new string[] { "1", "2", "3", "16", "A" };
		///			
		///			var lj = strings1.FullJoin(
		///				strings2,
		///				a => a,
		///				b => b,
		///				(a, b) => (a ?? "null") + "-" + (b ?? "null"),
		///				StringComparer.OrdinalIgnoreCase)
		///				.ToList();
		///		}
		///	}
		///	</code>
		/// </example>
		public static IEnumerable<TResult> FullJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
			Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			var leftInner = outer.LeftJoin(inner, outerKeySelector, innerKeySelector, (o, i) => new { o, i }, comparer);
			var defOuter = default(TOuter);
			var right = outer.RightJoin(inner, outerKeySelector, innerKeySelector, (o, i) => new { o, i }, comparer)
				.Where(oi => oi.o == null || oi.o.Equals(defOuter));
			return leftInner.Concat(right).Select(oi => resultSelector(oi.o, oi.i));
		}

		/// <summary>
		/// Correlates the elements of two sequences based on matching keys. The default
		/// equality comparer is used to compare keys.
		/// </summary>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from two combined elements.</param>
		/// <returns>
		/// An System.Collections.Generic.IEnumerable&lt;T&gt; that has elements of type TResult
		/// that are obtained by performing a full outer join on two sequences.
		/// </returns>
		/// <example>
		/// Example:
		/// <code>
		/// class TestClass
		/// {
		///		static int Main()
		///		{
		///			var strings1 = new string[] { "1", "2", "3", "4", "a" };
		///			var strings2 = new string[] { "1", "2", "3", "16", "A" };
		///			
		///			var lj = strings1.FullJoin(
		///				strings2,
		///				a => a,
		///				b => b,
		///				(a, b) => (a ?? "null") + "-" + (b ?? "null"))
		///				.ToList();
		///		}
		///	}
		///	</code>
		/// </example>
		public static IEnumerable<TResult> FullJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
			Func<TOuter, TInner, TResult> resultSelector)
		{
			return outer.FullJoin(inner, outerKeySelector, innerKeySelector, resultSelector, default(IEqualityComparer<TKey>));
		}

	}
}