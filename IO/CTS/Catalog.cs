using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("WarWolfWorks")]
namespace WarWolfWorks.IO.CTS
{
    /// <summary>
    /// Core class for catalog saving.
    /// </summary>
    public sealed class Catalog
    {
        /// <summary>
        /// A regex expression to find if a line is a category start.
        /// </summary>
        public static readonly Regex Expression_Category_Start = new Regex(@"^\[(?!\/).+\]$");
        /// <summary>
        /// A regex expression to find if a line is a category end.
        /// </summary>
        public static readonly Regex Expression_Category_End = new Regex(@"^\[\/.+\]$");
        /// <summary>
        /// A regex expression to find if a line has a valid name pattern for saving a variable.
        /// </summary>
        public static readonly Regex Expression_Variable_Name = new Regex(@"^\w\s");

        internal static readonly string[] Splitter = new string[] { SVARV_VARIABLE_POINTER };

        /// <summary>
        /// The "Equals", or the string value which "points" or "splits" the value from the name of the variable.
        /// </summary>
        public const string SVARV_VARIABLE_POINTER = " IS ";
        /// <summary>
        /// Character which starts the wrapping of a category.
        /// </summary>
        public const char SVARV_CATEGORY_WRAP_BEGIN = '[';
        /// <summary>
        /// Character which ends the wrapping of a category.
        /// </summary>
        public const char SVARV_CATEGORY_WRAP_END = ']';
        /// <summary>
        /// Character which indicates the end of a category.
        /// </summary>
        public const string SVARV_CATEGORY_END = "/";

        /// <summary>
        /// The file path towards this catalog.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The specific "Category" under which this <see cref="Catalog"/> operates.
        /// </summary>
        public string Category { get; private set; }

        /// <summary>
        /// Returns true when <see cref="Override"/> has been successfully called.
        /// </summary>
        public bool Overriden { get; private set; }

        /// <summary>
        /// All lines of the given category.
        /// </summary>
        private List<Variable> Lines;

        /// <summary>
        /// Pre-Override lines.
        /// </summary>
        private List<Variable> PreviousLines;

        /// <summary>
        /// The starting lines when this catalog was loaded.
        /// </summary>
        private List<Variable> StartLines;

        private readonly string h_Password;

        /// <summary>
        /// Returns true if this <see cref="Catalog"/> is encrypted. (Created with the password constructor)
        /// </summary>
        public bool IsEncrypted => !string.IsNullOrEmpty(h_Password);

        /// <summary>
        /// Returns a line that start the category.
        /// </summary>
        /// <returns></returns>
        public string GetCategoryStart()
        {
            return SVARV_CATEGORY_WRAP_BEGIN + Category + SVARV_CATEGORY_WRAP_END;
        }
        
        /// <summary>
        /// Returns a line that ends the category.
        /// </summary>
        /// <returns></returns>
        public string GetCategoryEnd()
        {
            return SVARV_CATEGORY_WRAP_BEGIN + SVARV_CATEGORY_END + Category + SVARV_CATEGORY_WRAP_END;
        }

        /// <summary>
        /// Returns every line inside of the catalog (this specific category).
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Variable> GetAllLines()
            => Lines;

