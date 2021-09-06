using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _Subpanel : MonoBehaviour {

    Transform online, vsIA;

    public Vector3 pos_IA = new Vector3(-50, 16, 0);
    public Vector3 pos_online = new Vector3(50, 16, 0);
    Vector3 final_scale = new Vector3(1.3f, 1.3f, 1);
    public Color green_flojo, green_fuerte;

    void OnEnable()
    {
        online = online ?? transform.Find("Online");
        vsIA = vsIA ?? transform.Find("vsIA");
        online.localPosition = vsIA.localPosition = Vector3.zero; 
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            float t = Time.deltaTime * 10;
            online.localPosition = Vector3.Lerp(online.localPosition, pos_online, t);
            vsIA.localPosition = Vector3.Lerp(vsIA.localPosition, pos_IA, t);
            online.localScale = Vector3.Lerp(online.localScale, final_scale, t);
            vsIA.localScale = Vector3.Lerp(vsIA.localScale, final_scale, t);
        }
    }

    void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
