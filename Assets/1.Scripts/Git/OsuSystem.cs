using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class OsuSystem : MonoBehaviour {

    public GameObject osu_ball;
    public static OsuSystem Instance;
    Vector3 escala = new Vector3(0.5f, 0.5f, 0.5f);

    void Awake()
    {
        Instance = this;
    }


    void PutBall(Vector3 position, Speed speed)
    {
        GameObject go = Instantiate(osu_ball);
        go.GetComponent<OsuBall>().SetSpeed(speed);
        RectTransform rectT = go.GetComponent<RectTransform>();
        go.transform.SetParent(transform);
        rectT.localScale = escala;
        rectT.localPosition = position;
        go.gameObject.SetActive(true);
    }

    void PutLastBall(Vector3 position, Speed speed)
    {
        GameObject go = Instantiate(osu_ball);
        OsuBall ball = go.GetComponent<OsuBall>();
        ball.SetSpeed(speed);
        ball.isLastBall = true;
        RectTransform rectT = go.GetComponent<RectTransform>();
        go.transform.SetParent(transform);
        rectT.localScale = escala;
        rectT.localPosition = position;
        go.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Bolas();
    }

    public void Bolas()
    {
        StartCoroutine(PatronRandomHorizontal_3());
    }

    IEnumerator PatronRandomHorizontal_3()
    {
        int difficult = BattleSystem.Instance.difficult;
        float y = Random.Range(-100f, -25f);
        int distanceX = 55;
        int dir = Random.Range(0, 2) == 1 ? 1 : -1;
        Vector3 puntoInicial = new Vector3(-75 * dir, y, 1);
        PutBall(puntoInicial, SpeedByInt(2));
        yield return new WaitForSeconds(Random.Range(0.15f - 0.015f * difficult, 0.6f - 0.06f * difficult));
        PutBall(puntoInicial + Vector3.right * (distanceX + Random.Range(0, 21)) * dir, SpeedByInt(1));
        yield return new WaitForSeconds(Random.Range(0.05f - 0.005f * difficult, 0.6f - 0.06f * difficult));
        PutLastBall(puntoInicial + Vector3.right * ((distanceX + Random.Range(0, 21)) * 2) * dir, SpeedByInt(0));
    }

    Speed SpeedByInt(int i)
    {
        Speed speed = Speed.Normal;
        switch (i)
        {
            case 0: speed = Speed.Slow;   break;
            case 1: speed = Speed.Normal; break;
            case 2: speed = Speed.Fast;   break;
        }
        return speed;
    }


}
