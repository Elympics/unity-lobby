using System;
using Elympics.Models.Authentication;

#nullable enable

namespace ElympicsLobbyPackage.DataStorage
{
    [Serializable]
    internal struct SerializableAuthData
    {
        public string _userId;
        public string _jwtToken;
        public string _nickname;
        public int _authType;

        public SerializableAuthData(AuthData authData)
        {
            _userId = authData.UserId.ToString();
            _jwtToken = authData.JwtToken;
            _authType = (int)authData.AuthType;
            _nickname = authData.Nickname;
        }

        public readonly AuthData? AuthData
        {
            get
            {
                try
                {
                    return new(Guid.Parse(_userId), _jwtToken, _nickname, (AuthType)_authType);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
