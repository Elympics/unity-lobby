using System;
using UnityEngine;

namespace ElympicsLobbyPackage.Blockchain.WebGLTools
{
    /// <summary>
    /// Base for browser events receivers. https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html
    /// </summary>
    public abstract class BaseWebGLEventsReceiver : MonoBehaviour
    {
        /// <summary>
        /// The receiver needs to be on a Game Object that has the same name as declared in SendMessage in index.html
        /// </summary>
        protected abstract string RequiredGameObjectName { get; }
        
        protected virtual string LogPrefix => "[WebGL Events]";

        private void Awake()
        {
            if (gameObject.name != RequiredGameObjectName)
            {
                InternalLog($"The {GetType().Name} will not work if the gameObject name does not match object name in event handling in index.html" +
                            $" Expected {RequiredGameObjectName}, but was {gameObject.name}.",
                    LogType.Error);
            }
        }

        protected void InternalLog(string message, LogType logType = LogType.Log)
        {
            Action<string, UnityEngine.Object> logMethod = logType switch
            {
                LogType.Error => Debug.LogError,
                _ => Debug.Log
            };
            message = $"{LogPrefix} {message}";
            logMethod.Invoke(message, this);
        }
    }
}