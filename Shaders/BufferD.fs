out vec4 C;
uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;
uniform vec2 iResolution;
uniform vec4 iMouse;
uniform float iTime;
uniform int iFrame;

#define Res2 textureSize(iChannel2,0)
#define Res1 textureSize(iChannel1,0)
//#define iResolution Res0

#define zoom 1.
#define Res  iResolution.xy

#define randSamp iChannel1
#define colorSamp iChannel0

const float BlackFill=1.;
const float bright=1.1;
const float contourStrength=1.;
const float reflection=.5;
const float reflectStrength=.1;
const float reflectSize=.25;
#define reflectPos (Res*(.2+.15*sin(iTime+vec2(0,2.))))

const float BGAlpha=0.;
vec4  getRand(vec2 pos)
{
    return texture(iChannel1,pos/Res/iResolution.y*1200.);
}



#define AngleNum 3

#define SampNum 9

#define PI2 6.28318530717959
#define N(a) ((a).yx*vec2(1,-1))
//const float BlackFill=1.;
const float flicker=1.;
const float flickerFreq=10.;

float htPattern(vec2 pos, float phase)
{
    float pat=0.;
    float cnt=0.;
    vec2 offs=vec2(.001,.1)*floor(iTime*flickerFreq)/10.*flicker;
    float phaseOffs = 10.*getRand(floor(iTime*flickerFreq)*vec2(.01,.1)).x*flicker;
    vec2 gr=/*getGrad(floor(pos/13.)*13.,1.)+*/1.01*normalize(pos-.5*Res);
    for(float ang=0.0;ang<PI2;ang+=PI2/4.3)
    {
        vec2 b=normalize(sin(vec2(0,PI2/4.) + ang + phase + phaseOffs + 0.6 )*vec2(.5,1.5));
        vec2 uv=((pos.x-pos.y*pos.y*.0004)*b+(pos.y+pos.x*pos.x*.0004)*N(b))/Res1*vec2(7,.3)*.3;
        pat+=.5*texture(iChannel1,uv*.25+offs).x;
        pat+=1.*texture(iChannel1,uv+offs).x;
        cnt+=1.5;
    }
    return pat/cnt;
}

float halfTone(float val,vec2 pos, float phase)
{
    return smoothstep(.6,1.4,val+htPattern(pos,phase));
}



vec4 getCol(vec2 pos)
{
    // take aspect ratio into account
    vec2 uv=pos/Res.xy;
    vec4 c1=texture(iChannel0,uv);
    vec4 e=smoothstep(vec4(-0.05),vec4(-0.0),vec4(uv,vec2(1)-uv));
    c1=mix(vec4(1,1,1,0),c1,e.x*e.y*e.z*e.w);
    float d=clamp(dot(c1.xyz,vec3(-.5,1.,-.5)),0.0,1.0);
    vec4 c2=vec4(.7);
    return min(mix(c1,c2,1.8*d),.7);
}

vec4 getColHT(vec2 pos)
{
    return smoothstep(0.795,1.05,getCol(pos)*.8+.2+1.0);
}

float getVal(vec2 pos)
{
    vec4 c=getCol(pos);
    return pow(dot(c.xyz,vec3(.333)),1.)*1.;
}

vec2 getGrad(vec2 pos, float eps)
{
    vec2 d=vec2(eps,0.);
    return vec2(
        getVal(pos+d.xy)-getVal(pos-d.xy),
        getVal(pos+d.yx)-getVal(pos-d.yx)
    )/eps/2.;
}


float lum( vec3 c)
{
    return dot(c, vec3(0.3, 0.59, 0.11));
}


vec3 clipcolor( vec3 c)
{
    float l = lum(c);
    float n = min(min(c.r, c.g), c.b);
    float x = max(max(c.r, c.g), c.b);

     if (n < 0.0)
     {
         c.r = l + ((c.r - l) * l) / (l - n);
         c.g = l + ((c.g - l) * l) / (l - n);
         c.b = l + ((c.b - l) * l) / (l - n);
     }
     if (x > 1.25)
     {
         c.r = l + ((c.r - l) * (1.0 - l)) / (x - l);
         c.g = l + ((c.g - l) * (1.0 - l)) / (x - l);
         c.b = l + ((c.b - l) * (1.0 - l)) / (x - l);
     }
     return c;
 }

 vec3 setlum( vec3 c,  float l)
{
     float d = l - lum(c);
     c = c + vec3(d);
     return clipcolor(0.85*c);
 }

