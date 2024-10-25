using System.Collections.Generic;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Game.Map.Structures.Events;

public class ShaderEvent : IMapEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    // [JsonProperty("shader")]
    // public string ShaderName
    // {
    //     get => Type.ToString();
    //     set
    //     {
    //         if (Enum.TryParse<ShaderType>(value, out var type))
    //             Type = type;
    //         else
    //             Logger.Log($"Failed to parse {value} as {nameof(ShaderType)}!", LoggingTarget.Runtime, LogLevel.Error);
    //     }
    // }

    // [JsonIgnore]
    // public ShaderType Type { get; set; } = ShaderType.Bloom;

    private string shaderName;

    [JsonProperty("shader")]
    public string ShaderName
    {
        get => shaderName;
        set
        {
            // Validate the shader name against the available shaders in ShaderSettings
            if (ShaderSettings.Shaders.ContainsKey(value))
            {
                shaderName = value;
            }
            else
            {
                Logger.Log($"Failed to find shader with name '{value}' in ShaderSettings!", LoggingTarget.Runtime, LogLevel.Error);
                shaderName = "Unknown"; // Assign a default value or handle as needed
            }
        }
    }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    [JsonProperty("use-start")]
    public bool UseStartParams { get; set; }

    // store dynamic parameters as a dictionary, instead of hardcoding fields
    [JsonProperty("start-params")]
    public Dictionary<string, ShaderParameter> StartParameters { get; set; } = new();

    [JsonProperty("end-params")]
    public Dictionary<string, ShaderParameter> EndParameters { get; set; } = new();

    // initialize parameters by copying ShaderSettings
    public void InitializeParameters()
    {
        var shaderInfo = ShaderSettings.Shaders[ShaderName];
        StartParameters = new Dictionary<string, ShaderParameter>();
        EndParameters = new Dictionary<string, ShaderParameter>();

        foreach (var param in shaderInfo.Parameters)
        {
            // deep copy start parameters
            StartParameters[param.Key] = param.Value.Clone();

            // only add sliders to the end parameters
            if (param.Value is SliderParameter)
            {
                EndParameters[param.Key] = param.Value.Clone();
            }
        }
    }

    // set specific start slider parameter
    public void SetStartParameter(string paramName, float value)
    {
        if (StartParameters.ContainsKey(paramName) && StartParameters[paramName] is SliderParameter slider)
        {
            slider.Value = value;
        }
    }

    // set specific end slider parameter
    public void SetEndParameter(string paramName, float value)
    {
        if (EndParameters.ContainsKey(paramName) && EndParameters[paramName] is SliderParameter slider)
        {
            slider.Value = value;
        }
    }

    // get start slider parameter value
    public float GetStartParameter(string paramName)
    {
        if (StartParameters.ContainsKey(paramName) && StartParameters[paramName] is SliderParameter slider)
        {
            return slider.Value;
        }
        return 0f; // default fallback
    }

    // get end slider parameter value
    public float GetEndParameter(string paramName)
    {
        if (EndParameters.ContainsKey(paramName) && EndParameters[paramName] is SliderParameter slider)
        {
            return slider.Value;
        }
        return 0f; // default fallback
    }
}
