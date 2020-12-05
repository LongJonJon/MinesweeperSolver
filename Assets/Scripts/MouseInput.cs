using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour {
    void LateUpdate () {
        RaycastHit hit;
        Ray ray = this.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);

        if (Physics.Raycast (ray, out hit)) {
            switch (hit.transform.gameObject.layer) {
                case 8:
                    hit.transform.gameObject.GetComponent<Cell> ().Hover ();
                    if (Input.GetMouseButtonDown (0)) this.gameObject.GetComponent<MinesweeperCore> ().Cascade (hit.transform.gameObject.GetComponent<Cell> ().Show ());
                    if (Input.GetMouseButtonDown (1)) {
                        hit.transform.gameObject.GetComponent<Cell> ().Mark ();
                        this.gameObject.GetComponent<MinesweeperCore> ().BombCount ();
                    }
                    break;
                case 9:
                    hit.transform.gameObject.GetComponentInParent<Cell> ().Hover ();
                    if (Input.GetMouseButtonDown (0)) this.gameObject.GetComponent<MinesweeperCore> ().Carve (hit.transform.gameObject.GetComponentInParent<Cell> ().gameObject.name);
                    break;
            }
        }
    }
}