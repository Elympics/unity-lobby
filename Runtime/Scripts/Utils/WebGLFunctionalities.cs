using System;
using ElympicsLobbyPackage.Blockchain.Communication;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Protocol.WebMessages.Models;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    internal class WebGLFunctionalities
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
                default:
                    throw new ArgumentException($"Message type {messageObject.type} is not supported");
            }
        }

        private static void OnKeyboardInputControlsRequested(string webMessageMessage)
        {
            var inputControlRequest = JsonUtility.FromJson<WebGLKeyboardInputControlMessage>(webMessageMessage);
#if UNITY_WEBGL_API
            WebGLInput.captureAllKeyboardInput = !inputControlRequest.isKeyboardControlRequested;
#endif
        }
    }
}