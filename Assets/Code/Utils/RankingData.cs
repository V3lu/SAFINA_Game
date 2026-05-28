using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Code.Utils
{
    [System.Serializable]
    public class RankingEntry
    {
        public string tag;
        public float time;
        public bool won;

        public RankingEntry(string tag, float time, bool won)
        {
            this.tag = tag;
            this.time = time;
            this.won = won;
        }
    }

    [System.Serializable]
    public class RankingData
    {
        public List<RankingEntry> entries = new List<RankingEntry>();

        static string FilePath => Path.Combine(Application.persistentDataPath, "ranking.json");

        public static RankingData Load()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    string json = File.ReadAllText(FilePath);
                    RankingData data = JsonUtility.FromJson<RankingData>(json);
                    if (data != null && data.entries != null)
                        return data;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[RankingData] Failed to load ranking data: {ex.Message}");
                }
            }
            return new RankingData();
        }

        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(this, true);
                File.WriteAllText(FilePath, json);
                Debug.Log($"[RankingData] Saved {entries.Count} entries to {FilePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[RankingData] Failed to save ranking data: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new entry.
        /// Sort order: Winners first, then shortest times.
        /// </summary>
        public void AddEntry(string tag, float time, bool won)
        {
            entries.Add(new RankingEntry(tag, time, won));
            entries.Sort((a, b) => 
            {
                if (a.won != b.won)
                {
                    return a.won ? -1 : 1; // Winners first
                }
                // If both won or both lost, sort by time ascending (shortest first)
                return a.time.CompareTo(b.time);
            });
            if (entries.Count > 10)
            {
                entries.RemoveRange(10, entries.Count - 10);
            }
            Save();
        }

        /// <summary>
        /// Returns up to 'count' top entries sorted by fastest time.
        /// </summary>
        public List<RankingEntry> GetTopEntries(int count = 10)
        {
            int n = Mathf.Min(count, entries.Count);
            return entries.GetRange(0, n);
        }
    }
}
