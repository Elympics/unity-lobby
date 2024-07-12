#nullable enable
using System;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage.JWT;
using Newtonsoft.Json;

namespace ElympicsLobbyPackage.Blockchain.Communication
{
    public static class AuthDataExtension
    {
        public static ElympicsJwt ElympicsJwt(this AuthData authData)
        {
            var tokenAsString = JWT.JsonWebToken.Decode(authData.JwtToken, "", false);
            return JsonConvert.DeserializeObject<ElympicsJwt>(tokenAsString);
        }

        public static string ETHAddress(this AuthData authData)
        {
            if (authData.AuthType != AuthType.EthAddress)
                throw new Exception($"Auth data of type {authData.AuthType} does not contain ETH Address");
            return authData.ElympicsJwt().EthAddress;
        }
    }
}
