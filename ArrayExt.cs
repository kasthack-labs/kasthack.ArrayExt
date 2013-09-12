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
		const int SEQUENTIAL_THRESHOLD = 8192;
	
		/// <summary>
		/// QuickSort array
		/// <param name="array">Array</param>
		/// </summary>
		public static void Sort<T>(this T [] array) where T : IComparable<T>{
			SortRange(array, (a,b)=>a.CompareTo(b), 0, array.Length - 1); }
		/// <summary>
		/// QuickSort array with custom comparison
		/// <param name="array">Array</param>
		/// <param name="comparison">Comparison delegate. Example: (a,b)=>String.Compare(a,b)</param>
		/// </summary>
		public static void Sort<T>(this T [] array, Comparison<T> comparison) {
			SortRange(array, comparison, 0, array.Length - 1); }
		/// <summary>
		/// Parallel QuickSort array
		/// <param name="array">Array</param>
		/// </summary>
		public static void PSort<T>(this T[] array) where T : IComparable<T>{
			PSortRange(array, (a,b)=>a.CompareTo(b), 0, array.Length - 1); }
		/// <summary>
		/// Parallel QuickSort range with custom comparison
		/// <param name="array">Array</param>
		/// <param name="comparison">Comparison delegate. Example: (a,b)=>String.Compare(a,b)</param>
		/// </summary>
		public static void PSort<T>(this T[] array, Comparison<T> comparison){
			PSortRange(array, comparison, 0, array.Length - 1); }
		/// <summary>
		/// QuickSort range
		/// <param name="array">Array</param>
		/// <param name="comparison">Comparison delegate. Example: (a,b)=>String.Compare(a,b)</param>
		/// <param name="left">Left</param>
		/// <param name="right">Right</param>
		/// </summary>
		public static void SortRange<T>(T[] array, Comparison<T> comparison, int left, int right){
			if (right > left) {
				int pivot = Partition(array, comparison, left, right);
				SortRange(array, comparison, left, pivot - 1);
				SortRange(array, comparison, pivot + 1, right);
			}
		}
		/// <summary>
		/// Parallel QuickSort range
		/// <param name="array">Array</param>
		/// <param name="comparison">Comparison delegate. Example: (a,b)=>String.Compare(a,b)</param>
		/// <param name="left">Left</param>
		/// <param name="right">Right</param>
		/// </summary>
		public static void PSortRange<T>(T[] array, Comparison<T> comparison, int left, int right){
			if (right > left) {
				if (right - left < SEQUENTIAL_THRESHOLD)
					SortRange(array, left, right);
				else
				{
					int pivot = Partition(array, comparison, left, right);
					Parallel.Invoke(new Action[] {
						()=>PSortRange(array, comparison, left, pivot - 1),
						()=>PSortRange(array, comparison, pivot + 1, right)
					});
				}
			}
		}
		private static int Partition<T>(T[] array, Comparison<T> comparison, int low, int high){
			int pivotPos = (high + low) / 2;
			T pivot = array[pivotPos];
			Swap(array, low, pivotPos);
			int left = low;
			for (int i = low + 1; i <= high; i++)
				if (comparison(array[i],pivot) < 0)
					Swap(array, i, ++left);
			Swap(array, low, left);
			return left;
		}
		/// <summary>
		/// Binary search with custom comparison
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="value">Value to search</param>
		/// <param name="comparison">Comparison delegate. Example: (a,b)=>String.Compare(a,b)</param>
		/// <returns>Index of value or ~index of nearest if not found</returns>
		public static int BinarySearch<T>(this T[] array, T value, Comparison<T> comparison ) {
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
		public static int BinarySearch<T>(this T[] array, T value, Comparison<T> comparison, int min, int max ) {
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
		public static void Shuffle<T>(this this T[] array ) {
			Shuffle(array, null, array.Length - 1, 0);
		}
		/// <summary>
		/// Shuffle elements in array
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="r">Your instanse of RNG</param>
		public static void Shuffle<T>( this T[] array, Random r=null) {
			Shuffle(array, 0, array.Length - 1, r);
		}
		/// <summary>
		/// Shuffle array between two indexes
		/// </summary>
		/// <param name="array">Array</param>
		/// <param name="r">Your instanse of RNG</param>
		/// <param name="min">Min index(inclusive)</param>
		/// <param name="max">Min index(inclusive)</param>
		public static void Shuffle<T>( this T[] array, int min, int max,Random r=null ) {
			r=r??new Random();
			int cnt = max;
			int _cnt = cnt + 1;
			T o;
			int b = r.Next(min,_cnt);
			for ( int a = min; a < cnt; ) {
				//swap inlined
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
