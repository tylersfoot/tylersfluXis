﻿using Newtonsoft.Json;

namespace fluXis.Shared.API.Payloads.Auth.Multifactor;

public class TOTPEnablePayload
{
    [JsonProperty("pwd")]
    public string? Password { get; set; }

    [JsonProperty("key")]
    public string? SharedKey { get; set; }

    [JsonProperty("code")]
    public string? Code { get; set; }
}