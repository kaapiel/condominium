using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerLittleBedroonDoorController : MonoBehaviour
{
    [SerializeField] private Animator door = null;
    [SerializeField] private bool doorTrigger = false;
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
                        if (door.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
                            if(doorTrigger) {
                                door.Play("OpenLittleBedroomDoor", 0, 0.0f);
                            } else {
                                door.Play("CloseLittleBedroomDoor", 0, 0.0f);
                            }
                        }

                        doorTrigger = !doorTrigger;
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
