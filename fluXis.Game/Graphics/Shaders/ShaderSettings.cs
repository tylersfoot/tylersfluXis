using System.Collections.Generic;

namespace fluXis.Game.Graphics.Shaders;

public static class ShaderSettings
{
    // holds all of the shaders and their parameters/information
    public static Dictionary<string, ShaderInfo> Shaders = new()
    {
        { "Bloom", 
            new ShaderInfo
            {
                Name = "Bloom",
                Description = "Adds a bloom effect to the screen.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the bloom effect.",
                        }
                    }
                }
            }
        },
        { "ChromaticAbberation", 
            new ShaderInfo
            {
                Name = "Chromatic Abberation",
                Description = "Adds a chromatic abberation effect to the screen.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the chromatic abberation effect.",
                        }
                    }
                }
            }
        },
        { "ColorShift", 
            new ShaderInfo
            {
                Name = "Color Shift",
                Description = "Shifts the colors of the screen by a certain amount of degrees.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Degrees", new SliderParameter
                        {
                            Name = "Degrees",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The amount of degrees to shift the colors by. (360 = back to normal)",
                            SliderMin = 0f,
                            SliderMax = 360f,
                            SliderStep = 1f,
                        }
                    }
                }
            }
        },
        // SKIPPING DATAMOSH
        { "Glitch", 
            new ShaderInfo
            {
                Name = "Glitch",
                Description = "Glitches the screen.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the glitch effect.",
                        }
                    },
                    { "BlockSize", new SliderParameter
                        {
                            Name = "Block Size",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The size of the glitch blocks.",
                        }
                    },
                    { "ColorRate", new SliderParameter
                        {
                            Name = "Color Rate",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The rate at which the colors change.",
                        }
                    }
                }
            }
        },
        { "Greyscale", 
            new ShaderInfo
            {
                Name = "Greyscale",
                Description = "Makes the screen greyscale.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the greyscale effect.",
                        }
                    }
                }
            }
        },
        { "Invert", 
            new ShaderInfo
            {
                Name = "Invert",
                Description = "Inverts the colors of the screen.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the invert effect.",
                        }
                    }
                }
            }
        },
        { "Mosaic", 
            new ShaderInfo
            {
                Name = "Mosaic",
                Description = "Adds a mosaic effect to the screen.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the mosaic effect.",
                        }
                    }
                }
            }
        },
        { "Noise", 
            new ShaderInfo
            {
                Name = "Noise",
                Description = "Adds a noise effect to the screen.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the noise effect.",
                        }
                    }
                }
            }
        },
        { "Pixelate", 
            new ShaderInfo
            {
                Name = "Pixelate",
                Description = "Adds a pixelate effect to the screen.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the pixelate effect.",
                        }
                    }
                }
            }
        },
        { "Retro", 
            new ShaderInfo
            {
                Name = "Retro",
                Description = "Adds a retro effect to the screen.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the retro effect.",
                        }
                    }
                }
            }
        },
        { "Vignette", 
            new ShaderInfo
            {
                Name = "Vignette",
                Description = "Adds a vignette effect to the screen.",
                Parameters = new Dictionary<string, ShaderParameter>
                {
                    { "Strength", new SliderParameter
                        {
                            Name = "Strength",
                            Type = ShaderParameterType.Slider,
                            Tooltip = "The strength of the vignette effect.",
                        }
                    }
                }
            }
        },  
    };
}

public class ShaderInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<string, ShaderParameter> Parameters { get; set; }
}

public class ShaderParameter
{
    public string Name { get; set; }
    public ShaderParameterType Type { get; set; }
    public string Tooltip { get; set; } = string.Empty;
    public bool HasRequirements { get; set; } = false; // if true, needs Requirements to be on
    public List<string> Requirements { get; set; } = new(); // names of checkboxes that need to be on for this parameter to work
    public bool IsLocked { get; set; } = false; // if true, the parameter will be disabled in the editor

    public virtual ShaderParameter Clone()
    {
        return (ShaderParameter)MemberwiseClone();
    }
}

public class SliderParameter : ShaderParameter
{
    public float Value { get; set; } = 0f;
    public float DefaultValue { get; set; } = 0f;
    public float SliderMin { get; set; } = 0f;
    public float SliderMax { get; set; } = 1f;
    public float SliderStep { get; set; } = 0.01f;
    public float SliderDefaultValue { get; set; } = 0f;
}

public class DropdownParameter : ShaderParameter
{ 
    // not implemented yet
    public List<string> DropdownValues { get; set; } = new();
    public string Value { get; set; } = string.Empty;

}

public class CheckboxParameter : ShaderParameter
{
    public bool Value { get; set; } = false;
    public bool DefaultValue { get; set; } = false;
    public bool CheckboxDefaultValue { get; set; } = false;
}

public enum ShaderParameterType
{
    Slider,
    Checkbox,
    Dropdown
}
