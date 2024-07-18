#nullable enable
using System;
using System.Collections.Concurrent;
using ElympicsLobbyPackage.Blockchain.Communication.Exceptions;
using ElympicsLobbyPackage.Blockchain.EditorIntegration;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ElympicsLobbyPackage.ExternalCommunication
{
    internal class RequestMessageDispatcher
    {
        private readonly ConcurrentDictionary<int, Response> _ticketStatus = new();
        private int _requestCounter = 0;

        public RequestMessageDispatcher(IJsCommunicatorRetriever messageRetriever)
        {
            messageRetriever.ResponseObjectReceived += OnResponseObjectReceived;
        }

        public async UniTask<TReturn> RequestUniTaskOrThrow<TReturn>(int ticket)
        {
            Debug.Log($"[ApiConnect][{ticket}] request task");
            await UniTask.WaitUntil(() => IsWaitingForResponse(ticket) is false);
            if (IsErrorResponse(ticket, out var code))
            {
                Debug.Log($"[ApiConnect][{ticket}] found error in response: {ticket}");
                throw new ResponseException(code, GetErrorDescription(ticket));
            }
            var response = GetResponseData<TReturn>(ticket);
            UniTask.ReturnToMainThread();
            return response;
        }

        private bool IsWaitingForResponse(int ticket)
        {
            return _ticketStatus.ContainsKey(ticket) is false;
        }

        private string GetErrorDescription(int ticket)
        {
            var status = _ticketStatus[ticket];
            return RequestErrors.GetErrorMessage(status.status, status.type) + $"{Environment.NewLine}Details: {_ticketStatus[ticket].response}";
        }
        private bool IsErrorResponse(int ticket, out int code)
        {
            code = _ticketStatus[ticket].status;
            return code != 0;
        }
        private void OnResponseObjectReceived(string responseObject)
        {
            Debug.Log($"[ApiConnector] Response received: {responseObject}");
            var response = JsonUtility.FromJson<Response>(responseObject);
            if (_ticketStatus.TryAdd(response.ticket, response) is false)
                Debug.LogError($"Status map already contains response {response}.");
        }

        private TReturn? GetResponseData<TReturn>(int ticket)
        {
            var responseData = _ticketStatus[ticket];

            if (string.IsNullOrEmpty(responseData.response))
                throw new ProtocolException($"Response data is null or empty.", responseData.type);

            if (typeof(TReturn) == typeof(string))
                return (TReturn)(object)_ticketStatus[ticket].response;

            var fromJsonObject = JsonUtility.FromJson<TReturn>(_ticketStatus[ticket].response);
            if (fromJsonObject == null)
                throw new ProtocolException($"Failed to parse response data to {nameof(TReturn)}", responseData.type);

            return fromJsonObject;
        }
    }
}
