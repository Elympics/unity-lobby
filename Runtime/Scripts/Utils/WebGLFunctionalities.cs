using System;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Protocol.WebMessages.Models;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    internal class WebGLFunctionalities: IDisposable
    {
        private readonly JsCommunicator _jsCommunicator;

        public WebGLFunctionalities(JsCommunicator jsCommunicator)
        {
#if UNITY_WEBGL_API
            WebGLInput.captureAllKeyboardInput = true;
#endif
            _jsCommunicator = jsCommunicator;
            _jsCommunicator.WebObjectReceived += OnWebMessageReceived;

        }
        private static void OnWebMessageReceived(WebMessageObject messageObject)
        {
            switch (messageObject.type)
            {
                case WebMessages.WebGLKeyboardInputControl:
                    OnKeyboardInputControlsRequested(messageObject.message);
                    break;
            }
        }

        private static void OnKeyboardInputControlsRequested(string webMessageMessage)
        {
            var inputControlRequest = JsonUtility.FromJson<WebGLKeyboardInputControlMessage>(webMessageMessage);
#if UNITY_WEBGL_API
            WebGLInput.captureAllKeyboardInput = !inputControlRequest.isKeyboardControlRequested;
#endif
        }
        public void Dispose()
        {
            _jsCommunicator.WebObjectReceived -= OnWebMessageReceived;
        }
    }
}
