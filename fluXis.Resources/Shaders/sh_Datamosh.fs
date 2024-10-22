layout(std140, set = 0, binding = 0) uniform m_DatamoshParameters
{
    vec2 g_TexSize;
    float g_Strength;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
    vec2 uv = gl_FragCoord.xy / g_TexSize;  // UV for the current frame

    // Sample the current frame from the left half of the merged texture
    vec4 currentColor = texture(sampler2D(m_Texture, m_Sampler), uv);

    // Adjust the UV to sample from the right half (previous frame)
    vec2 previousUV = vec2(uv.x * 0.5 + 0.5, uv.y);  // Shift UV horizontally by half of the texture width
    vec4 previousColor = texture(sampler2D(m_Texture, m_Sampler), previousUV);

    // Combine the current and previous colors (datamosh effect)
    vec4 blendedColor = mix(currentColor, previousColor, 0.5);

    o_Colour = currentColor;
}
