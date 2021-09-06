using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class CofreAbierto : MonoBehaviour {

    public GameObject p_NewItem;
    public Image fadeBlack;
    public Transform t_NewItemsView;
    public static CofreAbierto Instance;
    List<string> nuevosItems;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator FadeTo(float value)
    {
        float t = 0f;
        Color toColor = new Color(fadeBlack.color.r, fadeBlack.color.g, fadeBlack.color.b, value);
        while (t < 1f)
        {
            fadeBlack.color = Color.Lerp(fadeBlack.color, toColor, t);
            t += Time.deltaTime * 4;
            yield return new WaitForEndOfFrame();
        }
        fadeBlack.color = toColor;
    }

    public void ShowOpenSprite()
    {
        Cofres_System.Instance.SetOpenedSprite();
    }


    public void GuardarNuevosItems(List<string> listaItemsNuevos)
    {
        nuevosItems = listaItemsNuevos;
    }

    public IEnumerator MostrarNuevosItems()
    {
        byte c = 0;
        t_NewItemsView.transform.position = transform.Find("Chest").position;
        foreach (string s in nuevosItems)
        {
            Item newItem = Items.Instance.GetItem(s);
            print(JsonUtility.ToJson(newItem));

            Sprite spriteItem = null;

            yield return Items.Instance.ItemSpriteByID(newItem.ID, result => spriteItem = result);

            Quality itemQuality = Quality.Common;
            switch (int.Parse(s.Substring(3, 1)))
            {
                case 1: itemQuality = Quality.Common; break;
                case 2: itemQuality = Quality.Rare; break;
                case 3: itemQuality = Quality.Epic; break;
                case 4: itemQuality = Quality.Legendary; break;
            }

            GameObject go = t_NewItemsView.Find(c.ToString()).GetChild(0).gameObject;
            go.GetComponent<Image>().color = EquipMenu.Instance.ColorByQuality(itemQuality);
            Image spriteImage = go.transform.Find("Image").GetComponent<Image>();
            spriteImage.sprite = spriteItem;
            spriteImage.preserveAspect = true;
            go.transform.localPosition = Vector3.zero;
            go.SetActive(true);
            c++;
        }
        t_NewItemsView.GetComponent<Animator>().Play("TextoFlotante", -1, 0f);
        
    }

    public IEnumerator AcabarCofreAnim()
    {
        StartCoroutine(FadeTo(0f));
        Cofres_System.Instance.MostrarIconoCofres();
        foreach (Transform t in t_NewItemsView) t.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Cofres_System.Instance.OnBack();
    }
}
