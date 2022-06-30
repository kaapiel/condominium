using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleLivingRoomLights : MonoBehaviour
{
    private GameObject actionButton = null;

    void Start() {
        Transform[] trs = GameObject.Find("JoystickCanvas").GetComponentsInChildren<Transform>(true);
        foreach(Transform t in trs) {
            if(t.name == "UI_Virtual_Button_Action") {
                actionButton = t.gameObject;
                break;
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            actionButton.SetActive(true);

            PointerEventData pointer = new PointerEventData(EventSystem.current);
            if (Input.touchCount > 0) {
                pointer.position = Input.GetTouch(0).position;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);
             
                foreach (var go in raycastResults){
                    if (go.gameObject.name == "UI_Virtual_Button_Action") {
                        GameObject piece = GameObject.Find("Living room");
                        foreach (Transform transform in piece.transform)
                        {
                            if (transform.name == "Lights") {
                                foreach(Transform light in transform)
                                {
                                    if(light.GetComponent<Light>().enabled == true)
                                    {
                                        light.GetComponent<Light>().enabled = false;
                                    } else {
                                        light.GetComponent<Light>().enabled = true;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        actionButton.SetActive(false);
    }
}
