using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

//http://jacksondunstan.com/articles/3066

[Serializable]
public class QuickList<T>
	: IEnumerable, ICollection, IList, ICollection<T>, IEnumerable<T>, IList<T>
{
	private const int DefaultCapacity = 4;
	private T[] _items;
	private int _size;
	private int _version;
	private static readonly T[] EmptyArray;

	bool ICollection<T>.IsReadOnly
	{
		get
		{
			return false;
		}
	}

	bool ICollection.IsSynchronized
	{
		get
		{
			return false;
		}
	}

	object ICollection.SyncRoot
	{
		get
		{
			return (object) this;
		}
	}

	bool IList.IsFixedSize
	{
		get
		{
			return false;
		}
	}

	bool IList.IsReadOnly
	{
		get
		{
			return false;
		}
	}

	object IList.this[int index]
	{
		get
		{
			return (object) this[index];
		}
		set
		{
			try
			{
				this[index] = (T) value;
				return;
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
			throw new ArgumentException("value");
		}
	}

	public int Capacity
	{
		get
		{
			return _items.Length;
		}
		set
		{
			if ((uint) value < (uint) _size)
				throw new ArgumentOutOfRangeException();
			Array.Resize<T>(ref _items, value);
		}
	}

	public int Count
	{
		get
		{
			return _size;
		}
	}

	public T this[int index]
	{
		get
		{
			if ((uint) index >= (uint) _size)
				throw new ArgumentOutOfRangeException("index");
			return _items[index];
		}
		set
		{
			if (index < 0 || (uint) index >= (uint) _size)
				throw new ArgumentOutOfRangeException("index");
			_items[index] = value;
		}
	}

	public T[] BackingArray
	{
		get
		{
			return _items;
		}
	}

	static QuickList()
	{
		EmptyArray = new T[0];
	}

	public QuickList()
	{
		_items = EmptyArray;
	}

	public QuickList(IEnumerable<T> collection)
	{
		CheckCollection(collection);
		ICollection<T> collection1 = collection as ICollection<T>;
		if (collection1 == null)
		{
			_items = EmptyArray;
			AddEnumerable(collection);
		}
		else
		{
			_items = new T[collection1.Count];
			AddCollection(collection1);
		}
	}

	public QuickList(int capacity)
	{
		if (capacity < 0)
			throw new ArgumentOutOfRangeException("capacity");
		_items = new T[capacity];
	}

	internal QuickList(T[] data, int size)
	{
		_items = data;
		_size = size;
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return (IEnumerator<T>) GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		Array.Copy((Array) _items, 0, array, arrayIndex, _size);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return (IEnumerator) GetEnumerator();
	}

	int IList.Add(object item)
	{
		try
		{
			Add((T) item);
			return _size - 1;
		}
		catch (NullReferenceException)
		{
		}
		catch (InvalidCastException)
		{
		}
		throw new ArgumentException("item");
	}

	bool IList.Contains(object item)
	{
		try
		{
			return Contains((T) item);
		}
		catch (NullReferenceException)
		{
		}
		catch (InvalidCastException)
		{
		}
		return false;
	}

	int IList.IndexOf(object item)
	{
		try
		{
			return IndexOf((T) item);
		}
		catch (NullReferenceException)
		{
		}
		catch (InvalidCastException)
		{
		}
		return -1;
	}

	void IList.Insert(int index, object item)
	{
		CheckIndex(index);
		try
		{
			Insert(index, (T) item);
			return;
		}
		catch (NullReferenceException)
		{
		}
		catch (InvalidCastException)
		{
		}
		throw new ArgumentException("item");
	}

	void IList.Remove(object item)
	{
		try
		{
			Remove((T) item);
		}
		catch (NullReferenceException)
		{
		}
		catch (InvalidCastException)
		{
		}
	}

	public void Add(T item)
	{
		if (_size == _items.Length)
			GrowIfNeeded(1);
		_items[_size++] = item;
		++_version;
	}

	private void GrowIfNeeded(int newCount)
	{
		int val2 = _size + newCount;
		if (val2 <= _items.Length)
			return;
		Capacity = Math.Max(Math.Max(Capacity * 2, 4), val2);
	}

	private void CheckRange(int idx, int count)
	{
		if (idx < 0)
			throw new ArgumentOutOfRangeException("index");
		if (count < 0)
			throw new ArgumentOutOfRangeException("count");
		if ((uint) (idx + count) > (uint) _size)
			throw new ArgumentException("index and count exceed length of list");
	}

	private void AddCollection(ICollection<T> collection)
	{
		int count = collection.Count;
		if (count == 0)
			return;
		GrowIfNeeded(count);
		collection.CopyTo(_items, _size);
		_size += count;
	}

	private void AddEnumerable(IEnumerable<T> enumerable)
	{
		foreach (T obj in enumerable)
			Add(obj);
	}

	public void AddRange(IEnumerable<T> collection)
	{
		CheckCollection(collection);
		ICollection<T> collection1 = collection as ICollection<T>;
		if (collection1 != null)
			AddCollection(collection1);
		else
			AddEnumerable(collection);
		++_version;
	}

	public ReadOnlyCollection<T> AsReadOnly()
	{
		return new ReadOnlyCollection<T>((IList<T>) this);
	}

	public int BinarySearch(T item)
	{
		return Array.BinarySearch<T>(_items, 0, _size, item);
	}

	public int BinarySearch(T item, IComparer<T> comparer)
	{
		return Array.BinarySearch<T>(_items, 0, _size, item, comparer);
	}

	public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
	{
		CheckRange(index, count);
		return Array.BinarySearch<T>(_items, index, count, item, comparer);
	}

	public void Clear()
	{
		Array.Clear((Array) _items, 0, _items.Length);
		_size = 0;
		++_version;
	}

	public bool Contains(T item)
	{
		return Array.IndexOf<T>(_items, item, 0, _size) != -1;
	}

	public QuickList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
	{
		if (converter == null)
			throw new ArgumentNullException("converter");
		QuickList<TOutput> list = new QuickList<TOutput>(_size);
		for (int index = 0; index < _size; ++index)
			list._items[index] = converter(_items[index]);
		list._size = _size;
		return list;
	}

	public void CopyTo(T[] array)
	{
		Array.Copy((Array) _items, 0, (Array) array, 0, _size);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Array.Copy((Array) _items, 0, (Array) array, arrayIndex, _size);
	}

	public void CopyTo(int index, T[] array, int arrayIndex, int count)
	{
		CheckRange(index, count);
		Array.Copy((Array) _items, index, (Array) array, arrayIndex, count);
	}

	public bool Exists(Predicate<T> match)
	{
		CheckMatch(match);
		return GetIndex(0, _size, match) != -1;
	}

	public T Find(Predicate<T> match)
	{
		CheckMatch(match);
		int index = GetIndex(0, _size, match);
		if (index != -1)
			return _items[index];
		return default (T);
	}

	private static void CheckMatch(Predicate<T> match)
	{
		if (match == null)
			throw new ArgumentNullException("match");
	}

	public QuickList<T> FindAll(Predicate<T> match)
	{
		CheckMatch(match);
		if (_size <= 65536)
			return FindAllStackBits(match);
		return FindAllList(match);
	}

	private unsafe QuickList<T> FindAllStackBits(Predicate<T> match)
	{
		uint* numPtr1 = stackalloc uint[(int) checked (unchecked ((uint) (_size / 32 + 1)) * 4U)];
		uint* numPtr2 = numPtr1;
		int size = 0;
		uint num1 = unchecked((uint) int.MinValue);
		for (int index = 0; index < _size; ++index)
		{
			if (match(_items[index]))
			{
				*numPtr2 = *numPtr2 | num1;
				++size;
			}
			num1 >>= 1;
			if ((int) num1 == 0)
			{
				++numPtr2;
				num1 = unchecked((uint) int.MinValue);
			}
		}
		T[] data = new T[size];
		uint num2 = unchecked((uint) int.MinValue);
		uint* numPtr3 = numPtr1;
		int num3 = 0;
		for (int index = 0; index < _size && num3 < size; ++index)
		{
			if (((int) *numPtr3 & (int) num2) == (int) num2)
				data[num3++] = _items[index];
			num2 >>= 1;
			if ((int) num2 == 0)
			{
				++numPtr3;
				num2 = unchecked((uint) int.MinValue);
			}
		}
		return new QuickList<T>(data, size);
	}

	private QuickList<T> FindAllList(Predicate<T> match)
	{
		QuickList<T> list = new QuickList<T>();
		for (int index = 0; index < _size; ++index)
		{
			if (match(_items[index]))
				list.Add(_items[index]);
		}
		return list;
	}

	public int FindIndex(Predicate<T> match)
	{
		CheckMatch(match);
		return GetIndex(0, _size, match);
	}

	public int FindIndex(int startIndex, Predicate<T> match)
	{
		CheckMatch(match);
		CheckIndex(startIndex);
		return GetIndex(startIndex, _size - startIndex, match);
	}

	public int FindIndex(int startIndex, int count, Predicate<T> match)
	{
		CheckMatch(match);
		CheckRange(startIndex, count);
		return GetIndex(startIndex, count, match);
	}

	private int GetIndex(int startIndex, int count, Predicate<T> match)
	{
		int num = startIndex + count;
		for (int index = startIndex; index < num; ++index)
		{
			if (match(_items[index]))
				return index;
		}
		return -1;
	}

	public T FindLast(Predicate<T> match)
	{
		CheckMatch(match);
		int lastIndex = GetLastIndex(0, _size, match);
		if (lastIndex == -1)
			return default (T);
		return this[lastIndex];
	}

	public int FindLastIndex(Predicate<T> match)
	{
		CheckMatch(match);
		return GetLastIndex(0, _size, match);
	}

	public int FindLastIndex(int startIndex, Predicate<T> match)
	{
		CheckMatch(match);
		CheckIndex(startIndex);
		return GetLastIndex(0, startIndex + 1, match);
	}

	public int FindLastIndex(int startIndex, int count, Predicate<T> match)
	{
		CheckMatch(match);
		int num = startIndex - count + 1;
		CheckRange(num, count);
		return GetLastIndex(num, count, match);
	}

	private int GetLastIndex(int startIndex, int count, Predicate<T> match)
	{
		int num = startIndex + count;
		while (num != startIndex)
		{
			if (match(_items[--num]))
				return num;
		}
		return -1;
	}

	public void ForEach(Action<T> action)
	{
		if (action == null)
			throw new ArgumentNullException("action");
		for (int index = 0; index < _size; ++index)
			action(_items[index]);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public QuickList<T> GetRange(int index, int count)
	{
		CheckRange(index, count);
		T[] data = new T[count];
		Array.Copy((Array) _items, index, (Array) data, 0, count);
		return new QuickList<T>(data, count);
	}

	public int IndexOf(T item)
	{
		return Array.IndexOf<T>(_items, item, 0, _size);
	}

	public int IndexOf(T item, int index)
	{
		CheckIndex(index);
		return Array.IndexOf<T>(_items, item, index, _size - index);
	}

	public int IndexOf(T item, int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException("index");
		if (count < 0)
			throw new ArgumentOutOfRangeException("count");
		if ((uint) (index + count) > (uint) _size)
			throw new ArgumentOutOfRangeException("index and count exceed length of list");
		return Array.IndexOf<T>(_items, item, index, count);
	}

	private void Shift(int start, int delta)
	{
		if (delta < 0)
			start -= delta;
		if (start < _size)
			Array.Copy((Array) _items, start, (Array) _items, start + delta, _size - start);
		_size += delta;
		if (delta >= 0)
			return;
		Array.Clear((Array) _items, _size, -delta);
	}

	private void CheckIndex(int index)
	{
		if (index < 0 || (uint) index > (uint) _size)
			throw new ArgumentOutOfRangeException("index");
	}

	public void Insert(int index, T item)
	{
		CheckIndex(index);
		if (_size == _items.Length)
			GrowIfNeeded(1);
		Shift(index, 1);
		_items[index] = item;
		++_version;
	}

	private void CheckCollection(IEnumerable<T> collection)
	{
		if (collection == null)
			throw new ArgumentNullException("collection");
	}

	public void InsertRange(int index, IEnumerable<T> collection)
	{
		CheckCollection(collection);
		CheckIndex(index);
		if (collection == this)
		{
			T[] array = new T[_size];
			CopyTo(array, 0);
			GrowIfNeeded(_size);
			Shift(index, array.Length);
			Array.Copy((Array) array, 0, (Array) _items, index, array.Length);
		}
		else
		{
			ICollection<T> collection1 = collection as ICollection<T>;
			if (collection1 != null)
				InsertCollection(index, collection1);
			else
				InsertEnumeration(index, collection);
		}
		++_version;
	}

	private void InsertCollection(int index, ICollection<T> collection)
	{
		int count = collection.Count;
		GrowIfNeeded(count);
		Shift(index, count);
		collection.CopyTo(_items, index);
	}

	private void InsertEnumeration(int index, IEnumerable<T> enumerable)
	{
		foreach (T obj in enumerable)
			Insert(index++, obj);
	}

	public int LastIndexOf(T item)
	{
		return Array.LastIndexOf<T>(_items, item, _size - 1, _size);
	}

	public int LastIndexOf(T item, int index)
	{
		CheckIndex(index);
		return Array.LastIndexOf<T>(_items, item, index, index + 1);
	}

	public int LastIndexOf(T item, int index, int count)
	{
		if (index < 0)
			throw new ArgumentOutOfRangeException("index", (object) index, "index is negative");
		if (count < 0)
			throw new ArgumentOutOfRangeException("count", (object) count, "count is negative");
		if (index - count + 1 < 0)
			throw new ArgumentOutOfRangeException("cound", (object) count, "count is too large");
		return Array.LastIndexOf<T>(_items, item, index, count);
	}

	public bool Remove(T item)
	{
		int index = IndexOf(item);
		if (index != -1)
			RemoveAt(index);
		return index != -1;
	}

	public int RemoveAll(Predicate<T> match)
	{
		CheckMatch(match);
		int index1 = 0;
		while (index1 < _size && !match(_items[index1]))
			++index1;
		if (index1 == _size)
			return 0;
		++_version;
		int index2;
		for (index2 = index1 + 1; index2 < _size; ++index2)
		{
			if (!match(_items[index2]))
				_items[index1++] = _items[index2];
		}
		if (index2 - index1 > 0)
			Array.Clear((Array) _items, index1, index2 - index1);
		_size = index1;
		return index2 - index1;
	}

	public void RemoveAt(int index)
	{
		if (index < 0 || (uint) index >= (uint) _size)
			throw new ArgumentOutOfRangeException("index");
		Shift(index, -1);
		Array.Clear((Array) _items, _size, 1);
		++_version;
	}

	public void RemoveRange(int index, int count)
	{
		CheckRange(index, count);
		if (count <= 0)
			return;
		Shift(index, -count);
		Array.Clear((Array) _items, _size, count);
		++_version;
	}

	public void Reverse()
	{
		Array.Reverse((Array) _items, 0, _size);
		++_version;
	}

	public void Reverse(int index, int count)
	{
		CheckRange(index, count);
		Array.Reverse((Array) _items, index, count);
		++_version;
	}

	public void ArraySort()
	{
		Array.Sort<T>(_items, 0, _size, (IComparer<T>) Comparer<T>.Default);
		++_version;
	}

	public void ArraySort(IComparer<T> comparer)
	{
		Array.Sort<T>(_items, 0, _size, comparer);
		++_version;
	}

	public void ArraySort(Comparison<T> comparison)
	{
		ArraySort(_items, _size, comparison);
		++_version;
	}

	public void ArraySort(int index, int count, IComparer<T> comparer)
	{
		CheckRange(index, count);
		Array.Sort<T>(_items, index, count, comparer);
		++_version;
	}

	public T[] ToArray()
	{
		T[] objArray = new T[_size];
		Array.Copy((Array) _items, (Array) objArray, _size);
		return objArray;
	}

	public void TrimExcess()
	{
		Capacity = _size;
	}

	public bool TrueForAll(Predicate<T> match)
	{
		CheckMatch(match);
		for (int index = 0; index < _size; ++index)
		{
			if (!match(_items[index]))
				return false;
		}
		return true;
	}

	private static void ArraySort(T[] array, int length, Comparison<T> comparison)
	{
		if (comparison == null)
			throw new ArgumentNullException("comparison");
		if (length <= 1)
			return;
		if (array.Length <= 1)
			return;
		try
		{
			int low0 = 0;
			int high0 = length - 1;
			ArrayQsort(array, low0, high0, comparison);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("Comparison threw an exception.", ex);
		}
	}

	private static void ArrayQsort(T[] array, int low0, int high0, Comparison<T> comparison)
	{
		if (low0 >= high0)
			return;
		int index1 = low0;
		int index2 = high0;
		int index3 = index1 + (index2 - index1) / 2;
		T obj = array[index3];
		while (true)
		{
			while (index1 >= high0 || comparison(array[index1], obj) >= 0)
			{
				while (index2 > low0 && comparison(obj, array[index2]) < 0)
					--index2;
				if (index1 <= index2)
				{
					ArraySwap(array, index1, index2);
					++index1;
					--index2;
				}
				else
				{
					if (low0 < index2)
						ArrayQsort(array, low0, index2, comparison);
					if (index1 >= high0)
						return;
					ArrayQsort(array, index1, high0, comparison);
					return;
				}
			}
			++index1;
		}
	}

	private static void ArraySwap(T[] array, int i, int j)
	{
		T obj = array[i];
		array[i] = array[j];
		array[j] = obj;
	}

	[Serializable]
	public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
	{
		private QuickList<T> l;
		private int next;
		private int ver;
		private T current;

		object IEnumerator.Current
		{
			get
			{
				VerifyState();
				if (next <= 0)
					throw new InvalidOperationException();
				return (object) current;
			}
		}

		public T Current
		{
			get
			{
				return current;
			}
		}

		internal Enumerator(QuickList<T> l)
		{
			this.l = l;
			next = 0;
			ver = l._version;
			current = l._items[0];
		}

		void IEnumerator.Reset()
		{
			VerifyState();
			next = 0;
		}

		public void Dispose()
		{
			l = (QuickList<T>) null;
		}

		private void VerifyState()
		{
			if (l == null)
				throw new ObjectDisposedException(GetType().FullName);
			if (ver != l._version)
				throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
		}

		public bool MoveNext()
		{
			VerifyState();
			if (next < 0)
				return false;
			if (next < l._size)
			{
				current = l._items[next++];
				return true;
			}
			next = -1;
			return false;
		}
	}
}