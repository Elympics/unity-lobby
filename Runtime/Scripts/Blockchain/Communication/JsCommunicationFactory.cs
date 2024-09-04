using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.Blockchain.Communication.DTO;
using UnityEngine;

namespace ElympicsLobbyPackage.Blockchain.Communication
{
	public class JsCommunicationFactory
	{
		private readonly string _protocolVersion;
		public JsCommunicationFactory(string protocolVersion)
		{
			_protocolVersion = protocolVersion;
		}

		public string GetVoidMessageJson<T>(string voidMessageType, T payload)
		{
			var voidMessage = new VoidMessage<T>()
			{
				protocolVersion = _protocolVersion,
				type = voidMessageType,
				message = payload,
			};
			return JsonUtility.ToJson(voidMessage);
		}

        public string GetDebugMessageJson<T>(string debugType, T payload)
        {
            var debugMessage = new DebugMessage<T>()
            {
                debugType = debugType,
                message = payload,
            };
            return JsonUtility.ToJson(debugMessage);
        }

		public string GenerateRequestMessageJson<TInput>(int requestNumber, string type, string address, TInput message)
		{
			var toSerialize = new Message<TInput>()
			{
				protocolVersion = _protocolVersion,
				ticket = requestNumber,
				type = type,
				parameters = new Parameters<TInput>()
				{
					address = address,
					message = message,
				}
			};
			return JsonUtility.ToJson(toSerialize);
		}
	}
}