#define AngleNum 3

#define SampNum 9

#define PI2 6.28318530717959
 /*

         Cube Circle Sketch
         ------------------

         I see a lot of really classy geometric styled pencil drawings on
     the internet by various mathematical artists -- The effort that
     goes into some of them is really impressive. Reproducing them in
     code form is kind of cheating, but it's a little hard to resist. :)

         This particular arrangement is pretty common and something I've
     seen many times in various forms, but most tend to be based off of
     the works of mathematical artist Regolo Bizzi -- His Escheresque
     geometric designs are everywhere, but if you've never chanced upon
     one of them, I've provided a link below. A lot of love and effort
     would have gone into the original sketch, but the effort that went
     into the code version was far less impressive: Render 12 hexagons
     on the border of a circle, orient them a bit (by PI/6), then shade
     the faces.

     There's some extra code to give it that tech drawing feel, fake
     lighting and a mediocre sketch algorithm included, but that's it.

         Anyway, I was more interested in producing a halfway passable look
         in order to render more interesting patterns. Suggestions for
         simple improvements are always welcome. :)



     // Links.

     // You can find some of Regolo Bizzi's work at the following:
     http://impossible-world.blogspot.com/2014/10/new-images-by-regolo-bizzi.html
     https://www.behance.net/regolo

 */


 // Cube subdivision lines. Interesting, but a bit much, I think. :)
 //#define SUBDIV_LINES

 // Greyscale sketch -- Sans color.
 //#define GREYSCALE

 // Blinking lights: It was an attempt to animate the sketch in a believable
 // way... This isn't it. :D
 //#define BLINKING_LIGHTS

