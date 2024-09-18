using System;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Tournament.Util;
using Elympics;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    public class StandaloneExternalAuthenticator : IExternalAuthenticator
    {
        private readonly StandaloneExternalAuthorizerConfig _config;
        public StandaloneExternalAuthenticator(StandaloneExternalAuthorizerConfig config)
        {
            _config = config;
        }

        public async UniTask<ExternalAuthData> InitializationMessage(string gameId, string gameName, string versionName, string sdkVersion, string lobbyPackageVersion)
        {
            // var jsonObject =
            // 	"{\"authData\":{\"jwt\":\"eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIzNDNlNTg4Ni1iMzA2LTRlMzEtYjFlOS1lNGFiZDAzOTM3MGYiLCJhdXRoLXR5cGUiOiJ0ZWxlZ3JhbS1hdXRoIiwiZ2FtZS1pZCI6IjRmMjM5Zjg3LWEzNDUtNDI3NS1hMGYxLTBjNDc0ZTkyMDFkZSIsImdhbWUtbmFtZSI6IkN1dCBUaGUgWmVybyIsInZlcnNpb24tbmFtZSI6IjMyNCIsImNoYWluLWlkIjoiIiwiZXRoLWFkZHJlc3MiOiIweGRhZjVhNTFkNjBlZDEyM2YwN2FiMzg1NmYwZThjNGEwMjZmOTFjOTAiLCJuYmYiOjE3MTk1MjY2NjYsImV4cCI6MTcxOTYxMzA2NiwiaWF0IjoxNzE5NTI2NjY2fQ.kIyMQMQx_wCUv6i3qbz6pl_WghHIXktvo4VjZ9F2Pel5ASUb6PLn2l0GN_Pw7Mq0Ody3OkHWC-yakArD9zJmzIEjlGYNLtfBZ-0XY0-BHE2CMtsy1ulWYC0sCEbHa3Ag7SD65zB79iU0zWoxhsFQJk-fp0FtYrgEDio6glVUayEWLLx3dnlg8v--eJKV1_lzTg7DPJ8Ymmr6B5z8O07SooSW5Jon5E5jje7K_xawhihaQo7GmSaHNqyAQ2yTtvvf5f9ztsfWFt2QT1lFD1IrplRBA3ePmPzcBrM8SwSnx6n7nip13rgN852rP_1uYHMnZWVQaP54wQx_7DpEXGyXDufibLX6Pkrcm_3F2lsNBhpFVKZsflUggmbyR82NbPL_eT6GmYu30xIsDSJrDONWOmgyRBlJD5AcPVeQ262JXODVEJ6Pn1g0rortlkxioPIenpfaUaC2wtO38A4aTjLzzTW5MjzeGwUF82I8IEC7FXbYzLpJac6ZKNeBfHifRZ0IrdXiOdVuV2un6BUUT0nnc_0iplFT7ROTFGsQAKwBCcOOpueTwGF3F3TUjbDpClZThakntD34J9qYT3mMHOFBHJDQj2uel2GYePXDK25xBhideG_J9AdcoGFBNVzV_xsK7Y4Z7jzrnnQ3hwwQ8qDdg_gafo5Z5HkxYPQZoXR6nqU\",\"userId\":\"343e5886-b306-4e31-b1e9-e4abd039370f\",\"nickname\":\"Sleek Koala\"},\"capabilities\":3,\"device\":\"desktop\"}";
            // var toreturn = JsonUtility.FromJson<InitializationResponse>(jsonObject);
            //
            var result = await UniTask.FromResult(new InitializationResponse
            {
                device = "desktop",
                authData = _config.Capabilities is Capabilities.None ? default : new AuthDataRaw()
                {
                    nickname = "StandAloneNickname",
                    jwt = "",
                    userId = Guid.NewGuid().ToString(),
                },
                capabilities = (int)_config.Capabilities,
                closestRegion = _config.ClosestRegion
            });
            if (_config.IsTournamentAvailable)
            {
                result.tournamentData = new TournamentDataDto
                {
                    leaderboardCapacity = 5,
                    name = _config.TournamentName,
                    ownerId = _config.OwnerId,
                    state = _config.TournamentState,
                    createDate = _config.CreatedDate,
                    startDate = _config.StartDate,
                    endDate = _config.EndDate,
                    participants = _config.Participants,
                };
            }
            await ElympicsLobbyClient.Instance!.ConnectToElympicsAsync(new ConnectionData()
            {
                AuthType = _config.AuthType
            });
            return new ExternalAuthData(ElympicsLobbyClient.Instance.AuthData, result.device == "Mobile", (Capabilities)result.capabilities, result.environment, result.closestRegion, result.tournamentData?.ToTournamentInfo());

        }
    }
}
