using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestEndCall : MonoBehaviour {

	public void AnimEnded()
    {
        StartCoroutine(CofreAbierto.Instance.AcabarCofreAnim());
    }
}
