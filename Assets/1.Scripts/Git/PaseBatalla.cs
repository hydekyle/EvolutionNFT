using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaseBatalla : MonoBehaviour {

    public static PaseBatalla Instance;

    Image img_face;
    Text value_nivel, value_victorias, value_derrotas;
    Slider slider_level;
    public Image elo_image;
    public Sprite elo_bronze, elo_silver, elo_gold, elo_diamond;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        Instance = this;
        Transform playerInfo = transform.Find("PlayerInfo");
        img_face = playerInfo.Find("img_Face").GetComponent<Image>();
        value_nivel = playerInfo.Find("value_Nivel").GetComponent<Text>();
        value_victorias = playerInfo.Find("Negro_Victorias").Find("Text").GetComponent<Text>();
        value_derrotas = playerInfo.Find("Negro_Derrotas").Find("Text").GetComponent<Text>();
        slider_level = playerInfo.Find("bar_Mask").Find("Slider").GetComponent<Slider>();
    }

    public void MostrarPlayerInfo()
    {
        UserDB dataPlayer = GameManager.Instance.userdb;
        value_nivel.text = dataPlayer.nivel.Substring(0, 1) == "0" ? dataPlayer.nivel.Substring(1, 1) : dataPlayer.nivel.Substring(0, 2);
        value_victorias.text = dataPlayer.victorias.ToString();
        value_derrotas.text = dataPlayer.derrotas.ToString();
        slider_level.value = int.Parse(dataPlayer.nivel.Substring(2, 2)) / 100f;
        //DisplayElo(dataPlayer.elo);
        StartCoroutine(Moverse());
    }

    private void DisplayElo(int actualElo)
    {
        if (actualElo > 2500) elo_image.sprite = elo_diamond;
        else if (actualElo > 2000) elo_image.sprite = elo_gold;
        else if (actualElo > 1500) elo_image.sprite = elo_silver;
        else elo_image.sprite = elo_bronze;
        elo_image.preserveAspect = true;
    }

    public void MostrarImg(Sprite sprite)
    {
        img_face.sprite = sprite;
        img_face.preserveAspect = true;
    }

    IEnumerator Moverse()
    {
        float t = 0;
        Vector3 destino = Application.platform == RuntimePlatform.Android ? new Vector3(0, 30, 0) : Vector3.zero;
        while (t < 1.0f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, destino, t);
            t += Time.deltaTime * 2;
            Mathf.Clamp(t, 0.0f, 1.0f);
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = destino;
    }
}
