#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElympicsLobbyPackage
{
    public readonly struct SmartContract
    {
        public string? ChainId { get; init; }
        public string Address { get; init; }
        public string ABI { get; init; }

    }
}
