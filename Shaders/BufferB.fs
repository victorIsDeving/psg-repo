//uniform vec2 iResolution;
uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;
uniform vec2 iResolution;
uniform vec4 iMouse;
uniform float iTime;
uniform int iFrame;
uniform bool resetKEY;
out vec4 C;

// created by florian berger (flockaroo) - 2018
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// ballpoint line drawing

// some particles (the actual ballpoint tips)

#define N(v) (v.yx*vec2(1,-1))

void propagate(inout Particle p)
{
    float dt=.04 + 0.025*(0.5+0.5*sin(iTime*TWOPI/10.));
    p.pos+=p.vel*dt;
    float sc=(iResolution.x/1500.);

    // gradient, its length, and unit vector
    vec2 g = 1.0*getGrad(p.pos,2.5*sc,iChannel2,iResolution.xy)*sc;
    // add some noise to gradient so plain areas get some texture
    float gl=length(g);

    //vec2 g_r =g+ (getRand(p.pos/sc,iChannel1).xy-.5)*.001;  //getRand is pixel based so we divide arg by sc so that it looks the same on all scales
    vec2 g_r =g+ (getRand(p.pos/sc,iChannel1).xy-.5)*0.003;  //getRand is pixel based so we divide arg by sc so that it looks the same on all scales
    //g+=normalize(p.pos-iResolution.xy*.5)*.001;
    gl = length(g_r);
    vec2 gu=normalize(g_r);

    // calculate velocity change
    vec2 dvel=vec2(0);

    float dir = (float(p.idx%2)*2.-1.); // every 2nd particle is bent left/right

    // apply some randomness to velocity
    dvel += .75*(getRand(p.pos/sc,iChannel1).xy-.5)/(.03+gl*gl)*sc;

    // vel tends towards gradient
    dvel -= 20.*gu*(1.+sqrt(gl*2.))*sc;

    // vel tends towards/away from normal to gradient (every second particle)
    dvel -= 15.*N(gu)/(1.+1.*sqrt(gl*100.))*sc*dir;

    // vel bends right/left (every second particle)
    //dvel += p.vel.yx*vec2(1,-1)*.06;
    dvel += .1*N(p.vel)/(1.+gl*50.)*dir;

    p.vel += dvel;

    // minimum vel
    //p.vel = normalize(p.vel)*max(length(p.vel),30.*sc);

    // anisotropic vel damping
    p.vel-=gu*dot(p.vel,gu)*(.1+2.*gl);
    //p.vel-=gu*dot(p.vel,gu)*.1;
    p.vel-=N(gu)*dot(p.vel,N(gu))*-.02;
    //p.vel*=.95;
    //float ge=length(g_r);
    //p.vel*=ge;
}

void main()
{
    Res = iResolution;
    int lNum = 20;
    Particle p;

    //if(ti>0.85)

    int idx = particleIdx(gl_FragCoord.xy,iChannel0);
    readParticle(p,idx%PNUM,iChannel0);
    vec2 pi = p.pos/iResolution.y;

    //vec2 pos = fract(p*12.);
    vec2 id = floor(pi*12.);

    float a =-(id.x-id.y)/15.-iTime*.75;
    float ti = abs(PIOVERTWO*fract(a));
    float fact=max(1/300.,1/iFrame);

    if ((idx<PNUM)/*&&(((PIOVERTWO)-ti)>.8*fact)*/)
    {
        // make a little pre propagation, so particles start already near a bigger gradient
        float sc=(iResolution.x/1200.);

        // gradient, its length, and unit vector
        vec2 g = 1.0*getGrad(p.pos,2.5*sc,iChannel2,iResolution.xy)*sc;
        float gl= length(g);
        float ge =exp(-gl*gl*2.);
        p.pos -= 0.01*ge*mix(1.,2.*getRand(idx,iChannel1).x,.8)/g;
        propagate(p);
        propagate(p);
        propagate(p);
        //propagate(p);
        //propagate(p);
        int atOnce=PNUM/100;
        //if (int(getRand(iFrame%PNUM,iChannel1).x*float(PNUM/2)) == p.idx/30) p.pos=getRand((iFrame+p.idx)%PNUM,iChannel1).xy*iResolution.xy;
        //if (int(getRand(iFrame).x*float(PNUM/atOnce)) == p.idx/atOnce)
        if ((p.idx+iFrame)%lNum == lNum-1)
        {
            p.pos=getRand(p.idx+iFrame+iFrame/19,iChannel1).yz*iResolution.xy;
            for(int i=0;i<50;i++) propagate(p);
        }
        //initParticle(p);
    } else if (idx>PNUM*2) discard;
    if (iFrame<20) initParticle(p,iChannel0,iChannel1,iFrame);
    //if (idx>PNUM) { p.pos=vec2(0,0); p.vel=vec2(1,1); readParticle(p,idx-PNUM); }
    writeParticle(p,C,gl_FragCoord.xy,iChannel0);

}

