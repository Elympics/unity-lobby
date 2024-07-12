#nullable enable
using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using ElympicsLobbyPackage.ExternalCommunication;
using JetBrains.Annotations;
using UnityEngine;

namespace ElympicsLobbyPackage.Blockchain.Communication
{
	internal class JsCommunicator : MonoBehaviour, IJsCommunicatorRetriever
	{
		public const string ProtocolVersion = "0.1.0";
		public event Action<string>? ResponseObjectReceived;
		public event Action<string>? WebObjectReceived;

		private int _requestCounter = 0;
		private const string GameObjectName = "JsReceiver";

		private static JsCommunicator instance = null!;
		private JsCommunicationFactory _messageFactory = null!;
		private RequestMessageDispatcher _dispatcher = null!;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(this);
				gameObject.name = GameObjectName;
				_messageFactory = new JsCommunicationFactory(ProtocolVersion);
                _dispatcher = new RequestMessageDispatcher(this);
            }
			else
				Destroy(gameObject);
		}

		public async UniTask<TReturn> SendRequestMessage<TInput, TReturn>(string messageType, TInput payload)
		{
			var ticket = _requestCounter;
			++_requestCounter;
			var message = _messageFactory.GenerateRequestMessageJson(ticket, messageType, string.Empty, payload);
			Debug.Log($"[{nameof(JsCommunicator)}] Send {messageType} message: {message}");
			DispatchHandleMessage(message);
			return await _dispatcher.RequestUniTaskOrThrow<TReturn>(ticket);
		}

		public void SendVoidMessage<TInput>(string messageType, TInput payload)
		{
			var message = _messageFactory.GetVoidMessageJson(messageType, payload);
			Debug.Log($"[{nameof(JsCommunicator)}] Send {messageType} message: {message}");
			DispatchVoidMessage(message);
		}

		[UsedImplicitly]
		public void HandleResponse(string responseObject)
		{
			ResponseObjectReceived?.Invoke(responseObject);
		}

		[UsedImplicitly]
		public void HandleWebEvent(string messageObject)
		{
			WebObjectReceived?.Invoke(messageObject);
		}

		[UsedImplicitly]
		[DllImport("__Internal")]
		public static extern void DispatchMessage(string eventName, string json);

		private static void DispatchHandleMessage(string json)
		{
#if UNITY_EDITOR || !UNITY_WEBGL
			Debug.Log($"[JS]: Handle Message {json}");
#else
			DispatchMessage(ReactHandlers.HandleMessage, json);
#endif

		}

		private static void DispatchVoidMessage(string json)
		{
#if UNITY_EDITOR || !UNITY_WEBGL
			Debug.Log($"[JS]: Void Message {json}");
#else
			DispatchMessage(ReactHandlers.VoidMessage, json);
#endif

		}
	}
}
