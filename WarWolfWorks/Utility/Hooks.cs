using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using WarWolfWorks.Debugging;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Internal;
using WarWolfWorks.Security;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A class which contains 20+Gadzillion-billion-yes methods for various utilities.
    /// </summary>
    public static class Hooks
    {

        #region Extension Methods
        /// <summary>
        /// Puts a string value into a rainbow text using Unity's Rich Text format.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="frequency"></param>
        /// <param name="colorsToUse"></param>
        /// <returns></returns>
        public static string ToRainbow(this string original, int frequency, Color[] colorsToUse)
            => Text.ToRainbow(original, frequency, colorsToUse);
        /// <summary>
        /// Extention method for <see cref="Text.Cutout(string, string, string)"/>.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string Cutout(this string original, string from, string to)
            => Text.Cutout(original, from, to);
        /// <summary>
        /// Extention method for <see cref="Text.Cutout(string, char, char)"/>.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string Cutout(this string original, char from, char to)
           => Text.Cutout(original, from, to);
        /// <summary>
        /// Returns true if numberToApproximate is not lower than or higer than numberToCompare using approximation as range.
        /// </summary>
        /// <param name="NumberToApproximate"></param>
        /// <param name="NumberToCompare"></param>
        /// <param name="approximation"></param>
        /// <returns></returns>
        public static bool IsApproximate(this float NumberToApproximate, float NumberToCompare, float approximation)
            => MathF.IsApproximate(NumberToApproximate, NumberToCompare, approximation);
        /// <summary>
        /// Cuts a float to digits length.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static float Truncate(this float value, int digits) => MathF.Truncate(value, digits);
        /// <summary>
        /// If value given is negative, it will be turned into it's positive value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToPositive(this float value) => MathF.ToPositive(value);
        /// <summary>
        /// If value given is positive, it will be turned into it's negative value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToNegative(this float value) => MathF.ToNegative(value);
        /// <summary>
        /// If value given is negative, it will be turned into it's positive value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToPositive(this int value) => MathF.ToPositive(value);
        /// <summary>
        /// If value given is positive, it will be turned into it's negative value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToNegative(this int value) => MathF.ToNegative(value);
        /// <summary>
        /// If value given is negative, it will be turned into it's positive value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToPositive(this long value) => MathF.ToPositive(value);
        /// <summary>
        /// If value given is positive, it will be turned into it's negative value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToNegative(this long value) => MathF.ToNegative(value);
        /// <summary>
        /// Puts all values of a given color into negatives. (If a value was negative, it will be put into positive)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color ToNegative(this Color value) => Colors.ToNegative(value);
        /// <summary>
        /// Extention method. Points to <see cref="MathF.SetAnchoredUI(RectTransform, Vector4)"/>.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public static void SetAnchoredUI(this RectTransform rt, Vector4 Position)
            => MathF.SetAnchoredUI(rt, Position);
        /// <summary>
        /// Extention method. Points to <see cref="MathF.SetAnchoredUI(RectTransform, Vector2, Vector2)"/>.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void SetAnchoredUI(this RectTransform rt, Vector2 min, Vector2 max)
            => MathF.SetAnchoredUI(rt, min, max);
        /// <summary>
        /// Extention method. Points to <see cref="MathF.SetAnchoredUI(RectTransform, float, float, float, float)"/>.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        public static void SetAnchoredUI(this RectTransform rt, float minX, float minY, float maxX, float maxY)
            => MathF.SetAnchoredUI(rt, minX, minY, maxX, maxY);
        /// <summary>
        /// Returns the anchored position of a <see cref="RectTransform"/> in <see cref="Vector4"/>: X = minX, Y = minY, z = maxX, W = maxY.
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public static Vector4 GetAnchoredPosition(this RectTransform rt)
            => MathF.GetAnchoredPosition(rt);
        /// <summary>
        /// Returns true if the given position is within the bounds given. (<see cref="Vector4"/> bounds: X = minX, Y = minY, z = maxX, W = maxY)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static bool IsInsideBounds(this Vector2 position, Vector4 bounds)
            => MathF.IsInsideBounds(position, bounds);
        /// <summary>
        /// Returns true if the given position is within the bounds given.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsInsideBounds(this Vector2 position, Vector2 min, Vector2 max)
            => MathF.IsInsideBounds(position, min, max);
        /// <summary>
        /// <see cref="Enumeration.Find{T}(T[], Predicate{T})"/> Pointer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toUse"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static T Find<T>(this T[] toUse, Predicate<T> match)
            => Array.Find(toUse, match);
        /// <summary>
        /// <see cref="Enumeration.FindIndex{T}(T[], Predicate{T})"/> Pointer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toUse"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this T[] toUse, Predicate<T> match)
            => Array.FindIndex(toUse, match);
        /// <summary>
        /// <see cref="Enumeration.ForEach{T}(T[], Action{T})"/> Pointer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this T[] array, Action<T> action)
            => Array.ForEach(array, action);
        /// <summary>
        /// Returns a list with all null elements removed (if any)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveNull<T>(this IEnumerable<T> list)
        {
            return list.Where(t => t is not null);
        }

        /// <summary>
        /// Itterates through each element <typeparamref name="T"/>, calls it's <typeparamref name="T"/>.ToString()
        /// and returns all of them in a string array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string[] EnumerableToString<T>(this IEnumerable<T> collection)
        {
            List<string> list = new List<string>();
            foreach (T item in collection)
            {
                list.Add(item.ToString());
            }
            return list.ToArray();
        }

        /// <summary>
        /// Returns the index of the first element T equal to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetNullIndex<T>(this List<T> list)
         => list.FindIndex(t => t is null);
        /// <summary>
        /// Returns the index of the first element T equal to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int GetNullIndex<T>(this T[] array)
        => array.FindIndex(t => t is null);
        /// <summary>
        /// Returns the index of the first element T equal to null starting from specified index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetNullIndex<T>(this T[] array, int index)
            => Array.FindIndex(array, index, t => t is null);
        /// <summary>
        /// Returns the index of the first element T equal to null starting from specified index up to count times upwards in the enumerator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int GetNullIndex<T>(this T[] array, int index, int count)
            => Array.FindIndex(array, index, count, t => t is null);
        /// <summary>
        /// Returns index of the first element T equal to null starting from IntRange.Min to IntRange.Max.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int GetNullIndex<T>(this T[] array, IntRange range)
            => Array.FindIndex(array, range.Min, range.Max - range.Min, t => t is null);

        /// <summary>
        /// Returns the index of the first element T equal to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static int GetNullIndex<T>(this IEnumerable<T> collection)
        {
            int index = 0;
            foreach (T t in collection)
            {
                if (t is null)
                    return index;

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Returns a new <see cref="ICollection{T}"/> of given size if collection passed was null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<T> SetCollectionSizeIfNull<T>(this IEnumerable<T> collection, int size)
        {
            if (collection == null || collection.Count() == 0)
            {
                return new T[size];
            }
            return collection;
        }
        /// <summary>
        /// Takes <paramref name="size"/> elements of the given collection and returns them as a <see cref="Queue{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Collection to itterate through.</param>
        /// <param name="size">Amount of items from <paramref name="collection"/> to itterate through.</param>
        /// <param name="fromEnd">If true, it will start from collection's Count-1 and go in descending order to get items, instead of 0 in ascending order.</param>
        /// <returns></returns>
        public static Queue<T> ToQueueSized<T>(this IEnumerable<T> collection, int size, bool fromEnd)
        {
            int count = collection.Count();
            if (count <= 0)
            {
                return null;
            }
            int num = (count - 1 < size) ? (count - 1) : size;
            int index = fromEnd ? (count - num) : 0;
            List<T> range = collection.ToList().GetRange(index, num);
            return new Queue<T>(range);
        }
        /// <summary>
        /// Takes <paramref name="size"/> elements of the given collection and returns them as a <see cref="Stack{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Collection to itterate through.</param>
        /// <param name="size">Amount of items from <paramref name="collection"/> to itterate through.</param>
        /// <param name="fromEnd">If true, it will start from collection's Count-1 and go in descending order to get items, instead of 0 in ascending order.</param>
        /// <returns></returns>
        public static Stack<T> ToStackSized<T>(this IEnumerable<T> collection, int size, bool fromEnd)
        {
            int count = collection.Count();
            if (count <= 0)
            {
                return null;
            }
            int num = (count - 1 < size) ? (count - 1) : size;
            int index = fromEnd ? (count - num) : 0;
            List<T> range = collection.ToList().GetRange(index, num);
            return new Stack<T>(range);
        }
        /// <summary>
        /// Gets all <typeparamref name="T"/> items inside a <see cref="ValueTuple{T1, T2}"/> based on item index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item">If 0, it returns Item1, otherwise returns Item2.</param>
        /// <returns></returns>
        public static T[] GetItemsFromTupleIndex<T>(this IEnumerable<(T, T)> collection, int item)
        {
            List<T> list = new List<T>();
            foreach (var item2 in collection)
            {
                list.Add((item == 0) ? item2.Item1 : item2.Item2);
            }
            return list.ToArray();
        }
        /// <summary>
        /// Gets all <typeparamref name="T"/> items inside a <see cref="ValueTuple{T1, T2, T3}"/> based on item index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item">If 0, it returns Item1; if 1, returns Item2; otherwise returns Item3.</param>
        /// <returns></returns>
        public static T[] GetItemsFromTupleIndex<T>(this IEnumerable<(T, T, T)> collection, int item)
        {
            List<T> list = new List<T>();
            foreach (var item3 in collection)
            {
                List<T> list2 = list;
                object item2 = item switch
                {
                    1 => item3.Item2,
                    0 => item3.Item1,
                    _ => item3.Item3,
                };
                list2.Add((T)item2);
            }
            return list.ToArray();
        }
        /// <summary>
        /// Removes an array of strings from original.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="removers"></param>
        /// <completionlist cref="string"/>
        /// <returns></returns>
        public static string RemoveArrayFromString(this string original, IEnumerable<string> removers)
        {
            if (removers == null)
            {
                goto Returner;
            }
            foreach (string remover in removers)
            {
                original = original.Replace(remover, string.Empty);
            }
            Returner:
            return original;
        }
        /// <summary>
        /// Creates a rotation from original position to destination.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="destination"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        public static Quaternion RotateTo(this Vector3 original, Vector3 destination, bool reversed)
        {
            if (reversed)
            {
                return Quaternion.LookRotation(original - destination);
            }
            return Quaternion.LookRotation(destination - original);
        }

        /// <summary>
        ///  Creates a rotation from center position to destination using Atan2.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="destination"></param>
        /// <param name="adder">Z rotation to add onto the result.</param>
        /// <returns></returns>
        public static Quaternion RotateTowards2D(this Vector2 center, Vector2 destination, float adder)
        {
            Vector3 used = center - destination;
            float num = Mathf.Atan2(used.y, used.x) * Mathf.Rad2Deg;
            Vector3 euler = new Vector3(0f, 0f, num + adder);
            return Quaternion.Euler(euler);
        }

        /// <summary>
        /// Equivalent to <see cref="Quaternion.LookRotation(Vector3)"/> with both vectors being normalized before calculation.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static Quaternion RotateToNormalized(this Vector3 original, Vector3 destination)
        {
            Vector3 normalized = (destination - original).normalized;
            return Quaternion.LookRotation(normalized);
        }
        /// <summary>
        /// Equivalent to ASCII <see cref="Encoding.GetBytes(char[])"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        /// <summary>
        /// Equivalent to ASCII <see cref="Encoding.GetString(byte[])"/>.
        /// </summary>
        /// <param name="byt"></param>
        /// <returns></returns>
        public static string ToStringFromBytes(this byte[] byt)
        {
            return Encoding.ASCII.GetString(byt);
        }

        /// <summary>
        /// Returns an attribute value based on type and func given.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="type"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector)
        where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default;
        }

        /// <summary>
        /// Starts a singleton-type unity coroutine.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="routine"></param>
        /// <param name="isRunningBool"></param>
        public static void StartCoroutine(this MonoBehaviour caller, IEnumerator routine, ref bool isRunningBool)
        {
            if (isRunningBool)
            {
                AdvancedDebug.Log("Couldn't start " + routine.ToString() + " enumerator as it is already running!", 6);
                return;
            }
            isRunningBool = true;
            caller.StartCoroutine(routine);
        }
        /// <summary>
        /// Stops a singleton-type unity coroutine.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="routine"></param>
        /// <param name="isRunningBool"></param>
        public static void StopCoroutine(this MonoBehaviour caller, IEnumerator routine, ref bool isRunningBool)
        {
            if (isRunningBool)
            {
                isRunningBool = false;
                caller.StopCoroutine(routine);
            }
        }
        #endregion


        /// <summary>
        /// Subclass with all utility methods for use of a random factor.
        /// </summary>
        public static class Random
        {
            /// <summary>
            /// Returns anything between -Vector3.one and Vector3.one.
            /// </summary>
            public static Vector3 RandomVector01WithNeg => new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

            /// <summary>
            /// Returns anything between Vector3.zero and Vector3.one.
            /// </summary>
            public static Vector3 RandomVector01 => new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

            /// <summary>
            /// Returns a random vector2 with a magniture of 1.
            /// </summary>
            /// <param name="includeNegative">If true, the vector returned can also go into negative values.</param>
            /// <returns></returns>
            public static Vector2 GetRandomVector2(bool includeNegative)
            {
                if (includeNegative)
                    return new Vector2(UnityEngine.Random.Range(-1f, 1f), 
                        UnityEngine.Random.Range(-1f, 1f));

                return new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
            }

            /// <summary>
            /// Returns a random vector3 with a magniture of 1.
            /// </summary>
            /// <param name="includeNegative">If true, the vector returned can also go into negative values.</param>
            /// <returns></returns>
            public static Vector3 GetRandomVector3(bool includeNegative)
            {
                if (includeNegative)
                    return new Vector3(UnityEngine.Random.Range(-1f, 1f), 
                        UnityEngine.Random.Range(-1f, 1f), 
                        UnityEngine.Random.Range(-1f, 1f));

                return new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            }

            /// <summary>
            /// Returns a random vector4 with a magniture of 1.
            /// </summary>
            /// <param name="includeNegative">If true, the vector returned can also go into negative values.</param>
            /// <returns></returns>
            public static Vector3 GetRandomVector4(bool includeNegative)
            {
                if (includeNegative)
                    return new Vector4(UnityEngine.Random.Range(-1f, 1f), 
                        UnityEngine.Random.Range(-1f, 1f), 
                        UnityEngine.Random.Range(-1f, 1f), 
                        UnityEngine.Random.Range(-1f, 1f));

                return new Vector4(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            }

            /// <summary>
            /// Shuffles all items inside a list, changing their index.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <returns></returns>
            public static List<T> Shuffle<T>(List<T> list)
            {
                int num = list.Count;
                while (num > 1)
                {
                    num--;
                    int index = UnityEngine.Random.Range(0, num + 1);
                    T value = list[index];
                    list[index] = list[num];
                    list[num] = value;
                }
                return list;
            }

            /// <summary>
            /// Returns a random item from an IEnumerable value.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="List"></param>
            /// <returns></returns>
            public static T RandomItem<T>(IEnumerable<T> List)
            {
                T[] array = List.ToArray();
                int num = UnityEngine.Random.Range(0, array.Length);
                return array[num];
            }

            /// <summary>
            /// Returns a <see cref="Vector2"/> with each of it's values being a random number between min and max.
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static Vector2 Range(Vector2 min, Vector2 max)
            {
                return new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
            }

            /// <summary>
            /// Returns a <see cref="Vector3"/> with each of it's values being a random number between min and max.
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static Vector3 Range(Vector3 min, Vector3 max)
            {
                return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
            }

            /// <summary>
            /// Returns a <see cref="Vector3"/> with each of it's values being a random number between min and max.
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static Vector4 Range(Vector4 min, Vector4 max)
            {
                return new Vector4(UnityEngine.Random.Range(min.x, max.x), 
                    UnityEngine.Random.Range(min.y, max.y), 
                    UnityEngine.Random.Range(min.z, max.z),
                    UnityEngine.Random.Range(min.w, max.w));
            }

            /// <summary>
            /// Returns a random string with characters between a-z, A-Z and 0-9.
            /// </summary>
            /// <param name="length"></param>
            /// <param name="characters">Characters used to return the random string.</param>
            /// <returns></returns>
            public static string GetRandomString(int length, 
                string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
            {
                return new string(Enumerable.Repeat(characters, length)
                  .Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
            }
        }

        /// <summary>
        /// Subclass with all streaming and saving/loading methods.
        /// </summary>
        public static class Streaming
        {
            /// <summary>
            /// Attempts to create a folder. Returns true if the folder was successfully created.
            /// </summary>
            /// <param name="folderPath"></param>
            /// <returns></returns>
            public static bool CreateFolder(string folderPath)
            {
                if (Enumerable.Contains(folderPath, '.'))
                {
                    folderPath = Path.GetDirectoryName(folderPath);
                }
                bool flag = Directory.Exists(folderPath);
                if (!flag)
                {
                    Directory.CreateDirectory(folderPath);
                }
                return !flag;
            }

            /// <summary>
            /// Gets all files inside a given folder.
            /// </summary>
            /// <param name="folderPath"></param>
            /// <param name="extention"></param>
            /// <param name="includeFolderPath"></param>
            /// <returns></returns>
            public static string[] GetAllFilesInFolder(string folderPath, string extention, bool includeFolderPath)
            {
                string actFolderPath = Path.GetDirectoryName(folderPath);
                if (!Directory.Exists(actFolderPath))
                {
                    return null;
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(actFolderPath);
                FileInfo[] files = directoryInfo.GetFiles(extention);
                List<string> list = new List<string>();
                FileInfo[] array = files;
                foreach (FileInfo fileInfo in array)
                {
                    list.Add(includeFolderPath ? (actFolderPath + fileInfo.Name) : fileInfo.Name);
                }
                return list.ToArray();
            }

            private static bool InternalEncryptionFile(string path, string password, bool encrypt)
            {
                try
                {
                    string VanillaText = File.ReadAllText(path);
                    string Encryption = encrypt ? RijndaelEncryption.Encrypt(VanillaText, password) : RijndaelEncryption.Decrypt(VanillaText, password);
                    if (VanillaText.Equals(Encryption))
                        return false;

                    StringBuilder writer = new StringBuilder(Encryption);
                    if (encrypt)
                    {
                        for (int i = STREAMING_FILE_ENCRYPTION_JUMPER; i < writer.Length; i += STREAMING_FILE_ENCRYPTION_JUMPER)
                        {
                            writer.Insert(i, '\n');
                        }
                    }
                    else
                        writer.Replace("\n", string.Empty);

                    File.WriteAllText(path, writer.ToString());

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// Encrypts all contents of a file using <see cref="RijndaelEncryption"/>.
            /// </summary>
            /// <param name="path"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static bool EncryptFile(string path, string password)
                => InternalEncryptionFile(path, password, true);

            /// <summary>
            /// Decrypts all contents of a file using <see cref="RijndaelEncryption"/>, assuming it was previously encrypted using <see cref="EncryptFile(string, string)"/>.
            /// </summary>
            /// <param name="path"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static bool DecryptFile(string path, string password)
            => InternalEncryptionFile(path, password, false);

            /// <summary>
            /// Gets all lines from a StreamReader and returns them as a string array. (THIS METHOD DOES NOT FLUSH OR DISPOSE THE STREAMREADER)
            /// </summary>
            /// <param name="sr"></param>
            /// <returns></returns>
            public static string[] GetAllStreamedLines(StreamReader sr)
            {
                List<string> list = new List<string>();
                string item;
                while ((item = sr.ReadLine()) != null)
                {
                    list.Add(item);
                }
                return list.ToArray();
            }

            /// <summary>
            /// Overrides a line at the given index of a file.
            /// </summary>
            /// <param name="newText"></param>
            /// <param name="filePath"></param>
            /// <param name="index"></param>
            public static void LineChanger(string filePath, int index, string newText)
            {
                string[] array = File.ReadAllLines(filePath);
                array[index - 1] = newText;
                File.WriteAllLines(filePath, array);
            }

            /// <summary>
            /// Returns the StreamingAssets folder path in windows form.
            /// </summary>
            public static string StreamingAssetsPath => Application.streamingAssetsPath.Replace("/", @"\");
            /// <summary>
            /// Returns the Assets folder path in windows form.
            /// </summary>
            public static string AssetsPath => Application.dataPath.Replace("/", @"\");
            /// <summary>
            /// Returns a merged path between <see cref="StreamingAssetsPath"/> and fileName given.
            /// </summary>
            /// <param name="fileName"></param>
            /// <returns></returns>
            public static string GetStreamingAssetsFilePath(string fileName) => Path.Combine(StreamingAssetsPath, fileName);
            /// <summary>
            /// Returns a merged path between <see cref="AssetsPath"/> and fileName given.
            /// </summary>
            /// <param name="fileName"></param>
            /// <returns></returns>
            public static string GetAssetsFilePath(string fileName) => Path.Combine(AssetsPath, fileName);
        }
        
        internal static class EncryptionUtilityInternal
        {
            internal const int Keysize = 256;
            internal const int DerivationIterations = 1000;
            internal static byte[] Generate256BitsOfRandomEntropy()
            {
                byte[] randomBytes = new byte[32]; // 32 Bytes => 256 bits.
                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetBytes(randomBytes);
                }
                return randomBytes;
            }

            internal static string EncryptingKey(string value)
            {
                StringBuilder Sb = new StringBuilder();

                using (SHA256 hash = SHA256Managed.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                    foreach (Byte b in result)
                        Sb.Append(b.ToString("x2"));
                }

                return Sb.ToString();
            }
        }

        /// <summary>
        /// Encryption class which uses the Rijndael encryption algorithm.
        /// </summary>
        public static class RijndaelEncryption
        {
            private static string EncryptInternal(string input, string password, CipherMode mode, PaddingMode padding)
            {
                try
                {
                    password = EncryptionUtilityInternal.EncryptingKey(password);

                    byte[] saltStringBytes = EncryptionUtilityInternal.Generate256BitsOfRandomEntropy();
                    byte[] ivStringBytes = EncryptionUtilityInternal.Generate256BitsOfRandomEntropy();
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(input);
                    using (Rfc2898DeriveBytes passwordIntern = new Rfc2898DeriveBytes(password, saltStringBytes, EncryptionUtilityInternal.DerivationIterations))
                    {
                        byte[] keyBytes = passwordIntern.GetBytes(EncryptionUtilityInternal.Keysize / 8);
                        using (RijndaelManaged symmetricKey = new RijndaelManaged())
                        {
                            symmetricKey.BlockSize = EncryptionUtilityInternal.Keysize;
                            symmetricKey.Mode = mode;
                            symmetricKey.Padding = padding;
                            using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                            {
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                    {
                                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                        cryptoStream.FlushFinalBlock();
                                        byte[] cipherTextBytes = saltStringBytes;
                                        cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                        cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                        memoryStream.Close();
                                        cryptoStream.Close();
                                        return Convert.ToBase64String(cipherTextBytes);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(CryptographicException)
                {
                    return input;
                }
            }

            /// <summary>
            /// Returns an encrypted version of this string using the given password; CipherMode is set to CBC and PaddingMode to PKCS7.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static string Encrypt(string input, string password)
                => EncryptInternal(input, password, CipherMode.CBC, PaddingMode.PKCS7);

            /// <summary>
            /// Returns an encrypted version of this string using the given password; PaddingMode is set to PKCS7.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <param name="mode"></param>
            /// <returns></returns>
            public static string Encrypt(string input, string password, CipherMode mode)
                => EncryptInternal(input, password, mode, PaddingMode.PKCS7);

            /// <summary>
            /// Returns an encrypted version of this string using the given password.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <param name="mode"></param>
            /// <param name="padding"></param>
            /// <returns></returns>
            public static string Encrypt(string input, string password, CipherMode mode, PaddingMode padding)
               => EncryptInternal(input, password, mode, padding);

            private static string DecryptInternal(string input, string password, CipherMode mode, PaddingMode padding)
            {
                try
                {
                    password = EncryptionUtilityInternal.EncryptingKey(password);

                    byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(input);
                    byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(EncryptionUtilityInternal.Keysize / 8).ToArray();
                    byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(EncryptionUtilityInternal.Keysize / 8).Take(EncryptionUtilityInternal.Keysize / 8).ToArray();
                    byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((EncryptionUtilityInternal.Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((EncryptionUtilityInternal.Keysize / 8) * 2)).ToArray();

                    using (Rfc2898DeriveBytes passwordInternal = new Rfc2898DeriveBytes(password, saltStringBytes, EncryptionUtilityInternal.DerivationIterations))
                    {
                        byte[] keyBytes = passwordInternal.GetBytes(EncryptionUtilityInternal.Keysize / 8);
                        using (RijndaelManaged symmetricKey = new RijndaelManaged())
                        {
                            symmetricKey.BlockSize = EncryptionUtilityInternal.Keysize;
                            symmetricKey.Mode = mode;
                            symmetricKey.Padding = padding;
                            using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                            {
                                using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                                {
                                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                    {
                                        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                        memoryStream.Close();
                                        cryptoStream.Close();
                                        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(CryptographicException)
                {
                    return input;
                }
            }

            /// <summary>
            /// Decrypts a string previously encrypted using <see cref="RijndaelEncryption"/>; CipherMode is set to CBC and PaddingMode to PKCS7.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static string Decrypt(string input, string password)
                => DecryptInternal(input, password, CipherMode.CBC, PaddingMode.PKCS7);

            /// <summary>
            /// Decrypts a string previously encrypted using <see cref="RijndaelEncryption"/>; PaddingMode is set to PKCS7.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <param name="mode"></param>
            /// <returns></returns>
            public static string Decrypt(string input, string password, CipherMode mode)
                => DecryptInternal(input, password, mode, PaddingMode.PKCS7);

            /// <summary>
            /// Decrypts a string previously encrypted using <see cref="RijndaelEncryption"/>.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="password"></param>
            /// <param name="mode"></param>
            /// <param name="padding"></param>
            /// <returns></returns>
            public static string Decrypt(string input, string password, CipherMode mode, PaddingMode padding)
                => DecryptInternal(input, password, mode, padding);
        }

        /// <summary>
        /// Cipher algorithms.
        /// </summary>
        public static class Ciphers
        {
            /// <summary>
            /// Ceasar cipher which delays each character in a string by shift.
            /// </summary>
            /// <param name="source"></param>
            /// <param name="shift"></param>
            /// <returns></returns>
            public static string Caesar(string source, int shift)
            {
                var maxChar = Convert.ToInt32(char.MaxValue);
                var minChar = Convert.ToInt32(char.MinValue);

                var buffer = source.ToCharArray();

                for (var i = 0; i < buffer.Length; i++)
                {
                    var shifted = Convert.ToInt32(buffer[i]) + shift;

                    if (shifted > maxChar)
                    {
                        shifted -= maxChar;
                    }
                    else if (shifted < minChar)
                    {
                        shifted += maxChar;
                    }

                    buffer[i] = Convert.ToChar(shifted);
                }

                return new string(buffer);
            }
        }

        /// <summary>
        /// Contains all methods concerning the mouse/cursor.
        /// </summary>
        public static class Cursor
        {
            /// <summary>
            /// Returns the anchored position of the mouse in Vector2.
            /// </summary>
            public static Vector2 MousePosInPercent => Input.mousePosition / new Vector2(Screen.width, Screen.height);

            /// <summary>
            /// Returns a rotation from position based on the camera, which would rotate towards the mouse.
            /// </summary>
            /// <param name="camera"></param>
            /// <param name="position"></param>
            /// <param name="adder"></param>
            /// <returns></returns>
            public static Quaternion RotateTowardsMouse(Camera camera, Vector2 position, float adder = 0f)
            {
                position = camera.WorldToViewportPoint(position);
                Vector2 vector = camera.ScreenToViewportPoint(Input.mousePosition);
                float z = Mathf.Atan2(vector.y - position.y, vector.x - position.x) * Mathf.Rad2Deg;
                Vector3 euler = new Vector3(0f, 0f, camera.transform.eulerAngles.z + z + adder);
                return Quaternion.Euler(euler);
            }


            /// <summary>
            /// Gets the world position of the mouse.
            /// </summary>
            /// <param name="camera"></param>
            /// <returns></returns>
            public static Vector3 GetMouseWorldPosition(Camera camera)
            {
                return camera.ScreenToWorldPoint(Input.mousePosition);
            }

            /// <summary>
            /// Returns the Mouse's World position, 
            /// pixel position on screen if convertToViewPort is false, 
            /// otherwise it returns by using camera's ScreenToViewportPoint.
            /// </summary>
            /// <param name="camera"></param>
            /// <param name="convertToViewPort"></param>
            /// <returns></returns>
            public static Vector2 GetMouseUIPosition(Camera camera, bool convertToViewPort)
            {
                Vector2 result = Input.mousePosition;
                if (convertToViewPort)
                {
                    result = camera.ScreenToViewportPoint(Input.mousePosition);
                }
                return result;
            }
        }

        /// <summary>
        /// Contains all Color utilities.
        /// </summary>
        public static class Colors
        {
            #region Basic colors
            /// <summary>The White color.</summary>
            public static readonly Color White = new Color(1f, 1f, 1f);
            /// <summary>The Black color.</summary>
            public static readonly Color Black = new Color(0f, 0f, 0f);
            /// <summary>The Red color.</summary>
            public static readonly Color Red = new Color(1f, 0f, 0f);
            /// <summary>The Orange color.</summary>
            public static readonly Color Orange = new Color(1f, .647f, 0f);
            /// <summary>The Yellow color.</summary>
            public static readonly Color Yellow = new Color(1f, 1f, 0f);
            /// <summary>The Green color.</summary>
            public static readonly Color Green = new Color(0f, 1f, 0f);
            /// <summary>The Blue color.</summary>
            public static readonly Color Blue = new Color(0f, 0f, 1f);
            /// <summary>The Cyan color.</summary>
            public static readonly Color Cyan = new Color(0f, 1f, 1f);
            /// <summary>The Pink color.</summary>
            public static readonly Color Pink = new Color(1f, .753f, .796f);
            /// <summary>The Purple color.</summary>
            public static readonly Color Purple = new Color(.5f, 0f, .5f);
            /// <summary>The Magenta/Fuchsia color.</summary>
            public static readonly Color Magenta = new Color(1f, 0f, 1f);
            #endregion

            #region Specific colors
            /// <summary>
            /// The best color. (Orange color with a red-ish hue)
            /// </summary>
            public static readonly Color Tangelo = new Color(0.976f, 0.302f, 0f);
            /// <summary>
            /// The Crimson color. (light red)
            /// </summary>
            public static readonly Color Crimson = new Color(.863f, .078f, .235f);
            /// <summary>
            /// The Crimson Red color. (dark red, not to be confused with <see cref="Crimson"/>)
            /// </summary>
            public static readonly Color CrimsonRed = new Color(.6f, 0f, 0f);
            /// <summary>
            /// The Midnight Blue color. (dark blue)
            /// </summary>
            public static readonly Color MidnightBlue = new Color(.098f, .098f, .439f);
            /// <summary>
            /// The Dodger Blue color. (vivid blue)
            /// </summary>
            public static readonly Color DodgerBlue = new Color(.012f, .056f, 1f);

            /// <summary>
            /// The Wisteria color. (dark purple)
            /// </summary>
            public static readonly Color Wisteria = new Color(.79f, .63f, .86f);
            /// <summary>
            /// The Cotton Candy color. (dark, high contrast pink)
            /// </summary>
            public static readonly Color CottonCandy = new Color(1f, .74f, .85f);
            /// <summary>
            /// The Daffodil color. (slightly darker yellow)
            /// </summary>
            public static readonly Color Daffodil = new Color(1f, 1f, .19f);
            /// <summary>
            /// The Azure color. (very light blue)
            /// </summary>
            public static readonly Color Azure = new Color(.94f, 1f, 1f);
            /// <summary>
            /// The Ao/Office Green color. (dark green)
            /// </summary>
            public static readonly Color Ao = new Color(0f, .5f, 0f);
            /// <summary>
            /// The Electric Ultramarine color. (high contrast purple)
            /// </summary>
            public static readonly Color ElectricUltramarine = new Color(.25f, 0f, 1f);
            /// <summary>
            /// The Ferrari Red color. (slightly orange red)
            /// </summary>
            public static readonly Color FerrariRed = new Color(1f, .16f, 0f);
            /// <summary>
            /// The Gold color. (high contrast yellow)
            /// </summary>
            public static readonly Color Gold = new Color(1f, .84f, 0f);
            /// <summary>
            /// The Lapis Lazuli color. (pale blue)
            /// </summary>
            public static readonly Color LapisLazuli = new Color(.15f, .38f, .61f);
            /// <summary>
            /// The Lawn Green color. (VERY high contract green)
            /// </summary>
            public static readonly Color LawnGreen = new Color(.49f, .99f, 0f);
            /// <summary>
            /// The Oxford Blue color. (very dark blue)
            /// </summary>
            public static readonly Color OxfordBlue = new Color(0f, .13f, .28f);
            /// <summary>
            /// The Psychedelic Purple/Phlox color. (slightly lighter <see cref="Color.magenta"/>)
            /// </summary>
            public static readonly Color PsychedelicPurple = new Color(.87f, 0f, 1f);
            /// <summary>
            /// The Royal Blue color. (slightly pale blue)
            /// </summary>
            public static readonly Color RoyalBlue = new Color(.25f, .41f, .88f);
            /// <summary>
            /// The Timberwolf color. (gray with a brown tint)
            /// </summary>
            public static readonly Color Timberwolf = new Color(.86f, .84f, .82f);
            /// <summary>
            /// The Zinnwaldite Brown color. (very dark brown)
            /// </summary>
            public static readonly Color ZinnwalditeBrown = new Color(.17f, .09f, .04f);
            #endregion

            /// <summary>
            /// Returns the Vector4.MoveTowards equivalent for colors.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="destination"></param>
            /// <param name="speed"></param>
            /// <returns></returns>
            public static Color MoveTowards(Color point, Color destination, float speed)
            {
                return MathF.MoveTowards(point, destination, speed);
            }

            /// <summary>
            /// Returns the original color with it's values being put into negatives. (If a value is negative, it will be put back to positive)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static Color ToNegative(Color original)
                => new Color(-original.r, -original.g, -original.b, -original.a);

            /// <summary>
            /// Returns the original color with it's values being put into negatives. (If a value is negative, it will be kept as is)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static Color ToAbsoluteNegative(Color original)
                => new Color(MathF.ToNegative(original.r), MathF.ToNegative(original.g), MathF.ToNegative(original.b), MathF.ToNegative(original.a));

            /// <summary>
            /// Short for <see cref="Color.white"/> - original.
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static Color Reverse(Color original) => Color.white - original;

            /// <summary>
            /// Returns in order: Color.red, Color.yellow, Color.blue.
            /// </summary>
            public static readonly Color[] PrimaryColors = new Color[]
            {
                Red,
                Yellow,
                Blue
            };

            /// <summary>
            /// Returns colors in following order:
            /// Red -> Tangelo -> Orange -> Yellow -> Green -> Ao -> Cyan -> DodgerBlue -> Blue -> Purple -> Magenta.
            /// </summary>
            public static readonly Color[] ColorWheel = new Color[]
            {
                Red,
                Tangelo,
                Orange,
                Yellow,
                Green,
                Ao,
                Cyan,
                DodgerBlue,
                Blue,
                Purple,
                Magenta,
            };

            /// <summary>
            /// Returns colors in following order:
            /// Red -> Orange -> Yellow -> Green -> Cyan -> Dodger Blue -> Magenta.
            /// </summary>
            public static readonly Color[] RainbowColors = new Color[]
            {
                Red,
                Orange,
                Yellow,
                Green,
                Cyan,
                DodgerBlue,
                Magenta
            };

            /// <summary>
            /// Returns the hexcode value of a color. (Pointer of <see cref="ColorUtility.ToHtmlStringRGB(Color)"/>)
            /// </summary>
            /// <param name="color"></param>
            /// <returns></returns>
            public static string ColorToHex(Color color)
            {
                return ColorUtility.ToHtmlStringRGB(color);
            }

            /// <summary>
            /// Returns the median of two given colors.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static Color MiddleMan(Color a, Color b)
            {
                float r = a.r + b.r / 2f;
                float g = a.g + b.g / 2f;
                float b2 = a.b + b.b / 2f;
                float a2 = a.a + b.a / 2f;
                return new Color(r, g, b2, a2);
            }

            /// <summary>
            /// Returns the median of two given colors, where percentage 0 is a, 0.5 is the exact median, and 1 is b.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="percentage"></param>
            /// <returns></returns>
            public static Color MiddleMan(Color a, Color b, float percentage)
            {
                float num = 1f - percentage;
                float num2 = a.r * num;
                float num3 = a.g * num;
                float num4 = a.b * num;
                float num5 = a.a * num;
                float num6 = b.r * percentage;
                float num7 = b.g * percentage;
                float num8 = b.b * percentage;
                float num9 = b.a * percentage;
                return new Color(num2 + num6, num3 + num7, num4 + num8, num5 + num9);
            }

            /// <summary>
            /// Returns the average of all colors inside a collection.
            /// </summary>
            /// <param name="colors"></param>
            /// <returns></returns>
            public static Color MiddleMan(ICollection<Color> colors)
            {
                float num = 0f;
                float num2 = 0f;
                float num3 = 0f;
                float num4 = 0f;
                foreach (Color color in colors)
                {
                    num += color.r;
                    num2 += color.g;
                    num3 += color.b;
                    num4 += color.a;
                }
                num /= (float)colors.Count;
                num2 /= (float)colors.Count;
                num3 /= (float)colors.Count;
                num4 /= (float)colors.Count;
                return new Color(num, num2, num3, num4);
            }
        }

        /// <summary>
        /// Contains some utility methods concerting enumeration and generic collections.
        /// </summary>
        public static class Enumeration
        {
            /// <summary>
            /// Returns the amount of times an item is present in a collection.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <param name="item"></param>
            /// <returns></returns>
            public static int GetItemCount<T>(IEnumerable<T> collection, T item)
            {
                int count = 0;
                foreach(T colItem in collection)
                {
                    if (colItem.Equals(item))
                        count++;
                }
                return count;
            }

            /// <summary>
            /// Returns the amount of times a condition matched.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <param name="match"></param>
            /// <returns></returns>
            public static int GetMatchCount<T>(IEnumerable<T> collection, Predicate<T> match)
            {
                int count = 0;
                foreach (T colItem in collection)
                {
                    if (match(colItem))
                        count++;
                }
                return count;
            }

            /// <summary>
            /// Returns the first empty index of a list; Returns -1 if none were found.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <returns></returns>
            public static int GetEmptyIndex<T>(IList<T> list)
            {
                for (int i = 0; i < list.Count; i++)
                    if (list[i] == null)
                        return i;

                return -1;
            }
            
            /// <summary>
            /// Returns the first empty index of an array; Returns -1 if none were found.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array"></param>
            /// <returns></returns>
            public static int GetEmptyIndex<T>(T[] array)
            {
                for (int i = 0; i < array.Length; i++)
                    if (array[i] == null)
                        return i;

                return -1;
            }

            /// <summary>
            /// Returns true if a list contains an item of type. (Only works on inherited classes)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public static bool ListContainsType<T>(List<T> list, Type type)
            {
                foreach (T item in list)
                {
                    if (item.GetType() == type)
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Returns an array which is a merged version of array1 and array2.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array1"></param>
            /// <param name="array2"></param>
            /// <returns></returns>
            public static T[] ArrayMerger<T>(T[] array1, T[] array2)
            {
                T[] array3 = new T[array1.Length + array2.Length];
                Array.Copy(array1, array3, array1.Length);
                Array.Copy(array2, 0, array3, array1.Length, array2.Length);
                return array3;
            }

            /// <summary>
            /// Equivalent to <paramref name="collection"/>.Intersect(<paramref name="objectsToFind"/>).Any().
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <param name="objectsToFind"></param>
            /// <returns></returns>
            public static bool EnumerableContains<T>(IEnumerable<T> collection, IEnumerable<T> objectsToFind)
            {
                return collection.Intersect(objectsToFind).Any();
            }

            /// <summary>
            /// Returns true if the Array or all of it's elements are null.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <returns></returns>
            public static bool IsEmpty<T>(IEnumerable<T> collection)
            {
                if (collection == null || collection.Count() < 1)
                {
                    return true;
                }
                foreach (T item in collection)
                {
                    if (item != null)
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Equivalent to <see cref="List{T}.Find(Predicate{T})"/> for Arrays.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="toUse"></param>
            /// <param name="match"></param>
            /// <returns></returns>
            public static T Find<T>(T[] toUse, Predicate<T> match)
                => Array.Find(toUse, match);

            /// <summary>
            /// Equivalent to <see cref="List{T}.FindIndex(Predicate{T})"/> for Arrays.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="toUse"></param>
            /// <param name="match"></param>
            /// <returns></returns>
            public static int FindIndex<T>(T[] toUse, Predicate<T> match)
                => Array.FindIndex(toUse, match);

            /// <summary>
            /// Equivalent to <see cref="List{T}.ForEach(Action{T})"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array"></param>
            /// <param name="action"></param>
            public static void ForEach<T>(T[] array, Action<T> action)
                => Array.ForEach(array, action);

            /// <summary>
            /// Removes all null elements inside a collection.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            public static IEnumerable<T> RemoveNull<T>(IEnumerable<T> list)
            {
                return list.Where(t => t != null);
            }

            /// <summary>
            /// Removes all default elements inside a collection.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            public static IEnumerable<T> RemoveDefault<T>(IEnumerable<T> list)
            {
                return list.Where(t => t.Equals(default(T)));
            }

            /// <summary>
            /// Returns a generic <see cref="List{T}"/> from a non-generic <see cref="IList"/>. In case of an incorrect cast, it will return null.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="linqReturn">If false, it will return a new list instead of using System.Linq to generate a list.</param>
            /// <returns></returns>
            public static List<T> ToGenericList<T>(IList list, bool linqReturn = true)
            {
                try
                {
                    if (linqReturn)
                        return list.Cast<T>().ToList();
                    else return new List<T>(list.Cast<T>());
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>
            /// Returns a generic <see cref="IEnumerable{T}"/> from a non-generic <see cref="IEnumerable"/>. In case of an incorrect cast, it will return null.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="enumerable"></param>
            /// <returns></returns>
            public static IEnumerable<T> ToGeneric<T>(IEnumerable enumerable)
            {
                try
                {
                    return enumerable.Cast<T>();
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>
            /// Returns a <see cref="IEnumerable{T}"/> of instantiated objects (unity copy). (Supports <see cref="IInstantiatable"/> interface)
            /// </summary>
            /// <param name="objects"></param>
            /// <returns></returns>
            public static List<T> InstantiateList<T>(IEnumerable<T> objects) where T : UnityEngine.Object
            {
                List<T> toReturn = new List<T>(objects);
                for (int i = 0; i < toReturn.Count; i++)
                {
                    toReturn[i] = UnityEngine.Object.Instantiate(toReturn[i]);
                    if (toReturn[i] is IInstantiatable instantiatable) instantiatable.PostInstantiate();
                }

                return toReturn;
            }

            /// <summary>
            /// Returns an array of instantiated objects. (Supports <see cref="IInstantiatable"/> interface)
            /// </summary>
            /// <param name="objects"></param>
            /// <returns></returns>
            public static T[] InstantiateList<T>(T[] objects) where T : UnityEngine.Object
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    objects[i] = UnityEngine.Object.Instantiate(objects[i]);
                    if (objects[i] is IInstantiatable instantiatable) instantiatable.PostInstantiate();
                }

                return objects;
            }
        }

        /// <summary>
        /// Raycasts and physics.
        /// </summary>
        public static class Physics
        {
            /// <summary>
            /// Casts multiple raycasts in given directions from origin.
            /// </summary>
            /// <param name="origin">Center of the raycast.</param>
            /// <param name="directions">Directions which will be used for raycasts. Note that it will return a number of 
            /// <see cref="RaycastHit2D"/> equal to the directions given, in the same order.</param>
            /// <param name="bitmask">The layermask(s) to use.</param>
            /// <returns></returns>
            public static RaycastHit2D[] CastMultiple(Vector2 origin, Vector2[] directions, int bitmask)
            {
                RaycastHit2D[] toReturn = new RaycastHit2D[directions.Length];

                for(int i = 0; i < toReturn.Length; i++)
                {
                    toReturn[i] = Physics2D.Raycast(origin, directions[i], float.PositiveInfinity, bitmask);
                }

                return toReturn;
            }

            /// <summary>
            /// Performs a <see cref="CastMultiple(Vector2, Vector2[], int)"/> and gives the index of the closest raycast to the center.
            /// </summary>
            /// <param name="origin">Center of the raycast.</param>
            /// <param name="directions">Directions which will be used for raycasts. Note that it will return a number of 
            /// <see cref="RaycastHit2D"/> equal to the directions given, in the same order.</param>
            /// <param name="bitmask">The layermask(s) to use.</param>
            /// <param name="closestIndex">The index of the raycast that hit closest to the center. Returns -1 if none hit.</param>
            /// <returns></returns>
            public static RaycastHit2D[] CastMultiple(Vector2 origin, Vector2[] directions, int bitmask, out int closestIndex)
            {
                RaycastHit2D[] toReturn = new RaycastHit2D[directions.Length];

                closestIndex = -1;
                float closestDist = float.PositiveInfinity;
                for (int i = 0; i < toReturn.Length; i++)
                {
                    toReturn[i] = Physics2D.Raycast(origin, directions[i], float.PositiveInfinity, bitmask);
                    
                    if (!toReturn[i].collider) //Skips itteration if nothing was hit
                        continue;

                    float dist = Vector2.Distance(origin, toReturn[i].point);
                    if(dist < closestDist)
                    {
                        closestDist = dist;
                        closestIndex = i;
                    }
                }

                return toReturn;
            }
        }

        /// <summary>
        /// Contains all methods Math related.
        /// </summary>
        public static class MathF
        {
            #region Vectors
            public static readonly Vector3 Up = new Vector3(0f, 1f, 0f);
            public static readonly Vector3 Down = new Vector3(0f, -1f, 0f);
            public static readonly Vector3 Left = new Vector3(-1f, 0f, 0f);
            public static readonly Vector3 Right = new Vector3(1f, 0f, 0f);
            public static readonly Vector3 Forward = new Vector3(0f, 0f, 1f);
            public static readonly Vector3 Back = new Vector3(0f, 0f, -1f);
            public static readonly Vector3 One = new Vector3(1f, 1f, 1f);
            public static readonly Vector3 Zero = new Vector3(0f, 0f, 0f);

            public static readonly Vector2 Up2 = new Vector2(0f, 1f);
            public static readonly Vector2 Down2 = new Vector2(0f, -1f);
            public static readonly Vector2 Left2 = new Vector2(-1f, 0f);
            public static readonly Vector2 Right2 = new Vector2(1f, 0f);
            public static readonly Vector2 One2 = new Vector2(1f, 1f);
            public static readonly Vector2 Zero2 = new Vector2(0f, 0f);

            /// <summary>
            ///   <para>Moves a point current towards target.</para>
            /// </summary>
            /// <param name="current"></param>
            /// <param name="target"></param>
            /// <param name="maxDistanceDelta"></param>
            public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
            {
                Vector2 a = target - current;
                float magnitude = Magnitude(a);
                if (!(magnitude <= maxDistanceDelta) && magnitude != 0f)
                {
                    return current + a / magnitude * maxDistanceDelta;
                }
                return target;
            }

            /// <summary>
            /// Moves a point current towards target.
            /// </summary>
            /// <param name="current"></param>
            /// <param name="target"></param>
            /// <param name="maxDistanceDelta"></param>
            public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
            {
                Vector3 a = target - current;
                float magnitude = Magnitude(a);
                if (!(magnitude <= maxDistanceDelta) && magnitude != 0f)
                {
                    return current + a / magnitude * maxDistanceDelta;
                }
                return target;
            }

            /// <summary>
            ///   <para>Moves a point current towards target.</para>
            /// </summary>
            /// <param name="current"></param>
            /// <param name="target"></param>
            /// <param name="maxDistanceDelta"></param>
            public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta)
            {
                Vector4 a = target - current;
                float magnitude = Magnitude(a);
                if (!(magnitude <= maxDistanceDelta) && magnitude != 0f)
                {
                    return current + a / magnitude * maxDistanceDelta;
                }
                return target;
            }

            /// <summary>
            /// Returns the magnitude of a vector. (Square root of it's dot product)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static float Magnitude(Vector2 original)
            {
                return Mathf.Sqrt(Vector2.Dot(original, original));
            }

            /// <summary>
            /// Returns the square root of f.
            /// </summary>
            /// <param name="f"></param>
            /// <returns></returns>
            public static float Sqrt(float f)
            {
                return (float)Math.Sqrt(f);
            }

            /// <summary>
            /// Returns the magnitude of a vector. (Square root of it's dot product)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static float Magnitude(Vector3 original)
            {
                return Mathf.Sqrt(Vector3.Dot(original, original));
            }

            /// <summary>
            /// Returns the magnitude of a vector. (Square root of it's dot product)
            /// </summary>
            /// <param name="original"></param>
            /// <returns></returns>
            public static float Magnitude(Vector4 original)
            {
                return Mathf.Sqrt(Vector4.Dot(original, original));
            }

            /// <summary>
            /// Returns direction which would make Position look at Destination.
            /// </summary>
            /// <param name="Position"></param>
            /// <param name="Destination"></param>
            /// <returns></returns>
            public static Vector3 DirectionTowards(Vector3 Position, Vector3 Destination)
            {
                Vector3 toReturn = (Destination - Position);
                toReturn.Normalize();
                return toReturn;
            }

            /// <summary>
            /// Returns the total of vectors divided by Length of array given.
            /// </summary>
            /// <param name="values"></param>
            /// <returns></returns>
            public static Vector3 Average(Vector3[] values)
            {
                Vector3 toReturn = Vector3.zero;

                values.ForEach(v => toReturn += v);

                toReturn /= values.Length;

                return toReturn;
            }

            /// <summary>
            /// Returns in order: Vector3.back, Vector3.down, Vector3.forward, Vector3.left, Vector3.one, Vector3.right, Vector3.up
            /// </summary>
            public static Vector3[] AllDirections => new Vector3[7]
            {
                Vector3.back,
                Vector3.down,
                Vector3.forward,
                Vector3.left,
                Vector3.one,
                Vector3.right,
                Vector3.up
            };
            /// <summary>
            /// Returns in order: Vector2.right, Vector2.one, Vector2.up, new Vector2(-1f, 1f), Vector2.left, -Vector2.one, Vector2.down, new Vector2(1f, -1f)
            /// </summary>
            public static Vector2[] AllDirections2D => new Vector2[8]
            {
                Vector2.right,
                Vector2.one,
                Vector2.up,
                new Vector2(-1f, 1f),
                Vector2.left,
                -Vector2.one,
                Vector2.down,
                new Vector2(1f, -1f)
            };

            /// <summary>
            /// Returns in order: Vector2.one, new Vector2(-1f, 1f), -Vector2.one, new Vector2(1f, -1f)
            /// </summary>
            public static Vector2[] DiagonalDirections2D => new Vector2[4]
            {
                Vector2.one,
                new Vector2(-1f, 1f),
                -Vector2.one,
                new Vector2(1f, -1f)
            };

            /// <summary>
            /// Returns in order: Vector2.right, Vector2.up, Vector2.left, Vector2.down
            /// </summary>
            public static Vector2[] MainDirections2D => new Vector2[4]
            {
                Vector2.right,
                Vector2.up,
                Vector2.left,
                Vector2.down
            };

            /// <summary>
            /// Returns in order: Vector2.left, Vector2.right
            /// </summary>
            public static Vector2[] HorizontalSides2D => new Vector2[2]
            {
                Vector2.left,
                Vector2.right
            };

            /// <summary>
            /// Returns in order: Vector2.up, Vector2.down
            /// </summary>
            public static Vector2[] VerticalSides2D => new Vector2[2]
            {
                Vector2.up,
                Vector2.down
            };

            /// <summary>
            /// Returns the Anchored position of a RectTransform in Vector4,
            /// where x = anchorMin.x, y = anchorMin.y, z = anchorMax.x, w = anchorMax.y.
            /// </summary>
            /// <param name="rt"></param>
            /// <returns></returns>
            public static Vector4 GetAnchoredPosition(RectTransform rt)
            {
                Vector2 anchorMin = rt.anchorMin;
                Vector2 anchorMax = rt.anchorMax;
                return new Vector4(anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y);
            }

            /// <summary>
            /// Sets the anchored position of an RectTransform with it's offset set to 0.
            /// Vector4 use: x = anchorMin.x, y = anchorMin.y, z = anchorMax.x, w = anchorMax.y.
            /// </summary>
            /// <param name="rt"></param>
            /// <param name="Position"></param>
            /// <returns></returns>
            public static void SetAnchoredUI(RectTransform rt, Vector4 Position)
            {
                rt.anchorMin = new Vector2(Position.x, Position.y);
                rt.anchorMax = new Vector2(Position.z, Position.w);
                rt.offsetMin = rt.offsetMax = Vector2.zero;
            }

            /// <summary>
            /// Sets the anchored position of an RectTransform with it's offset set to 0.
            /// </summary>
            /// <param name="rt"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static void SetAnchoredUI(RectTransform rt, Vector2 min, Vector2 max)
            {
                rt.anchorMin = min;
                rt.anchorMax = max;
                rt.offsetMin = rt.offsetMax = Vector2.zero;
            }

            /// <summary>
            /// Sets the anchored position of an RectTransform with it's offset set to 0.
            /// </summary>
            /// <param name="rt"></param>
            /// <param name="minX"></param>
            /// <param name="minY"></param>
            /// <param name="maxX"></param>
            /// <param name="maxY"></param>
            /// <returns></returns>
            public static void SetAnchoredUI(RectTransform rt, float minX, float minY, float maxX, float maxY)
            {
                rt.anchorMin = new Vector2(minX, minY);
                rt.anchorMax = new Vector2(maxX, maxY);
                rt.offsetMin = rt.offsetMax = Vector2.zero;
            }

            /// <summary>
            /// Fits all RectTransforms given inside a parent horizontally, where anchorMin.y = 0 and anchorMax.y = 1.
            /// </summary>
            /// <param name="rts"></param>
            public static void SetAllAnchoredUIFitHorizontal(IEnumerable<RectTransform> rts)
            {
                RectTransform[] transforms = rts.ToArray();
                int size = transforms.Length;
                for (int i = 0; i < transforms.Length; i++)
                {
                    float か = (i + 1);
                    transforms[i].SetAnchoredUI((1f / size) * i, 0, (1f / size) * か, 1);
                }
            }

            /// <summary>
            /// Fits all RectTransforms given inside a parent horizontally, where anchorMin.y = 0 and anchorMax.y = 1, with x values being limited from 0 to maxSize01.
            /// </summary>
            /// <param name="rts"></param>
            /// <param name="maxSize01"></param>
            public static void SetAllAnchoredUIFitHorizontal(IEnumerable<RectTransform> rts, float maxSize01)
            {
                RectTransform[] transforms = rts.ToArray();
                float size = maxSize01;
                int collectionSize = transforms.Length;
                for (int i = 0; i < transforms.Length; i++)
                {
                    float か = (i + 1);
                    transforms[i].SetAnchoredUI((size / collectionSize) * i, 0, (size / collectionSize) * か, 1);
                }
            }

            /// <summary>
            /// Fits all RectTransforms given inside a parent horizontally, where anchorMin.x = 0 and anchorMax.x = 1.
            /// </summary>
            /// <param name="rts"></param>
            public static void SetAllAnchoredUIFitVertical(IEnumerable<RectTransform> rts)
            {
                RectTransform[] transforms = rts.ToArray();
                int size = transforms.Length;
                for (int i = 0; i < transforms.Length; i++)
                {
                    float か = (i + 1);
                    transforms[i].SetAnchoredUI(0, (1f / size) * i, 1, (1f / size) * か);
                }
            }

            /// <summary>
            /// Fits all RectTransforms given inside a parent horizontally, where anchorMin.y = 0 and anchorMax.y = 1, with x values being limited from 0 to maxSize01.
            /// </summary>
            /// <param name="rts"></param>
            /// <param name="maxSize01"></param>
            public static void SetAllAnchoredUIFitVertical(IEnumerable<RectTransform> rts, float maxSize01)
            {
                RectTransform[] transforms = rts.ToArray();
                float size = maxSize01;
                int collectionSize = transforms.Length;
                for (int i = 0; i < transforms.Length; i++)
                {
                    float か = (i + 1);
                    transforms[i].SetAnchoredUI(0, (size / collectionSize) * i, 1, (size / collectionSize) * か);
                }
            }

            /// <summary>
            /// Gets the aspect ratio of the game in Vector2.
            /// </summary>
            /// <returns></returns>
            public static Vector2 GetAspectRatio() => GetAspectRatio(Screen.currentResolution);

            /// <summary>
            /// Gets the aspect ratio of a given Resolution.
            /// </summary>
            /// <param name="resolution"></param>
            /// <returns></returns>
            public static Vector2 GetAspectRatio(Resolution resolution)
            {
                float f = resolution.width / resolution.height;
                int i = 0;
                while (true)
                {
                    i++;
                    if (Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
                        break;
                }
                return new Vector2((float)Math.Round(f * i, 2), i);
            }

            /// <summary>
            /// Rotates point around pivot based on angle (Euler).
            /// </summary>
            /// <param name="point"></param>
            /// <param name="pivot"></param>
            /// <param name="angle"></param>
            /// <returns></returns>
            public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
            {
                return Quaternion.Euler(angle) * (point - pivot) + pivot;
            }

            /// <summary>
            /// Rotates point around pivot based on angle (Quaternion).
            /// </summary>
            /// <param name="point"></param>
            /// <param name="pivot"></param>
            /// <param name="angle"></param>
            /// <returns></returns>
            public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
            {
                return angle * (point - pivot) + pivot;
            }

            /// <summary>
            /// Returns a Vector2 that has the highest axis value kept, and the lowest set to 0.
            /// </summary>
            /// <param name="evaluator"></param>
            /// <returns></returns>
            public static Vector2 HighestValue(Vector2 evaluator)
            {
                float num = evaluator.x;
                float num2 = evaluator.y;
                bool flag = false;
                bool flag2 = false;
                if (num < 0f)
                {
                    flag = true;
                    num = 0f - num;
                }
                if (num2 < 0f)
                {
                    flag2 = true;
                    num2 = 0f - num2;
                }
                if (num == 0f && flag2)
                {
                    flag = true;
                }
                if (num2 == 0f && flag)
                {
                    flag2 = true;
                }
                Vector2 result = new Vector2((num > num2) ? ((!flag) ? 1 : (-1)) : 0, (num2 > num) ? ((!flag2) ? 1 : (-1)) : 0);
                return result;
            }

            /// <summary>
            /// Converts a Vector3 to a Vector3Int.
            /// </summary>
            /// <param name="vector"></param>
            /// <returns></returns>
            public static Vector3Int ToInt(Vector3 vector)
            {
                return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
            }

            /// <summary>
            /// Convert all values of a given Vector3 into Int32 values without turning it into a <see cref="Vector3Int"/>.
            /// </summary>
            /// <param name="vector"></param>
            /// <returns></returns>
            public static Vector3 ToIntNormal(Vector3 vector)
            {
                return new Vector3(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
            }

            /// <summary>
            /// Convert all values of a given Vector3 into Int32 values without turning it into a <see cref="Vector3Int"/>, and ignores the corresponding values for conversion.
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="ignoreX"></param>
            /// <param name="ignoreY"></param>
            /// <param name="ignoreZ"></param>
            /// <returns></returns>
            public static Vector3 ToIntNormal(Vector3 vector, bool ignoreX, bool ignoreY, bool ignoreZ)
            {
                return new Vector3(ignoreX ? vector.x : ((float)Mathf.RoundToInt(vector.x)), ignoreY ? vector.y : ((float)Mathf.RoundToInt(vector.y)), ignoreZ ? vector.z : ((float)Mathf.RoundToInt(vector.z)));
            }

            /// <summary>
            /// Snaps a vector to given factor. Useful to make grid-like behaviour.
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="factor"></param>
            /// <returns></returns>
            public static Vector3 Snap(Vector3 vector, float factor)
            {
                factor = factor.ToPositive();
                float x = Mathf.Round(vector.x / factor) * factor;
                float y = Mathf.Round(vector.y / factor) * factor;
                float z = Mathf.Round(vector.z / factor) * factor;
                return new Vector3(x, y, z);
            }

            /// <summary>
            /// Snaps a vector to given factor, while ignoring all corresponding Ignore values.
            /// </summary>
            /// <param name="vector"></param>
            /// <param name="factor"></param>
            /// <param name="IgnoreX"></param>
            /// <param name="IgnoreY"></param>
            /// <param name="IgnoreZ"></param>
            /// <returns></returns>
            public static Vector3 Snap(Vector3 vector, float factor, bool IgnoreX, bool IgnoreY, bool IgnoreZ)
            {
                factor = factor.ToPositive();
                float x = IgnoreX ? vector.x : (Mathf.Round(vector.x / factor) * factor);
                float y = IgnoreY ? vector.y : (Mathf.Round(vector.y / factor) * factor);
                float z = IgnoreZ ? vector.z : (Mathf.Round(vector.z / factor) * factor);
                return new Vector3(x, y, z);
            }

            /// <summary>
            /// Equivalent to <see cref="Quaternion.RotateTowards(Quaternion, Quaternion, float)"/>.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <param name="speed"></param>
            /// <returns></returns>
            public static Vector3 EulerRotateTowards(Vector3 from, Vector3 to, float speed)
            {
                return Quaternion.RotateTowards(Quaternion.Euler(from), Quaternion.Euler(to), speed).eulerAngles;
            }

            /// <summary>
            /// Returns a Vector2 that has the lowest axis value kept, and the highest set to 0.
            /// </summary>
            /// <param name="evaluator"></param>
            /// <returns></returns>
            public static Vector2 LowestValue(Vector2 evaluator)
            {
                float num = evaluator.x;
                float num2 = evaluator.y;
                bool flag = false;
                bool flag2 = false;
                if (num < 0f)
                {
                    flag = true;
                    num = 0f - num;
                }
                if (num2 < 0f)
                {
                    flag2 = true;
                    num2 = 0f - num2;
                }
                if (num == 0f && flag2)
                {
                    flag = true;
                }
                if (num2 == 0f && flag)
                {
                    flag2 = true;
                }
                Vector2 result = new Vector2((num < num2) ? ((!flag) ? 1 : (-1)) : 0, (num2 < num) ? ((!flag2) ? 1 : (-1)) : 0);
                return result;
            }

            /// <summary>
            /// Equivalent to <see cref="Vector3.Distance(Vector3, Vector3)"/>
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static float DistanceBetween(GameObject a, GameObject b)
            {
                float result = 0f;
                if (a && b)
                {
                    result = Vector3.Distance(a.transform.position, b.transform.position);
                }
                return result;
            }

            /// <summary>
            /// Equivalent to <see cref="Vector2.Distance(Vector2, Vector2)"/>
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static float DistanceBetween(Vector2 a, Vector2 b)
            {
                return Vector2.Distance(a, b);
            }

            /// <summary>
            /// Returns the distance of an axis between two Vector2 values.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="axisChoice"></param>
            /// <returns></returns>
            public static float DistanceBetween(Vector2 a, Vector2 b, Axis axisChoice)
            {
                float num;
                float num2;
                if (axisChoice == Axis.X)
                {
                    num = a.x;
                    num2 = b.x;
                }
                else
                {
                    num = a.y;
                    num2 = b.y;
                }
                float num3 = num - num2;
                if (num3 < 0f)
                {
                    num3 = 0f - num3;
                }
                return num3;
            }

            /// <summary>
            /// Returns true if position is wihin bounds. (bounds are: x and y as min, z and w as max (z is max X and w is max Y))
            /// </summary>
            /// <param name="position"></param>
            /// <param name="bounds"></param>
            /// <returns></returns>
            public static bool IsInsideBounds(Vector2 position, Vector4 bounds)
            {
                float x = position.x;
                float y = position.y;
                return x > bounds.x && x < bounds.z && y > bounds.y && y < bounds.w;
            }

            /// <summary>
            /// Returns true if position is within size from center. 
            /// </summary>
            /// <param name="center"></param>
            /// <param name="size"></param>
            /// <param name="position"></param>
            /// <returns></returns>
            [Obsolete]
            public static bool IsInsideBoundsOld(Vector3 position, Vector3 center, Vector3 size)
            {
                float minX = center.x - size.x, maxX = center.x + size.x;
                float minY = center.y - size.y, maxY = center.y + size.y;
                float minZ = center.z - size.z, maxZ = center.z + size.z;

                return position.x > minX && position.x < maxX &&
                    position.y > minY && position.y < maxY &&
                    position.z > minZ && position.z < maxZ;
            }

            /// <summary>
            /// Returns true if position is within size from center. 
            /// </summary>
            /// <param name="position"></param>
            /// <param name="center"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            public static bool IsInsideBounds(Vector3 position, Vector3 center, Vector3 size)
            {
                Vector3 compared = position - center;

                return compared.x > -size.x && compared.x < size.x
                    && compared.y > -size.y && compared.y < size.y
                    && compared.z > -size.z && compared.x < size.z;
            }

            /// <summary>
            /// Returns true if the position is within the radius of center.
            /// </summary>
            /// <param name="position"></param>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            /// <returns></returns>
            public static bool IsInsideBounds(Vector3 position, Vector3 center, float radius)
                => Vector3.Distance(position, center) < radius;

            /// <summary>
            /// Returns true if position is wihin min and max.
            /// </summary>
            /// <param name="position"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static bool IsInsideBounds(Vector2 position, Vector2 min, Vector2 max)
                => IsInsideBounds(position, new Vector4(min.x, min.y, max.x, max.y));

            /// <summary>
            /// Returns a Vector2 which has it's Y value set to the given Vector3 Z value.
            /// </summary>
            /// <param name="vector"></param>
            /// <returns></returns>
            public static Vector2 Vector3D(Vector3 vector)
            {
                return new Vector2(vector.x, vector.z);
            }

            /// <summary>
            /// Returns a Vector3 which moves a towards b based on speed. Accelerates based on the distance between both positions.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="speed"></param>
            /// <param name="acceleration"></param>
            /// <returns></returns>
            public static Vector3 MoveTowardsAccelerated(Vector3 a, Vector3 b, float speed, float acceleration)
            {
                float num = acceleration * Vector3.Distance(a, b);
                float maxDistanceDelta = speed + num;
                return Vector3.MoveTowards(a, b, maxDistanceDelta);
            }

            /// <summary>
            /// Returns a <see cref="Vector3.MoveTowards(Vector3, Vector3, float)"/> which accelerates when A and B are far away.
            /// </summary>
            /// <param name="a">The current position.</param>
            /// <param name="b">The desired position.</param>
            /// <param name="speed">Speed at which a moves towards b.</param>
            /// <param name="acceleration">How heavy the acceleration is.</param>
            /// <param name="accelerationMinDist">Acceleration is applied only when the distance between a and b is greater than this value.</param>
            /// <returns></returns>
            public static Vector3 MoveTowardsAccelerated(Vector3 a, Vector3 b, float speed, float acceleration, float accelerationMinDist)
            {
                float dist = Vector3.Distance(a, b);
                if (dist > accelerationMinDist)
                    return Vector3.MoveTowards(a, b, speed + (dist * acceleration));

                return Vector3.MoveTowards(a, b, speed);
            }

            /// <summary>
            /// Equivalent to <see cref="Mathf.Clamp(float, float, float)"/> for Vector3s.
            /// </summary>
            /// <param name="toClamp"></param>
            /// <param name="minX"></param>
            /// <param name="maxX"></param>
            /// <param name="minY"></param>
            /// <param name="maxY"></param>
            /// <returns></returns>
            public static Vector2 Clamp(Vector2 toClamp, float minX, float maxX, float minY, float maxY)
            {
                return new Vector2(Mathf.Clamp(toClamp.x, minX, maxX), Mathf.Clamp(toClamp.y, minY, maxY));
            }

            /// <summary>
            /// Equivalent to <see cref="Mathf.Clamp(float, float, float)"/> for Vector3s.
            /// </summary>
            /// <param name="toClamp"></param>
            /// <param name="minX"></param>
            /// <param name="maxX"></param>
            /// <param name="minY"></param>
            /// <param name="maxY"></param>
            /// <param name="minZ"></param>
            /// <param name="maxZ"></param>
            /// <returns></returns>
            public static Vector3 Clamp(Vector3 toClamp, float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
            {
                return new Vector3(Mathf.Clamp(toClamp.x, minX, maxX), Mathf.Clamp(toClamp.y, minY, maxY), Mathf.Clamp(toClamp.z, minZ, maxZ));
            }

            /// <summary>
            /// Equivalent to <see cref="Mathf.Clamp(float, float, float)"/> for Vector3s.
            /// </summary>
            /// <param name="toClamp"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static Vector3 Clamp(Vector3 toClamp, Vector3 min, Vector3 max)
            {
                return new Vector3(Mathf.Clamp(toClamp.x, min.x, max.x), Mathf.Clamp(toClamp.y, min.y, max.y), Mathf.Clamp(toClamp.z, min.z, max.z));
            }

            /// <summary>
            /// Sets NaN, null or Infinite values of the given Vector to 0.
            /// </summary>
            /// <param name="toReform"></param>
            /// <returns></returns>
            public static Vector3 Reformalize(Vector3 toReform)
            {
                Vector3 vector = new Vector3(MathF.IsFormal(toReform.x) ? 0f : toReform.x, MathF.IsFormal(toReform.y) ? 0f : toReform.y, MathF.IsFormal(toReform.z) ? 0f : toReform.z);
                return vector;
            }

            /// <summary>
            /// Returns true if PositionToApproximate is within approximation of PositionToCompare.
            /// </summary>
            /// <param name="PositionToApproximate"></param>
            /// <param name="PositionToCompare"></param>
            /// <param name="approximation"></param>
            /// <returns></returns>
            public static bool IsApproximate(Vector3 PositionToApproximate, Vector3 PositionToCompare, Vector3 approximation)
            {
                bool flag = PositionToApproximate.x.IsApproximate(PositionToCompare.x, approximation.x);
                bool flag2 = PositionToApproximate.y.IsApproximate(PositionToCompare.y, approximation.y);
                bool flag3 = PositionToApproximate.z.IsApproximate(PositionToCompare.z, approximation.z);
                return (flag && flag2) & flag3;
            }

            /// <summary>
            /// Returns a position between a and b, based on percent (0-1).
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="percent"></param>
            /// <returns></returns>
            public static Vector2 MiddleMan(Vector2 a, Vector2 b, float percent = 0.5f)
            {
                float num = MathF.MiddleMan(a.x, b.x, percent);
                float num2 = MathF.MiddleMan(a.y, b.y, percent);
                return new Vector2(a.x + num, a.y + num2);
            }

            /// <summary>
            /// Converts Vector2 to Vector2Int,
            /// </summary>
            /// <param name="vectorToConvert"></param>
            /// <returns></returns>
            public static Vector2Int VectorIntConverter(Vector2 vectorToConvert)
            {
                int x = (int)vectorToConvert.x;
                int y = (int)vectorToConvert.y;
                return new Vector2Int(x, y);
            }

            /// <summary>
            /// Gets the closest element to the given point from a given collection.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="point"></param>
            /// <param name="maxDistance"></param>
            /// <param name="collection"></param>
            /// <returns></returns>
            public static T GetClosestToPoint<T>(Vector3 point, float maxDistance, IEnumerable<T> collection) where T : Component
            {
                T result = null;
                float num = float.PositiveInfinity;
                foreach (T item in collection)
                {
                    float num2 = Vector3.Distance(item.transform.position, point);
                    if (!(num2 > maxDistance) && !(num2 > num))
                    {
                        result = item;
                        num = num2;
                    }
                }
                return result;
            }

            /// <summary>
            /// Gets the closest gameobject to the given point within a maximum distance.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="maxDistance"></param>
            /// <returns></returns>
            public static GameObject GetClosestToPoint(Vector3 point, float maxDistance)
            {
                GameObject result = null;
                float num = float.PositiveInfinity;
                GameObject[] gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (GameObject gameObject in gameObjects)
                {
                    float num2 = Vector3.Distance(gameObject.transform.position, point);
                    if (!(num2 > maxDistance) && !(num2 > num))
                    {
                        result = gameObject;
                        num = num2;
                    }
                }
                return result;
            }

            /// <summary>
            /// Gets the closest gameobject to the given point within a maximum distance.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="maxDistance"></param>
            /// <param name="excluded">Any gameobject in this list will be skipped.</param>
            /// <returns></returns>
            public static GameObject GetClosestToPoint(Vector3 point, float maxDistance, GameObject[] excluded)
            {
                GameObject result = null;
                float num = float.PositiveInfinity;
                GameObject[] gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (GameObject gameObject in gameObjects)
                {
                    if (Array.IndexOf(excluded, gameObject) != -1)
                        continue;

                    float num2 = Vector3.Distance(gameObject.transform.position, point);
                    if (!(num2 > maxDistance) && !(num2 > num))
                    {
                        result = gameObject;
                        num = num2;
                    }
                }
                return result;
            }

            /// <summary>
            /// Gets all GameObjects within radius based on their tags. (SLOW, uses GameObject.FindGameObjectsWithTag, avoid using in Update functions)
            /// </summary>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            /// <param name="tagUse"></param>
            /// <returns></returns>
            public static GameObject[] GetAllWithinDistance(Vector3 center, float radius, string tagUse)
            {
                List<GameObject> list = new List<GameObject>(GameObject.FindGameObjectsWithTag(tagUse));
                list.RemoveAll(g => Vector2.Distance(center, g.transform.position) < radius);

                return list.ToArray();
            }

            /// <summary>
            /// Gets all GameObjects within radius based on their tags. (SLOW, uses GameObject.FindGameObjectsWithTag, avoid using in Update functions)
            /// </summary>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            /// <param name="tagsUse"></param>
            /// <returns></returns>
            public static GameObject[] GetAllWithinDistance(Vector3 center, float radius, string[] tagsUse)
            {
                List<GameObject> list = new List<GameObject>();
                foreach (string tag in tagsUse)
                {
                    list.AddRange(GameObject.FindGameObjectsWithTag(tag));
                }
                list.RemoveAll(g => Vector2.Distance(center, g.transform.position) < radius);

                return list.ToArray();
            }

            /// <summary>
            /// Returns x + y + z / 3 if includeZValue is true, otherwise returns x + y / 2.
            /// </summary>
            /// <param name="vector3"></param>
            /// <param name="includeZValue"></param>
            /// <returns></returns>
            public static float AverageValue(Vector3 vector3, bool includeZValue)
            {
                return (vector3.x + vector3.y + (includeZValue ? vector3.z : 0f)) / (float)(includeZValue ? 3 : 2);
            }

            /// <summary>
            /// Gets all components who's .transform.position is the closest to center, based on amount requested.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="center"></param>
            /// <param name="from"></param>
            /// <param name="amount"></param>
            /// <exception cref="WWWException"/>
            /// <returns></returns>
            public static T[] GetClosestToPosition<T>(Vector3 center, IEnumerable<T> from, int amount) where T : Component
            {
                int count = from.Count();
                if (amount > count)
                    throw new Exception("Cannot use GetClosestToPosition when collection given is lower in size than amount requested. Aborting");
                else if (amount == count)
                {
                    AdvancedDebug.LogWarning("Using GetClosestToPosition with amount equal to the collection size is pointless. Returning original collection...", DEBUG_LAYER_WWW_INDEX);
                    return from.ToArray();
                }

                T[] toReturn = new T[amount];

                foreach (T item in from)
                {
                    int index = toReturn.FindIndex(i => i == null || Vector3.Distance(center, i.transform.position) > Vector3.Distance(center, item.transform.position));
                    if (index == -1)
                        continue;

                    toReturn[index] = item;
                }
                return toReturn;
            }

            /// <summary>
            /// Return all components who's .transform.position is the furthest from center.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="center"></param>
            /// <param name="from"></param>
            /// <param name="amount"></param>
            /// <returns></returns>
            public static T[] GetFurthestFromPosition<T>(Vector3 center, IEnumerable<T> from, int amount) where T : Component
            {
                int count = from.Count();
                if (amount > count)
                    throw new Exception("Cannot use GetFurthestFromPosition when collection given is lower in size than amount requested. Aborting");
                else if (amount == count)
                {
                    AdvancedDebug.LogWarning("Using GetFurthestFromPosition with amount equal to the collection size is pointless. Returning original collection...", DEBUG_LAYER_WWW_INDEX);
                    return from.ToArray();
                }

                T[] toReturn = new T[amount];

                foreach (T item in from)
                {
                    int index = toReturn.FindIndex(i => i == null || Vector3.Distance(center, i.transform.position) < Vector3.Distance(center, item.transform.position));
                    if (index == -1)
                        continue;

                    toReturn[index] = item;
                }
                return toReturn;
            }

            /// <summary>
            /// Gets all components who's .transform.position is within center by radius.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            /// <param name="used"></param>
            /// <returns></returns>
            public static T[] GetAllWithinDistance<T>(Vector3 center, float radius, IEnumerable<T> used) where T : Component
            {
                List<T> toReturn = new List<T>(used);
                toReturn.RemoveAll(t => Vector3.Distance(center, t.transform.position) > radius);

                return toReturn.ToArray();
            }

            /// <summary>
            /// Gets all components who's .transform.position is within center by range.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="center"></param>
            /// <param name="range"></param>
            /// <param name="used"></param>
            /// <returns></returns>
            public static T[] GetAllWithinRange<T>(Vector3 center, FloatRange range, IEnumerable<T> used) where T : Component
            {
                List<T> toReturn = new List<T>(used);
                toReturn.RemoveAll(
                    t =>
                    {
                        float Distance = Vector3.Distance(center, t.transform.position);
                        return Distance > range.Max || Distance < range.Min;
                    }
                    );

                return toReturn.ToArray();
            }
            #endregion

            #region Quaternions & Rotations
            /// <summary>
            /// Returns a quaternion from a euler rotation.
            /// </summary>
            /// <param name="euler">The rotation in Vector3 space.</param>
            /// <returns></returns>
            public static Quaternion Euler(Vector3 euler)
            {
                YPRRotation(euler.x, euler.y, euler.z, out Quaternion toReturn);
                return toReturn;
            }

            /// <summary>
            /// Returns a quaternion based on yaw, pitch and roll.
            /// </summary>
            /// <param name="yaw">The yaw, equivalent to the X axis in euler.</param>
            /// <param name="pitch">The pitch, equivalent to the Y axis in euler.</param>
            /// <param name="roll">The roll, equivalent to the Z axis in euler.</param>
            /// <param name="result">Returned quaternion.</param>
            public static void YPRRotation(float yaw, float pitch, float roll, out Quaternion result)
            {
                //Abbreviated for the sake of readability
                float hr = roll * 0.5f;
                float hp = pitch * 0.5f;
                float hy = yaw * 0.5f;

                float sr = Mathf.Sin(hr);
                float cr = Mathf.Cos(hr);
                float sp = Mathf.Sin(hp);
                float cp = Mathf.Cos(hp);
                float sy = Mathf.Sin(hy);
                float cy = Mathf.Cos(hy);

                result.x = cy * sp * cr + sy * cp * sr;
                result.y = sy * cp * cr - cy * sp * sr;
                result.z = cy * cp * sr - sy * sp * cr;
                result.w = cy * cp * cr + sy * sp * sr;
            }

            /// <summary>
            /// Returns a list of rotations based around the amount given; amount 2 would return 0 and 180, 3 would return 0, 120 and 240, 4 would return 0, 80, 160 and 240, etc...
            /// </summary>
            /// <param name="offset"></param>
            /// <param name="amount"></param>
            /// <param name="maxRotation"></param>
            /// <returns></returns>
            public static float[] Get2DRotationsAroundPoint(float offset, int amount, float maxRotation = 360)
            {
                float[] toReturn = new float[amount];
                for (int i = 0; i < amount; i++)
                {
                    toReturn[i] = ((maxRotation / amount) * i) + offset;
                }

                return toReturn;
            }
            #endregion

            #region Implicit Convert
            #endregion

            #region Misc
            /// <summary>
            /// Returns a value with the magnitude of x and the sign of y.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static double CopySign(double x, double y)
            {
                return y >= 0d ? x : -x;
            }

            /// <summary>
            /// Returns a value with the magnitude of x and the sign of y.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static float CopySign(float x, float y)
            {
                return y >= 0f ? x : -x;
            }
            
            /// <summary>
            /// Returns a value with the magnitude of x and the sign of y.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static int CopySign(int x, int y)
            {
                return y >= 0 ? x : -x;
            }
            
            /// <summary>
            /// Moves the value current towards target.
            /// </summary>
            /// <param name="current">The current value.</param>
            /// <param name="target">The value to move towards.</param>
            /// <param name="maxDelta">The maximum change that should be applied to the value.</param>
            public static float MoveTowards(float current, float target, float maxDelta)
            {
                if (!(Math.Abs(target - current) <= maxDelta))
                {
                    return current + Mathf.Sign(target - current) * maxDelta;
                }
                return target;
            }


            /// <summary>
            /// Returns: value / maxValue * (maxValue - value)
            /// </summary>
            /// <param name="value"></param>
            /// <param name="maxValue"></param>
            /// <returns></returns>
            public static float DeminishingReturnHalfCurve(float value, float maxValue)
            {
                return value / maxValue * (maxValue - value);
            }

            /// <summary>
            /// Returns a value that scales hyperbolically.
            /// </summary>
            /// <param name="max">The max amount reachable.</param>
            /// <param name="amount">This can be used for scaling stats, say max is 100, and a stat increases by 20 for each stack;
            /// the value passed in amount would be: 20 * stacks.</param>
            /// <returns></returns>
            public static float Hyperbolic(float max, float amount)
            {
                return max - max / (max + amount);
            }

            /// <summary>
            /// Returns a value that scales hyperbolically.
            /// </summary>
            /// <param name="max">The max amount reachable.</param>
            /// <param name="amount">This can be used for scaling stats, say max is 100, and a stat increases by 20 for each stack;
            /// the value passed in amount would be: 20 * stacks.</param>
            /// <returns></returns>
            public static double Hyperbolic(double max, double amount)
            {
                return max - max / (max + amount);
            }

            /// <summary>
            /// Returns a value that scales hyperbolically, to the power of P.
            /// </summary>
            /// <param name="max">The max amount reachable.</param>
            /// <param name="amount">This can be used for scaling stats, say max is 100, and a stat increases by 20 for each stack;
            /// the value passed in amount would be: 20 * stacks.</param>
            /// <param name="P"></param>
            /// <returns></returns>
            public static float HyperbolicP(float max, float amount, float P)
            {
                return Mathf.Pow(max - max / (max + amount), P);
            }

            /// <summary>
            /// Returns a value that scales hyperbolically, to the power of P.
            /// </summary>
            /// <param name="max">The max amount reachable.</param>
            /// <param name="amount">This can be used for scaling stats, say max is 100, and a stat increases by 20 for each stack;
            /// the value passed in amount would be: 20 * stacks.</param>
            /// <param name="P"></param>
            /// <returns></returns>
            public static double HyperbolicP(double max, double amount, double P)
            {
                return Math.Pow(max - max / (max + amount), P);
            }

            /// <summary>
            /// Clamps the given value between min (inclusive) and max (inclusive).
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static double Clamp(double value, double min, double max)
            {
                double aMin = Math.Min(min, max);
                double aMax = Math.Max(min, max);

                value = value > aMax ? aMax : value;
                return value < aMin ? aMin : value;
            }

            /// <summary>
            /// Clamps the given value between min (inclusive) and max (inclusive).
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static float Clamp(float value, float min, float max)
            {
                float aMin = Math.Min(min, max);
                float aMax = Math.Max(min, max);

                value = value > aMax ? aMax : value;
                return value < aMin ? aMin : value;
            }

            /// <summary>
            /// Clamps the given value between min (inclusive) and max (inclusive).
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static int Clamp(int value, int min, int max)
            {
                int aMin = Math.Min(min, max);
                int aMax = Math.Max(min, max);

                value = value > aMax ? aMax : value;
                return value < aMin ? aMin : value;
            }


            /// <summary>
            /// Attempts to clamp a euler rotation value.
            /// </summary>
            /// <param name="angle"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public static float ClampAngle(float angle, float from, float to)
            {
                if (angle < 0f)
                {
                    angle = 360f + angle;
                }
                if (angle > 180f)
                {
                    return Mathf.Max(angle, 360f + from);
                }
                return Mathf.Min(angle, to);
            }

            /// <summary>
            /// Attempts to clamp a euler rotation value.
            /// </summary>
            /// <param name="angle"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static float ClampAngle(float angle, FloatRange range)
            {
                if (angle < 0f)
                {
                    angle = 360f + angle;
                }
                if (angle > 180f)
                {
                    return Mathf.Max(angle, 360f + range.Min);
                }
                return Mathf.Min(angle, range.Max);
            }

            /// <summary>
            /// Cuts a float to digits length.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="digits"></param>
            /// <returns></returns>
            public static float Truncate(float value, int digits)
            {
                double num = Math.Pow(10.0, digits);
                double num2 = Math.Truncate(num * value) / num;
                return (float)num2;
            }

            /// <summary>
            /// Rounds a float value.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="favorLower"></param>
            /// <returns></returns>
            public static int RoundValue(float value, bool favorLower)
            {
                float num = value - (float)(int)value;
                if ((num <= 0.5f && favorLower) || (num < 0.5f && !favorLower))
                {
                    return (int)(value - num);
                }
                return (int)(1f - num + value);
            }

            /// <summary>
            /// Loops a value so it never goes below 0, and never above length.
            /// </summary>
            /// <param name="t"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public static float Repeat(float t, float length)
            {
                return Clamp(t - Mathf.Floor(t / length) * length, 0f, length);
            }

            /// <summary>
            /// Loops a value so it never goes below 0, and never above length.
            /// </summary>
            /// <param name="t"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public static int Repeat(int t, int length)
            {
                return Clamp(t - Mathf.FloorToInt(t / length) * length, 0, length);
            }

            /// <summary>
            /// Returns a value that will increment and decrement between 0 and length.
            /// </summary>
            /// <param name="t"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public static float PingPong(float t, float length)
            {
                t = Repeat(t, length * 2f);
                return length - Mathf.Abs(t - length);
            }

            /// <summary>
            /// Returns a value that will increment and decrement between 0 and length.
            /// </summary>
            /// <param name="t"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public static int PingPong(int t, int length)
            {
                t = Repeat(t, length * 2);
                return length - Mathf.Abs(t - length);
            }

            /// <summary>
            /// If value given is negative, it will be turned into it's positive value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static int ToPositive(int value)
                => Math.Abs(value);

            /// <summary>
            /// If value given is positive, it will be turned into it's negative value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static int ToNegative(int value)
            => -Math.Abs(value);

            /// <summary>
            /// If value given is negative, it will be turned into it's positive value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static float ToPositive(float value)
            => Math.Abs(value);

            /// <summary>
            /// If value given is positive, it will be turned into it's negative value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static float ToNegative(float value)
            => -Math.Abs(value);

            /// <summary>
            /// If value given is negative, it will be turned into it's positive value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ToPositive(long value)
            => Math.Abs(value);

            /// <summary>
            /// If value given is positive, it will be turned into it's negative value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ToNegative(long value)
            => -Math.Abs(value);

            /// <summary>
            /// Clamps the given value between range.Min and range.Max.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static float Clamp(float value, FloatRange range)
            => Clamp(value, range.Min, range.Max);

            /// <summary>
            /// Clamps the given value between range.Min and range.Max.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static int Clamp(int value, IntRange range)
                => Clamp(value, range.Min, range.Max);

            
            /// <summary>
            /// Returns true if a float is NaN, Infinity or Negative Infinity.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsFormal(float value)
            {
                return float.IsNaN(value) || float.IsInfinity(value) || float.IsNegativeInfinity(value);
            }

            /// <summary>
            /// Returns true if numberToApproximate is not lower than or higer than numberToCompare using approximation as range.
            /// </summary>
            /// <param name="NumberToApproximate"></param>
            /// <param name="NumberToCompare"></param>
            /// <param name="approximation"></param>
            /// <returns></returns>
            public static bool IsApproximate(float NumberToApproximate, float NumberToCompare, float approximation)
            {
                bool result = false;
                float num = (NumberToApproximate - NumberToCompare).ToPositive();
                if (num < approximation)
                {
                    result = true;
                }
                return result;
            }

            /// <summary>
            /// Returns a mid-point between two values.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="percent"></param>
            /// <returns></returns>
            public static float MiddleMan(float a, float b, float percent = 0.5f)
            {
                return (b + a) * percent;
            }

            /// <summary>
            /// Returns true if the value given is not higher than range.Max nor lower than range.Min.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static bool IsWithinRange(float value, FloatRange range)
                => value < range.Max && value > range.Min;

            /// <summary>
            /// Returns true if the value given is not higher than range.Max nor lower than range.Min.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static bool IsWithinRange(int value, IntRange range)
                => value < range.Max && value > range.Min;
            #endregion
        }

        /// <summary>
        /// Methods used for fun; They provide no actual utility.
        /// </summary>
        public static class Fun
        {
            /// <summary>
            /// Returns a metric value into Bald eagle per obese child.
            /// </summary>
            /// <param name="meters"></param>
            /// <returns></returns>
            public static float MeterToBaldEaglePerObeseChild(float meters)
            {
                //height is 130cm for an average child,
                //to reach obese BMI (30) a 130cm child would need to be 78kg;
                //an average waist of 152 cm (diameter of 48.38cm, aka 152/π)
                //of which volume is 483.8x 130y 483.8z = .3042 m³ (3.042 if x10)
                //
                //bald eagle's length is 86 (median of 70–102) and
                //a weight of 4.65kg (median of 3-6.3kg),
                //and because we're comparing eagles to obese children:
                //78 / 4.65 = 16.77
                //so diameter of an eagle would be 48.38 / 16.77 = 2.88cm 
                //(28.8cm if x10, otherwise it's a tiny eagle lol, reality can be whatever I want it to be).
                //Volume is 28.8x 86y 28.8z = 0.071m³
                //so, (0.071 + 3.042 / 2) / 3 = 0.578.
                return 0.578f * meters;
            }

            /// <summary>
            /// Freezes the program by creating an infinite for loop.
            /// </summary>
            public static void Freeze()
            {
                try
                {
                    if (!IsOnMainThread())
                        throw new Exception();

                    for(int i = 0; i < 1; i--)
                    {
                        AdvancedDebug.LogError("*Evil laugh*", DEBUG_LAYER_WWW_INDEX);
                    }
                }
                catch
                {
                    AdvancedDebug.LogError("Cannot use Freeze() when not on main thread!", DEBUG_LAYER_EXCEPTIONS_INDEX);
                }
            }

            /// <summary>
            /// Encrypts a local string value with a completely random encryption key.
            /// </summary>
            /// <param name="original"></param>
            public static void EncryptWithRandomKey(ref string original)
            {
                int size = 60;
                char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size * 4];
                using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                { crypto.GetBytes(data); }
                StringBuilder key = new StringBuilder(size);
                for(int i = 0; i < size; i++)
                {
                    uint rand = BitConverter.ToUInt32(data, i * 4);
                    long index = rand % chars.Length;

                    key.Append(chars[index]);
                }
                original = RijndaelEncryption.Encrypt(original, key.ToString());
            }

            /// <summary>
            /// Causes a stack overflow.
            /// </summary>
            /// <param name="cause">Only causes a stack overflow if true.</param>
            /// <returns></returns>
            public static bool CauseStackOverflow(bool cause) => cause ? CauseStackOverflow(cause) : cause;
            
            /// <summary>
            /// Overloads the first active Unity scene with random objects, 
            /// changes the position of original ones, might delete their components, or them.
            /// If extremity is over 50, it additionally causes a stack overflow per GameObject created...
            /// Because science!
            /// </summary>
            /// <param name="extremity">How "Intense" should the stress test be. 0 is nothing, 100 is massacre.</param>
            /// <param name="handler"></param>
            public static void UnityGameStressTest(float extremity, MonoBehaviour handler)
            {
                if (extremity <= 0)
                    return;

                handler.StartCoroutine(IUnityGameStressTest(extremity));
            }

            private static IEnumerator IUnityGameStressTest(float extremity)
            {
                extremity = Mathf.Clamp(extremity, 0f, 100f);
                int amount = (int)(100f * extremity);
                Transform parent = new GameObject("Tester").transform;
                for (int i = 0; i < amount; i++)
                {
                    GameObject g = new GameObject($"Stress tester {i}");
                    for (int j = 0; j < UnityEngine.Random.Range(2, 10 * extremity); j++)
                    {
                        g.AddComponent(GetRandomComponent());
                    }
                    CauseStackOverflow(extremity > 50);
                    Array.ForEach(GameObject.FindObjectsOfType<GameObject>(),
                        go =>
                        {
                            go.transform.position = Random.Range(-Vector3.one * 1000, Vector3.one * 1000);
                            go.transform.eulerAngles = Random.Range(-Vector3.zero, Vector3.one * 360);
                        });

                    GameObject toDelete = (UnityEngine.Random.Range(0f, 100f)) <= extremity ? GameObject.FindObjectOfType<GameObject>() : null;
                    if(toDelete) GameObject.Destroy(toDelete);

                    yield return null;
                }
            }

            /// <summary>
            /// Gets a random Unity component type.
            /// </summary>
            /// <returns></returns>
            public static Type GetRandomComponent()
            {
                Assembly assembly = typeof(Component).GetTypeInfo().Assembly;
                string nameSpace = "UnityEngine";
                Type[] types = assembly.GetTypes()
                .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
                return types[UnityEngine.Random.Range(0, types.Length)];
            }
        }

        /// <summary>
        /// Rendering, view and matrices.
        /// </summary>
        public static class Rendering
        {
            /// <summary>
            /// Returns LayerMask int value of all layers. Useful for Raycasting.
            /// </summary>
            /// <param name="layers"></param>
            /// <returns></returns>
            public static int GetMask(int[] layers)
            {
                string[] array = new string[layers.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = SortingLayer.IDToName(layers[i]);
                }
                return LayerMask.GetMask(array);
            }

            /// <summary>
            /// Converts given maskname into it's layer number.
            /// </summary>
            /// <param name="layer"></param>
            /// <returns></returns>
            public static int LayerNameToIndex(string layer)
                => MaskToLayer(LayerMask.NameToLayer(layer));

            /// <summary> 
            /// Converts given bitmask to it's layer number. 
            /// </summary>
            /// <returns></returns>
            public static int MaskToLayer(int bitmask)
            {
                int result = bitmask > 0 ? 0 : 31;
                while (bitmask > 1)
                {
                    bitmask >>= 1;
                    result++;
                }
                return result;
            }

            /// <summary>
            /// Returns true if a renderer is visible from a Camera's view.
            /// </summary>
            /// <param name="renderer"></param>
            /// <param name="camera"></param>
            /// <returns></returns>
            public static bool IsVisibleFrom(Renderer renderer, Camera camera)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
                return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
            }

            /// <summary>
            /// Returns how many anchored positions are between 0 and 1.
            /// </summary>
            /// <returns></returns>
            /// <param name="rect"></param>
            private static int GetVisibleCorners(RectTransform rect)
            {
                int toReturn = 0;

                Vector4 rectAnchors = rect.GetAnchoredPosition();

                FloatRange range = new FloatRange(0, 1);

                for(int i = 0; i < 4; i++)
                {
                    if (range.IsWithinRange(rectAnchors[i]))
                        toReturn++;
                }

                return toReturn;
            }

            /// <summary>
            /// Returns true if the given <see cref="RectTransform"/> is fully visible in anchored position bounds.
            /// </summary>
            /// <returns></returns>
            /// <param name="rectTransform"></param>
            public static bool IsFullyVisible(RectTransform rectTransform)
            {
                return GetVisibleCorners(rectTransform) == 4;
            }

            /// <summary>
            /// Determines if this <see cref="RectTransform"/> is visible to the camera.
            /// </summary>
            /// <returns></returns>
            /// <param name="rectTransform"></param>
            public static bool IsVisible(RectTransform rectTransform)
            {
                return GetVisibleCorners(rectTransform) > 0;
            }

            /// <summary>
            /// Retuns an array of layer names based on ids given.
            /// </summary>
            /// <param name="ids"></param>
            /// <returns></returns>
            public static string[] LayerIdsToNames(int[] ids)
            {
                string[] array = new string[ids.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = LayerMask.LayerToName(ids[i]);
                }
                return array;
            }
        }

        /// <summary>
        /// Contains all Regex, string and StringBuilder utilities.
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// A regex expression used to match a given string as an acceptable interface name. (Used for files, as .cs is also counted as true)
            /// </summary>
            public static readonly Regex Is_InterfaceFile_Name = new Regex("^I[A-Z].+$");

            /// <summary>
            /// A regex expression used to see if a given string is an acceptable interface name. (Used for full names only, .cs and other extensions is counted as invalid)
            /// </summary>
            public static readonly Regex Is_Interface_Name = new Regex("^I[A-Z]([a-zA-Z0-9_]||_)+$");

            /// <summary>
            /// Returns a string which is wrapped in rich-text to make it colored.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="color"></param>
            /// <returns></returns>
            public static string GetRichColoredText(string original, Color color)
            {
                return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{original}</color>";
            }

            /// <summary>
            /// Puts a string value into a rainbow color using Unity's Rich Text format.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="frequency"></param>
            /// <param name="colorsToUse"></param>
            /// <returns></returns>
            public static string ToRainbow(string original, int frequency, Color[] colorsToUse)
            {
                if (frequency <= 0)
                    throw new ArgumentOutOfRangeException("Cannot use Hooks.Text.ToRainbow(string, int, Color[]) when frequency is 0 or less.");
                if (colorsToUse is null || colorsToUse.Length < 2)
                    throw new ArgumentOutOfRangeException("Cannot use Hooks.Text.ToRainbow(string, int, Color[]) when colorsToUse is null or at length lesser than 2.");

                string text = "<color=#klrtgiv>";
                string str = "</color>";
                char[] array = original.ToCharArray();
                string[] array2 = new string[array.Length];
                for (int i = 0; i < array.Length; i += frequency)
                {
                    int num = Mathf.Clamp(i + frequency, 0, array.Length);
                    for (int j = i; j < num; j++)
                    {
                        array2[j] = text.Replace("klrtgiv", Colors.ColorToHex(colorsToUse[MathF.Repeat(i, colorsToUse.Length - 1)])) + array[j].ToString() + str;
                    }
                }

                string empty = string.Join(string.Empty, array2);
                return empty;
            }

            /// <summary>
            /// Puts a string value into a rainbow color using Unity's Rich Text format.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="frequency"></param>
            /// <param name="colorsToUse"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static string ToRainbow(string original, int frequency, Color[] colorsToUse, int offset)
            {
                string text = "<color=#klrtgiv>";
                string str = "</color>";
                char[] array = original.ToCharArray();
                string[] array2 = new string[array.Length];
                for (int i = 0; i < array.Length; i += frequency)
                {
                    int num = Mathf.Clamp(i + frequency, 0, array.Length);
                    for (int j = i; j < num; j++)
                    {
                        array2[j] = text.Replace("klrtgiv", Colors.ColorToHex(colorsToUse[MathF.Repeat(i + offset, colorsToUse.Length - 1)])) + array[j].ToString() + str;
                    }
                }

                string empty = string.Join(string.Empty, array2);
                return empty;
            }

            /// <summary>
            /// Finds the distance between two string in similarity.
            /// </summary>
            /// <param name="first"></param>
            /// <param name="second"></param>
            /// <returns></returns>
            public static int LevenshteinDistance(string first, string second)
            {
                int n = first.Length;
                int m = second.Length;
                int[,] d = new int[n + 1, m + 1];
                if (n == 0)
                {
                    return m;
                }
                if (m == 0)
                {
                    return n;
                }

                for (int i = 0; i <= n; d[i, 0] = i++);
                for (int j = 0; j <= m; d[0, j] = j++);

                for (int i = 1; i <= n; i++)
                {
                    for (int j = 1; j <= m; j++)
                    {
                        int cost = (second[j - 1] == first[i - 1]) ? 0 : 1;
                        d[i, j] = Math.Min(
                            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                            d[i - 1, j - 1] + cost);
                    }
                }
                return d[n, m];
            }

            /// <summary>
            /// Returns the hex string for the color used by the console.
            /// </summary>
            /// <param name="messageType"></param>
            /// <returns></returns>
            public static string GetConsoleHexColor(MessageType messageType)
            {
                Color toUse = messageType switch
                {
                    MessageType.Warning => Settings.DebugWarningColor,
                    MessageType.Error => Settings.DebugErrorColor,
                    _ => Settings.DebugInfoColor,
                };
                return ColorUtility.ToHtmlStringRGB(toUse);
            }

            /// <summary>
            /// Wraps original between startWrapper and endWrapper.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="startWrapper"></param>
            /// <param name="endWrapper"></param>
            /// <returns></returns>
            public static string StringWrapper(string original, char startWrapper, char endWrapper)
            {
                StringBuilder stringBuilder = new StringBuilder(3);
                stringBuilder.Insert(0, startWrapper);
                stringBuilder.Insert(1, original);
                stringBuilder.Insert(original.Length + 1, endWrapper);
                return stringBuilder.ToString();
            }

            /// <summary>
            /// Wraps original between startWrapper and endWrapper.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="startWrapper"></param>
            /// <param name="endWrapper"></param>
            /// <returns></returns>
            public static string StringWrapper(string original, string startWrapper, string endWrapper)
            {
                StringBuilder stringBuilder = new StringBuilder(3);
                return stringBuilder
                    .Append(startWrapper)
                    .Append(original)
                    .Append(endWrapper)
                    .ToString();
            }

            /// <summary>
            /// Cuts out a string between from and to.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public static string Cutout(string original, string from, string to)
            {
                int pFrom = original.IndexOf(from) + from.Length;
                int pTo = original.LastIndexOf(to);
                return original.Substring(pFrom, pTo - pFrom);
            }

            /// <summary>
            /// Returns true if the given string contains a Kanji character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsKanji(string value)
                => WWWResources.Expression_Kanji.IsMatch(value);

            /// <summary>
            /// Returns true if the given string contains a Hiragana character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsHiragana(string value)
                => WWWResources.Expression_Hiragana.IsMatch(value);

            /// <summary>
            /// Returns true if the given string contains a Katakana character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsKatakana(string value)
               => WWWResources.Expression_Katakana.IsMatch(value);

            /// <summary>
            /// Returns true if the given string contains either a Kanji, Hiragana or Katakana character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsJapanese(string value)
                => WWWResources.Expression_Japanese_Greedy.IsMatch(value);

            /// <summary>
            /// Cuts out a string between from and to in char values.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public static string Cutout(string original, char from, char to)
            {
                int pFrom = original.IndexOf(from) + 1;
                int pTo = original.LastIndexOf(to);

                return original.Substring(pFrom, pTo - pFrom);
            }
        }

        /// <summary>
        /// Returns MethodInfo of all public methods inside <see cref="Hooks"/>.
        /// </summary>
        public static IEnumerable<MethodInfo> HooksMethods = typeof(Hooks).GetMethods();

        /// <summary>
        /// Returns <see cref="MethodInfo"/> of all public methods inside <see cref="Hooks"/> and all of it's nested classes.
        /// </summary>
        public static List<MethodInfo> FullMethods
        {
            get
            {
                List<MethodInfo> toReturn = new List<MethodInfo>(typeof(Hooks).GetMethods());

                Type[] types = typeof(Hooks).GetNestedTypes();
                for(int i = 0; i < types.Length; i++)
                {
                    toReturn.AddRange(types[i].GetMethods());
                }

                return toReturn;
            }
        }

        /// <summary>
        /// Tries to parse a type with <see cref="Type.GetType(string)"/>; 
        /// if it fails, it then tries to itterate through every Assembly inside <see cref="AppDomain.CurrentDomain"/> to parse the given typeName.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type ParseType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }

            return null;
        }

        /// <summary>
        /// Itterates through each string, and uses <see cref="ParseType(string)"/>.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static List<Type> ParseTypes(IEnumerable<string> types)
        {
            List<Type> toReturn = new List<Type>(types.Count());
            foreach (string s in types)
                toReturn.Add(ParseType(s));

            return toReturn;
        }

        /// <summary>
        /// Returns true if type is implementing implementation.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public static bool Implements(Type type, Type implementation)
            => implementation.IsAssignableFrom(type);

        /// <summary>
        /// Returns true if type has a generic implementation.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericImplementation"></param>
        /// <returns></returns>
        public static bool ImplementsGeneric(Type type, Type genericImplementation)
            => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericImplementation);

        /// <summary>
        /// Tries to parse an Enum, if not successful, it returns null instead.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T? ParseNullable<T>(string input) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Generic Type 'T' must be an Enum");
            }
            if (!string.IsNullOrEmpty(input) && Enum.GetNames(typeof(T)).Any((string e) => e.Trim().ToUpperInvariant() == input.Trim().ToUpperInvariant()))
            {
                return (T)Enum.Parse(typeof(T), input, ignoreCase: true);
            }
            return null;
        }

        /// <summary>
        /// Equivalent to <see cref="Enum.Parse(Type, string, bool)"/>, with the painful parts taken care of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T Parse<T>(string input) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), input, ignoreCase: true);
        }

        /// <summary>
        /// Returns the first keycode pressed in the same frame as <see cref="GetKeyStroke(int)"/> was called in.
        /// </summary>
        /// <param name="type">0 = <see cref="Input.GetKey(KeyCode)"/>; 1 = <see cref="Input.GetKeyDown(KeyCode)"/>; 2 = <see cref="Input.GetKeyUp(KeyCode)"/></param>
        /// <returns></returns>
        public static KeyCode GetKeyStroke(int type)
        {
            foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
            {
                if ((type == 0 && Input.GetKey(value)) || (type == 1 && Input.GetKeyDown(value)) || (type == 2 && Input.GetKeyUp(value)))
                {
                    return value;
                }
            }
            return KeyCode.None;
        }

        /// <summary>
        /// Checks if the current thread is the main thread.
        /// </summary>
        public static bool IsOnMainThread()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA &&
                !Thread.CurrentThread.IsBackground && !Thread.CurrentThread.IsThreadPoolThread && Thread.CurrentThread.IsAlive)
            {
                MethodInfo correctEntryMethod = Assembly.GetEntryAssembly().EntryPoint;
                StackTrace trace = new StackTrace();
                StackFrame[] frames = trace.GetFrames();
                for (int i = frames.Length - 1; i >= 0; i--)
                {
                    MethodBase method = frames[i].GetMethod();
                    if (correctEntryMethod == method)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Equivalent to <see cref="float.Parse(string, IFormatProvider)"/> where <see cref="IFormatProvider"/>
        /// is <see cref="CultureInfo"/>.InvariantCulture.NumberFormat.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        /// <returns></returns>
        public static float ToSingle(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Destroys a Unity Object using Object.Destroy().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDestroy"></param>
        /// <exception cref="UnityException"/>
        public static void DestroyObject<T>(T objectToDestroy) where T : UnityEngine.Object
        {
            UnityEngine.Object obj = objectToDestroy;
            UnityEngine.Object.Destroy(obj);
        }

        /// <summary>
        /// Adds an event to a given trigger.
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="eventType"></param>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="UnityException"/>
        /// <exception cref="ArgumentException"/>
        public static void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, Action<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(callback.Invoke);
            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// Returns the parent of the collider. If collider is already the parent, it returns the collider's GameObject instead.
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public static GameObject GetParent(Collider2D collider)
        {
            try
            {
                GameObject result = collider.gameObject;
                if (collider.transform.parent != null)
                {
                    result = collider.transform.parent.gameObject;
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Populates a list with all children of a parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="list"></param>
        public static void GetAllChildren(Transform parent, ref List<Transform> list)
        {
            foreach (Transform child in parent)
            {
                list.Add(child);
                GetAllChildren(child, ref list);
            }
        }

        /// <summary>
        /// Populates a list with all children of a parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="list"></param>
        public static void GetAllChildren(Transform parent, ref List<GameObject> list)
        {
            foreach (Transform child in parent)
            {
                list.Add(child.gameObject);
                GetAllChildren(child, ref list);
            }
        }

        /// <summary>
        /// Gets all children of a parent who match a specified condition.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static List<Transform> GetChildren(Transform parent, Predicate<Transform> match)
        {
            List<Transform> toReturn = new List<Transform>();
            List<Transform> used = new List<Transform>(parent.childCount);

            GetAllChildren(parent, ref used);

            foreach (Transform transform in used)
            {
                if (match(transform))
                {
                    toReturn.Add(transform);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all children of a parent who match a specified condition.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static List<GameObject> GetChildren(Transform parent, Predicate<GameObject> match)
        {
            List<GameObject> toReturn = new List<GameObject>();
            List<Transform> used = new List<Transform>(parent.childCount);

            GetAllChildren(parent, ref used);

            foreach (Transform transform in used)
            {
                if (match(transform.gameObject))
                {
                    toReturn.Add(transform.gameObject);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets the oldest parent inside the hierarchy.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public static Transform GetOldestParent(Transform child)
        {
            try
            {
                Transform toReturn = child;
                while (toReturn.parent != null)
                {
                    toReturn = toReturn.parent;
                }

                return toReturn;
            }
            catch
            {
                return child;
            }
        }

        /// <summary>
        /// Returns the oldest parent in the hierarchy of the given object.
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static GameObject TrueParent(GameObject @object)
        {
            try
            {
                GameObject last = @object;
                while (last.transform.parent is not null)
                {
                    last = last.transform.parent.gameObject;
                }

                return last;
            }
            catch
            {
                return @object;
            }
        }

        #region Clipboard Copying
#pragma warning disable CS1591
        public enum ResultCode
        {
            Success,
            ErrorOpenClipboard,
            ErrorGlobalAlloc,
            ErrorGlobalLock,
            ErrorSetClipboardData,
            ErrorOutOfMemoryException,
            ErrorArgumentOutOfRangeException,
            ErrorException,
            ErrorInvalidArgs,
            ErrorGetLastError
        }

        public class Result
        {
            public ResultCode ResultCode
            {
                get;
                set;
            }

            public uint LastError
            {
                get;
                set;
            }

            public bool OK => ResultCode == ResultCode.Success;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [STAThread]
        public static Result PushStringToClipboard(string message)
        {
            if (message != null && message == Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(message)))
            {
                return PushUnicodeStringToClipboard(message);
            }
            return PushAnsiStringToClipboard(message);
        }

        [STAThread]
        public static Result PushUnicodeStringToClipboard(string message)
        {
            return __PushStringToClipboard(message, 13u);
        }

        [STAThread]
        public static Result PushAnsiStringToClipboard(string message)
        {
            return __PushStringToClipboard(message, 1u);
        }

        [STAThread]
        private static Result __PushStringToClipboard(string message, uint format)
        {
            try
            {
                try
                {
                    if (message == null)
                    {
                        return new Result
                        {
                            ResultCode = ResultCode.ErrorInvalidArgs
                        };
                    }
                    if (!OpenClipboard(IntPtr.Zero))
                    {
                        return new Result
                        {
                            ResultCode = ResultCode.ErrorOpenClipboard,
                            LastError = GetLastError()
                        };
                    }
                    try
                    {
                        uint num;
                        switch (format)
                        {
                            case 1u:
                                num = 1u;
                                break;
                            case 13u:
                                num = 2u;
                                break;
                            default:
                                throw new Exception("Not Reachable");
                        }
                        uint length = (uint)message.Length;
                        uint num2 = (length + 1) * num;
                        IntPtr intPtr = GlobalAlloc(66u, (UIntPtr)num2);
                        if (intPtr == IntPtr.Zero)
                        {
                            return new Result
                            {
                                ResultCode = ResultCode.ErrorGlobalAlloc,
                                LastError = GetLastError()
                            };
                        }
                        try
                        {
                            IntPtr intPtr2;
                            switch (format)
                            {
                                case 1u:
                                    intPtr2 = Marshal.StringToHGlobalAnsi(message);
                                    break;
                                case 13u:
                                    intPtr2 = Marshal.StringToHGlobalUni(message);
                                    break;
                                default:
                                    throw new Exception("Not Reachable");
                            }
                            try
                            {
                                IntPtr intPtr3 = GlobalLock(intPtr);
                                if (intPtr3 == IntPtr.Zero)
                                {
                                    return new Result
                                    {
                                        ResultCode = ResultCode.ErrorGlobalLock,
                                        LastError = GetLastError()
                                    };
                                }
                                try
                                {
                                    CopyMemory(intPtr3, intPtr2, num2);
                                }
                                finally
                                {
                                    GlobalUnlock(intPtr3);
                                }
                                if (SetClipboardData(format, intPtr).ToInt64() == 0)
                                {
                                    return new Result
                                    {
                                        ResultCode = ResultCode.ErrorSetClipboardData,
                                        LastError = GetLastError()
                                    };
                                }
                                intPtr = IntPtr.Zero;
                            }
                            finally
                            {
                                Marshal.FreeHGlobal(intPtr2);
                            }
                        }
                        catch (OutOfMemoryException)
                        {
                            return new Result
                            {
                                ResultCode = ResultCode.ErrorOutOfMemoryException,
                                LastError = GetLastError()
                            };
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            return new Result
                            {
                                ResultCode = ResultCode.ErrorArgumentOutOfRangeException,
                                LastError = GetLastError()
                            };
                        }
                        finally
                        {
                            if (intPtr != IntPtr.Zero)
                            {
                                GlobalFree(intPtr);
                            }
                        }
                    }
                    finally
                    {
                        CloseClipboard();
                    }
                    return new Result
                    {
                        ResultCode = ResultCode.Success
                    };
                }
                catch (Exception)
                {
                    return new Result
                    {
                        ResultCode = ResultCode.ErrorException,
                        LastError = GetLastError()
                    };
                }
            }
            catch (Exception)
            {
                return new Result
                {
                    ResultCode = ResultCode.ErrorGetLastError
                };
            }
        }
#pragma warning restore CS1591
        #endregion
        /// <summary>
        /// Sets the text/title of an application widow. Argument hwnd is a window, 
        /// which can be found using <see cref="FindWindow(string, string)"/> or
        /// <see cref="GetActiveWindow"/>.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lp"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern bool SetWindowText(IntPtr hwnd, string lp);
        /// <summary>
        /// Returns all info on a window using <see cref="IntPtr"/>; See https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-findwindowa
        /// for more info.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="windowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string className, string windowName);

        /// <summary>
        /// Returns the currently active window.
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetActiveWindow")]
        public static extern IntPtr GetActiveWindow();

        /// <summary>
        /// Sets the application's title to a given string.
        /// </summary>
        /// <param name="to"></param>
        public static void SetWindowText(string to)
        {
            SetWindowText(GetActiveWindow(), to);
        }

        /// <summary>
        /// Returns the parent gameobject of the given <see cref="GameObject"/>.
        /// </summary>
        /// <param name="Gobj"></param>
        /// <returns></returns>
        public static GameObject ObjectParent(GameObject Gobj)
        {
            GameObject result = Gobj;
            if (Gobj.transform.parent != null)
            {
                result = Gobj.transform.parent.gameObject;
            }
            return result;
        }

        /// <summary>
        /// Returns a sprite from a Unity multisprite spritesheet; 
        /// SpriteSheetName is the name of the file, spriteName is the name of the sprite itself. 
        /// (sprite file MUST be in a resources folder)
        /// </summary>
        /// <param name="SpriteSheetName"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite GetSpriteFromSpriteSheet(string SpriteSheetName, string spriteName)
        {
            Sprite[] source = Resources.LoadAll<Sprite>(SpriteSheetName);
            return (from Sprite s in source where s.name == spriteName select s).First();
        }

        /// <summary>
        /// Returns true if type is subclass of baseType.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsSubclassOf(Type type, Type baseType)
        {
            if (type == null || baseType == null || type == baseType)
            {
                return false;
            }
            if (!baseType.IsGenericType)
            {
                if (!type.IsGenericType)
                {
                    return type.IsSubclassOf(baseType);
                }
            }
            else
            {
                baseType = baseType.GetGenericTypeDefinition();
            }
            type = type.BaseType;
            Type typeFromHandle = typeof(object);
            while (type != typeFromHandle && type != null)
            {
                Type left = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (left == baseType)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Converts a given int value into an Enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <returns></returns>
        public static T ConvertEnum<T>(int i) where T : struct, IConvertible
        {
            return (T)(object)i;
        }
    }
}