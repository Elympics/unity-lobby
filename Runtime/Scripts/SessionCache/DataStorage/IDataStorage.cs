#nullable enable

namespace ElympicsLobbyPackage.DataStorage
{
    internal interface IDataStorage<T>
    {
        public void Set(string key, T value);
        public T? Get(string key);
        public void Clear(string key);
    }
}
