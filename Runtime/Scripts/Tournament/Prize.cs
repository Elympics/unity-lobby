#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElympicsLobbyPackage.Tournament
{
    public struct Prize
    {
        public string? Name { get; init; }
        public int? Amount { get; init; }
        public int Position { get; init; }
        public Texture2D? Texture { get; init; }
        public Prize[]? Prizes { get; init; }
    }
}
