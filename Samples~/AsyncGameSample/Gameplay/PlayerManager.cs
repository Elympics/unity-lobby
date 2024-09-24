using UnityEngine;
using Elympics;

namespace ElympicsLobbyPackage.Sample.AsyncGame
{
    public class PlayerManager : MonoBehaviour, IInputHandler
    {
        private readonly ElympicsBool isOnTheLeft = new ElympicsBool(false);
        public bool IsOnTheLeft => isOnTheLeft.Value;
        public void SetIsOnTheLeft(bool onTheLeft) => isOnTheLeft.Value = onTheLeft;

        private int currentInput;
        public void SetCurrentInput(int newDirection) => currentInput = newDirection;

        public void OnInputForBot(IInputWriter inputSerializer) { }

        public void OnInputForClient(IInputWriter inputSerializer)
        {
            inputSerializer.Write(currentInput);
            currentInput = 0;
        }
    }
}
