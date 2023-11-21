using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace QKit
{

    public static class HighscoreKeeper
    {
        #region Variables
        private static List<KeyValuePair<string, float>> _entries = new();
        private static AndroidHighscoreKeeper _androidSaveRunner = null;
        public static int maxEntriesToKeep = 10;
        public static string filePath = "/QKitResources/Highscores/";
        public static string saveFileName = "Save.txt";
        public static string defaultFileName = "Default.txt";

        //public static string savePathResources = "QKitResources/Highscores/Save";
        //public static string defaultPathResources = "QKitResources/Highscores/Default";
        public static string savePathStreamingAssets = Path.Combine(Application.persistentDataPath + filePath + saveFileName);
        public static string defaultPathStreamingAssets = Path.Combine(Application.streamingAssetsPath + filePath + defaultFileName);
        #endregion

        #region Properties
        /// <summary>
        /// A sorted list of all highscore entries, where Entries[i].Key is the name and Entries[i].Value is the score
        /// </summary>
        public static List<KeyValuePair<string, float>> Entries => _entries;
        /// <summary>
        /// A string of \n separated names in order from high to low
        /// </summary>
        public static string NameList
        {
            get
            {
                string names = "";
                for (int i = 0; i < _entries.Count; i++)
                {
                    names += _entries[i].Key;
                    if (i + 1 != _entries.Count)
                        names += "\n";
                }
                return names;
            }
        }
        /// <summary>
        /// A string of \n separated scores in order from high to low
        /// </summary>
        public static string ScoreList
        {
            get
            {
                string scores = "";
                for (int i = 0; i < _entries.Count; i++)
                {
                    scores += _entries[i].Value;
                    if (i + 1 != _entries.Count)
                        scores += "\n";
                }
                return scores;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialises HighscoreKeeper and loads the scores. You must call this prior to any use.
        /// </summary>
        public static void Initialise()
        {
            _androidSaveRunner = null;

            if (!Directory.Exists(Path.Combine(Application.persistentDataPath + filePath)))
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath + filePath));

            LoadEntries();
        }
        /// <summary>
        /// Attempt to add a new entry to the list with Key name and Value score. Returns true if the score makes it onto the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public static bool ValidateNewEntry(string name, float score)
        {
            KeyValuePair<string, float> entry = new(name, score);
            _entries.Add(entry);
            _entries = SortEntries(_entries);

            if (_entries.Count <= maxEntriesToKeep)
                return true;

            _entries.RemoveAt(_entries.Count - 1);
            return (_entries.Contains(entry));
        }
        /// <summary>
        /// Attempt to add a new entry to the list with Key name and Value score. Returns true if the score makes it onto the list, and outputs the position on the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public static bool ValidateNewEntry(string name, float score, out int position)
        {
            KeyValuePair<string, float> entry = new(name, score);
            _entries.Add(entry);
            _entries = SortEntries(_entries);
            position = _entries.IndexOf(entry);

            if (_entries.Count <= maxEntriesToKeep)
                return true;

            _entries.RemoveAt(_entries.Count - 1);
            return (_entries.Contains(entry));
        }
        /// <summary>
        /// Updates the name of the entry at the position provided
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        public static void UpdateEntryName(int position, string name)
        {
            KeyValuePair<string, float> newEntry = new(name, _entries[position].Value);
            _entries[position] = newEntry;
        }
        /// <summary>
        /// Load the saved high score entries from file into memory (will load default entries if no data found)
        /// </summary>
        public static void LoadEntries()
        {
            _entries.Clear();

            if (File.Exists(savePathStreamingAssets))
            {
                Debug.Log($"File found at {savePathStreamingAssets}");
                UnpackEntriesFromStringArray(LoadStringArrayFromFile(savePathStreamingAssets));
                Debug.Log($"Loaded file found at {savePathStreamingAssets}");
            }
            else
            {
                Debug.Log($"No file found at {savePathStreamingAssets}");
                ResetEntriesToDefault();
                Debug.Log($"Loaded default as no file found at {savePathStreamingAssets}");
            }
            _entries = SortEntries(_entries);
        }
        /// <summary>
        /// Save the current list of high scores to file
        /// </summary>
        public static void SaveEntries()
        {
            _entries = SortEntries(_entries);
            SaveStringArrayToFile(PackEntriesToStringArray(), savePathStreamingAssets);
        }
        /// <summary>
        /// Resets the saved high score file to default - this will permanently delete existing high score data
        /// </summary>
        public static void ResetEntriesToDefault()
        {
            LoadDefaultEntries();
            SaveEntries();
        }
        /// <summary>
        /// Returns the list of default high scores
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, float>> GetDefaultEntries()
        {
            List<KeyValuePair<string, float>> list = new();
            foreach (string save in LoadStringArrayFromFile(defaultPathStreamingAssets))
            {
                if (save == null || save == "")
                    continue;

                string[] splitData = save.Split(':');
                KeyValuePair<string, float> pair = new(splitData[0], float.Parse(splitData[1]));
                list.Add(pair);

                list = SortEntries(list);
            }
            return list;
        }
        #endregion

        #region Private Methods
        private static void LoadDefaultEntries()
        {
            _entries.Clear();

            if (Application.platform == RuntimePlatform.Android)
            {
                if (_androidSaveRunner == null)
                {
                    GameObject obj = new();
                    _androidSaveRunner = obj.AddComponent(typeof(AndroidHighscoreKeeper)) as AndroidHighscoreKeeper;
                    MonoBehaviour.Instantiate(obj);
                }
                _androidSaveRunner.Run(defaultPathStreamingAssets);

                return;
            }

            UnpackEntriesFromStringArray(LoadStringArrayFromFile(defaultPathStreamingAssets));
            _entries = SortEntries(_entries);
        }

        private static string[] LoadStringArrayFromFile(string path)
        {
            StreamReader reader = new(path);
            string[] array = new string[maxEntriesToKeep];
            string line;
            int i = 0;
            while ((line = reader.ReadLine()) != null && line != "" && i < maxEntriesToKeep)
            {
                array[i] = line;
                i++;
            }
            reader.Close();
            return array;
        }

        private static void SaveStringArrayToFile(string[] array, string path)
        {
            string save = "";
            foreach (string entry in array)
            {
                save += entry + "\n";
            }
            File.WriteAllText(path, save);
        }

        private static string[] PackEntriesToStringArray()
        {
            string[] array = new string[maxEntriesToKeep];
            for (int i = 0; i < maxEntriesToKeep && i < _entries.Count; i++)
            {
                array[i] = $"{_entries[i].Key}:{_entries[i].Value}";
            }
            return array;
        }

        private static void UnpackEntriesFromStringArray(string[] saveStrings)
        {
            foreach (string save in saveStrings)
            {
                if (save == null || save == "")
                    continue;

                string[] splitData = save.Split(':');
                KeyValuePair<string, float> pair = new(splitData[0], float.Parse(splitData[1]));
                _entries.Add(pair);
            }
        }

        private static List<KeyValuePair<string, float>> SortEntries(List<KeyValuePair<string, float>> list)
        {
            list.Sort((x, y) => y.Value.CompareTo(x.Value));
            return list;
        }
        #endregion

    }
}
