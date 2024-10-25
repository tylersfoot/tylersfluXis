using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Shared.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using SixLabors.ImageSharp;

namespace fluXis.Game.Map;

public class MapEvents : IDeepCloneable<MapEvents>
{
    [JsonProperty("laneswitch")]
    public List<LaneSwitchEvent> LaneSwitchEvents { get; private set; } = new();

    [JsonProperty("flash")]
    public List<FlashEvent> FlashEvents { get; private set; } = new();

    [JsonProperty("pulse")]
    public List<PulseEvent> PulseEvents { get; private set; } = new();

    [JsonProperty("playfieldmove")]
    public List<PlayfieldMoveEvent> PlayfieldMoveEvents { get; private set; } = new();

    [JsonProperty("playfieldscale")]
    public List<PlayfieldScaleEvent> PlayfieldScaleEvents { get; private set; } = new();

    [JsonProperty("playfieldfade")]
    public List<PlayfieldFadeEvent> PlayfieldFadeEvents { get; private set; } = new();

    [JsonProperty("playfieldrotate")]
    public List<PlayfieldRotateEvent> PlayfieldRotateEvents { get; private set; } = new();

    [JsonProperty("hitfade")]
    public List<HitObjectFadeEvent> HitObjectFadeEvents { get; private set; } = new();

    [JsonProperty("hitease")]
    public List<HitObjectEaseEvent> HitObjectEaseEvents { get; private set; } = new();

    [JsonProperty("shake")]
    public List<ShakeEvent> ShakeEvents { get; private set; } = new();

    [JsonProperty("shader")]
    public List<ShaderEvent> ShaderEvents { get; private set; } = new();

    [JsonProperty("beatpulse")]
    public List<BeatPulseEvent> BeatPulseEvents { get; private set; } = new();

    [JsonProperty("scroll-multiply")]
    public List<ScrollMultiplierEvent> ScrollMultiplyEvents { get; private set; } = new();

    [JsonProperty("time-offset")]
    public List<TimeOffsetEvent> TimeOffsetEvents { get; private set; } = new();

    [JsonProperty("notes")]
    public List<NoteEvent> NoteEvents { get; private set; } = new();

    [JsonIgnore]
    public bool Empty => LaneSwitchEvents.Count == 0
                         && FlashEvents.Count == 0
                         && PulseEvents.Count == 0
                         && PlayfieldMoveEvents.Count == 0
                         && PlayfieldScaleEvents.Count == 0
                         && PlayfieldRotateEvents.Count == 0
                         && PlayfieldFadeEvents.Count == 0
                         && HitObjectFadeEvents.Count == 0
                         && HitObjectEaseEvents.Count == 0
                         && ShakeEvents.Count == 0
                         && ShaderEvents.Count == 0
                         && BeatPulseEvents.Count == 0
                         && ScrollMultiplyEvents.Count == 0
                         && TimeOffsetEvents.Count == 0
                         && NoteEvents.Count == 0;


    // public static T Load<T>(string content)
    //     where T : MapEvents, new()
    // {
    //     if (!content.Trim().StartsWith('{'))
    //         return new T().loadLegacy(content) as T;

    //     var events = content.Deserialize<T>();
    //     return events.Sort() as T;
    // }

