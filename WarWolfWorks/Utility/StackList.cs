using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A stack which can use utility methods from other IEnumerables like Remove or Find.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class StackList<T> : IEquatable<List<T>>, IEnumerable<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// Creates an empty <see cref="StackList{T}"/>.
        /// </summary>
        public StackList()
        {
            pv_Items = new List<T>();
        }

        /// <summary>
        /// Creates a <see cref="StackList{T}"/> with the default capacity specified.
        /// </summary>
        /// <param name="capacity"></param>
        public StackList(int capacity)
        {
            pv_Items = new List<T>(capacity);
        }

        /// <summary>
        /// Creates a <see cref="StackList{T}"/> that contains elements from a given collection.
        /// </summary>
        /// <param name="collection"></param>
        public StackList(IEnumerable<T> collection)
        {
            pv_Items = new List<T>(collection);
        }

        /// <summary>
        /// Returns all elements inside the stack as an array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() => pv_Items.ToArray();
        /// <summary>
        /// Returns the number of elements contained inside the <see cref="StackList{T}"/>.
        /// </summary>
        public int Count => pv_Items.Count;
        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity
        {
            get => pv_Items.Capacity;
            set => pv_Items.Capacity = value;
        }
        /// <summary>
        /// Adds an element to the top of the <see cref="StackList{T}"/>.
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            pv_Items.Add(item);
        }
        /// <summary>
        /// Adds a collection of elements at the top of the <see cref="StackList{T}"/>.
        /// </summary>
        /// <param name="collection"></param>
        public void PushRange(IEnumerable<T> collection) => pv_Items.AddRange(collection);

        /// <summary>
        /// Removes and returns the element from the top of the <see cref="StackList{T}"/>.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            if (pv_Items.Count > 0)
            {
                int index = pv_Items.Count - 1;
                T temp = pv_Items[index];
                pv_Items.RemoveAt(index);
                return temp;
            }
            else
                return default;
        }

        /// <summary>
        /// Returns and removes a range of elements from the top of the <see cref="StackList{T}"/>.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public List<T> PopRange(int range)
        {
            if (range >= pv_Items.Count)
                throw new IndexOutOfRangeException("Cannot use WWWStack.PopRange(int) when the range given is higher than the length of the collection.");

            int count = pv_Items.Count - 1;
            int index = count - range;
            List<T> toReturn = pv_Items.GetRange(index, count);
            pv_Items.RemoveRange(index, count);

            return toReturn;
        }

        /// <summary>
        /// Returns the element at the top of the <see cref="StackList{T}"/>.
        /// </summary>
        /// <returns></returns>
        public T Peek() => pv_Items.Last();
        /// <summary>
        /// Returns the element at the bottom of the <see cref="StackList{T}"/>.
        /// </summary>
        /// <returns></returns>
        public T Lift() => pv_Items.First();

        /// <summary>
        /// Removes the first element that matches the specified predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public bool Remove(Predicate<T> match)
        {
            return pv_Items.Remove(pv_Items.Find(match));
        }

        /// <summary>
        /// Removes the given element from the <see cref="StackList{T}"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item) => pv_Items.Remove(item);

        /// <summary>
        /// Finds and returns the first item that matches the specified predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public T Find(Predicate<T> match) => pv_Items.Find(match);
        /// <summary>
        /// Finds and returns all elements inside the <see cref="StackList{T}"/> that match the given predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<T> FindAll(Predicate<T> match) => pv_Items.FindAll(match);
        /// <summary>
        /// Returns true if the given predicate matches all elements.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public bool TrueForAll(Predicate<T> match) => pv_Items.TrueForAll(match);
        /// <summary>
        /// Removes all elements from this <see cref="StackList{T}"/>.
        /// </summary>
        public void Clear() => pv_Items.Clear();
        /// <summary>
        /// Returns the zero-based index of the first occurance of the given element.
        /// </summary>
        /// <param name="item">Element to look for.</param>
        /// <returns>Zero-based index of the first occurance of the element in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int IndexOf(T item) => pv_Items.IndexOf(item);
        /// <summary>
        /// Returns the zero-based index of the first occurance of the given element within the range of the list that
        /// extends from the given index up until the last elements.
        /// </summary>
        /// <param name="item">Element to look for.</param>
        /// <param name="index">Start index of the search.</param>
        /// <returns>Zero-based index of the first occurance of the element in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int IndexOf(T item, int index) => pv_Items.IndexOf(item, index);
        /// <summary>
        /// Returns the zero-based index of the first occurance of the given element within the range of the list that
        /// extends from the given index for the specified amount.
        /// </summary>
        /// <param name="item">Element to look for.</param>
        /// <param name="index">Start index of the search.</param>
        /// <param name="count">Amount of elements to search.</param>
        /// <returns>Zero-based index of the first occurance of the element in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int IndexOf(T item, int index, int count) => pv_Items.IndexOf(item, index, count);
        /// <summary>
        /// Returns the zero-based index of the last occurance of the given element.
        /// </summary>
        /// <param name="item">Element to look for.</param>
        /// <returns>Zero-based index of the last occurance of the element in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int LastIndexOf(T item) => pv_Items.LastIndexOf(item);
        /// <summary>
        /// Returns the zero-based index of the last occurance of the given element within the range of the list that
        /// extends from the given index up until the last elements.
        /// </summary>
        /// <param name="item">Element to look for.</param>
        /// <param name="index">Start index of the search.</param>
        /// <returns>Zero-based index of the last occurance of the element in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int LastIndexOf(T item, int index) => pv_Items.LastIndexOf(item, index);
        /// <summary>
        /// Returns the zero-based index of the last occurance of the given element within the range of the list that
        /// extends from the given index for the specified amount.
        /// </summary>
        /// <param name="item">Element to look for.</param>
        /// <param name="index">Start index of the search.</param>
        /// <param name="count">Amount of elements to search.</param>
        /// <returns>Zero-based index of the last occurance of the element in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int LastIndexOf(T item, int index, int count) => pv_Items.LastIndexOf(item, index, count);
        /// <summary>
        /// Returns the index of the first element which matches the given predicate and returns it's zero-based index.
        /// </summary>
        /// <param name="match">Predicate to match.</param>
        /// <returns>Zero-based index of the first element found in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int FindIndex(Predicate<T> match) => pv_Items.FindIndex(match);
        /// <summary>
        /// Returns the index of the first element which matches the given predicate and returns it's zero-based index.
        /// </summary>
        /// <param name="index">Start index of the search.</param>
        /// <param name="match">Predicate to match.</param>
        /// <returns>Zero-based index of the first element found in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int FindIndex(int index, Predicate<T> match) => pv_Items.FindIndex(index, match);
        /// <summary>
        /// Returns the index of the first element which matches the given predicate and returns it's zero-based index.
        /// </summary>
        /// <param name="index">Start index of the search.</param>
        /// <param name="count">Amount of items to search.</param>
        /// <param name="match">Predicate to match.</param>
        /// <returns>Zero-based index of the first element found in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int FindIndex(int index, int count, Predicate<T> match) => pv_Items.FindIndex(index, count, match);
        /// <summary>
        /// Returns the index of the last element which matches the given predicate and returns it's zero-based index.
        /// </summary>
        /// <param name="match">Predicate to match.</param>
        /// <returns>Zero-based index of the last element found in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int FindLastIndex(Predicate<T> match) => pv_Items.FindLastIndex(match);
        /// <summary>
        /// Returns the index of the last element which matches the given predicate and returns it's zero-based index.
        /// </summary>
        /// <param name="index">Start index of the search.</param>
        /// <param name="match">Predicate to match.</param>
        /// <returns>Zero-based index of the last element found in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int FindLastIndex(int index, Predicate<T> match) => pv_Items.FindLastIndex(index, match);
        /// <summary>
        /// Returns the index of the last element which matches the given predicate and returns it's zero-based index.
        /// </summary>
        /// <param name="index">Start index of the search.</param>
        /// <param name="count">Amount of items to search.</param>
        /// <param name="match">Predicate to match.</param>
        /// <returns>Zero-based index of the last element found in the <see cref="StackList{T}"/>; Otherwise, -1.</returns>
        public int FindLastIndex(int index, int count, Predicate<T> match) => pv_Items.FindLastIndex(index, count, match);
        /// <summary>
        /// Returns a <see cref="ReadOnlyCollection{T}"/> from the elements inside the <see cref="StackList{T}"/>.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<T> AsReadonly() => pv_Items.AsReadOnly();
        /// <summary>
        /// Converts all elements to specified TOutput type,
        /// and returns a <see cref="StackList{T}"/> of the converted items.
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public StackList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
            => new StackList<TOutput>(pv_Items.ConvertAll<TOutput>(converter));
        /// <summary>
        /// Copies a range of elements from the <see cref="StackList{T}"/> to a compatible 
        /// one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source <see cref="StackList{T}"/> at which copying begins.</param>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied 
        /// from <see cref="StackList{T}"/>. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
            => pv_Items.CopyTo(index, array, arrayIndex, count);
        /// <summary>
        /// Copies a range of elements from the <see cref="StackList{T}"/> to a compatible 
        /// one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied 
        /// from <see cref="StackList{T}"/>. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
            => pv_Items.CopyTo(array, arrayIndex);
        /// <summary>
        /// Copies a range of elements from the <see cref="StackList{T}"/> to a compatible 
        /// one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied 
        /// from <see cref="StackList{T}"/>. The array must have zero-based indexing.
        /// </param>
        public void CopyTo(T[] array)
            => pv_Items.CopyTo(array);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="compared"></param>
        /// <returns></returns>
        public bool Equals(List<T> compared) => pv_Items == compared;

        /// <summary>
        /// Returns an enumerator that itterates through the <see cref="StackList{T}"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return pv_Items.GetEnumerator();
        }

        /// <summary>
        /// Returns a non-generic enumerator that itterates through the <see cref="StackList{T}"/>.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return pv_Items.GetEnumerator();
        }

        private List<T> pv_Items;
    }
}
