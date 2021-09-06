using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracySystem : MonoBehaviour {

    public static AccuracySystem Instance;


    void Awake()
    {
        Instance = this;
    }

    public void Iniciar()
    {
        transform.Find("LunaGame").gameObject.SetActive(true);
    }
}
