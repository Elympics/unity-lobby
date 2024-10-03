using System;
using System.Collections;
using System.Collections.Generic;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.JWT;
using UnityEngine;

namespace ElympicsLobbyPackage.Utils
{
    public static class JWTExt
    {
        public static UnityPayload ExtractUnityPayloadFromJwt(this string jwt)
        {
            var payload = JsonWebToken.Decode(jwt, string.Empty, false);
            if (payload is null)
            {
                throw new Exception("Couldn't decode payload form jwt token.");
            }
            var formattedPayload = AuthTypeRaw.ToUnityNaming(payload);
            return JsonUtility.FromJson<UnityPayload>(formattedPayload);
        }
    }
}
