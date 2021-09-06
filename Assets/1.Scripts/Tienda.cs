using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tienda : MonoBehaviour {

	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && gameObject.activeSelf) CanvasBase.Instance.BTN_COMPRAS_BACK();
	}
}
