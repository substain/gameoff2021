using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRoofZone : MonoBehaviour
{
    public GameObject ObjectToHide;
    
    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Player") { 
            ObjectToHide.SetActive(false);
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.gameObject.name == "Player") {
            ObjectToHide.SetActive(true);
        }
    }

}
