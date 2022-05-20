using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventsHandler : MonoBehaviour
{
  [SerializeField]
    private Text _toggleLightButtonText = null;
    
[SerializeField]
    private Text _toggleDepthButtonText = null;

    [SerializeField]
    private Text _toggleNormalButtonText = null;

    public  GameObject _camera;


    public void ToggleLight()
    {
        
        _camera.GetComponent<DepthBuffer>()._toggleLight = !_camera.GetComponent<DepthBuffer>()._toggleLight;
        if(_camera.GetComponent<DepthBuffer>()._toggleLight )
        {
            _toggleLightButtonText.text = "Disable light";
          
        }
        else{
            _toggleLightButtonText.text = "Enable light";
            _camera.GetComponent<DepthBuffer>()._toggleDepth = false; 
            _camera.GetComponent<DepthBuffer>()._toggleNormal = false;
            _toggleDepthButtonText.text = "Toggle depth (Off)";
             _toggleNormalButtonText.text = "Normal Map (Off)";
        }
    }

     public void ToggleDepth(){
        _camera.GetComponent<DepthBuffer>()._toggleNormal = false;
         _camera.GetComponent<DepthBuffer>()._toggleDepth = !_camera.GetComponent<DepthBuffer>()._toggleDepth;
        if(_camera.GetComponent<DepthBuffer>()._toggleDepth )
        {
            _camera.GetComponent<DepthBuffer>()._toggleLight = false;
            _toggleLightButtonText.text = "Enable light";
            _toggleDepthButtonText.text = "Toggle depth (On)";
        }
        else{
              _camera.GetComponent<DepthBuffer>()._toggleLight = true;
            _toggleLightButtonText.text = "Disable light";
            _toggleDepthButtonText.text = "Toggle depth (Off)";
        }
     }

      public void ToggleNormal()
      {

         _camera.GetComponent<DepthBuffer>()._toggleNormal = !_camera.GetComponent<DepthBuffer>()._toggleNormal;
         _camera.GetComponent<DepthBuffer>()._toggleDepth = false;
       
        if(_camera.GetComponent<DepthBuffer>()._toggleNormal )
        {
            _camera.GetComponent<DepthBuffer>()._toggleLight = false;
            _toggleLightButtonText.text = "Enable light";
            _toggleNormalButtonText.text = "Normal Map (On)";
        }
        else{
            _camera.GetComponent<DepthBuffer>()._toggleLight = true;
            _toggleLightButtonText.text = "Disable light";
            _toggleNormalButtonText.text = "Normal Map (Off)";
        }
      }
}
