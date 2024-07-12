using UnityEngine;

#nullable enable

namespace ElympicsLobbyPackage.DataStorage
{
    internal class JSONSerializingStorageDecorator<T> : IDataStorage<T>
    {
        private readonly IDataStorage<string> _innerStorage;

        public JSONSerializingStorageDecorator(IDataStorage<string> innerStorage)
        {
            _innerStorage = innerStorage;
        }

        public void Clear(string key)
        {
            _innerStorage.Clear(key);
        }

        public T? Get(string key)
        {
            try
            {
                return JsonUtility.FromJson<T>(_innerStorage.Get(key));
            }
            catch
            {
                return default;
            }
        }

        public void Set(string key, T value)
        {
            _innerStorage.Set(key, JsonUtility.ToJson(value));
        }
    }
}
