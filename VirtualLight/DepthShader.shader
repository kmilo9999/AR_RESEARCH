Shader "Custom/DepthShader"
{
      Properties
    {
        _MainTex ("_MainTexture", 2D) = "green" {}
        _DepthTex("_DepthTex", 2D) = "red" {}
       _lightPos("LighPos", Vector) = (0.65,-0.1,2.394767)
       _lightColor("LightColor",Vector) = (0.9,0.9,0.9,1.0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                //storage for our transformed depth uv
                float3 depth_uv : TEXCOORD1;
            };
            
            // Transforms used to sample the context awareness textures
            float4x4 _depthTransform;
            float4x4 _backProjectionTransform;
            float4x4 _cameraToWorldTransform;
            
            float _maxDistance;
            float _minDistance;

            float _bufferWidth;
            float _bufferHeight;

            float3 _kLightPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                //multiply the uv's by the depth transform to roate them correctly.

                // Camilo notes:
                // compute normal at a pixel. change the position of the light to see how light change
                // according to the position.
                // ligt positon * world to camera 

                o.depth_uv = mul(_depthTransform, float4(v.uv, 1.0f, 1.0f)).xyz;
                return o;
            }
 

           
            //our texture samplers
            sampler2D _MainTex;
            sampler2D _DepthTex;
            float3 _lightPos;
            float4 _lightColor;
            int _toggleLight;
            int _toggleDepth;
            int _toggleNormal;
            
            fixed4 frag (v2f i) : SV_Target
            {                

                // compute normal at a pixel. change the position of the light to see how light change
                // according to the position.
                // ligt positon * world to camera 

                // all the computations in camera space.`

                //our depth texture, we need to normalise the uv coords before using.
                float2 depthUV = float2(i.depth_uv.x / i.depth_uv.z, i.depth_uv.y / i.depth_uv.z);
                // texture coordinates of this fragment

                   

                //read the depth texture pixel
                // this is a distance

                 // Calculate the surface normal using screen-space partial derivatives of the height field
                float dx = 1.0/_bufferWidth;
                float dy = 1.0/_bufferHeight;
                float dzdx=( tex2D(_DepthTex,depthUV+float2(dx,0.0)).x - tex2D(_DepthTex,depthUV+float2(-dx,0.0)).x )/(2.0*dx);
                float dzdy=( tex2D(_DepthTex,depthUV+float2(0.0,dy)).x - tex2D(_DepthTex,depthUV+float2(0.0,-dy)).x )/(2.0*dy);
                float3 n=float3(-dzdx,-dzdy,1.0);
                float3 normal=normalize(n);

                //Look up depth texture
                float depthCol = tex2D(_DepthTex, depthUV).r;

                if(_toggleDepth == 1)
                {
                    return depthCol;
                }
                
                //Get world position from camera
                float4 pointRelativeToCamera = depthCol * mul(_backProjectionTransform,float4(depthUV,1.0f,1.0f));
                float3 worldPos = mul(_cameraToWorldTransform,pointRelativeToCamera);
                //return float4(worldPos.xyz,1);

                float3 sourceColor = tex2D(_MainTex, i.uv).rgb;
                
                float4 normalWorldSpace = mul(_cameraToWorldTransform,float4(normal,1));
                float3 normalWorldSpaceN = normalize(normalWorldSpace.xyz);
                float3 ambient = _lightColor.rgb  * 0.1;

                

                if(_toggleNormal == 1)
                {
                    return float4(normalWorldSpaceN.xyz,1.0 );
                }


                if(_toggleLight == 0)
                {
                    return float4(ambient*sourceColor,1.0 );
                }

                // phong shading
                // float3 lightDir = normalize( worldPos - _kLightPos);
                // float diff = max(dot(normalWorldSpaceN,lightDir),0.0);
                // float3 diffuse = diff * _lightColor.rgb;
                
                // float3 ambient = _lightColor.rgb  * 0.1;
                // float4 result = float4((ambient+diffuse) * sourceColor,1.0);
                // return result;

                
                //cel shading
                float iamb = 0.2;
                float3 lightDir = normalize( worldPos - _kLightPos);
                float diff = max(dot(normalWorldSpace,lightDir),0.0);
                float intensity = iamb + diff;
                float shadeItensity = ceil(intensity * 4)/4;
                return float4(sourceColor *shadeItensity ,1.0);

                // float3 lightDir = normalize( worldPos - _kLightPos);
                // float diff = max(dot(normalWorldSpaceN,lightDir),0.0);
                // float3 celIntensity;
                // if(diff > 0.5)
                // {
                //     celIntensity= float3(1.0,1.0,1.0);
                // }
                // else if(diff > 0.5)
                // {
                //     celIntensity= float3(0.33,0.33,0.33);  
                // }
                // else{
                //     celIntensity= float3(0.0,0.0,0.0);  
                // }


                // return float4(sourceColor *celIntensity ,1.0);
                //return float4(normal,1.0);
            }
            ENDCG
        }
    }
}
