using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        public void VirtualExitInput()
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }

        public void VirtualPrintScreenInput()
        {
            ScreenCapture.CaptureScreenshot("PrintScreen-" + System.DateTime.UtcNow.ToString("HH:mm:ss - dd_MMMM_yyyy") + ".png");
        }

        public void VirtualSwitchDayNightInput()
        {
            if(RenderSettings.skybox.name == "SkyMidnight") {
                Material skyMaterial = Resources.Load("Skyboxes/Materials/CloudyMorningSky", typeof(Material)) as Material;
                RenderSettings.skybox = skyMaterial;
                Image buttonImage = GameObject.Find("Image_Icon_Day_Night").GetComponent<Image>();
                buttonImage.sprite = Resources.Load("UI/sun", typeof(Sprite)) as Sprite;

                GameObject moon = GameObject.Find("Sun");
                Light moonComponent = moon.GetComponent<Light>();
                moonComponent.color = Color.white;
                RenderSettings.sun = moonComponent;
            } else {
                Material skyMaterial = Resources.Load("Skyboxes/Materials/SkyMidnight", typeof(Material)) as Material;
                RenderSettings.skybox = skyMaterial;
                Image buttonImage = GameObject.Find("Image_Icon_Day_Night").GetComponent<Image>();
                buttonImage.sprite = Resources.Load("UI/moon", typeof(Sprite)) as Sprite;
                
                GameObject moon = GameObject.Find("Sun");
                Light moonComponent = moon.GetComponent<Light>();
                moonComponent.color = Color.white / 2.0f;
                RenderSettings.sun = moonComponent;
            }
            
        }
        
    }

}
