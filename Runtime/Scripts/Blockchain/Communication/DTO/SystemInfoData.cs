using System;

namespace ElympicsLobbyPackage
{
    [Serializable]
    internal struct SystemInfoData
    {
        public int systemMemorySize;
        public string operatingSystemFamily;
        public string operatingSystem;
        public int graphicsMemorySize;
        public string graphicsDeviceVersion;
        public int graphicsDeviceVendorID;
        public string graphicsDeviceVendor;
        public string graphicsDeviceName;
        public int graphicsDeviceID;
    }
}
