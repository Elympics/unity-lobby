using Elympics.Models.Authentication;
using static ElympicsLobbyPackage.DUCK.Crypto.SimpleAESEncryption;

#nullable enable

namespace ElympicsLobbyPackage.DataStorage
{
    public class AuthDataStorage
    {
        private const string AUTH_DATA_KEY = "PLAYER_AUTH_DATA";
        private const string ENCRYPTION_KEY = "43c66d39dbbf8ca7456b9a8f0701fe20043e8115e20276160514145d0a55eeb8";
        private IDataStorage<SerializableAuthData> _storage;

        public AuthDataStorage()
        {
            _storage = new JSONSerializingStorageDecorator<SerializableAuthData>(
                new EncryptedDataStorageDecorator(
                    new JSONSerializingStorageDecorator<AESEncryptedText>(
                        new PlayerPrefsDataStorage()
                    ),
                    ENCRYPTION_KEY
                )
            );
        }

        // public void Set(AuthData authData)
        // {
        //     _storage.Set(AUTH_DATA_KEY, new SerializableAuthData(authData));
        // }
        // public AuthData? Get()
        // {
        //     return _storage.Get(AUTH_DATA_KEY).AuthData;
        // }

        // public void Clear()
        // {
        //     _storage.Clear(AUTH_DATA_KEY);
        // }
    }
}
