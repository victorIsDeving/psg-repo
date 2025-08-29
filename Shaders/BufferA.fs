in vec3 aColor;
in vec4 aPosition;
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

// ballpoint line drawing

// drawing a line segment from previous position to actual
// for every particle (ballpoint tip)


// draw line segment from A to B
float segment(vec2 pos, vec2 p1, vec2 p2, float r)
{
    vec2 g = p2 - p1;
    vec2 h = pos - p1;
    float d = length(h - g * clamp(dot(g, h) / dot(g,g), 0.0, 1.0));
    return smoothstep(r, 0.5*r, d);
}

float sdLine( vec2 pos, vec2 p1, vec2 p2, float crop )
{
    float l=length(p2-p1);
        if(l<.01) return 100000.;
    vec2 t=(p2-p1)/l;
    // crop a little from the ends, so subsequent segments will blend together well
    l-=crop;
    p2-=t*crop*.75;
    p1+=t*crop*.25;
    float pp = dot(pos-p1,t);
    float pn = dot(pos-p1,t.yx*vec2(-1,1));
    return max(max(pp-l,-pp),abs(pn));
}

float segDist( int idx, vec2 pos, float crop )
{
    Particle p,pp;
    readParticle(p,idx,iChannel0);
    readParticle(pp,idx+PNUM,iChannel0);
        //vec2 g=getGrad(p.pos,2.5*iResolution.x/1200.,iChannel0,iResolution.xy)*iResolution.x/1200.;
    //if(length(g)<.01) return 10000.;

    if(length(pos-p.pos)>30.*iResolution.x/1200.) return 10000.;
    if(length(p.pos-pp.pos)>30.*iResolution.x/1200.) return 10000.;
    return sdLine(pos,p.pos,pp.pos,crop);
    //return  segment(pos,p.pos,pp.pos,crop);
}

void main()
{
    Res = iResolution;
    vec3 col=vec3(0.05);
    vec3 c=vec3(0);
    float w=2.7*sqrt(iResolution.x/1200.);

    for(int i=0; i<PNUM; i++)
    {
        c+=(-col+1.)*clamp(w*(0.45+min(0.25,getRand(gl_FragCoord.xy,iChannel1).x))-segDist(i,gl_FragCoord.xy,w*.5),0.,1.);
    }
    /*Particle p;
    readParticle(p,gl_FragCoord.xy,iChannel0);
    float sc =2.5*iResolution.x/1200.;
    vec2 g=getGrad(p.pos,sc,iChannel0,iResolution.xy)*sc;
    if(length(g)<10000.0) g=vec2(0.);
    C=vec4(vec3(g.x,g.y,1.),1.);*/
    C=vec4(c,1.0);
}

