layout(std140, set = 0, binding = 0) uniform m_PixelateParameters
{
    vec2 g_TexSize;   // Texture size
    float g_Strength; // 0 = no pixelation, 1 = maximum pixelation
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;

    // Invert g_Strength so 0 = no pixelation, 1 = max pixelation
    float pixelSizeFactor = mix(1.0, min(g_TexSize.x, g_TexSize.y), 1.0 - g_Strength);
    
    // Correct aspect ratio to ensure square pixels
    vec2 pixelSize = vec2(pixelSizeFactor, pixelSizeFactor * (g_TexSize.y / g_TexSize.x));

    // Snap UV to the center of the pixel grid
    vec2 pixelatedUV = (floor(uv * pixelSize) + 0.5) / pixelSize;

    // Sample the texture at the pixelated UV coordinates, explicitly using mipmap level 0 (highest resolution)
    vec4 colour = textureLod(sampler2D(m_Texture, m_Sampler), pixelatedUV, 0.0);
    
    // Output the pixelated color
    o_Colour = colour;
}
