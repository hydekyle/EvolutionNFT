using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGame : MonoBehaviour {

    public float velocidad = 40;
    public int dificultad = 1;
    public LayerMask hitLayer;
    float angle = 0f;

    Transform circle;
    RectTransform rotor;
    Transform pointer;
    RectTransform firstOne;
    GameObject trazo;

    void OnEnable()
    {
        pointer = pointer ?? transform.Find("Pointer");
        circle = circle ?? transform.Find("Circle");
        rotor = rotor ?? transform.Find("Rotor").GetComponent<RectTransform>();
        trazo = trazo ?? transform.Find("Trazo").gameObject;
        firstOne = firstOne ?? rotor.GetChild(0).GetComponent<RectTransform>();
        rotor.localRotation = Quaternion.Euler(Vector3.zero);
        angle = 0;
        firstOne.gameObject.SetActive(true);
        dificultad = BattleSystem.Instance.difficult + 1;
        StartCoroutine(Giro());
    }

    void SetCopy(string name)
    {
        GameObject go = Instantiate(firstOne.gameObject);
        RectTransform rectGO = go.GetComponent<RectTransform>();
        go.name = name;
        go.transform.SetParent(rotor.transform);
        rectGO.localScale = firstOne.localScale;
        rectGO.localPosition = firstOne.localPosition;
        go.transform.SetParent(circle);
    }

    void Update()
    {
        angle = Time.deltaTime * velocidad;
        //if (Input.GetKeyDown(KeyCode.X)) StartCoroutine(Giro());
    }

    IEnumerator Giro()
    {
        float z = 0f;
        int nBalls = 5 + (int)(dificultad * 0.5f);
        nBalls = Mathf.Clamp(nBalls, 5, 7);
        float porcion = 360 / nBalls;
        var inicialPorcion = porcion;
        byte colocados = 0;
        while (z < 360.1f)
        {
            rotor.localRotation = Quaternion.Lerp(rotor.localRotation, Quaternion.Euler(0, 0, z), z);
            z += Time.deltaTime * velocidad * 4;
            if (porcion < z) {
                rotor.localRotation = Quaternion.Euler(new Vector3(0, 0, porcion));
                SetCopy(colocados.ToString());
                colocados++;
                porcion += inicialPorcion;
            }
            yield return new WaitForEndOfFrame();
        }
        rotor.localRotation = Quaternion.Euler(Vector3.zero);
        SpellSystem.Instance.GoPatron(colocados, circle, pointer, dificultad, trazo);
        firstOne.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        foreach(Transform t in transform.Find("Trazado"))
        {
            Destroy(t.gameObject);
        }
        foreach(Transform t in transform.Find("Circle"))
        {
            Destroy(t.gameObject);
        }
    }



}
