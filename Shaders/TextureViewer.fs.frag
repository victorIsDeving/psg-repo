out vec4 FragColor;
in  vec2 TexCoords;


uniform sampler2D fboAttachment;
uniform vec2 iResolution;
uniform float iTime;
void main()
{
    vec2 pos = gl_FragCoord.xy/iResolution;
    FragColor = texture(fboAttachment, pos);
}
