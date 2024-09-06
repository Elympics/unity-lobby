using System;
using System.Numerics;
using Cysharp.Threading.Tasks;
using Elympics.Models.Authentication;
using ElympicsLobbyPackage;
using ElympicsLobbyPackage.Authorization;
using ElympicsLobbyPackage.Blockchain.Communication.DTO;
using ElympicsLobbyPackage.ExternalCommunication;
using ElympicsLobbyPackage.Plugins.ElympicsLobby.Runtime.Scripts.ExternalCommunicators;
using NSubstitute;
using UnityEngine;

namespace ElympicsLobby.Tests.PlayMode
{
    public static class ExternalCommunicatorComponentMocker
    {
        public static void MockIExternalAuthenticatorAndSet(this ElympicsExternalCommunicator communicator, AuthData authData, Capabilities mockCapabilities, string environment, string closestRegion)
        {
            var authMock = Substitute.For<IExternalAuthenticator>();
            _ = authMock.InitializationMessage(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(UniTask.FromResult(new ExternalAuthData(authData, true, mockCapabilities, environment, closestRegion)));
            communicator.SetCustomExternalAuthenticator(authMock);
        }

        public static void MockExternalWalletCommunicatorAndSet(this ElympicsExternalCommunicator communicator, string address, string chainId)
        {
            var walletMock = Substitute.For<IExternalWalletCommunicator>();
            walletMock.Connect(Arg.Any<BigInteger>()).Returns(UniTask.FromResult(new ConnectionResponse()
            {
                address = address,
                chainId = chainId,
            }));
            communicator.SetCustomExternalWalletCommunicator(walletMock);
        }

        public static void MockExternalWalletCommunicatorWithDisconnectionAndSet(this ElympicsExternalCommunicator communicator, string address, string chainId)
        {
            var walletMock = Substitute.For<IExternalWalletCommunicator>();
            IWalletConnectionListener listener = null;
            walletMock.When(x => x.SetWalletConnectionListener(Arg.Any<IWalletConnectionListener>())).Do(x =>
            {
                listener = x.Arg<IWalletConnectionListener>();
            });

            walletMock.Connect(Arg.Any<BigInteger>()).ReturnsForAnyArgs(x => UniTask.FromResult(new ConnectionResponse()
            {
                address = address,
                chainId = chainId,
            })).AndDoes(x => listener.OnWalletDisconnected());
            communicator.SetCustomExternalWalletCommunicator(walletMock);
        }

        public static void MockExternalWalletCommunicatorWithConnectedAndSet(this ElympicsExternalCommunicator communicator, string address, string chainId)
        {
            var walletMock = Substitute.For<IExternalWalletCommunicator>();
            IWalletConnectionListener listener = null;
            walletMock.When(x => x.SetWalletConnectionListener(Arg.Any<IWalletConnectionListener>())).Do(x =>
            {
                listener = x.Arg<IWalletConnectionListener>();
            });

            walletMock.Connect(Arg.Any<BigInteger>()).ReturnsForAnyArgs(x => UniTask.FromResult(new ConnectionResponse()
            {
                address = string.Empty,
                chainId = string.Empty,
            })).AndDoes(x => listener.OnWalletConnected(address, chainId));
            communicator.SetCustomExternalWalletCommunicator(walletMock);
        }
    }
}
