using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ElympicsLobbyPackage.Blockchain.Trust.Command
{
    public abstract class TrustIncreaseCommand
    {
        private TrustIncreaseCommandState _state = TrustIncreaseCommandState.NotStarted;
        public TrustIncreaseCommandState State => _state;
        public string Title => TitleForState(State);
        public string Description => DescriptionForState(State);

        public async UniTask Execute()
        {
            if (State != TrustIncreaseCommandState.NotStarted)
                throw new Exception("[TrustIncrease] command already started");
            try
            {
                _state = TrustIncreaseCommandState.Signing;
                await DoExecute();
                _state = TrustIncreaseCommandState.Finished;
            }
            catch (Exception exception)
            {
                Debug.Log($"[TrustIncrease] failed: {exception}");
                _state = TrustIncreaseCommandState.Failed;
                throw;
            }
        }

        protected abstract UniTask DoExecute();
        public abstract int EstimatedTimeMilliseconds { get; }

        public abstract string TitleForState(TrustIncreaseCommandState state);
        public abstract string DescriptionForState(TrustIncreaseCommandState state);

        protected void ProceedToMiningStateIfPossible()
        {
            if (State == TrustIncreaseCommandState.Signing)
                _state = TrustIncreaseCommandState.Mining;
        }

    }
}
