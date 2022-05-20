using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Niantic.ARDK.AR.ARSessionEventArgs;

using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Awareness;
using Niantic.ARDK.AR.Awareness.Depth;
using Niantic.ARDK.AR.Camera;

using Niantic.ARDK.AR.Configuration;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Helpers;

public class DepthBuffer : MonoBehaviour
{
    public ARDepthManager _depthManager;

    public ARCameraPositionHelper _cameraPosHelper;

    //private IDepthBufferProcessor 	_depth

    public IARSession _arSession;

    public IARCamera arCamera;

    //public ARSe
    Texture2D _depthTexture;

    public Material _shaderMaterial;

    public Transform _lightPos;

    Vector2 _halfScreen; 

    private Matrix4x4 _BackProjectionTransform;

    private Matrix4x4 _CameraToWorldTransform;


    // Start is called before the first frame update

    private float _maxDistance;
    private float _minDistance;

    private float _bufferWidth;

    private float _bufferHeight;

    private GameObject _cubeLight;

    private Vector3 _arCameraTransform;

    public bool _toggleLight;
    public bool _toggleDepth;
    public bool _toggleNormal;

    

    void Start()
    {
        //_depthManager.ToggleDebugVisualization(true);
        //_arSession.FrameUpdated += _FrameUpdated;
        ARSessionFactory.SessionInitialized += _OnSessionInitialized;
        _depthManager.DepthBufferUpdated += OnDepthBufferUpdated;
        //_cameraPosHelper.FrameUpdated += _FrameUpdated;
       // ARSessionFactory.SessionInitialized += ARSessionFactory_SessionInitialized;
        //_depthManager.DepthBufferProcessor;
        _halfScreen.x = Screen.width/2;
        _halfScreen.y = Screen.height/2;
        _toggleLight = true;
        _toggleDepth = false;
        _toggleNormal = false;
        //_cubeLight =  GameObject.CreatePrimitive(PrimitiveType.Cube);

    }

    private void OnDestroy()
    {
      ARSessionFactory.SessionInitialized -= _OnSessionInitialized;
    }

     private void _OnSessionInitialized(AnyARSessionInitializedArgs args)
    {
      var oldSession = _arSession;
      if (oldSession != null)
        oldSession.FrameUpdated -= _FrameUpdated;

      var newSession = args.Session;
      _arSession = newSession;
      newSession.FrameUpdated += _FrameUpdated;
       Debug.Log("MY _OnSessionInitialized" );
    }

    private void _FrameUpdated(FrameUpdatedArgs args)
    {
      
      // Set the camera's position.
      var worldTransform = args.Frame.Camera.GetViewMatrix(Screen.orientation).inverse;
      _arCameraTransform = worldTransform.GetColumn(3);
      
    }

    private void OnDepthBufferUpdated(ContextAwarenessArgs<IDepthBuffer> args)
    {
        IDepthBuffer depthBuffer = args.Sender.AwarenessBuffer;
        _maxDistance = depthBuffer.NearDistance;
        _minDistance = depthBuffer.FarDistance;

        // var _intrinsicsFit = args.Sender.AwarenessBuffer.CalculateDisplayTransform;
        _BackProjectionTransform =  args.Sender.BackProjectionTransform;
        _CameraToWorldTransform = args.Sender.CameraToWorldTransform;

        _bufferWidth = depthBuffer.Width;
        _bufferHeight = depthBuffer.Height;

        depthBuffer.CreateOrUpdateTextureRFloat(
            ref _depthTexture
        ); 
        

    }

    // Update is called once per frame
    void Update()
    {
         float d = _depthManager.DepthBufferProcessor.GetDistance((int)_halfScreen.x,(int)_halfScreen.y);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _shaderMaterial.SetTexture("_DepthTex",_depthTexture);
        _shaderMaterial.SetMatrix("_depthTransform",_depthManager.DepthBufferProcessor.SamplerTransform);
        //_shaderMaterial.SetVector("_kLightPos",_lightPos.position);
        _shaderMaterial.SetVector("_kLightPos",_arCameraTransform);

        // back projection is just the inverser of the projection. I strill ned inverse of the camera
        _shaderMaterial.SetMatrix("_backProjectionTransform",_BackProjectionTransform);
        _shaderMaterial.SetMatrix("_cameraToWorldTransform", _CameraToWorldTransform );

        _shaderMaterial.SetFloat("_maxDistance",_maxDistance);
        _shaderMaterial.SetFloat("_minDistance",_minDistance);
    
        _shaderMaterial.SetFloat("_bufferWidth",_bufferWidth);
        _shaderMaterial.SetFloat("_bufferHeight",_bufferHeight);

        _shaderMaterial.SetInt("_toggleLight",_toggleLight ? 1:0);
        _shaderMaterial.SetInt("_toggleDepth",_toggleDepth ? 1:0);
        _shaderMaterial.SetInt("_toggleNormal",_toggleNormal ? 1:0);

        
        Graphics.Blit(source,destination, _shaderMaterial);
    }

}
