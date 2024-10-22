layout(std140, set = 0, binding = 0) uniform m_GlitchParameters
{
    vec2 g_TexSize;        // Texture size
    float g_Strength;      // Vibration strength
    float g_Time;          // Time value
    float g_BlockSize;     // Vibration block size (0 = pixel size, 1 = whole screen)
    float g_ColorRate;     // Color separation rate (0.0 to 1.0)
};

layout(set = 1, binding = 0) uniform texture2D m_Texture; 
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

// Improved random function for blockiness
highp float random(highp vec2 st, float seed)
{
    // Mix the time value into the randomness, but use UV as well to create spatial consistency
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233) + seed)) * 43758.5453123);
}


void main(void)
{
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    
    // Compute block size in pixels, where 0 -> pixel-sized blocks and 1 -> whole screen-sized block
    float blockSizeInPixels = mix(1.0, min(g_TexSize.x, g_TexSize.y), g_BlockSize);
    
    // Divide UV space into blocks, where each block will have the same shift
    vec2 blockUV = floor(uv * blockSizeInPixels) / blockSizeInPixels;
    
    // Generate a random shift for the block
    float randomShift = (random(blockUV, g_Time) - 0.5) * g_Strength;

    // Apply the horizontal shift to the entire block
    vec2 fixedUV = uv;
    fixedUV.x += randomShift;
    
    // Sample the texture at the modified UV coordinates
    vec4 pixelColor = textureLod(sampler2D(m_Texture, m_Sampler), fixedUV, 0.0);

    // Apply color separation by shifting the red and blue channels
    pixelColor.r = textureLod(sampler2D(m_Texture, m_Sampler), fixedUV + vec2(g_ColorRate, 0.0), 0.0).r;
    pixelColor.b = textureLod(sampler2D(m_Texture, m_Sampler), fixedUV + vec2(-g_ColorRate, 0.0), 0.0).b;

    // Output the final pixel color with glitch effect applied
    o_Colour = pixelColor;
}
