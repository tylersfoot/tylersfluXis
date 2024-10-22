layout(std140, set = 0, binding = 0) uniform m_DatamoshParameters
{
    vec2 g_TexSize;   // Texture size
    float g_Strength; // Controls how much moshing effect is applied
};

layout(set = 1, binding = 0) uniform texture2D m_CurrentFrame;   // Current frame (Buffer B)
layout(set = 1, binding = 1) uniform texture2D m_PreviousFrame;  // Previous frame (Buffer A)
layout(set = 1, binding = 2) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;

    // Fetch the current frame's pixel color
    vec4 currentColor = texture(sampler2D(m_CurrentFrame, m_Sampler), uv);
    
    // Fetch the previous frame's pixel color
    vec4 previousColor = texture(sampler2D(m_PreviousFrame, m_Sampler), uv);

    // Calculate the difference between current and previous frame's pixel color
    float diff = distance(currentColor.rgb, previousColor.rgb);

    // If the difference is small enough (pixels have not changed much), apply moshing
    vec2 offsetUV = uv;
    if (diff < 0.1 * g_Strength) {
        // Offset UV to create a moshing effect
        vec2 offset = vec2(0.01, 0.01) * vec2(sin(uv.y * 10.0), cos(uv.x * 10.0));
        offsetUV += offset;
    }

    // Sample the previous frame at the offset UV coordinates
    vec4 moshedColor = texture(sampler2D(m_PreviousFrame, m_Sampler), offsetUV);
    
    // Mix the current color with the moshed previous frame color based on the difference and g_Strength
    o_Colour = mix(currentColor, moshedColor, g_Strength * step(diff, 0.1 * g_Strength));
}
