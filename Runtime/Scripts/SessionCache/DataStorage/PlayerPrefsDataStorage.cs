using UnityEngine;

#nullable enable
namespace ElympicsLobbyPackage.DataStorage
{
    internal class PlayerPrefsDataStorage : IDataStorage<string>
    {
        public void Clear(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        public string? Get(string key) => PlayerPrefs.GetString(key);

        public void Set(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
    }
}
