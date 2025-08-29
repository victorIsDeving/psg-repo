out vec4 C;
uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;
uniform vec2 iResolution;
uniform vec4 iMouse;
uniform float iTime;
uniform int iFrame;


// created by florian berger (flockaroo) - 2018
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// nanquim line drawing

// accumulating up the line segments
// slowly fading out older cotent


void main()
{
    //Res = iResolution;
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    //C = max(texture(iChannel0,uv),clamp(texture(iChannel1,uv)-.003,0.,1.));
    //C = clamp(texture(iChannel0,uv)+texture(iChannel1,uv)-.003,0.,1.);
    C = (texture(iChannel0,uv)+texture(iChannel1,uv))*(1.-.006/2000.*float(PNUM));
    C.w=1.;
    if(iFrame<20) C=vec4(0,0,0,1);
}
