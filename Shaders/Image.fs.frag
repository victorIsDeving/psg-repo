uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;
uniform float iTime;
uniform vec4 iMouse;
uniform vec2 iResolution;
uniform bool shiftKEY;
uniform int iFrame;
out vec4 C;

void main()
{

    C.xyz=texture(iChannel0,gl_FragCoord.xy/iResolution.xy).xyz;
    C.w=1.0;
}