float t(vec2 uv, float a){

    float d = step(uv.y,1.+tan(-PI/2.+a)*(1.-uv.x));
    float b = step(uv.y,uv.x*tan(a));

    if(a>PI/4.){
        d *= b;
    } else {
        d += b;
    }

    return clamp(d,0.,1.);
}


 void main(){

     // Aspect correct screen coordinates.
     vec2 uv = (gl_FragCoord.xy - iResolution.xy*.5)/iResolution.y;

     // Scaling and translation.
     const float gSc = 1.;

     // Smoothing factor.
     float sf = gSc/iResolution.y;

     // Rotation speed -- Rotation seemed like a good idea at the time, but
     // I decided against it. :)
     float rT = 0.;//sin(-iTime/6.*3.)/12.;

     // Scaling and rotation.
     vec2 p = rot2(rT)*uv*gSc;

     // Distance field holders for the cubes, lines and the previous
     // cube (used for CSG related to overlap).
     float d = 1e5, ln = 1e5, prevD = 1e5;

     // Edge width.
     const float ew = .0025;


     // The cubes are rendered along the sides of a dodecahedron. These are just some standard
     // measurements to help place the cubes in the correct positions.
     const float cR = .3; // Larger circle radius.
     const float cAp = cR*cos(6.2831/24.); // Apothem.
     const float sL = cR*acos(6.2831/24.)/2.; // Side length, which will relate to the hexagon scale.


     // The offset vertex information.
     // Hexagon vertices with scaling to enable rendering back in normal space.
     vec2 hSc = sL*vec2(.5, .8660254);
     vec2[6] svV = vec2[6](vID[0]*hSc, vID[1]*hSc, vID[2]*hSc, vID[3]*hSc, vID[4]*hSc, vID[5]*hSc);


     // Coordinate perturbation. There's small rigid one to enhance the hand-drawn look, and
     // a larger animated one to wave the paper around a bit.
     vec2 offs = vec2(fbm(p*16.), fbm(p*16. + .35));
     vec2 offs2 = vec2(fbm(p*1. + iTime/4.), fbm(p*1. + .5 + iTime/4.));
     const float oFct = .007;
     const float oFct2 = .05;
     p -= (offs - .5)*oFct;
     p -= (offs2 - .5)*oFct2;




     float lnL = -cR - sL*.75; // Line length.
     float a0 = 6.2831/24.; // Initial reference angle.
     float dA = 6.2831/12.; // One twelth revolution.
     float inR =  cR - sL/2.*.8660254; // Inner radius.


     // Some distance field holders.
     float gHex = 1e5, gD = 1e5, qLn = 1e5, dSh = 1e5;
     // Z buffer, for shadows.
     float zBuf = 0.;



     // Fake lighting.
     vec3 lp = vec3(-.75, 3, -1.5);
     vec3 ld = normalize(lp - vec3(uv, 0));
     ld.xy = rot2(rT)*ld.xy;


     // Initialize the background.
     vec3 col = vec3(.95, .975, 1);


     vec2 q;

     // Apply some graph lines.
     // float dim = 18.; // For the lines to match up: dim = 9./(cR*3.);
     //q = p;//uv*gSc - (offs - .5)*oFct  - (offs2 - .5)*oFct2;//uv*gSc - (offs - .5)*oFct;//p;//
     //q = abs(mod(q, 1./dim) - .5/dim);
     //float ln3 = abs(max(q.x, q.y) - .5/dim);
     //col = mix(col, vec3(.05, .35, .7), (1. - smoothstep(0., sf*2., ln3))*.8);
     /*dim *= 2.;
     q = p;//uv*gSc - (offs - .5)*oFct  - (offs2 - .5)*oFct2;//uv*gSc - (offs - .5)*oFct;//p;//
     q = abs(mod(q, 1./dim) - .5/dim);
     ln3 = max(abs(max(q.x, q.y) - .5/dim), -ln3);
     col = mix(col, vec3(.35, .65, 1), (1. - smoothstep(0., sf*2., ln3))*.65);*/

     // Applying light and a bit of noise to the background.
     col *= clamp(dot(normalize(vec3(p, -1.5)), ld), 0., 1.)*.2 + .8;
     col *= fbm(p*8.)*.1 + .9;
     C=vec4(col,1.0);

     // Rough gamma correction and output to screen.

     vec2 pos = (iResolution.xy-gl_FragCoord.xy)*vec2(-1.,1.);
     vec3 c=getCol(pos,iChannel2,iResolution.xy).xyz;
     float bright=dot(c,vec3(0.2126, 0.7152, 0.0722));
     float blackTone=halfTone(bright*1.5+.25,(pos-Res*.5)*zoom*1.2,floor(sqrt(bright)*8.)/8.*2.7);

     blackTone = mix(1., blackTone,BlackFill);
     float refl=clamp(pow((col.x*(blackTone)),1.),0.,1.);
     vec4 origCol =vec4(c,1.0);
     vec3 col3=col;
     //col3*=vec3(col.x)*blackTone;
     col3+=.1*getRand(pos*.7).xxx;
     C=vec4(col3,1.0);
     vec2 pd = gl_FragCoord.xy/iResolution.y;

     vec2 pos_d = fract(pd*32.);
     vec2 id = floor(pd*32.);

     float a = (id.x-id.y)/2.-iTime*5.;
     float ti = PI*fract(a)/2.;

     float di = t(pos, ti);
     a = floor(mod(a,2.));
     di = mod(a + di, 2.);

     if(di>0.5)
     {


         vec3 giz = texture(iChannel0,pos/iResolution.xy).xyz;
         vec4 gray = getGray(pos,iResolution.xy,iChannel2);
         float dg = length(giz);
         vec3 cinv = vec3(1.)-c;
//         vec3 blue= vec3(0.07,0.85,0.83); //original
         vec3 white = vec3(1.,1.,1.);
         col3*=(1.-.95*mix(giz,mix(1.2*cinv,(gray.x)*white,gray.x),clamp(dg,0.0,1.0)));
         //col3=giz;
         //C=vec4(col3,1.0);


         C = vec4(col3*1.1+.1*getRand(pos*.7).xyz-.1*getRand(pos*.7-.6).xyz,1);
         //C.xyz=1.-C.xyz;
         C.w=mix(1.,1.-min(min(C.x,C.y),C.z),BGAlpha);
     }
     else
     C=texture(iChannel3,gl_FragCoord.xy/iResolution);

 }
