﻿using MessagePack;

namespace fluXis.Game.Online.Spectator;

[MessagePackObject]
public class SpectatorState
{
    [Key(0)]
    public long? MapID { get; set; }
}