    public static T Load<T>(string content)
        where T : MapEvents, new()
    {
        // legacy legacy check
        if (!content.Trim().StartsWith('{'))
            return new T().loadLegacy(content) as T;

        // First, parse the content into a JObject to inspect and modify if necessary.
        var jsonObject = JsonConvert.DeserializeObject<JObject>(content);

        // Check if it contains shader events, and if so, iterate over them to detect and convert the legacy format.
        var shaderEvents = jsonObject["shader"]?.ToObject<JArray>();

        if (shaderEvents != null)
        {

            foreach (var shaderEvent in shaderEvents)
            {
                try
                {
                    var shaderEventObject = shaderEvent as JObject;

                    // Get the shader name from the event
                    string shaderName = shaderEventObject["shader"]?.ToString();

                    if (ShaderSettings.Shaders.TryGetValue(shaderName, out var shaderInfo))
                    {
                        Logger.Log($"fuck! {shaderName}");
                        if (shaderEventObject["start-params"] is JObject startParamsObject && shaderEventObject["end-params"] is JObject endParamsObject)
                        {
                            var convertedStartParams = new JObject();
                            var convertedEndParams = new JObject();

                            // Iterate through all parameters in the shader's settings and match with the legacy format by name
                            foreach (var shaderParam in shaderInfo.Parameters)
                            {
                                string shaderParamKey = shaderParam.Key; // e.g., Strength, BlockSize, etc.
                                ShaderParameter paramInfo = ShaderSettings.ConvertToParameterType(shaderParam.Value);

                                // Normalize the shaderParamKey to kebab-case for comparison with legacy format e.g., strength, block-size, etc.
                                // string jsonKey = string.Concat(shaderParamKey.Select((x, i) =>
                                //     i > 0 && char.IsUpper(x) ? "-" + char.ToLower(x) : char.ToLower(x).ToString()));
                                string jsonKey = shaderParamKey;

                                Logger.Log($"jeez! {startParamsObject}");

                                // Try to match the legacy parameter in the start-params and end-params objects
                                float? startValue = startParamsObject.ContainsKey(jsonKey) ? startParamsObject[jsonKey].Value.Value<float>() : null;
                                float? endValue = endParamsObject.ContainsKey(jsonKey) ? endParamsObject[jsonKey].Value<float>() : null;

                                Logger.Log($"god damn it balls hell no! {startParamsObject.ContainsKey(jsonKey)} {startParamsObject} {jsonKey} {shaderParamKey}");
                                // If both start and end values are found, proceed
                                if (startValue.HasValue && endValue.HasValue)
                                {
                                    Logger.Log($"fucking balls hell yeah {startValue}");

                                    if (paramInfo is SliderParameter slider) // add checkbox shit later
                                    {
                                        // Create new parameter objects with the appropriate value
                                        var newStartParam = new SliderParameter
                                        {
                                            Name = slider.Name,
                                            Tooltip = slider.Tooltip,
                                            SliderMin = slider.SliderMin,
                                            SliderMax = slider.SliderMax,
                                            SliderStep = slider.SliderStep,
                                            Value = startValue.Value
                                        };

                                        var newEndParam = new SliderParameter
                                        {
                                            Name = slider.Name,
                                            Tooltip = slider.Tooltip,
                                            SliderMin = slider.SliderMin,
                                            SliderMax = slider.SliderMax,
                                            SliderStep = slider.SliderStep,
                                            Value = endValue.Value
                                        };

                                        convertedStartParams[shaderParamKey] = JObject.FromObject(newStartParam);
                                        convertedEndParams[shaderParamKey] = JObject.FromObject(newEndParam);
                                    }
                                }
                            }

                            // Replace arrays with dictionaries in the shader event object
                            shaderEventObject["start-params"] = convertedStartParams;
                            shaderEventObject["end-params"] = convertedEndParams;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and skip this shader event
                    Logger.Error(ex, $"Error processing shader event");
                    continue; // Skip to the next shader event
                }
            }

            jsonObject["shader"] = shaderEvents;
        }


        // Now, convert the modified JObject back to a JSON string
        var updatedContent = jsonObject.ToString();

        // Proceed with deserializing the modified JSON content to MapEvents
        var events = JsonConvert.DeserializeObject<T>(updatedContent);
        return events.Sort() as T;
    }

    // private static string toKebabCase(string input)
    // {
    //     return string.Concat(input.Select((x, i) =>
    //         i > 0 && char.IsUpper(x) ? "-" + char.ToLower(x) : char.ToLower(x).ToString()));
    // }


    private MapEvents loadLegacy(string content)
    {
        var lines = content.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            int index = line.IndexOf('(');
            int index2 = line.IndexOf(')');

            if (index == -1 || index2 == -1)
                continue;

            var type = line[..index];
            var args = line[(index + 1)..index2].Split(',');

            switch (type)
            {
                case "LaneSwitch":
                {
                    var laneSwitch = new LaneSwitchEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Count = int.Parse(args[1])
                    };

                    if (args.Length > 2)
                        laneSwitch.Duration = float.Parse(args[2], CultureInfo.InvariantCulture);

                    LaneSwitchEvents.Add(laneSwitch);
                    break;
                }

                case "Flash":
                    if (args.Length < 8)
                        continue;

                    float duration = float.Parse(args[1], CultureInfo.InvariantCulture);
                    bool inBackground = args[2] == "true";
                    Easing easing = (Easing)Enum.Parse(typeof(Easing), args[3]);
                    Colour4 startColor = Colour4.FromHex(args[4]);
                    float startOpacity = float.Parse(args[5], CultureInfo.InvariantCulture);
                    Colour4 endColor = Colour4.FromHex(args[6]);
                    float endOpacity = float.Parse(args[7], CultureInfo.InvariantCulture);

                    FlashEvents.Add(new FlashEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Duration = duration,
                        InBackground = inBackground,
                        Easing = easing,
                        StartColor = startColor,
                        StartOpacity = startOpacity,
                        EndColor = endColor,
                        EndOpacity = endOpacity
                    });
                    break;

                case "Pulse":
                    PulseEvents.Add(new PulseEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture)
                    });
                    break;

