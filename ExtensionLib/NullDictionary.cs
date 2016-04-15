//	NullDictionary: Created 02/29/2016 - Johnny Olsa

using System;
using System.Linq;

namespace System.Collections.Generic
{
	public enum DictionaryMergeOptions
	{
		IgnoreDuplicateKey,
		OverwriteDuplicateKey
	}

	/// <summary>
	/// A dictionary of objects or nullable type that returns null if the key does not exist.
	/// Saves the step of checking .ContainsKey first.
	/// </summary>
	public class NullDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		/// <summary>
		/// Initializes a new instance of the NullDictionary class that is empty and uses the default equality 
		/// comparer for the key type.
		/// </summary>
		public NullDictionary() : this(new Dictionary<TKey, TValue>(), default(IEqualityComparer<TKey>)) { }
		/// <summary>
		/// Initializes a new instance of the NullDictionary class that is empty and uses the specified equality 
		/// comparer.
		/// </summary>
		public NullDictionary(IEqualityComparer<TKey> comparer) : this(new Dictionary<TKey, TValue>(), comparer) { }
		/// <summary>
		/// Initializes a new instance of a NullDictionary that contains elements copied from the specified
		/// Dictionary and uses the default equality comparer for the key type.
		/// </summary>
		public NullDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, default(IEqualityComparer<TKey>)) { }
		/// <summary>
		/// Initializes a new instance of a NullDictionary that contains elements copied from the specified
		/// Dictionary and uses the specified equality comparer.
		/// </summary>
		public NullDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
		{
			//	Ensure TValue is nullable
			new Dictionary<string, string>();
			var t = typeof(TValue);
			if (Nullable.GetUnderlyingType(t) == null && !t.IsClass)
				throw new InvalidOperationException("TValue type must be nullable or reference type");
		}
		public new TValue this[TKey key]
		{
			get { return ContainsKey(key) ? base[key] : default(TValue); }
			set
			{
				if (ContainsKey(key))
					base[key] = value;
				else
					Add(key, value);
			}
		}
	}

	public static class NullDictionaryExt
	{
		/// <summary>
		/// Returns merged dictionaries, ignoring duplicate keys.
		/// NOTE: original dictionary will be modified (will be equal to return value)
		/// </summary>
		/// <param name="otherDict">Dictionary to merge into this one.</param>
		public static IDictionary<TKey, TValue> MergeDictionary<TKey, TValue>(this IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> otherDict)
		{
			return original.MergeDictionary(otherDict, DictionaryMergeOptions.IgnoreDuplicateKey);
		}
		/// <summary>
		/// Returns merged dictionaries, either ignoring duplicate keys or overwriting as specified by option parameter.
		/// NOTE: original dictionary will be modified (will be equal to return value)
		/// </summary>
		/// <param name="otherDict">Dictionary to merge into this one.</param>
		/// <param name="option">Overwrite duplicate keys or ignore them</param>
		public static IDictionary<TKey, TValue> MergeDictionary<TKey, TValue>(this IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> otherDict, DictionaryMergeOptions option)
		{
			//	If overwriting duplicates, do that first
			if (option == DictionaryMergeOptions.OverwriteDuplicateKey)
				otherDict.Where(kv => original.ContainsKey(kv.Key)).ToList().ForEach(kv => original[kv.Key] = kv.Value);
			//	Add Missing
			otherDict.Where(kv => !original.ContainsKey(kv.Key)).ToList().ForEach(kv => original.Add(kv.Key, kv.Value));
			return original;
		}
		public static NullDictionary<TKey, TSource> ToNullDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return new NullDictionary<TKey, TSource>(source.ToDictionary(keySelector));
		}
		public static NullDictionary<TKey, TSource> ToNullDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return new NullDictionary<TKey, TSource>(source.ToDictionary(keySelector, comparer), comparer);
		}
		public static NullDictionary<TKey, TElement> ToNullDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return new NullDictionary<TKey, TElement>(source.ToDictionary(keySelector, elementSelector));
		}
		public static NullDictionary<TKey, TElement> ToNullDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			return new NullDictionary<TKey, TElement>(source.ToDictionary(keySelector, elementSelector, comparer), comparer);
		}
	}
}
