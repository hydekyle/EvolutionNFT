using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class Message : MonoBehaviour {

    public static Message Instance{ get; set; }
    public float fadeVelocity = 12.0f;
    Text text;
    //Text text_shadow;
    Color fadeOutTextColor;
    Color fadeOutShadowColor;
    Transform t_cofreVIP;
    Image backPanel;

    void Awake()
    {
        Instance = this;
        Initialize();
    }

    void Initialize()
    {
        text = transform.Find("Text").GetComponent<Text>();
        backPanel = text.transform.Find("Panel").GetComponent<Image>();
        //text_shadow = transform.Find("Text_Shadow").GetComponent<Text>();
        t_cofreVIP = transform.Find("Cofre_VIP");
    }

    public void SwitchMessagePosition()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
        backPanel.color = new Color(backPanel.color.r, backPanel.color.g, backPanel.color.b, 0.0f);
        text.rectTransform.localPosition = new Vector3(text.rectTransform.localPosition.x, -100f, 0);
    }

    public void NewMessage(string message)
    {
        text.text = message;
        //text_shadow.text = message;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 255);
        //text_shadow.color = new Color(text_shadow.color.r, text_shadow.color.g, text_shadow.color.b, 255);
    }

    void LateUpdate()
    {
        Color letrasColor = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 0), Time.deltaTime * 2);
        text.color = letrasColor;
        backPanel.color = new Color(backPanel.color.r, backPanel.color.g, backPanel.color.b, letrasColor.a / 4);
        //text_shadow.color = Color.Lerp(text_shadow.color, new Color(text_shadow.color.r, text_shadow.color.g, text_shadow.color.b, 0), Time.deltaTime);
    }

    public void MostrarCofresVIP()
    {
        int coronas = GameManager.Instance.userdb.coronas;

        if (coronas < 4)
        {
            coronas = GameManager.Instance.userdb.coronas;
            VisualizarCoronas(coronas);
            t_cofreVIP.GetComponent<Animator>().Play("Cofre_VIP_show");
        }
        else if (GameManager.Instance.userdb.cofres.Count < 4)
        {
            UserDB userdb = GameManager.Instance.userdb;
            UserDB newData = new UserDB()
            {
                //chests = userdb.chests,
                //chests_VIP = userdb.chests_VIP + 1,
                coronas = 0,
                derrotas = userdb.derrotas,
                gold = userdb.gold,
                gold_VIP = userdb.gold_VIP,
                victorias = userdb.victorias,
                nivel = userdb.nivel,
                cofres = userdb.cofres,
                last_time_reward = userdb.last_time_reward
            };
            newData.cofres.Add(UnityEngine.Random.Range(1, 11) <= 8 ? 1 : 2);

            Database.Instance.ReferenceDB().Child("data").SetRawJsonValueAsync(JsonUtility.ToJson(newData)).ContinueWith(task =>
            {
                VisualizarCoronas(coronas);
                t_cofreVIP.GetComponent<Animator>().Play("Cofre_VIP_show");
                StartCoroutine(CambioCoronas());
                GameManager.Instance.userdb = newData;
            });
        }
        else
        {
            NewMessage("Cofres full");
        }
    }

    IEnumerator CambioCoronas()
    {
        yield return new WaitForSeconds(1f);
        VisualizarCoronas(0);
        NewMessage("¡Has conseguido un cofre!");
    }

    private void VisualizarCoronas(int n)
    {
        if(n == 0)
        {
            for (var x = 1; x <= 4; x++)
            {
                t_cofreVIP.Find(x.ToString()).gameObject.SetActive(false);
            }
        }

        for (var x = 1; x <= n; x++)
        {
            t_cofreVIP.Find(x.ToString()).gameObject.SetActive(true);
        }

    }
}
