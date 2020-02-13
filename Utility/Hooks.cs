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
using UnityEngine.UI;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Internal;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A class which contains 20+Gadzillion-billion-yes methods for various utilities.
    /// </summary>
    public static class Hooks
    {
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
            public static Vector3 RandomVector01 => new Vector3(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1));

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
            /// Deprecated. Use <see cref="RandomItem{T}(IEnumerable{T})"/> instead.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="objectList"></param>
            /// <returns></returns>
            [Obsolete("Deprecated. Use RandomItem<T>(IEnumerable<T> List) instead.", true)]
            public static T RandomObject<T>(ICollection<T> objectList)
            {
                int count = objectList.Count;
                if (count == 0)
                {
                    return default(T);
                }
                int num = UnityEngine.Random.Range(0, count);
                return (T)objectList.ToArray()[num];
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
            /// Returns a <see cref="Vector2"/> with each of it's values being a random number between min and max.
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static Vector2 Range(Vector2 min, Vector2 max)
            {
                return new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
            }
        }

        /// <summary>
        /// Subclass with all streaming and saving/loading methods.
        /// </summary>
        public static class Streaming
        {
            internal const char STREAM_CATEGORY_WRAPPER_START = '[';
            internal const char STREAM_CATEGORY_WRAPPER_END = ']';
            internal const char STREAM_CATEGORY_END = '/';
            /// <summary>
            /// Value which separates the name from the value; Only has one entry at index 0.
            /// Example: ValueName IS value; In here, " IS " is the separator.
            /// </summary>
            public static readonly string[] STREAM_VALUE_POINTER = new string[] { " IS " };

            private static string DefaultPassword = "MereoleonaBestGrill";
            private static string DefaultPath = string.Empty;
            /// <summary>
            /// A series of string values used to save/load from <see cref="Streaming"/>.
            /// </summary>
            public struct Catalog
            {
                internal enum CatalogType
                {
                    INVALID,
                    LOADER,
                    SAVER
                }

                /// <summary>
                /// File path towards which this catalog will be saved.
                /// </summary>
                public string Path;

                /// <summary>
                /// Category in which this catalog will be stored.
                /// </summary>
                public string Category;
                /// <summary>
                /// Name of the variable saved by this catalog.
                /// </summary>
                public string Name;
                /// <summary>
                /// Value in string of the saved catalog.
                /// </summary>
                public string Value;

                /// <summary>
                /// If true, this catalog's value will be used as DefaultValue.
                /// </summary>
                public bool UsesDefaultValue;

                /// <summary>
                /// Password used for encryption. NOTE: VERY heavy, use only on high-end pc's or small save sections.
                /// </summary>
                public string Password;

                internal CatalogType Type;

                /// <summary>
                /// Determines if this category is protected by a password.
                /// </summary>
                public bool Protected => !string.IsNullOrEmpty(Password);

                /// <summary>
                /// Encrypts a string value with this catalog's password.
                /// </summary>
                /// <param name="input"></param>
                /// <returns></returns>
                public string Encrypt(string input)
                    => RijndaelEncryption.Encrypt(input, Password, CipherMode.CBC, PaddingMode.PKCS7);

                /// <summary>
                /// Decrypts a string value with this catalog's password.
                /// </summary>
                /// <param name="input"></param>
                /// <returns></returns>
                public string Decrypt(string input)
                    => RijndaelEncryption.Decrypt(input, Password, CipherMode.CBC, PaddingMode.PKCS7);

                /// <summary>
                /// Creates a <see cref="Catalog"/> using the default password and path.
                /// </summary>
                /// <param name="category"></param>
                /// <param name="name"></param>
                /// <param name="value"></param>
                /// <returns></returns>
                public static Catalog DefaultSaver(string category, string name, string value)
                {
                    if (DefaultPath.Length == 0)
                        throw new StreamingException(StreamingResult.DEFAULT_PATH_NULL);

                    if (category == null || name == null || value == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    return new Catalog()
                    {
                        Path = DefaultPath,
                        Category = category,
                        Name = name,
                        Value = value,
                        Password = DefaultPassword,
                        UsesDefaultValue = false,
                        Type = CatalogType.SAVER
                    };
                }

                /// <summary>
                /// Creates a <see cref="Catalog"/> used to save a value.
                /// </summary>
                /// <param name="path"></param>
                /// <param name="category"></param>
                /// <param name="name"></param>
                /// <param name="value"></param>
                /// <returns></returns>
                public static Catalog Saver(string path, string category, string name, string value)
                {
                    if (path == null || category == null || name == null
                        || value == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    return new Catalog()
                    {
                        Path = path,
                        Category = category,
                        Name = name,
                        Value = value,
                        Password = null,
                        UsesDefaultValue = false,
                        Type = CatalogType.SAVER
                    };
                }

                /// <summary>
                /// Creates a <see cref="Catalog"/> used to save a value.
                /// </summary>
                /// <param name="path"></param>
                /// <param name="category"></param>
                /// <param name="name"></param>
                /// <param name="value"></param>
                /// <param name="password"></param>
                /// <returns></returns>
                public static Catalog Saver(string path, string category, string name, string value, string password)
                {
                    if (path == null || category == null || name == null
                        || value == null || password == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    return new Catalog()
                    {
                        Path = path,
                        Category = category,
                        Name = name,
                        Value = value,
                        Password = password,
                        UsesDefaultValue = false,
                        Type = CatalogType.SAVER
                    };
                }

                /// <summary>
                /// Creates a collection of catalogs with the same path and category.
                /// </summary>
                /// <param name="path"></param>
                /// <param name="category"></param>
                /// <param name="names"></param>
                /// <param name="values"></param>
                /// <returns></returns>
                public static Catalog[] Savers(string path, string category, string[] names, string[] values)
                {
                    if (path == null || category == null || names == null || values == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    if (names.Length != values.Length)
                        throw new StreamingException(StreamingResult.INVALID_COLLECTION_SIZE);

                    Catalog[] toReturn = new Catalog[names.Length];

                    for(int i = 0; i < toReturn.Length; i++)
                    {
                        toReturn[i] = new Catalog()
                        {
                            Path = path,
                            Category = category,
                            Name = names[i],
                            Value = values[i],
                            Password = null,
                            UsesDefaultValue = false,
                            Type = CatalogType.SAVER
                        };
                    }

                    return toReturn;
                }

                /// <summary>
                /// Creates a collection of catalogs with the same path, category and password.
                /// </summary>
                /// <param name="path"></param>
                /// <param name="category"></param>
                /// <param name="names"></param>
                /// <param name="values"></param>
                /// <param name="password"></param>
                /// <returns></returns>
                public static Catalog[] Savers(string path, string category, string[] names, string[] values, string password)
                {
                    if (path == null || category == null || names == null || values == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    if (names.Length != values.Length)
                        throw new StreamingException(StreamingResult.INVALID_COLLECTION_SIZE);

                    Catalog[] toReturn = new Catalog[names.Length];

                    for (int i = 0; i < toReturn.Length; i++)
                    {
                        toReturn[i] = new Catalog()
                        {
                            Path = path,
                            Category = category,
                            Name = names[i],
                            Value = values[i],
                            Password = password,
                            UsesDefaultValue = false,
                            Type = CatalogType.SAVER
                        };
                    }

                    return toReturn;
                }

                /// <summary>
                /// Creates a <see cref="Catalog"/> to load a variable with the path and password being default.
                /// </summary>
                /// <param name="category"></param>
                /// <param name="name"></param>
                /// <returns></returns>
                public static Catalog DefaultLoader(string category, string name)
                {
                    if (DefaultPath.Length == 0)
                        throw new StreamingException(StreamingResult.DEFAULT_PATH_NULL);

                    if (category == null || name == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    return new Catalog()
                    {
                        Path = DefaultPath,
                        Category = category,
                        Name = name,
                        Value = null,
                        Password = DefaultPassword,
                        UsesDefaultValue = false,
                        Type = CatalogType.LOADER
                    };
                }

                /// <summary>
                /// To be used with <see cref="LoadAll(Catalog, bool)"/>
                /// </summary>
                /// <param name="path"></param>
                /// <param name="category"></param>
                /// <returns></returns>
                public static Catalog LoaderFull(string path, string category)
                {
                    if (path == null || category == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    return new Catalog()
                    {
                        Path = path,
                        Category = category,
                        Name = null,
                        Value = null,
                        Password = null,
                        UsesDefaultValue = false,
                        Type = CatalogType.LOADER
                    };
                }

                /// <summary>
                /// Creates a <see cref="Catalog"/> to load or remove a value.
                /// </summary>
                /// <param name="path"></param>
                /// <param name="category"></param>
                /// <param name="name"></param>
                /// <returns></returns>
                public static Catalog Loader(string path, string category, string name)
                {
                    if (path == null || category == null || name == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    return new Catalog()
                    {
                        Path = path,
                        Category = category,
                        Name = name,
                        Value = null,
                        Password = null,
                        UsesDefaultValue = false,
                        Type = CatalogType.LOADER
                    };
                }

                /// <summary>
                /// Creates a <see cref="Catalog"/> to load or remove a value.
                /// </summary>
                /// <param name="path"></param>
                /// <param name="category"></param>
                /// <param name="name"></param>
                /// <param name="defaultValue"></param>
                /// <param name="useDefaultValue"></param>
                /// <returns></returns>
                public static Catalog Loader(string path, string category, string name, string defaultValue, bool useDefaultValue)
                {
                    if (path == null || category == null || name == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    return new Catalog()
                    {
                        Path = path,
                        Category = category,
                        Name = name,
                        Value = defaultValue,
                        Password = null,
                        UsesDefaultValue = useDefaultValue,
                        Type = CatalogType.LOADER
                    };
                }

                /// <summary>
                /// Creates a <see cref="Catalog"/> to load or remove a value.
                /// </summary>
                /// <param name="path"></param>
                /// <param name="category"></param>
                /// <param name="name"></param>
                /// <param name="defaultValue"></param>
                /// <param name="useDefaultValue"></param>
                /// <param name="password"></param>
                /// <returns></returns>
                public static Catalog Loader(string path, string category, string name, string defaultValue, bool useDefaultValue, string password)
                {
                    if (path == null || category == null || name == null || password == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    return new Catalog()
                    {
                        Path = path,
                        Category = category,
                        Name = name,
                        Value = defaultValue,
                        Password = DefaultPassword,
                        UsesDefaultValue = useDefaultValue,
                        Type = CatalogType.LOADER
                    };
                }

                /// <summary>
                /// Creates a <see cref="Catalog"/> to load or remove a value.
                /// </summary>
                /// <param name="path"></param>
                /// <param name="category"></param>
                /// <param name="name"></param>
                /// <param name="password"></param>
                /// <returns></returns>
                public static Catalog Loader(string path, string category, string name, string password)
                {
                    if (path == null || category == null || name == null || password == null)
                        throw new StreamingException(StreamingResult.INVALID_ARG);

                    return new Catalog()
                    {
                        Path = path,
                        Category = category,
                        Name = name,
                        Value = null,
                        Password = DefaultPassword,
                        UsesDefaultValue = false,
                        Type = CatalogType.LOADER
                    };
                }
            }

            /// <summary>
            /// Sets the default password for use with <see cref="Catalog"/>.
            /// </summary>
            /// <param name="to"></param>
            public static void SetDefaultPassword(string to)
            {
                DefaultPassword = to;
            }

            /// <summary>
            /// Gets the current Default password.
            /// </summary>
            /// <returns></returns>
            public static string GetDefaultPassword() => DefaultPassword;

            /// <summary>
            /// Sets the default path for use with <see cref="Catalog"/>.
            /// </summary>
            /// <param name="to"></param>
            public static void SetDefaultPath(string to)
            {
                DefaultPath = to;
            }

            /// <summary>
            /// Gets the current Default path.
            /// </summary>
            /// <returns></returns>
            public static string GetDefaultPath() => DefaultPath;

            internal static string CategoryWrapper(Catalog catalog) 
                => Text.StringWrapper(catalog.Category, STREAM_CATEGORY_WRAPPER_START, STREAM_CATEGORY_WRAPPER_END);
            internal static string CategoryEndWrapper(Catalog catalog) 
                => Text.StringWrapper(
                    $"{STREAM_CATEGORY_END}{catalog.Category}", 
                    STREAM_CATEGORY_WRAPPER_START, 
                    STREAM_CATEGORY_WRAPPER_END);

            /// <summary>
            /// Returns a catalog's name with <see cref="STREAM_VALUE_POINTER"/> and it's value. (e.g: Score = 25)
            /// </summary>
            /// <param name="catalog"></param>
            /// <returns></returns>
            internal static string ValueSaver(Catalog catalog) => $"{catalog.Name}{STREAM_VALUE_POINTER[0]}{catalog.Value}";

            /// <summary>
            /// Returns a catalog's name with <see cref="STREAM_VALUE_POINTER"/>. (e.g: Score =)
            /// </summary>
            /// <param name="catalog"></param>
            /// <returns></returns>
            internal static string NameNoValSaver(Catalog catalog) => $"{catalog.Name}{STREAM_VALUE_POINTER[0]}";

            private static bool IsValidCatalog(Catalog catalog, Catalog.CatalogType type)
            {
                if (type == Catalog.CatalogType.INVALID || catalog.Type != type)
                    return false;

                return true;
            }

            internal static void CheckCategoryFileValidity(string filePath, out bool folderNull, out bool fileNull)
            {
                folderNull = false;
                fileNull = false;

                if (CreateFolder(filePath))
                {
                    AdvancedDebug.Log($"Given directory of {filePath} did not exist, creating new...", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
                    folderNull = true;
                }
                if (!File.Exists(filePath))
                {
                    AdvancedDebug.Log($"Given file under {filePath} did not exist, creating new...", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                    fs.Dispose();

                    fileNull = true;
                }
            }

            private static void InternalSave(Catalog catalog, bool fromLoad)
            {
                if (!fromLoad && !IsValidCatalog(catalog, Catalog.CatalogType.SAVER))
                    throw new StreamingException(StreamingResult.INVALID_CATALOG);

                CheckCategoryFileValidity(catalog.Path, out _, out _);

                List<string> lines = new List<string>(File.ReadAllLines(catalog.Path));
                string categoryStartName = CategoryWrapper(catalog), categoryEndName = CategoryEndWrapper(catalog);
                int startIndex = lines.FindIndex(l => (catalog.Protected ? catalog.Decrypt(l) : l) == categoryStartName);
                int endIndex = lines.FindIndex(l => (catalog.Protected ? catalog.Decrypt(l) : l) == categoryEndName);

                if (startIndex == -1 || endIndex == -1)
                {
                    startIndex = 0;
                    endIndex = startIndex + 1;
                    lines.Insert(startIndex, catalog.Protected ? catalog.Encrypt(categoryStartName) : categoryStartName);
                    lines.Insert(endIndex, catalog.Protected ? catalog.Encrypt(categoryEndName) : categoryEndName);
                }

                int writeIndex = lines.FindIndex(startIndex, endIndex - startIndex, 
                    s => (catalog.Protected ? catalog.Decrypt(s) : s).Split(STREAM_VALUE_POINTER, StringSplitOptions.None)[0].Equals(catalog.Name));
                if (writeIndex == -1)
                    lines.Insert(startIndex + 1, catalog.Protected ? catalog.Encrypt(ValueSaver(catalog)) : ValueSaver(catalog));
                else
                    lines[writeIndex] = catalog.Protected ? catalog.Encrypt(ValueSaver(catalog)) : ValueSaver(catalog);

                File.WriteAllLines(catalog.Path, lines);
            }
            
            /// <summary>
            /// Saves a value to a folder file under category.
            /// </summary>
            /// <param name="saver"></param>
            public static void Save(Catalog saver)
            {
                InternalSave(saver, false);
            }

            internal static void InternalSaveAll(Catalog[] catalogs)
            {
                if (catalogs == null || catalogs.Length < 2)
                    throw new StreamingException(StreamingResult.INVALID_CATALOG_COLLECTION_SIZE);

                Catalog reference = catalogs[0];

                for (int i = 1; i < catalogs.Length; i++)
                {
                    if (!catalogs[i].Category.Equals(reference.Category))
                        throw new StreamingException(StreamingResult.CATALOG_MISSMATCH_CATEGORY);

                    if (catalogs[i].Password != null && !catalogs[i].Password.Equals(reference.Password))
                        throw new StreamingException(StreamingResult.CATALOG_MISSMATCH_PASSWORD);

                    if (!catalogs[i].Path.Equals(reference.Path))
                        throw new StreamingException(StreamingResult.CATALOG_MISSMATCH_FILEPATH);

                    if (!IsValidCatalog(catalogs[i], Catalog.CatalogType.SAVER))
                        throw new StreamingException(StreamingResult.INVALID_CATALOG);
                }

                CheckCategoryFileValidity(reference.Path, out _, out _);

                List<string> lines = new List<string>(File.ReadAllLines(reference.Path));
                string categoryStartName = CategoryWrapper(reference), categoryEndName = CategoryEndWrapper(reference);
                int startIndex = lines.FindIndex(l => (reference.Protected ? reference.Decrypt(l) : l) == categoryStartName);
                int endIndex = lines.FindIndex(l => (reference.Protected ? reference.Decrypt(l) : l) == categoryEndName);

                if (startIndex == -1 || endIndex == -1)
                {
                    startIndex = 0;
                    endIndex = startIndex + 1;
                    lines.Insert(startIndex, reference.Protected ? reference.Encrypt(categoryStartName) : categoryStartName);
                    lines.Insert(endIndex, reference.Protected ? reference.Encrypt(categoryEndName) : categoryEndName);
                }

                for (int i = 0; i < catalogs.Length; i++)
                {
                    int writeIndex = lines.FindIndex(startIndex, endIndex - startIndex,
                        s => (catalogs[i].Protected ? catalogs[i].Decrypt(s) : s).Split(STREAM_VALUE_POINTER, StringSplitOptions.None)[0].Equals(catalogs[i].Name));
                    if (writeIndex == -1)
                        lines.Insert(startIndex + 1, catalogs[i].Protected ? catalogs[i].Encrypt(ValueSaver(catalogs[i])) : ValueSaver(catalogs[i]));
                    else
                        lines[writeIndex] = catalogs[i].Protected ? catalogs[i].Encrypt(ValueSaver(catalogs[i])) : ValueSaver(catalogs[i]);
                }

                File.WriteAllLines(reference.Path, lines);
            }

            /// <summary>
            /// Saves all given values under category file. All categories inside a catalog must be the same value.
            /// </summary>
            /// <param name="catalogs"></param>
            public static void SaveAll(Catalog[] catalogs)
            {
                InternalSaveAll(catalogs);
            }

            private static void LoadLines(out List<string> list, Catalog catalog)
            {
                list = new List<string>(File.ReadAllLines(catalog.Path));

                int start = list.FindIndex(s => (catalog.Protected ? catalog.Decrypt(s) : s) == CategoryWrapper(catalog));
                int end = list.FindIndex(s => (catalog.Protected ? catalog.Decrypt(s) : s) == CategoryEndWrapper(catalog));

                list.RemoveRange(end, list.Count - end);
                list.RemoveRange(0, start + 1);
            }

            private static void LoadLines(out List<string> list, out int start, out int end, Catalog catalog)
            {
                list = new List<string>(File.ReadAllLines(catalog.Path));

                start = list.FindIndex(s => (catalog.Protected ? catalog.Decrypt(s) : s) == CategoryWrapper(catalog));
                end = list.FindIndex(s => (catalog.Protected ? catalog.Decrypt(s) : s) == CategoryEndWrapper(catalog));
            }

            /// <summary>
            /// Attempts to reorder all values inside the given catalog's category;
            /// (Note: When comparing, the separator is included, so make sure to use <see cref="STREAM_VALUE_POINTER"/> to split the string.)
            /// </summary>
            /// <param name="catalog"></param>
            /// <param name="order"></param>
            public static void Reorder(Catalog catalog, Func<string, int> order)
            {
                if (!IsValidCatalog(catalog, Catalog.CatalogType.LOADER))
                    throw new StreamingException(StreamingResult.INVALID_CATALOG);

                try
                {
                    List<string> lines = new List<string>(File.ReadAllLines(catalog.Path));
                    string categoryStartName = CategoryWrapper(catalog), categoryEndName = CategoryEndWrapper(catalog);
                    int startIndex = lines.FindIndex(l => (catalog.Protected ? catalog.Decrypt(l) : l) == categoryStartName);
                    int endIndex = lines.FindIndex(l => (catalog.Protected ? catalog.Decrypt(l) : l) == categoryEndName);

                    if (startIndex == -1 || endIndex == -1)
                    {
                        startIndex = 0;
                        endIndex = startIndex + 1;
                        lines.Insert(startIndex, catalog.Protected ? catalog.Encrypt(categoryStartName) : categoryStartName);
                        lines.Insert(endIndex, catalog.Protected ? catalog.Encrypt(categoryEndName) : categoryEndName);
                    }

                    List<string> cutLines = lines.GetRange(startIndex, endIndex - startIndex);
                    lines.RemoveRange(startIndex, endIndex - startIndex);

                    cutLines = new List<string>(cutLines.OrderBy(order));

                    lines.InsertRange(startIndex, cutLines);

                    File.WriteAllLines(catalog.Path, lines);
                }
                catch
                {

                }
            }

            /// <summary>
            /// Reorders a variable into the given index.
            /// </summary>
            /// <param name="catalog"></param>
            /// <param name="to"></param>
            public static void ReorderSingle(Catalog catalog, int to)
            {
                if (!IsValidCatalog(catalog, Catalog.CatalogType.LOADER))
                    throw new StreamingException(StreamingResult.INVALID_CATALOG);

                try
                {
                    List<string> lines = new List<string>(File.ReadAllLines(catalog.Path));
                    string categoryStartName = CategoryWrapper(catalog), categoryEndName = CategoryEndWrapper(catalog);
                    int startIndex = lines.FindIndex(l => (catalog.Protected ? catalog.Decrypt(l) : l) == categoryStartName);
                    int endIndex = lines.FindIndex(l => (catalog.Protected ? catalog.Decrypt(l) : l) == categoryEndName);

                    if (startIndex == -1 || endIndex == -1)
                    {
                        startIndex = 0;
                        endIndex = startIndex + 1;
                        lines.Insert(startIndex, catalog.Protected ? catalog.Encrypt(categoryStartName) : categoryStartName);
                        lines.Insert(endIndex, catalog.Protected ? catalog.Encrypt(categoryEndName) : categoryEndName);
                    }

                    int index = lines.FindIndex(curItm => curItm.Split(STREAM_VALUE_POINTER, StringSplitOptions.None)[0].Equals(catalog.Name));
                    string item = lines[index];
                    lines.Remove(item);
                    lines.Insert(to, item);

                    File.WriteAllLines(catalog.Path, lines);
                }
                catch
                {

                }
            }

            /// <summary>
            /// Returns true if the given <see cref="Catalog"/>'s name exists under it's path.
            /// </summary>
            /// <param name="catalog"></param>
            /// <returns></returns>
            public static bool Contains(Catalog catalog)
            {
                if (!IsValidCatalog(catalog, Catalog.CatalogType.LOADER))
                    throw new StreamingException(StreamingResult.INVALID_CATALOG);

                try
                {
                    List<string> lines;
                    LoadLines(out lines, catalog);
                    string toReturn = lines.Find(s => (catalog.Protected ? catalog.Decrypt(s) : s).Split(STREAM_VALUE_POINTER, StringSplitOptions.None)[0].Equals(catalog.Name));
                    return !string.IsNullOrEmpty(toReturn);
                }
                catch
                {
                    return false;
                }
            }
            
            private static string LoadInternal(Catalog catalog)
            {
                if (!IsValidCatalog(catalog, Catalog.CatalogType.LOADER))
                    throw new StreamingException(StreamingResult.INVALID_CATALOG);

                try
                {
                    List<string> lines;
                    LoadLines(out lines, catalog);
                    string toReturn = lines.Find(s => (catalog.Protected ? catalog.Decrypt(s) : s).Split(STREAM_VALUE_POINTER, StringSplitOptions.None)[0].Equals(catalog.Name));
                    if (string.IsNullOrEmpty(toReturn))
                    {
                        goto Saver;
                    }

                    return (catalog.Protected ? catalog.Decrypt(toReturn) : toReturn)
                        .Split(STREAM_VALUE_POINTER, StringSplitOptions.None)[1];
                }
                catch
                {
                    goto Saver;
                }

            Saver:
                if (catalog.UsesDefaultValue)
                {
                    InternalSave(catalog, true);
                    return catalog.Value;
                }
                else return string.Empty;
            }

            /// <summary>
            /// Loads a variable previously saved using <see cref="Streaming"/>.
            /// </summary>
            /// <param name="catalog"></param>
            /// <returns></returns>
            public static string Load(Catalog catalog)
            {
                return LoadInternal(catalog);
            }

            private static IEnumerable<string> InternalLoadAll(Catalog catalog, bool includeName)
            {
                if (!IsValidCatalog(catalog, Catalog.CatalogType.LOADER))
                    throw new StreamingException(StreamingResult.INVALID_CATALOG);

                try
                {
                    List<string> lines;
                    LoadLines(out lines, catalog);

                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (catalog.Protected) lines[i] = catalog.Decrypt(lines[i]);
                        if (!includeName) lines[i] = lines[i].Split(STREAM_VALUE_POINTER, StringSplitOptions.None)[1];
                    }
                    

                    return lines;
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>
            /// Loads all variables inside given catalog's file category. In case of an exception, returns null.
            /// </summary>
            /// <returns></returns>
            public static IEnumerable<string> LoadAll(Catalog catalog, bool includeName)
            {
                return InternalLoadAll(catalog, includeName);
            }

            /// <summary>
            /// Changes the names of a variable and returns true if it was changed successfully.
            /// </summary>
            /// <param name="catalog"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public static bool ChangeVariableName(Catalog catalog, string to)
            {
                if (!IsValidCatalog(catalog, Catalog.CatalogType.LOADER))
                    throw new StreamingException(StreamingResult.INVALID_CATALOG);

                try
                {
                    LoadLines(out List<string> lines, out int catStart, out int catEnd, catalog);

                    string toReturn = lines.Find(s => (catalog.Protected ? catalog.Decrypt(s) : s).Split(STREAM_VALUE_POINTER, StringSplitOptions.None)[0].Equals(catalog.Name));
                    
                    if (string.IsNullOrEmpty(toReturn))
                        return false;

                    string keep = (catalog.Protected ? catalog.Decrypt(toReturn) : toReturn).Split(STREAM_VALUE_POINTER, StringSplitOptions.None)[2];
                    string toSave = $"{to}{STREAM_VALUE_POINTER}{keep}";
                    toReturn = catalog.Protected ? catalog.Encrypt(toSave) : toSave;

                    File.WriteAllLines(catalog.Path, lines);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private static bool InternalRemove(Catalog catalog)
            {
                if (!IsValidCatalog(catalog, Catalog.CatalogType.LOADER))
                    throw new StreamingException(StreamingResult.INVALID_CATALOG);

                try
                {
                    LoadLines(out List<string> lines, out int catStart, out int catEnd, catalog);

                    int index = lines.FindIndex(s => (catalog.Protected ? catalog.Decrypt(s) : s).Contains(catalog.Name));
                    if (index == -1 || index > catEnd || index < catStart)
                        return false;

                    lines.RemoveAt(index);

                    File.WriteAllLines(catalog.Path, lines);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// Removes a value under catergory file of the given catalog.
            /// </summary>
            /// <returns></returns>
            public static bool Remove(Catalog loader)
            {
                return InternalRemove(loader);
            }

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

            private const int STREAMING_FILE_ENCRYPTION_JUMPER = 85;

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
        /// Encryption class which uses <see cref="ProtectedData"/>.
        /// NOTE: ONLY USE FOR LOCAL ENCRYPTION LIKE OFFLINE CONFIG/SAVE FILES; IF A STRING IS ENCRYPTED ON A MACHINE, IT CAN ONLY BE DECRYPTED BY THAT SAME MACHINE.
        /// </summary>
        public static class LocalEncryption
        {
            private static readonly byte[] Salt = new byte[]
            {
                243, 21, 75, 44,
                21, 75, 44, 192,
                75, 44, 192, 122,
                44, 192, 122, 254,
                192, 122, 254, 243,
                122, 254, 243, 21,
                254, 243, 21, 75,
                11, 22, 33, 44
            };

            /// <summary>
            /// Encrypts a string.
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static string Encrypt(string input)
            {
                return Convert.ToBase64String(ProtectedData.Protect(Encoding.ASCII.GetBytes(input), Salt, DataProtectionScope.CurrentUser));
            }

            /// <summary>
            /// Decrypts a string previously encrypted with <see cref="Encrypt(string)"/>.
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static string Decrypt(string input)
            {
                return Encoding.ASCII.GetString(ProtectedData.Unprotect(Convert.FromBase64String(input), Salt, DataProtectionScope.CurrentUser));
            }
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
        /// Contains some utility methods concerting enumeration and generic collections.
        /// </summary>
        public static class Enumeration
        {
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
                return list.Where(t => t != default);
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
            /// Returns a <see cref="IEnumerable{T}"/> of instantiated objects (unity copy).
            /// </summary>
            /// <param name="objects"></param>
            /// <returns></returns>
            public static IEnumerable<T> InstantiateList<T>(IEnumerable<T> objects) where T : UnityEngine.Object
            {
                T[] toReturn = objects.ToArray();
                for (int i = 0; i < toReturn.Length; i++)
                {
                    toReturn[i] = UnityEngine.Object.Instantiate(toReturn[i]);
                    if (toReturn[i] is IInstantiatable) ((IInstantiatable)toReturn[i]).PostInstantiate();
                }

                return toReturn;
            }

            /// <summary>
            /// Returns an array of instantiated objects.
            /// </summary>
            /// <param name="objects"></param>
            /// <returns></returns>
            public static T[] InstantiateList<T>(T[] objects) where T : UnityEngine.Object
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    objects[i] = UnityEngine.Object.Instantiate(objects[i]);
                    if (objects[i] is IInstantiatable) ((IInstantiatable)objects[i]).PostInstantiate();
                }

                return objects;
            }
        }

        /// <summary>
        /// Contains all methods Position and Vector related.
        /// </summary>
        public static class Vectors
        {
            /// <summary>
            /// Returns direction which would make Position look at Destination.
            /// </summary>
            /// <param name="Position"></param>
            /// <param name="Destination"></param>
            /// <returns></returns>
            public static Vector3 DirectionTowards(Vector3 Position, Vector3 Destination)
                => (Destination - Position).normalized;

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
            public static bool IsInsideBounds(Vector3 position, Vector3 center, Vector3 size)
            {
                float minX = center.x - size.x, maxX = center.x + size.x;
                float minY = center.y - size.y, maxY = center.y + size.y;
                float minZ = center.z - size.z, maxZ = center.z + size.z;

                return position.x > minX && position.x < maxX &&
                    position.y > minY && position.y < maxY &&
                    position.z > minZ && position.z < maxZ;
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
            /// Returns a Vector3 which moves a towards b based on speed. Accelerates based on the distance between both positions.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="speed"></param>
            /// <param name="acceleration"></param>
            /// <returns></returns>
            public static Vector3 MoveTowardsAccelerated(Transform a, Transform b, float speed, float acceleration)
            {
                return MoveTowardsAccelerated(a.position, b.position, speed, acceleration);
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
                Vector3 vector = new Vector3(WWWMath.IsFormal(toReform.x) ? 0f : toReform.x, WWWMath.IsFormal(toReform.y) ? 0f : toReform.y, WWWMath.IsFormal(toReform.z) ? 0f : toReform.z);
                return toReform;
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
                float num = Hooks.WWWMath.MiddleMan(a.x, b.x, percent);
                float num2 = Hooks.WWWMath.MiddleMan(a.y, b.y, percent);
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
            /// Gets closest element to center from collection.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="maxDistance"></param>
            /// <param name="center"></param>
            /// <param name="collection"></param>
            /// <returns></returns>
            public static T GetClosestToDistance<T>(float maxDistance, Vector3 center, ICollection<T> collection) where T : Component
            {
                T result = null;
                float num = float.PositiveInfinity;
                foreach (T item in collection)
                {
                    float num2 = Vector3.Distance(item.transform.position, center);
                    if (!(num2 > maxDistance) && !(num2 > num))
                    {
                        result = item;
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
                    throw new WWWException("Cannot use GetClosestToPosition when collection given is lower in size than amount requested. Aborting");
                else if (amount == count)
                {
                    AdvancedDebug.LogWarning("Using GetClosestToPosition with amount equal to the collection size is pointless. Returning original collection...", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
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
                    throw new WWWException("Cannot use GetFurthestFromPosition when collection given is lower in size than amount requested. Aborting");
                else if (amount == count)
                {
                    AdvancedDebug.LogWarning("Using GetFurthestFromPosition with amount equal to the collection size is pointless. Returning original collection...", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
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

        }

        /// <summary>
        /// Contains all methods Raycast and Physics related.
        /// </summary>
        public static class RPhysics
        {
            /// <summary>
            /// Casts a single 2D raycast.
            /// </summary>
            /// <param name="startingPos"></param>
            /// <param name="direction"></param>
            /// <param name="distance"></param>
            /// <param name="layerMasks"></param>
            /// <returns></returns>
            public static RaycastHit2D RayCastSingle(Vector2 startingPos, Vector2 direction, float distance, string[] layerMasks)
            {
                LayerMask mask = LayerMask.GetMask(layerMasks);
                return Physics2D.Raycast(startingPos, direction, distance, mask);
            }

            /// <summary>
            /// Casts a single raycast.
            /// </summary>
            /// <param name="startingPos"></param>
            /// <param name="direction"></param>
            /// <param name="distance"></param>
            /// <param name="layerMasks"></param>
            /// <returns></returns>
            public static RaycastHit RayCastSingle(Vector3 startingPos, Vector3 direction, float distance, string[] layerMasks)
            {
                LayerMask mask = LayerMask.GetMask(layerMasks);
                Physics.Raycast(startingPos, direction, out RaycastHit hitInfo, distance, mask);
                return hitInfo;
            }

            /// <summary>
            /// Casts multiple 2D raycasts.
            /// </summary>
            /// <param name="startingPos"></param>
            /// <param name="directions"></param>
            /// <param name="distance"></param>
            /// <param name="layerMasks"></param>
            /// <returns></returns>
            public static RaycastHit2D[] RayCastSides(Vector2 startingPos, Vector2[] directions, float distance, string[] layerMasks)
            {
                RaycastHit2D[] array = new RaycastHit2D[directions.Length];
                for (int i = 0; i < directions.Length; i++)
                {
                    LayerMask mask = LayerMask.GetMask(layerMasks);
                    array[i] = Physics2D.Raycast(startingPos, directions[i], distance, mask);
                }
                return array;
            }

            /// <summary>
            /// Casts multiple raycasts.
            /// </summary>
            /// <param name="startingPos"></param>
            /// <param name="directions"></param>
            /// <param name="distance"></param>
            /// <param name="layerMasks"></param>
            /// <returns></returns>
            public static RaycastHit[] RayCastSides(Vector3 startingPos, Vector3[] directions, float distance, string[] layerMasks)
            {
                RaycastHit[] array = new RaycastHit[directions.Length];
                for (int i = 0; i < directions.Length; i++)
                {
                    LayerMask mask = LayerMask.GetMask(layerMasks);
                    Physics.Raycast(startingPos, directions[i], out array[i], distance, mask);
                }
                return array;
            }

            /// <summary>
            /// Casts multiple 2D raycasts; If multiple raycasts hit, closestIndex will return the index of the closest hit to the startingPos.
            /// </summary>
            /// <param name="startingPos"></param>
            /// <param name="directions"></param>
            /// <param name="distance"></param>
            /// <param name="layerMasks"></param>
            /// <param name="closestIndex"></param>
            /// <returns></returns>
            public static RaycastHit2D[] RayCastSides(Vector2 startingPos, Vector2[] directions, float distance, string[] layerMasks, ref int closestIndex)
            {
                RaycastHit2D[] array = new RaycastHit2D[directions.Length];
                float num = float.PositiveInfinity;
                closestIndex = 0;
                for (int i = 0; i < directions.Length; i++)
                {
                    LayerMask mask = LayerMask.GetMask(layerMasks);
                    array[i] = Physics2D.Raycast(startingPos, directions[i], distance, mask);
                    if (array[i].distance < num)
                    {
                        num = array[i].distance;
                        closestIndex = i;
                    }
                }
                return array;
            }

            /// <summary>
            /// Casts multiple 2D raycasts where a transform's position is the center.
            /// </summary>
            /// <param name="startingPos"></param>
            /// <param name="directions"></param>
            /// <param name="distance"></param>
            /// <param name="layerMasks"></param>
            /// <param name="local"></param>
            /// <returns></returns>
            public static RaycastHit2D[] RayCastSides(Transform startingPos, Vector2[] directions, float distance, string[] layerMasks, bool local)
            {
                RaycastHit2D[] array = new RaycastHit2D[directions.Length];
                for (int i = 0; i < directions.Length; i++)
                {
                    LayerMask mask = LayerMask.GetMask(layerMasks);
                    array[i] = Physics2D.Raycast(startingPos.position, local ? ((Vector2)startingPos.InverseTransformPoint(directions[i])) : directions[i], distance, mask);
                }
                return array;
            }

            /// <summary>
            /// Casts multiple 2D raycasts where a transform's position is the center; If multiple raycasts hit, closestIndex will return the index of the closest hit to the startingPos.
            /// </summary>
            /// <param name="startingPos"></param>
            /// <param name="directions"></param>
            /// <param name="distance"></param>
            /// <param name="layerMasks"></param>
            /// <param name="local"></param>
            /// <param name="closestIndex"></param>
            /// <returns></returns>
            public static RaycastHit2D[] RayCastSides(Transform startingPos, Vector2[] directions, float distance, string[] layerMasks, bool local, ref int closestIndex)
            {
                RaycastHit2D[] array = new RaycastHit2D[directions.Length];
                float num = float.PositiveInfinity;
                closestIndex = 0;
                for (int i = 0; i < directions.Length; i++)
                {
                    LayerMask mask = LayerMask.GetMask(layerMasks);
                    array[i] = Physics2D.Raycast(startingPos.position, local ? ((Vector2)startingPos.InverseTransformPoint(directions[i])) : directions[i], distance, mask);
                    if (array[i].distance < num)
                    {
                        num = array[i].distance;
                        closestIndex = i;
                    }
                }
                return array;
            }
        }

        /// <summary>
        /// Contains all methods Math related.
        /// </summary>
        public static class WWWMath
        {
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
            /// If value given is negative, it will be turned into it's positive value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static int ToPositive(int value)
            {
                if (value < 0)
                {
                    value = -value;
                }
                return value;
            }

            /// <summary>
            /// If value given is positive, it will be turned into it's negative value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static int ToNegative(int value)
            {
                if (value > 0)
                {
                    value = -value;
                }
                return value;
            }

            /// <summary>
            /// If value given is negative, it will be turned into it's positive value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static float ToPositive(float value)
            {
                if (value < 0f)
                {
                    value = -value;
                }
                return value;
            }

            /// <summary>
            /// If value given is positive, it will be turned into it's negative value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static float ToNegative(float value)
            {
                if (value > 0)
                {
                    value = -value;
                }
                return value;
            }

            /// <summary>
            /// If value given is negative, it will be turned into it's positive value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ToPositive(long value)
            {
                if (value < 0f)
                {
                    value = -value;
                }
                return value;
            }

            /// <summary>
            /// If value given is positive, it will be turned into it's negative value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ToNegative(long value)
            {
                if (value > 0)
                {
                    value = -value;
                }
                return value;
            }

            /// <summary>
            /// Clamps the given value between range.Min and range.Max.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static float Clamp(float value, FloatRange range)
            => Mathf.Clamp(value, range.Min, range.Max);

            /// <summary>
            /// Clamps the given value between range.Min and range.Max.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static int Clamp(int value, IntRange range)
                => Mathf.Clamp(value, range.Min, range.Max);

            /// <summary>
            /// Clamps a value between min and max, and if the value goes outside the boundries, it goes back to it's opposite boundry and starts counting from it.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <exception cref="WWWException"/>
            /// <returns></returns>
            public static float ClampRounded(float value, float min, float max)
            {
                float num = value;
                if (min >= max)
                {
                    throw new WWWException("Cannot use Utilities.ClampRounded if the minimal value is equal or higher than the maximal value.");
                }
                while (num > max)
                {
                    num = max == 0 ? min : num - max;
                }
                while(num < min)
                {
                    num = min == 0 ? max : num + min;
                }
                return num;
            }

            /// <summary>
            /// Clamps a value between min and max, and if the value goes outside the boundries, it goes back to it's opposite boundry and starts counting from it.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static float ClampRounded(float value, FloatRange range)
            {
                float num = value;
                if (range.Min >= range.Max)
                {
                    throw new WWWException("Cannot use Utilities.ClampRounded if the minimal value is equal or higher than the maximal value.");
                }
                while (num > range.Max)
                {
                    num = range.Max == 0 ? range.Min : num - range.Max;
                }
                while (num < range.Min)
                {
                    num = range.Min == 0 ? range.Max : num + range.Min;
                }
                return num;
            }

            /// <summary>
            /// Clamps a value between min and max, and if the value goes outside the boundries, it goes back to it's opposite boundry and starts counting from it.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <exception cref="WWWException"/>
            /// <returns></returns>
            public static int ClampRounded(int value, int min, int max)
            {
                int i = value;
                if (min >= max)
                {
                    throw new WWWException("Cannot use Utilities.ClampRounded if the minimal value is equal or higher than the maximal value.");
                }
                while (i > max || i < min)
                {
                    if (i > max) i = max == 0 ? min : i - max;
                    else
                    {
                        if (min == 0)
                        {
                            i = 0;
                            break;
                        }
                        else
                            i += min;
                    }
                }
                return i;
            }

            /// <summary>
            /// Clamps a value between min and max, and if the value goes outside the boundries, it goes back to it's opposite boundry and starts counting from it.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static int ClampRounded(int value, IntRange range)
            {
                int i = value;
                if (range.Min >= range.Max)
                {
                    throw new WWWException("Cannot use Utilities.ClampRounded if the minimal value is equal or higher than the maximal value.");
                }
                while (i > range.Max || i < range.Min)
                {
                    if (i > range.Max) i = range.Max == 0 ? range.Min : i - range.Max;
                    else
                    {
                        if (range.Min == 0)
                        {
                            i = 0;
                            break;
                        }
                        else
                            i += range.Min;
                    }
                }
                return i;
            }

            /// <summary>
            /// Clamps a value between min and max; When a value reaches min or max, it will start counting towards the opposite direction in a "ping-pong" style.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static int ClampPingPong(int value, int min, int max)
            {
                int num = value;
                if (min >= max)
                {
                    throw new Exception("Cannot use Utilities.ClampPingPong if the minimal value is equal or higher than the maximal value.");
                }
                while (true)
                {
                    if (num > max)
                    {
                        int num2 = num - max;
                        num -= num2;
                        continue;
                    }
                    if (num < min)
                    {
                        int num3 = min - num;
                        num += num3;
                        continue;
                    }
                    break;
                }
                return num;
            }

            /// <summary>
            /// Clamps a value between min and max; When a value reaches min or max, it will start counting towards the opposite direction in a "ping-pong" style.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static int ClampPingPong(int value, IntRange range)
            {
                int num = value;
                if (range.Min >= range.Max)
                {
                    throw new Exception("Cannot use Utilities.ClampPingPong if the minimal value is equal or higher than the maximal value.");
                }
                while (true)
                {
                    if (num > range.Max)
                    {
                        int num2 = num - range.Max;
                        num -= num2;
                        continue;
                    }
                    if (num < range.Min)
                    {
                        int num3 = range.Min - num;
                        num += num3;
                        continue;
                    }
                    break;
                }
                return num;
            }

            /// <summary>
            /// Clamps a value between min and max; When a value reaches min or max, it will start counting towards the opposite direction in a "ping-pong" style.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static float ClampPingPong(float value, float min, float max)
            {
                float num = value;
                if (min >= max)
                {
                    throw new Exception("Cannot use Utilities.ClampPingPong if the minimal value is equal or higher than the maximal value.");
                }
                while (true)
                {
                    if (num > max)
                    {
                        float num2 = num - max;
                        num -= num2 * 2f;
                        continue;
                    }
                    if (num < min)
                    {
                        float num3 = min - num;
                        num += num3 * 2f;
                        continue;
                    }
                    break;
                }
                return num;
            }

            /// <summary>
            /// Clamps a value between min and max; When a value reaches min or max, it will start counting towards the opposite direction in a "ping-pong" style.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="range"></param>
            /// <returns></returns>
            public static float ClampPingPong(float value, FloatRange range)
            {
                float num = value;
                if (range.Min >= range.Max)
                {
                    throw new Exception("Cannot use Utilities.ClampPingPong if the minimal value is equal or higher than the maximal value.");
                }
                while (true)
                {
                    if (num > range.Max)
                    {
                        float num2 = num - range.Max;
                        num -= num2;
                        continue;
                    }
                    if (num < range.Min)
                    {
                        float num3 = range.Min - num;
                        num += num3;
                        continue;
                    }
                    break;
                }
                return num;
            }

            /// <summary>
            /// Moves a towards b by speed.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="speed"></param>
            /// <returns></returns>
            public static float MoveTowards(float a, float b, float speed)
            {
                Vector2 current = new Vector2(a, 0f);
                Vector2 target = new Vector2(b, 0f);
                Vector2 vector = Vector2.MoveTowards(current, target, speed);
                return vector.x;
            }

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
            /// 
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="percent"></param>
            /// <returns></returns>
            public static float MiddleMan(float a, float b, float percent = 0.5f)
            {
                float num = b - a;
                percent = Mathf.Clamp(percent, 0f, 1f);
                return num * percent;
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
        }

        /// <summary>
        /// Don't.
        /// </summary>
        public static class Disappointment
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
            /// Freezes the program using this library.
            /// </summary>
            public static void Freeze()
            {
                try
                {
                    CheckForMainThread();

                    for(int i = 0; i < 1; i--)
                    {
                        AdvancedDebug.LogError("*Evil laugh*", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
                    }
                }
                catch
                {
                    AdvancedDebug.LogError("Cannot use Freeze() when not on main thread!", AdvancedDebug.DEBUG_LAYER_EXCEPTIONS_INDEX);
                }
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
        /// Contains all methods related to rendering.
        /// </summary>
        public static class Rendering
        {
            /// <summary>
            /// Returns LayerMask int value of all layers. Useful for Raycasting.
            /// </summary>
            /// <param name="layers"></param>
            /// <returns></returns>
            public static int MaskGetter(int[] layers)
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
                => Rendering.MaskToLayer(LayerMask.NameToLayer(layer));

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
            /// Determines the shaking type of <see cref="Shake(MonoBehaviour, Vector3, Vector3, float, float, float, RotationShakeType)"/>.
            /// </summary>
            public enum RotationShakeType
            {
                /// <summary>
                /// Doesn't shake.
                /// </summary>
                None,
                /// <summary>
                /// Shakes the Z rotation of a camera. (2D)
                /// </summary>
                zAxis,
                /// <summary>
                /// Shakes every axis. (3D)
                /// </summary>
                FullRandom,
            }

            /// <summary>
            /// Shakes a given Vector and Quaternion value. Make sure to pass a property for both values or this method will not work.
            /// </summary>
            /// <param name="host"></param>
            /// <param name="shake"></param>
            /// <param name="eulerRotation"></param>
            /// <param name="intensity"></param>
            /// <param name="duration"></param>
            /// <param name="speed"></param>
            /// <param name="rotationShake"></param>
            public static void Shake(MonoBehaviour host, Vector3 shake, Vector3 eulerRotation, float intensity, float duration, float speed, RotationShakeType rotationShake)
            {
                host.StartCoroutine(IShake(host, shake, eulerRotation, intensity, duration, speed, rotationShake));
            }

            private static IEnumerator IShake(MonoBehaviour host, Vector3 Shake, Vector3 Rotation, float intensity, float duration, float speed, RotationShakeType rotationShake)
            {
                float dur = duration;

                while(true)
                {
                    Shake = Vector3.MoveTowards(Shake, Random.RandomVector01WithNeg * intensity, speed);
                    switch(rotationShake)
                    {
                        default: Rotation = Vector3.MoveTowards(Rotation, Random.RandomVector01WithNeg * intensity * 10, speed); break;
                        case RotationShakeType.None: Rotation = Vector3.zero; break;
                        case RotationShakeType.zAxis: Rotation = Vector3.MoveTowards(Rotation, Vector3.forward * UnityEngine.Random.Range(-1f, 1f) * intensity, speed); break;
                    }
                    dur -= Time.deltaTime;
                    if(dur <= 0)
                    {
                        break;
                    }
                    yield return null;
                }

                host.StopCoroutine("ICameraShake");
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
            /// Used to tag a string section to start a rainbow text.
            /// </summary>
            public const string RainbowTextStarter = "(Color=Rainbow)";

            /// <summary>
            /// Used to tag a string section to stop a rainbow text.
            /// </summary>
            public const string RainbowTextEnder = "(/Color=Rainbow)";

            /// <summary>
            /// Puts a string value into a rainbow color using Unity's Rich Text format.
            /// </summary>
            /// <param name="original"></param>
            /// <param name="frequency"></param>
            /// <param name="colorsToUse"></param>
            /// <returns></returns>
            public static string ToRainbow(string original, int frequency, Color[] colorsToUse)
            {
                if (!original.Contains(RainbowTextStarter) || !original.Contains(RainbowTextEnder))
                {
                    AdvancedDebug.LogWarning("Message did not contain required Starter/Ender for text conversion. Make sure the text you want to modify is wrapped with (Color=Rainbow) (/Color=Rainbow)", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
                    return original;
                }
                string text = "<color=#klrtgiv>";
                string str = "</color>";
                string[] separator = new string[1]
                {
                    RainbowTextStarter
                };
                string[] separator2 = new string[1]
                {
                    RainbowTextEnder
                };
                string text2 = original.Split(separator, StringSplitOptions.RemoveEmptyEntries)[1].Split(separator2, StringSplitOptions.RemoveEmptyEntries)[0];
                char[] array = text2.ToCharArray();
                string[] array2 = new string[array.Length];
                for (int i = 0; i < array.Length; i += frequency)
                {
                    int num = Mathf.Clamp(i + frequency, 0, array.Length);
                    for (int j = i; j < num; j++)
                    {
                        array2[j] = text.Replace("klrtgiv", Colors.ColorToHex(colorsToUse[WWWMath.ClampRounded(i, 0, colorsToUse.Length - 1)])) + array[j].ToString() + str;
                    }
                }
                string empty = string.Empty;
                empty = EnumerableConcat(empty, array2.ToList());
                string[] array3 = original.Split(separator, StringSplitOptions.None);
                string str2 = array3[0];
                string str3 = array3[1].Split(separator2, StringSplitOptions.None)[1];
                return str2 + empty + str3;
            }

            /// <summary>
            /// Wraps original between wrappers.Item1 and wrappers.Item2
            /// </summary>
            /// <param name="original"></param>
            /// <param name="wrappers"></param>
            /// <returns></returns>
            public static string StringWrapper(string original, (char, char) wrappers)
            {
                StringBuilder stringBuilder = new StringBuilder(3);
                stringBuilder.Insert(0, wrappers.Item1);
                stringBuilder.Insert(1, original);
                stringBuilder.Insert(original.Length + 1, wrappers.Item2);
                return stringBuilder.ToString();
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

            private static readonly Regex kanjiRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
            private static readonly Regex hiraganaRegex = new Regex(@"\p{IsHiragana}");
            private static readonly Regex katakanaRegex = new Regex(@"\p{IsKatakana}");
            /// <summary>
            /// Returns true if the given string contains a Kanji character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsKanji(string value)
                => kanjiRegex.IsMatch(value);

            /// <summary>
            /// Returns true if the given string contains a Hiragana character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsHiragana(string value)
                => hiraganaRegex.IsMatch(value);

            /// <summary>
            /// Returns true if the given string contains a Katakana character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsKatakana(string value)
               => katakanaRegex.IsMatch(value);

            /// <summary>
            /// Returns true if the given string contains either a Kanji, Hiragana or Katakana character.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool IsJapanese(string value)
                => IsKanji(value) || IsHiragana(value) || IsKatakana(value);

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
        /// Contains all Color utilities.
        /// </summary>
        public static class Colors
        {
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
                => new Color(WWWMath.ToNegative(original.r), WWWMath.ToNegative(original.g), WWWMath.ToNegative(original.b), WWWMath.ToNegative(original.a));

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
                Color.red,
                Color.yellow,
                Color.blue
            };

            /// <summary>
            /// Returns in order: Color.red, Color.yellow, Color.cyan, Color.blue, Color.magenta
            /// </summary>
            public static readonly Color[] MainColors = new Color[6]
            {
                Color.red,
                Color.yellow,
                Color.green,
                Color.cyan,
                Color.blue,
                Color.magenta
            };

            /// <summary>
            /// Returns all colors of the rainbow in descending order.
            /// </summary>
            public static readonly Color[] RainbowColors = new Color[7]
            {
                new Color(1f, 0f, 0f),
                new Color(1f, 0.35f, 0f),
                new Color(1f, 1f, 0f),
                new Color(0f, 1f, 0f),
                new Color(0f, 1f, 1f),
                new Color(0f, 0.35f, 1f),
                new Color(1f, 0f, 1f)
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
            => WWWMath.IsApproximate(NumberToApproximate, NumberToCompare, approximation);
        /// <summary>
        /// Cuts a float to digits length.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static float Truncate(this float value, int digits) => WWWMath.Truncate(value, digits);
        /// <summary>
        /// If value given is negative, it will be turned into it's positive value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToPositive(this float value) => WWWMath.ToPositive(value);
        /// <summary>
        /// If value given is positive, it will be turned into it's negative value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToNegative(this float value) => WWWMath.ToNegative(value);
        /// <summary>
        /// If value given is negative, it will be turned into it's positive value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToPositive(this int value) => WWWMath.ToPositive(value);
        /// <summary>
        /// If value given is positive, it will be turned into it's negative value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToNegative(this int value) => WWWMath.ToNegative(value);
        /// <summary>
        /// If value given is negative, it will be turned into it's positive value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToPositive(this long value) => WWWMath.ToPositive(value);
        /// <summary>
        /// If value given is positive, it will be turned into it's negative value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToNegative(this long value) => WWWMath.ToNegative(value);
        /// <summary>
        /// Puts all values of a given color into negatives. (If a value was negative, it will be put into positive)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color ToNegative(this Color value) => Colors.ToNegative(value);
        /// <summary>
        /// Extention method. Points to <see cref="Vectors.SetAnchoredUI(RectTransform, Vector4)"/>.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public static void SetAnchoredUI(this RectTransform rt, Vector4 Position)
            => Vectors.SetAnchoredUI(rt, Position);
        /// <summary>
        /// Extention method. Points to <see cref="Vectors.SetAnchoredUI(RectTransform, Vector2, Vector2)"/>.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void SetAnchoredUI(this RectTransform rt, Vector2 min, Vector2 max)
            => Vectors.SetAnchoredUI(rt, min, max);
        /// <summary>
        /// Extention method. Points to <see cref="Vectors.SetAnchoredUI(RectTransform, float, float, float, float)"/>.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        public static void SetAnchoredUI(this RectTransform rt, float minX, float minY, float maxX, float maxY)
            => Vectors.SetAnchoredUI(rt, minX, minY, maxX, maxY);
        /// <summary>
        /// Returns the anchored position of a <see cref="RectTransform"/> in <see cref="Vector4"/>: X = minX, Y = minY, z = maxX, W = maxY.
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public static Vector4 GetAnchoredPosition(this RectTransform rt)
            => Vectors.GetAnchoredPosition(rt);
        /// <summary>
        /// Returns true if the given position is within the bounds given. (<see cref="Vector4"/> bounds: X = minX, Y = minY, z = maxX, W = maxY)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static bool IsInsideBounds(this Vector2 position, Vector4 bounds)
            => Vectors.IsInsideBounds(position, bounds);
        /// <summary>
        /// Returns true if the given position is within the bounds given.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsInsideBounds(this Vector2 position, Vector2 min, Vector2 max)
            => Vectors.IsInsideBounds(position, min, max);
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
            return list.Where(t => t != null);
        }
        /// <summary>
        /// Returns true if the given <see cref="ICollection{T}"/> has toLookfor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="toLookfor"></param>
        /// <returns></returns>
        public static bool EnumerableContains<T>(this IEnumerable<T> list, T toLookfor)
        {
            if (list.Count() < 1)
            {
                return false;
            }
            foreach (T item in list)
            {
                if (item.Equals(toLookfor))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns an item from the given collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="toLookFor"></param>
        /// <returns></returns>
        public static T ReturnFromEnumerable<T>(this IEnumerable<T> list, T toLookFor)
        {
            foreach (T item in list)
            {
                if (item.Equals(toLookFor))
                {
                    return item;
                }
            }
            return default;
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
        /// Returns the first instance of a null item inside a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T GetEmptyItem<T>(this IEnumerable<T> collection) where T : class
        {
            foreach (T item in collection)
            {
                if (item == null)
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns the index of the first element T equal to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this List<T> list)
         => list.FindIndex(t => t == null);
        /// <summary>
        /// Returns the index of the first element T equal to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this T[] array)
        => array.FindIndex(t => t == null);
        /// <summary>
        /// Returns the index of the first element T equal to null starting from specified index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this T[] array, int index)
            => Array.FindIndex(array, index, t => t == null);
        /// <summary>
        /// Returns the index of the first element T equal to null starting from specified index up to count times upwards in the enumerator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this T[] array, int index, int count)
            => Array.FindIndex(array, index, count, t => t == null);
        /// <summary>
        /// Returns index of the first element T equal to null starting from IntRange.Min to IntRange.Max.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this T[] array, IntRange range)
            => Array.FindIndex(array, range.Min, range.Max - range.Min, t => t == null);
        /// <summary>
        /// Returns the index of the first element T equal to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static int GetEmptyIndex<T>(this IEnumerable<T> collection)
        {
            try
            {
                T[] array = collection.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    T val = array[i];
                    if (val == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// Returns the index of the first element equal to <typeparamref name="T"/> given.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        public static int GetItemIndex<T>(this IEnumerable<T> collection, T Item) where T : class
        {
            List<T> list = new List<T>(collection);
            for (int i = 0; i < collection.Count(); i++)
            {
                if (list[i] != null && list[i].Equals(Item))
                {
                    return i;
                }
                if (list[i] == null && Item == null)
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// Returns a new <see cref="ICollection{T}"/> of given size if collection passed was null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static ICollection<T> SetCollectionSizeIfNull<T>(this ICollection<T> collection, int size)
        {
            if (collection == null || collection.Count == 0)
            {
                return new T[size];
            }
            return collection;
        }
        /// <summary>
        /// Takes <paramref name="size"/> elements of the given collection and puts them as a <see cref="Queue{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Collection to itterate through.</param>
        /// <param name="size">Amount of items from <paramref name="collection"/> to itterate through.</param>
        /// <param name="fromEnd">If true, it will start from collection's Count-1 and go in descending order to get items, instead of 0 in ascending order.</param>
        /// <returns></returns>
        public static Queue<T> ToQueueSized<T>(this ICollection<T> collection, int size, bool fromEnd)
        {
            if (collection.Count <= 0)
            {
                return null;
            }
            int num = (collection.Count - 1 < size) ? (collection.Count - 1) : size;
            int index = fromEnd ? (collection.Count - 1 - num) : 0;
            List<T> range = collection.ToList().GetRange(index, num);
            return new Queue<T>(range);
        }
        /// <summary>
        /// Takes <paramref name="size"/> elements of the given collection and puts them as a <see cref="Stack{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Collection to itterate through.</param>
        /// <param name="size">Amount of items from <paramref name="collection"/> to itterate through.</param>
        /// <param name="fromEnd">If true, it will start from collection's Count-1 and go in descending order to get items, instead of 0 in ascending order.</param>
        /// <returns></returns>
        public static Stack<T> ToStackSized<T>(this ICollection<T> collection, int size, bool fromEnd)
        {
            if (collection.Count <= 0)
            {
                return null;
            }
            int num = (collection.Count - 1 < size) ? (collection.Count - 1) : size;
            int index = fromEnd ? (collection.Count - 1 - num) : 0;
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
        public static T[] GetItemsFromTupleIndex<T>(this ICollection<(T, T)> collection, int item)
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
        public static T[] GetItemsFromTupleIndex<T>(this ICollection<(T, T, T)> collection, int item)
        {
            List<T> list = new List<T>();
            foreach (var item3 in collection)
            {
                List<T> list2 = list;
                object item2;
                switch (item)
                {
                    default:
                        item2 = item3.Item3;
                        break;
                    case 1:
                        item2 = item3.Item2;
                        break;
                    case 0:
                        item2 = item3.Item1;
                        break;
                }
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
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
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
        /// <param name="reversed"></param>
        /// <param name="adder">Z rotation to add onto the result.</param>
        /// <returns></returns>
        public static Quaternion RotateTowards2D(this Vector2 center, Vector2 destination, bool reversed, float adder)
        {
            Vector3 used = center - destination;
            float num = Mathf.Atan2(used.y, used.x) * Mathf.Rad2Deg;
            if (reversed)
            {
                num += 180f;
            }
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
        /// Returns true if a given object has a method named with methodName.
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static bool HasMethod(this object objectToCheck, string methodName)
        {
            try
            {
                Type type = objectToCheck.GetType();
                return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
            }
            catch (AmbiguousMatchException)
            {
                return true;
            }
        }
        /// <summary>
        /// Attempts to call a method by name from an object.
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        public static void CallMethod(this object objectToCheck, string methodName, object[] args)
        {
            try
            {
                Type type = objectToCheck.GetType();
                MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                method.Invoke(objectToCheck, args);
            }
            catch (Exception e)
            {
                AdvancedDebug.LogException(e);
            }
        }
        /// <summary>
        /// Attempts to call a method by name from an object using custom bindings.
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <param name="customBind"></param>
        public static void CallMethod(this object objectToCheck, string methodName, object[] args, BindingFlags customBind)
        {
            try
            {
                Type type = objectToCheck.GetType();
                MethodInfo method = type.GetMethod(methodName, customBind);
                method.Invoke(objectToCheck, args);
            }
            catch (Exception e)
            {
                AdvancedDebug.LogException(e);
            }
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
        /// Returns the CompareTo method based on integer type of the value given; If the object(s) given are non-standard data (like int, float, double, etc) it will return 0.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="matchType">If true, both objects must be of the same type to work.</param>
        /// <returns></returns>
        public static int CompareTo(object a, object b, bool matchType)
        {
            try
            {
                if (matchType)
                {
                    switch (a)
                    {
                        default: return 0;
                        case byte Byte when b is byte:
                            return Byte.CompareTo((byte)b);
                        case sbyte sByte when b is sbyte:
                            return sByte.CompareTo((sbyte)b);
                        case short Short when b is short:
                            return Short.CompareTo((short)b);
                        case ushort uShort when b is ushort:
                            return uShort.CompareTo((ushort)b);
                        case int Int when b is int:
                            return Int.CompareTo((int)b);
                        case uint uInt when b is uint:
                            return uInt.CompareTo((uint)b);
                        case long Long when b is long:
                            return Long.CompareTo((long)b);
                        case ulong uLong when b is ulong:
                            return uLong.CompareTo((ulong)b);
                        case float Float when b is float:
                            return Float.CompareTo((float)b);
                        case double Double when b is double:
                            return Double.CompareTo((double)b);
                    }
                }
                else
                {
                    switch (a)
                    {
                        default: return 0;
                        case byte Byte when b is byte:
                            return Byte.CompareTo(b);
                        case sbyte sByte when b is sbyte:
                            return sByte.CompareTo(b);
                        case short Short when b is short:
                            return Short.CompareTo(b);
                        case ushort uShort when b is ushort:
                            return uShort.CompareTo(b);
                        case int Int when b is int:
                            return Int.CompareTo(b);
                        case uint uInt when b is uint:
                            return uInt.CompareTo(b);
                        case long Long when b is long:
                            return Long.CompareTo(b);
                        case ulong uLong when b is ulong:
                            return uLong.CompareTo(b);
                        case float Float when b is float:
                            return Float.CompareTo(b);
                        case double Double when b is double:
                            return Double.CompareTo(b);
                    }
                }
            }
            catch
            {
                return 0;
            }
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
        /// Returns true if type is implementing the generic implementation.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericImplementation"></param>
        /// <returns></returns>
        public static bool ImplementsGeneric(Type type, Type genericImplementation)
            => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericImplementation);

        private const uint CF_TEXT = 1u;

        private const uint CF_UNICODETEXT = 13u;

        /// <summary>
        /// Returns the default player tag for Unity.
        /// </summary>
        public const string TAG_PLAYER = "Player";

        /// <summary>
        /// <see cref="Time.smoothDeltaTime"/> Pointer.
        /// </summary>
        [Obsolete("Kept for backwards compatibility. Use Time.smoothDeltaTime instead.")]
        public static float RealTimeSecond => Time.smoothDeltaTime;
        /// <summary>
        /// <see cref="Time.fixedDeltaTime"/> Pointer.
        /// </summary>
        [Obsolete("Kept for backwards compatibility. Use Time.fixedDeltaTime instead.")]
        public static float FixedSecond => Time.fixedDeltaTime;
        /// <summary>
        /// Returns 60 * <see cref="Time.fixedDeltaTime"/>.
        /// </summary>
        public static float FixedFullSecond => 60 * Time.fixedDeltaTime;

        /// <summary>
        /// Removes element at toRemove index from ref list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="toRemove"></param>
        public static void RemoveElement(ref List<int> list, int toRemove)
        {
            list.Remove(toRemove);
        }
        
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
        public static void CheckForMainThread()
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
                        return;
                    }
                }
            }

            throw new WWWException("Not on main thread!");
        }

#if WWWREG
        public static RegistryKey RegistryFromPath(string RegKeyPath, RegistryHive CategorySearch)
    {
        switch (CategorySearch)
        {
            default:
                return Registry.CurrentUser.OpenSubKey(RegKeyPath);
            case RegistryHive.LocalMachine:
                return Registry.LocalMachine.OpenSubKey(RegKeyPath);
            case RegistryHive.ClassesRoot:
                return Registry.ClassesRoot.OpenSubKey(RegKeyPath);
            case RegistryHive.CurrentConfig:
                return Registry.LocalMachine.OpenSubKey(RegKeyPath);
            case RegistryHive.PerformanceData:
                return Registry.PerformanceData.OpenSubKey(RegKeyPath);
            case RegistryHive.Users:
                return Registry.Users.OpenSubKey(RegKeyPath);
        }
    }

    public static string[] GetSubkeyNamesFromKey(string RegKeyPath, RegistryHive CategorySearch)
    {
        return RegistryFromPath(RegKeyPath, CategorySearch)?.GetValueNames() ?? null;
    }
#endif

        private static Canvas utilCanvas = null;
        /// <summary>
        /// Canvas which is created under the name UtilitiesCanvas if no canvas was present on the scene.
        /// </summary>
        internal static Canvas UtilityCanvas
        {
            get
            {
                if (!utilCanvas)
                {
                    switch(Settings.GetUtilityCanvasType())
                    {
                        default:
                            utilCanvas = UnityEngine.Object.FindObjectOfType<Canvas>();
                            break;
                        case Settings.UtilityCanvasType.PREFABBED:
                            utilCanvas = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(Settings.GetUtilityCanvasResourcesPath())).GetComponent<Canvas>();
                            break;
                        case Settings.UtilityCanvasType.BY_NAME_IN_SCENE:
                            utilCanvas = GameObject.Find(Settings.GetUtilityCanvasNameLoad())?.GetComponent<Canvas>();
                            break;
                        case Settings.UtilityCanvasType.INSTANTIATE_NEW:
                            utilCanvas = new GameObject(Settings.CanvasDefaultName).AddComponent<Canvas>();
                            utilCanvas.gameObject.AddComponent<CanvasScaler>();
                            utilCanvas.gameObject.AddComponent<GraphicRaycaster>();
                            break;
                    }
                }

                return utilCanvas;
            }
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
        /// Returns a list of rotations based around the amount given; amount 2 would return 0 and 180, 3 would return 0, 120 and 240, 4 would return 0, 80, 160 and 240, etc...
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="amount"></param>
        /// <param name="maxRotation"></param>
        /// <returns></returns>
        public static float[] Get2DRotationsAroundPoint(float offset, int amount, float maxRotation = 360)
        {
            float[] toReturn = new float[amount];
            for(int i = 0; i < amount; i++)
            {
                toReturn[i] = ((maxRotation / amount) * i) + offset;
            }

            return toReturn;
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

        ///<summary>
        /// Deprecated.
        ///</summary>
        [Obsolete("Use GetParent instead.")]
        public static GameObject ObjectParent(Collider2D collision)
         => GetParent(collision);

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
                while (last.transform.parent != null)
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

        /// <summary>
        /// Calls a method by name inside a static class.
        /// </summary>
        /// <param name="classToCheck"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <param name="specificBind"></param>
        public static void CallStaticMethod(Type classToCheck, string methodName, object[] args, BindingFlags specificBind)
        {
            try
            {
                MethodInfo method = classToCheck.GetMethod(methodName, specificBind);
                method.Invoke(null, args);
            }
            catch (Exception e)
            {
                AdvancedDebug.LogException(e);
            }
        }

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
            return source.Single((Sprite s) => s.name == spriteName);
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
        /// Returns all values merged into one with origin as base value, from an <see cref="IEnumerable{T}"/> where T is string.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static string EnumerableConcat(string origin, IEnumerable<string> adders)
        {
            if (adders == null)
            {
                return origin;
            }
            foreach (string adder in adders)
            {
                origin += adder;
            }
            return origin;
        }
        /// <summary>
        /// Returns all values merged into one with origin as base value, from an <see cref="IEnumerable{T}"/> where T is string.
        /// </summary>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static string EnumerableConcat(IEnumerable<string> adders)
            => EnumerableConcat(string.Empty, adders);

        /// <summary>
        /// Returns all values merged into one with origin as base value, from an <see cref="IEnumerable{T}"/> where T is float.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static float EnumerableConcat(float origin, IEnumerable<float> adders)
        {
            float toReturn = origin;
            foreach (float f in adders)
                toReturn += f;

            return toReturn;
        }
        /// <summary>
        /// Returns all values merged into one from an <see cref="IEnumerable{T}"/> where T is float.
        /// </summary>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static float EnumerableConcat(IEnumerable<float> adders)
            => EnumerableConcat(0, adders);

        /// <summary>
        /// Merges all int values of a list into one value.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static int EnumerableConcat(int origin, IEnumerable<int> adders)
        {
            if (adders == null)
            {
                return origin;
            }
            foreach (int adder in adders)
            {
                origin += adder;
            }
            return origin;
        }
        /// <summary>
        /// Merges all int values of a list into one value.
        /// </summary>
        /// <param name="adders"></param>
        /// <returns></returns>
        public static int EnumerableConcat(IEnumerable<int> adders)
           => EnumerableConcat(0, adders);

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