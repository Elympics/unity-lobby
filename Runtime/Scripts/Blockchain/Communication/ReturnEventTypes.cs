namespace ElympicsLobbyPackage.Blockchain.Communication
{
    public static class ReturnEventTypes
    {
        public const string SignTypedData = "SignTypedData";
        public const string Connect = "Connect";
        public const string Dummy = "Dummy";
        public const string EncodeFunctionData = "EncodeFunctionData";
        public const string GetValue = "GetValue";
        public const string SendTransaction = "SendTransaction";
        public const string Handshake = "Handshake";
    }

    public static class WebMessages
    {
        public const string WalletConnection = "WalletConnection";
        public const string WebGLKeyboardInputControl = "WebGLKeyboardInputControl";
    }

    public static class VoidEventTypes
    {
        public const string GameStateChanged = "GameStateChanged";
        public const string ShowConnectToWallet = "ShowConnectUI";
        public const string ShowChainSelectionUI = "ShowChainSelectionUI";
        public const string GameFinished = "GameFinished";
        public const string ShowAccountUI = "ShowAccountUI";
        public const string ApplicationInitialized = "ApplicationInitialized";
        public const string Debug = "Debug";
    }

    public static class DebugMessageTypes
    {
        public const string RTT = "RTT";
    }

    public static class ReactHandlers
    {
        public const string HandleMessage = "HandleMessage";
        public const string VoidMessage = "HandleVoidMessage";
    }
}
