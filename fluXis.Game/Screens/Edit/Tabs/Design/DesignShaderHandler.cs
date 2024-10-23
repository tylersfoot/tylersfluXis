using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Framework.Logging;

namespace fluXis.Game.Screens.Edit.Tabs.Design;

public partial class DesignShaderHandler : CompositeComponent
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public ShaderStackContainer ShaderStack { get; set; }

    protected override void Update()
    {
        base.Update();

        if (ShaderStack is null)
            return;

        var groups = map.MapEvents.ShaderEvents.GroupBy(x => x.Type);

        foreach (var group in groups)
            handleGroup(group.Key, group);
    }

    private void handleGroup(ShaderType type, IEnumerable<ShaderEvent> events)
    {
        var container = ShaderStack.GetShader(type);

        if (container == null)
            return;

        var current = events.LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (current == null)
        {
            resetParameters(container);
            return;
        }

        var progress = (clock.CurrentTime - current.Time) / current.Duration;

        if (progress >= 1)
        {
            applyParameters(container, current.EndParameters);
            return;
        }

        var previous = events.LastOrDefault(e => e.Time < current.Time);
        var startParameters = current.UseStartParams ? current.StartParameters : previous?.EndParameters ?? null;

        if (progress < 0)
        {
            applyParameters(container, startParameters);
            return;
        }

        // Interpolate and apply parameter values
        interpolateAndApplyParameters(container, startParameters, current.EndParameters, current.Time, current.Duration);
    }

    private void resetParameters(ShaderContainer container)
    {
        container.Strength = 0;

        // dynamically reset all shader parameters
        foreach (var param in ShaderSettings.Shaders[container.Type.ToString()].Parameters)
        {
            if (container.GetType().GetProperty(param.Key)?.PropertyType == typeof(float))
            {
                container.GetType().GetProperty(param.Key)?.SetValue(container, 0f);
            }
        }
    }

    private void applyParameters(ShaderContainer container, Dictionary<string, ShaderParameter> parameters)
    {
        foreach (var param in parameters)
        {
            if (param.Value is SliderParameter slider)
            {
                var property = container.GetType().GetProperty(param.Key);
                if (property != null && property.PropertyType == typeof(float))
                {
                    property.SetValue(container, slider.Value);
                }
            }
        }
    }

    private void interpolateAndApplyParameters(ShaderContainer container, Dictionary<string, ShaderParameter> startParameters, Dictionary<string, ShaderParameter> endParameters, double startTime, double duration)
    {
        // Check for null dictionaries
        if (startParameters == null || endParameters == null)
        {
            Logger.Log($"Start or End parameters are null for ShaderType {container.Type}");
            return;
        }

        var currentTime = clock.CurrentTime;

        // Get all properties from the container to log them
        var properties = container.GetType().GetProperties();

        foreach (var param in startParameters)
        {
            Logger.Log($"Processing shader parameter {param.Key}");

            if (param.Value is SliderParameter startSlider)
            {
                // Check if the corresponding end parameter exists
                if (endParameters.TryGetValue(param.Key, out var endParam) && endParam is SliderParameter endSlider)
                {
                    var startValue = startSlider.Value;
                    var endValue = endSlider.Value;

                    // Calculate the interpolated value
                    var interpolatedValue = Interpolation.ValueAt(currentTime, startValue, endValue, startTime, startTime + duration);
                    
                    // Find the property in the container by the parameter name (key)
                    var property = container.GetType().GetProperty(param.Key);

                    // Log the parameter key and whether we found the property or not
                    if (property != null)
                    {
                        Logger.Log($"Found property {property.Name} for parameter {param.Key}");

                        if (property.PropertyType == typeof(float))
                        {
                            // Apply the interpolated value to the container's property
                            property.SetValue(container, interpolatedValue);
                        }
                        else
                        {
                            Logger.Log($"Property {property.Name} is not of type float");
                        }
                    }
                    else
                    {
                        Logger.Log($"Property {param.Key} not found in {container.GetType().Name}");
                    }
                }
                else
                {
                    // Log a warning if the end parameter is missing or null
                    Logger.Log($"End parameter {param.Key} not found or invalid for ShaderType {container.Type}");
                }
            }
        }

        // Log all available properties in the container to compare
        foreach (var prop in properties)
        {
            Logger.Log($"Available property: {prop.Name}, Type: {prop.PropertyType}");
        }
    }



    // private void handleGroup(ShaderType type, IEnumerable<ShaderEvent> events)
    // {
    //     var container = ShaderStack.GetShader(type);

    //     if (container == null)
    //         return;

    //     var current = events.LastOrDefault(e => e.Time <= clock.CurrentTime);

    //     if (current == null)
    //     {
    //         container.Strength = 0;

    //         // Reset BlockSize and ColorRate if it's a GlitchContainer
    //         if (container is GlitchContainer glitchContainer)
    //         {
    //             glitchContainer.BlockSize = 0;
    //             glitchContainer.ColorRate = 0;
    //         }
    //         return;
    //     }

    //     var progress = (clock.CurrentTime - current.Time) / current.Duration;
    //     var endStrength = current.EndParameters.Strength;

    //     if (progress >= 1)
    //     {
    //         container.Strength = endStrength;

    //         // Set BlockSize and ColorRate if it's a GlitchContainer
    //         if (container is GlitchContainer glitchContainer)
    //         {
    //             glitchContainer.BlockSize = current.EndParameters.BlockSize;
    //             glitchContainer.ColorRate = current.EndParameters.ColorRate;
    //         }
    //         return;
    //     }

    //     var previous = events.LastOrDefault(e => e.Time < current.Time);
    //     var startStrength = current.UseStartParams ? current.StartParameters.Strength : previous?.EndParameters.Strength ?? 0;

    //     if (progress < 0)
    //     {
    //         container.Strength = startStrength;

    //         // Set initial BlockSize and ColorRate if it's a GlitchContainer
    //         if (container is GlitchContainer glitchContainer)
    //         {
    //             glitchContainer.BlockSize = current.StartParameters.BlockSize;
    //             glitchContainer.ColorRate = current.StartParameters.ColorRate;
    //         }
    //         return;
    //     }

    //     // Interpolate strength value
    //     container.Strength = Interpolation.ValueAt(clock.CurrentTime, startStrength, endStrength, current.Time, current.Time + current.Duration);

    //     // Interpolate BlockSize and ColorRate for GlitchContainer
    //     if (container is GlitchContainer glitch)
    //     {
    //         var startBlockSize = current.UseStartParams ? current.StartParameters.BlockSize : previous?.EndParameters.BlockSize ?? 0;
    //         var endBlockSize = current.EndParameters.BlockSize;
    //         glitch.BlockSize = Interpolation.ValueAt(clock.CurrentTime, startBlockSize, endBlockSize, current.Time, current.Time + current.Duration);

    //         var startColorRate = current.UseStartParams ? current.StartParameters.ColorRate : previous?.EndParameters.ColorRate ?? 0;
    //         var endColorRate = current.EndParameters.ColorRate;
    //         glitch.ColorRate = Interpolation.ValueAt(clock.CurrentTime, startColorRate, endColorRate, current.Time, current.Time + current.Duration);
    //     }
    // }
}
