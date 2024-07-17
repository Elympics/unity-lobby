using static ElympicsLobbyPackage.DUCK.Crypto.SimpleAESEncryption;

#nullable enable

namespace ElympicsLobbyPackage.DataStorage
{
    internal class EncryptedDataStorageDecorator : IDataStorage<string>
    {
        private readonly IDataStorage<AESEncryptedText> _innerStorage;
        private readonly string _key;

        public EncryptedDataStorageDecorator(IDataStorage<AESEncryptedText> innerStorage, string key)
        {
            _innerStorage = innerStorage;
            _key = key;
        }

        public void Set(string key, string value)
        {
            AESEncryptedText cipherText = Encrypt(value, _key);
            _innerStorage.Set(key, cipherText);
        }

        public string? Get(string key)
        {
            var cipherText = _innerStorage.Get(key);
            return Decrypt(cipherText, _key);
        }

        public void Clear(string key)
        {
            _innerStorage.Clear(key);
        }
    }
}
