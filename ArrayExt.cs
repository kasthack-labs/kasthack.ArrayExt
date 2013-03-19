/* github.com/kasthack/ArrayExt
 * ----------------------------------------------------------------------------
 * "THE BORSCHT-WARE LICENSE" (Revision 42):
 * <kasthack@epicm.org> wrote this file. As long as you retain this notice you
 * can do whatever you want with this stuff. If we meet some day, and you think
 * this stuff is worth it, you can buy me a Borscht.
 * ----------------------------------------------------------------------------
 */
using System;
namespace kasthack.Tools {
	public static class ArrayExt {
		/// <summary>
		/// Binary search with custom comparison
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="value">Value to search</param>
		/// <param name="comparison">Comparison delegate. Example: (a,b)=>String.Compare(a,b)</param>
		/// <returns>Index of value or ~index of nearest if not found</returns>
		public static int BinarySearch<T>( T[] array, T value, Comparison<T> comparison ) {
			return BinarySearch<T>(array, value, comparison, 0, array.Length);
		}
		/// <summary>
		/// Binary search with custom comparison
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="value">Value to search</param>
		/// <param name="comparison">Comparison delegate. Example: (a,b)=>String.Compare(a,b)</param>
		/// <param name="min">Min index(inclusive)</param>
		/// <param name="max">Max index(exclusive)</param>
		/// <returns>Index of value or ~index of nearest if not found</returns>
		public static int BinarySearch<T>( T[] array, T value, Comparison<T> comparison, int min, int max ) {
			if ( array == null )
				throw new ArgumentNullException("array");
			if ( array.Rank > 1 )
				throw new RankException("Only single dimension arrays are supported.");
			if ( array.Length == 0 )
				return -1;
			if ( comparison == null )
				throw new ArgumentNullException("comparison");
			if ( ( value != null ) && !( value is IComparable ) )
				throw new ArgumentException("comparer is null and value does not support IComparable.");
			int iMin = min,
					iMax = min + max - 1,
					iCmp = 0;
			try {
				int iMid;
				while ( iMin <= iMax ) {
					iMid = iMin + ( ( iMax - iMin ) / 2 );
					iCmp = comparison(array[iMid], value);
					if ( iCmp == 0 ) return iMid;
					if ( iCmp > 0 ) iMax = iMid - 1;
					else iMin = iMid + 1;
				}
			}
			catch ( Exception e ) {
				throw new InvalidOperationException("Comparer threw an exception.", e);
			}
			return ~iMin;
		}
		/// <summary>
		/// Shuffle elements in array
		/// </summary>
		/// <param name="array">Array</param>
		public static void Shuffle<T>( this T[] array ) {
			Shuffle(array, new Random(), array.Length - 1, 0);
		}
		/// <summary>
		/// Shuffle elements in array
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="r">Your instanse of RNG</param>
		public static void Shuffle<T>( this T[] array, Random r) {
			Shuffle(array, r, 0, array.Length - 1);
		}
		/// <summary>
		/// Shuffle array between two indexes
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="r">Your instanse of RNG</param>
		/// <param name="min">Min index(inclusive)</param>
		/// <param name="max">Min index(inclusive)</param>
		public static void Shuffle<T>( this T[] array, Random r, int min, int max ) {
			int cnt = max;
			int _cnt = cnt + 1;
			T o;
			int b = r.Next(min,_cnt);
			for ( int a = min; a < cnt; ) {
				o = array[a];
				array[a] = array[b];
				array[b] = o;
				b = r.Next(++a, _cnt);
			}
		}
		/// <summary>
		/// Swap two elements in array
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="a">Index of first element</param>
		/// <param name="b">Index of second element</param>
		public static void Swap<T>( this T[] array, int a, int b ) {
			T o = array[a];
			array[a] = array[b];
			array[b] = o;
		}
	}
}
