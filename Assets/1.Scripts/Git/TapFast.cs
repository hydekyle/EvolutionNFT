using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapFast : MonoBehaviour {

    public static TapFast Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Iniciar()
    {
        GameOn();
    }

    void GameOn()
    {
        transform.Find("TapBase").gameObject.SetActive(true);
    }

}