                case "PlayfieldMove":
                    if (args.Length < 4)
                        continue;

                    PlayfieldMoveEvents.Add(new PlayfieldMoveEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        OffsetX = float.Parse(args[1], CultureInfo.InvariantCulture),
                        Duration = float.Parse(args[2], CultureInfo.InvariantCulture),
                        Easing = Enum.TryParse<Easing>(args[3], out var ease) ? ease : Easing.None
                    });
                    break;

                case "PlayfieldScale":
                    if (args.Length < 5)
                        continue;

                    PlayfieldScaleEvents.Add(new PlayfieldScaleEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        ScaleX = float.Parse(args[1], CultureInfo.InvariantCulture),
                        ScaleY = float.Parse(args[2], CultureInfo.InvariantCulture),
                        Duration = float.Parse(args[3], CultureInfo.InvariantCulture),
                        Easing = Enum.TryParse<Easing>(args[4], out var ease2) ? ease2 : Easing.None
                    });
                    break;

                case "Shake":
                    if (args.Length < 3)
                        continue;

                    ShakeEvents.Add(new ShakeEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Duration = float.Parse(args[1], CultureInfo.InvariantCulture),
                        Magnitude = float.Parse(args[2], CultureInfo.InvariantCulture)
                    });
                    break;

                case "PlayfieldFade":
                    if (args.Length < 3)
                        continue;

                    PlayfieldFadeEvents.Add(new PlayfieldFadeEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        Duration = float.Parse(args[1], CultureInfo.InvariantCulture),
                        Alpha = float.Parse(args[2], CultureInfo.InvariantCulture)
                    });
                    break;
                case "Shader":
                    // ! PROBABLY DOESNT WORK
                    if (args.Length < 3)
                        continue;

                    var startIdx = line.IndexOf('{');
                    var endIdx = line.LastIndexOf('}');

                    // returns -1 if {} is missing
                    if (startIdx == -1 || endIdx == -1)
                        continue;

                    var dataJson = line[startIdx..(endIdx + 1)];

                    // Get the shader name (args[1]) and fetch its settings
                    var shaderName = args[1];
                    if (!ShaderSettings.Shaders.TryGetValue(shaderName, out _))
                        continue; // if no matching shader, skip

                    // Parse the legacy dataJson into a Dictionary
                    var parsedData = JsonConvert.DeserializeObject<Dictionary<string, float>>(dataJson);

                    // Create a new ShaderEvent and populate its EndParameters dynamically
                    var shaderEvent = new ShaderEvent
                    {
                        Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                        ShaderName = shaderName
                    };

                    // Initialize the parameters based on the shader settings
                    shaderEvent.InitializeParameters();

                    foreach (var kvp in parsedData)
                    {
                        if (shaderEvent.EndParameters.ContainsKey(kvp.Key) && shaderEvent.EndParameters[kvp.Key] is SliderParameter slider)
                        {
                            // Assign the parsed float value to the corresponding parameter
                            slider.Value = kvp.Value;
                        }
                    }

                    ShaderEvents.Add(shaderEvent);
                    break;

                // previous code
                // case "Shader":
                //     if (args.Length < 3)
                //         continue;

                //     var startIdx = line.IndexOf('{');
                //     var endIdx = line.LastIndexOf('}');

                //     if (startIdx == -1 || endIdx == -1)
                //         continue;

                //     var dataJson = line[startIdx..(endIdx + 1)];
                //     var data = dataJson.Deserialize<ShaderEvent.ShaderParameters>();

                //     ShaderEvents.Add(new ShaderEvent
                //     {
                //         Time = float.Parse(args[0], CultureInfo.InvariantCulture),
                //         ShaderName = args[1],
                //         EndParameters = data
                //     });
                //     break;
            }
        }

        return Sort();
    }

    public MapEvents Sort()
    {
        LaneSwitchEvents.Sort(compare);
        FlashEvents.Sort(compare);
        PulseEvents.Sort(compare);
        PlayfieldMoveEvents.Sort(compare);
        PlayfieldScaleEvents.Sort(compare);
        PlayfieldFadeEvents.Sort(compare);
        HitObjectFadeEvents.Sort(compare);
        HitObjectEaseEvents.Sort(compare);
        ShakeEvents.Sort(compare);
        ShaderEvents.Sort(compare);
        BeatPulseEvents.Sort(compare);
        PlayfieldRotateEvents.Sort(compare);
        ScrollMultiplyEvents.Sort(compare);
        TimeOffsetEvents.Sort(compare);
        NoteEvents.Sort(compare);

        return this;
    }

    private static int compare(ITimedObject a, ITimedObject b)
    {
        var val = a.Time.CompareTo(b.Time);
        return val == 0 ? a.GetHashCode().CompareTo(b.GetHashCode()) : val;
    }

    public string Save() => Sort().Serialize();

    public MapEvents DeepClone()
    {
        if (MemberwiseClone() is not MapEvents clone)
            throw new InvalidOperationException("Failed to clone MapEvents");

        clone.LaneSwitchEvents = new List<LaneSwitchEvent>(LaneSwitchEvents);
        clone.FlashEvents = new List<FlashEvent>(FlashEvents);
        clone.PulseEvents = new List<PulseEvent>(PulseEvents);
        clone.PlayfieldMoveEvents = new List<PlayfieldMoveEvent>(PlayfieldMoveEvents);
        clone.PlayfieldScaleEvents = new List<PlayfieldScaleEvent>(PlayfieldScaleEvents);
        clone.PlayfieldRotateEvents = new List<PlayfieldRotateEvent>(PlayfieldRotateEvents);
        clone.PlayfieldFadeEvents = new List<PlayfieldFadeEvent>(PlayfieldFadeEvents);
        clone.HitObjectFadeEvents = new List<HitObjectFadeEvent>(HitObjectFadeEvents);
        clone.HitObjectEaseEvents = new List<HitObjectEaseEvent>(HitObjectEaseEvents);
        clone.ShakeEvents = new List<ShakeEvent>(ShakeEvents);
        clone.ShaderEvents = new List<ShaderEvent>(ShaderEvents);
        clone.BeatPulseEvents = new List<BeatPulseEvent>(BeatPulseEvents);
        clone.ScrollMultiplyEvents = new List<ScrollMultiplierEvent>(ScrollMultiplyEvents);
        clone.TimeOffsetEvents = new List<TimeOffsetEvent>(TimeOffsetEvents);
        clone.NoteEvents = new List<NoteEvent>(NoteEvents);
        return clone;
    }
}