        private List<Variable> LoadLines()
        {
            List<Variable> toReturn = new List<Variable>();
            List<string> used = new List<string>(File.ReadLines(Path));

            if (RemoveNonCategoryLines(ref used))
            {
                for (int i = 0; i < used.Count; i++)
                {
                    if (IsEncrypted)
                        used[i] = InternalHooks.Decrypt(used[i], h_Password);

                    string[] split = used[i].Split(Splitter, StringSplitOptions.None);
                    Variable toAdd = new Variable(split[0], split[1]);
                    toReturn.Add(toAdd);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Overrides all current variables with a new list.
        /// </summary>
        /// <param name="variables"></param>
        public void Override(List<Variable> variables)
        {
            if (Lines == variables)
                return;

            Overriden = true;
            PreviousLines = Lines;
            Lines = variables;
        }

        /// <summary>
        /// If this <see cref="Catalog"/> has been overriden it cancels it.
        /// </summary>
        public void CancelOverride()
        {
            if(Overriden)
            {
                Lines = PreviousLines;
                PreviousLines = null;
                Overriden = false;
            }
        }

        /// <summary>
        /// Returns the index of variable by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetIndex(string name)
            => Lines.FindIndex(s => s.Name == name);

        /// <summary>
        /// Returns the count of all variables inside this <see cref="Catalog"/>.
        /// </summary>
        /// <returns></returns>
        public int GetCount() => Lines.Count;

        /// <summary>
        /// Moves a variable into a given index.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool Move(string name, int to)
        {
            if (to == -1 || to >= Lines.Count)
            {
                throw new IndexOutOfRangeException("The index given is lower than 0 or higher than the length of the variables in the catalog.");
            }

            int index = Lines.FindIndex(s => s.Name == name);
            if (index == -1)
                return false;

            Variable toUse = Lines[index];
            Lines.RemoveAt(index);
            Lines.Insert(to, toUse);

            return true;
        }

        /// <summary>
        /// Sets or gets a value based off of it's name. If the value retrieved does not exist it will return <see cref="string.Empty"/>
        /// without adding it to the list. If the value set didn't exist previously it will be added to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                string toReturn = string.Empty;
                for (int i = 0; i < Lines.Count; i++)
                {
                    if (Lines[i].Name == name)
                    {
                        toReturn = Lines[i].Value;
                        break;
                    }
                }

                return toReturn;
            }
            set
            {
                bool add = true;
                for (int i = 0; i < Lines.Count; i++)
                {
                    if(Lines[i].Name == name)
                    {
                        Lines[i].Value = value;
                        add = false;
                        break;
                    }
                }

                if(add)
                {
                    Lines.Insert(0, new Variable(name, value));
                }
            }
        }

        /// <summary>
        /// Attempts to retrieve a variable under a given name; If the variable was not found, it will return the defaultValue instead.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetSafe(string name, string defaultValue)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].Name == name)
                {
                    return Lines[i].Value;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Creates a catalog with it's category and path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="category"></param>
        public Catalog(string path, string category)
        {
            Path = path;
            Category = category;

            if (!Expression_Category_Start.IsMatch(GetCategoryStart()))
                throw new ArgumentException(@"Category given has an invalid format; Make sure it has no new lines.");

            if (File.Exists(path))
            {
                Refresh();
            }
            else
            {
                Lines = new List<Variable>();
            }

            StartLines = Lines.ToList();
        }

        /// <summary>
        /// Creates a catalog with it's category and path; Additionally, it encrypts the data before saving it to it's file through the given password.
        /// (Required if you previously saved a catalog with a password, otherwise reading will not be possible.)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="category"></param>
        /// <param name="password"></param>
        public Catalog(string path, string category, string password)
        {
            Path = path;
            Category = category;
            h_Password = password;

            if (!Expression_Category_Start.IsMatch(GetCategoryStart()))
                throw new ArgumentException(@"Category given has an invalid format; Make sure it has no new lines.");

            if (File.Exists(path))
            {
                Refresh();
            }
            else
            {
                Lines = new List<Variable>();
            }

            StartLines = Lines.ToList();
        }

        /// <summary>
        /// Updates the catalog to the current state of the file. If chages were made to this <see cref="Catalog"/>, they will be discarded.
        /// </summary>
        public void Refresh()
        {
            Lines = LoadLines();
        }

        private bool RemoveNonCategoryLines(ref List<string> lines)
        {
            int start = lines.FindIndex(s => Expression_Category_Start.Match(s).Value == GetCategoryStart());
            int end = lines.FindIndex(s => Expression_Category_End.Match(s).Value == GetCategoryEnd());

            if (start == -1 || end == -1)
            {
                return false;
            }

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (i >= end || i <= start)
                    lines.RemoveAt(i);
            }

            return true;
        }

        /// <summary>
        /// Removes all variables inside this catalog.
        /// </summary>
        public void RemoveAll()
        {
            Lines.Clear();
        }

        /// <summary>
        /// Removes a specific variable from this catalog.
        /// </summary>
        /// <param name="variable"></param>
        public bool Remove(Variable variable)
        {
            return Lines.Remove(variable);
        }

        /// <summary>
        /// Attempts to remove a specific variable by it's name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            for(int i = 0; i < Lines.Count; i++)
            {
                if(Lines[i].Name == name)
                {
                    Lines.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds a specific line.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public Variable Find(Predicate<Variable> match)
            => Lines.Find(match);

        /// <summary>
        /// Removes all variables matching the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int RemoveAll(Predicate<Variable> match)
            => Lines.RemoveAll(match);

        /// <summary>
        /// Attempts to rename a existing variable, and returns true if the attempt was successful.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool Rename(string from, string to)
        {
            if (from == to)
                return false;

            for(int i = 0; i < Lines.Count; i++)
            {
                if(Lines[i].Name == from)
                {
                    Lines[i].Name = to;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reorders all variables inside this <see cref="Catalog"/> by the key selector.
        /// </summary>
        /// <param name="keySelector"></param>
        public void OrderBy(Func<Variable, int> keySelector)
        {
            Lines = new List<Variable>(Lines.OrderBy(keySelector));
        }

        /// <summary>
        /// Reverts ALL changes that were made to this catalog.
        /// </summary>
        public void Reset()
        {
            Lines = StartLines;
        }

        /// <summary>
        /// Saves all changes made to this catalog to it's file.
        /// </summary>
        public void Apply()
        {
            if (Lines == StartLines)
                return;

            if(!File.Exists(Path))
            {
                FileStream fs = File.Create(Path);
                fs.Dispose();
            }

            List<string> used = new List<string>(File.ReadAllLines(Path));

            int start = used.FindIndex(s => Expression_Category_Start.Match(s).Value == GetCategoryStart());
            int end = used.FindIndex(s => Expression_Category_End.Match(s).Value == GetCategoryEnd());

            if (start == -1 || end == -1)
            {
                used.Add(GetCategoryStart());
                for (int i = 0; i < Lines.Count; i++)
                {
                    string original = Lines[i].ToString();
                    used.Add(IsEncrypted ? InternalHooks.Encrypt(original, h_Password) : original);
                }
                used.Add(GetCategoryEnd());
            }
            else
            {
                used.RemoveRange(start, end - start + 1);
                used.Insert(start, GetCategoryStart());
                for (int i = 0; i < Lines.Count; i++)
                {
                    string original = Lines[i].ToString();
                    used.Insert(start + i + 1, IsEncrypted ? InternalHooks.Encrypt(original, h_Password) : original);
                }
                used.Insert(start + Lines.Count + 1, GetCategoryEnd());
            }

            File.WriteAllLines(Path, used);

            PreviousLines = null;
            StartLines = Lines.ToList();
        }
    }
}
