using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Menu : MonoBehaviour {

    public static Menu Instance;
    public Equipment loadedEquipment = new Equipment();
    [SerializeField]
    public Visor visor_player1, visor_player2;
    public bool inicialized;
    

    void Awake()
    {
        Instance = this;
        visor_player1 = new Visor();
        visor_player2 = new Visor();
        Initialize();
    }

    public void SetMaterialVisor(Visor visor, Material material)
    {
        visor.headgear.material = material;
        visor.body.material = material;
        visor.leg_left.material = material;
        visor.leg_right.material = material;
        visor.arm_left.material = material;
        visor.arm_right.material = material;
    }


    void Initialize()
    {
        visor_player1.myTransform = transform.Find("VISOR");
        visor_player2.myTransform = transform.Find("VISOR2");
    }

    public void InitializeVisor(Visor visor, Vector3 visorPos, bool flip)
    {
        visor.myTransform.gameObject.SetActive(false);
        visor.myTransform.localPosition = Vector3.zero;
        Transform player = visor.myTransform.Find("Player").Find("Player");
        visor.headgear = player.Find("HEADGEAR").Find("HEADGEAR").GetComponent<Image>();
        visor.body = player.Find("BODY").Find("BODY").GetComponent<Image>();
        visor.back = player.Find("BACK").Find("BACK").GetComponent<Image>();
        visor.arm_left = player.Find("ARM_LEFT").Find("ARM_LEFT").GetComponent<Image>();
        visor.arm_right = player.Find("ARM_RIGHT").Find("ARM_RIGHT").GetComponent<Image>();
        visor.leg_left = player.Find("LEG_LEFT").Find("LEG_LEFT").GetComponent<Image>();
        visor.leg_right = player.Find("LEG_RIGHT").Find("LEG_RIGHT").GetComponent<Image>();
        visor.shadow = player.parent.parent.Find("SHADOW").GetComponent<Image>();
        StartCoroutine(Visualizar(visor.myTransform, visorPos, flip));
    }

    public void SetVisorPosition(int playerN, Vector3 newPos, bool flip)
    {
        Visor visor = new Visor();

        if (playerN == 1) visor = visor_player1; else visor = visor_player1;
        visor.myTransform.localPosition = newPos;
        if (flip) visor.myTransform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    IEnumerator Visualizar(Transform visor, Vector3 localPos, bool flip)
    {
        visor.localPosition = localPos;
        yield return new WaitForEndOfFrame();
        if (flip) visor.rotation = Quaternion.Euler(0, 180, 0);
        visor.gameObject.SetActive(true);
    }

    public IEnumerator SendImage(int id, Visor visor)
    {
        Sprite spriteBuscado = null;
        yield return StartCoroutine(Items.Instance.ItemSpriteByID(id, value => spriteBuscado = value));
        SetImageVisor(spriteBuscado, id, visor);
    }

    public void SetImageVisor(Sprite sprite, int id, Visor visor)
    {
        int listNumber = int.Parse(id.ToString().Substring(0, 1));
        switch (listNumber)
        {
            case 1: visor.headgear.sprite = sprite; visor.headgear.preserveAspect = true;
                if (PaseBatalla.Instance) PaseBatalla.Instance.MostrarImg(sprite); break;
            case 2: visor.body.sprite = sprite; visor.body.preserveAspect = true; break;
            case 3: visor.arm_left.sprite = visor.arm_right.sprite = sprite; visor.arm_left.preserveAspect = true; visor.arm_right.preserveAspect = true; break;
            case 4: visor.leg_left.sprite = visor.leg_right.sprite = sprite; visor.leg_left.preserveAspect = true; visor.leg_right.preserveAspect = true; break;
            case 5: visor.back.sprite = sprite; visor.back.preserveAspect = true; break;
        }
    }

    public IEnumerator VisualizarEquipamiento(Equipment e, int playerNumber)
    {
        if (playerNumber == 1 && !inicialized) inicialized = true;
        //Actuar solo si el equipamiento ha cambiado. Para el jugador 2 se muestra sin checkear.
        if (loadedEquipment.body.ID != e.body.ID || playerNumber == 2)
        {
            yield return StartCoroutine(SendImage(e.body.ID, GetPlayerVisor(playerNumber)));
        }

        if (loadedEquipment.head.ID != e.head.ID || playerNumber == 2)
        {
            yield return StartCoroutine(Animations.Instance.CargarHeadPose(playerNumber, e.head.ID, onEnded => {
                //print("Una llamada");
            }));
            yield return StartCoroutine(SendImage(e.head.ID, GetPlayerVisor(playerNumber)));
        }

        if (loadedEquipment.arms.ID != e.arms.ID || playerNumber == 2)
        {
            yield return StartCoroutine(SendImage(e.arms.ID, GetPlayerVisor(playerNumber)));
        }

        if (loadedEquipment.legs.ID != e.legs.ID || playerNumber == 2)
        {
            yield return StartCoroutine(SendImage(e.legs.ID, GetPlayerVisor(playerNumber)));
        }

        loadedEquipment = e;
        ColocarPiezas(e, GetPlayerVisor(playerNumber));
    }

    void ColocarPiezas(Equipment e, Visor visor)
    {
        ColocarBody(e.body.ID, visor);
        FixScales(e, visor);
    }

    void ColocarBody(int bigID, Visor visor)
    {
        int id = int.Parse(bigID.ToString().Substring(0, 3));
        ColocarPiezas(Database.Instance.LeerBodyBounds(id), visor);
        ColocarSombra(visor);
    }

    private void ColocarPiezas(BodyBounds bounds, Visor visor)
    {
        visor.leg_left.transform.parent.localPosition = bounds.leg_left_POS;
        visor.leg_right.transform.parent.localPosition = bounds.leg_right_POS;

        visor.arm_left.transform.parent.localPosition = bounds.arm_left_POS;
        visor.arm_right.transform.parent.localPosition = bounds.arm_right_POS;

        visor.headgear.transform.parent.localPosition = bounds.head_POS;

        visor.body.transform.localScale = bounds.special_scale.magnitude > 0 ? bounds.special_scale : Vector3.one;
        visor.body.transform.localPosition = bounds.special_postion.magnitude > 0 ? bounds.special_postion : Vector3.zero;

        //visor.back.rectTransform.localPosition = visor.body.rectTransform.localPosition + bounds.back_POS;
    }

    private void ColocarSombra(Visor visor)
    {
        visor.shadow.gameObject.SetActive(true);
        //print(Vector3.Distance(visor.leg_left.transform.localPosition, visor.leg_right.transform.localPosition));
    }

    void FixScales(Equipment e, Visor visor)
    {
        int id = int.Parse(e.head.ID.ToString().Substring(0, 3));
        string ruta = "FixedScale/" + id.ToString();
        TextAsset txt = (TextAsset)Resources.Load(ruta);
        if (txt != null)  //HEADGEAR
        {
            FixedScale fix = JsonUtility.FromJson<FixedScale>(txt.text);
            Vector3 fixScale = fix.customScale;
            Vector3 fixPosition = fix.customPosition;
            visor.headgear.rectTransform.localScale    = fixScale;
            visor.headgear.rectTransform.localPosition = fixPosition;
        }else
        {
            visor.headgear.rectTransform.localScale    = new Vector3(0.8f, 0.8f, 1f);
            visor.headgear.rectTransform.localPosition = Vector3.zero;
        }

        id = int.Parse(e.arms.ID.ToString().Substring(0, 3));
        ruta = "FixedScale/" + id.ToString();
        txt = null;
        txt = (TextAsset)Resources.Load(ruta);
        if (txt != null)  //ARMS
        {
            FixedScale fix = JsonUtility.FromJson<FixedScale>(txt.text);
            Vector3 fixScale = fix.customScale;
            Vector3 fixPosition = fix.customPosition;
            visor.arm_left.rectTransform.localScale     = visor.arm_right.rectTransform.localScale = fixScale;
            visor.arm_left.rectTransform.localPosition  = fix.customPosition;
            visor.arm_right.rectTransform.localPosition = fix.customPosition_right;
            //print(fixPosition);
        }
        else
        {
            visor.arm_left.rectTransform.localScale     = Vector3.one;
            visor.arm_right.rectTransform.localScale    = Vector3.one;
            visor.arm_left.rectTransform.localPosition  = Database.Instance.LeerBodyBounds(e.body.ID).arm_left_POS;
            visor.arm_right.rectTransform.localPosition = Database.Instance.LeerBodyBounds(e.body.ID).arm_right_POS;
        }

        id = int.Parse(e.legs.ID.ToString().Substring(0, 3));
        ruta = "FixedScale/" + id.ToString();
        txt = null;
        txt = (TextAsset)Resources.Load(ruta);
        if (txt != null)  //LEGS
        {
            FixedScale fix = JsonUtility.FromJson<FixedScale>(txt.text);
            Vector3 fixScale = fix.customScale;
            Vector3 fixPosition = fix.customPosition;
            visor.leg_left.rectTransform.localScale     = visor.leg_right.rectTransform.localScale = fixScale;
            visor.leg_left.rectTransform.localPosition  = fix.customPosition;
            visor.leg_right.rectTransform.localPosition = fix.customPosition_right;
            print(fix.customRotation_right);
            visor.leg_right.rectTransform.localRotation = Quaternion.Euler(fix.customRotation_right);
        }
        else
        {
            visor.leg_left.rectTransform.localScale     = Vector3.one;
            visor.leg_right.rectTransform.localScale    = Vector3.one;
            visor.leg_left.rectTransform.localPosition  = Database.Instance.LeerBodyBounds(e.body.ID).leg_left_POS;
            visor.leg_right.rectTransform.localPosition = Database.Instance.LeerBodyBounds(e.body.ID).leg_right_POS;
            visor.leg_right.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
        }


    }

    void SaveLegsScale()
    {
        int id = int.Parse(GameManager.Instance.player.criatura.equipment.legs.ID.ToString().Substring(0, 3));
        int bodyID = int.Parse(GameManager.Instance.player.criatura.equipment.body.ID.ToString().Substring(0, 3));
        string ruta = "Assets/Resources/FixedScale/" + id.ToString() + ".txt";
        FixedScale f = new FixedScale()
        {
            customRotation_right = Mathf.Approximately(visor_player1.leg_right.rectTransform.rotation.eulerAngles.y, 0f) ? Vector3.zero : visor_player1.leg_right.rectTransform.rotation.eulerAngles,
            customPosition = visor_player1.leg_left.rectTransform.localPosition,
            customPosition_right = visor_player1.leg_right.rectTransform.localPosition,
            customScale = visor_player1.leg_right.rectTransform.localScale
        };
        File.WriteAllText(ruta, JsonUtility.ToJson(f));
        print("Saving " + id + " " +f.customScale);
    }

    void SaveArmsScale()
    {
        int id = int.Parse(GameManager.Instance.player.criatura.equipment.arms.ID.ToString().Substring(0, 3));
        int bodyID = int.Parse(GameManager.Instance.player.criatura.equipment.body.ID.ToString().Substring(0, 3));
        string ruta = "Assets/Resources/FixedScale/" + id.ToString() + ".txt";
        FixedScale f = new FixedScale()
        {
            customRotation_right = Mathf.Approximately(visor_player1.arm_right.rectTransform.rotation.eulerAngles.y, 0f) ? Vector3.zero : visor_player1.arm_right.rectTransform.rotation.eulerAngles,
            customPosition = visor_player1.arm_left.rectTransform.localPosition,
            customPosition_right = visor_player1.arm_right.rectTransform.localPosition,
            customScale = visor_player1.arm_right.rectTransform.localScale
        };
        File.WriteAllText(ruta, JsonUtility.ToJson(f));
        print("Saving " + id);
    }

    void SaveHeadScale()
    {
        int id = int.Parse(GameManager.Instance.player.criatura.equipment.head.ID.ToString().Substring(0, 3));
        string ruta = "Assets/Resources/FixedScale/" + id.ToString() + ".txt";
        FixedScale f = new FixedScale()
        {
            customRotation_right = Mathf.Approximately(visor_player1.headgear.rectTransform.rotation.eulerAngles.y, 0f) ? Vector3.zero : visor_player1.headgear.rectTransform.rotation.eulerAngles,
            customPosition = visor_player1.headgear.rectTransform.localPosition,
            customScale = visor_player1.headgear.rectTransform.localScale
        };
        File.WriteAllText(ruta, JsonUtility.ToJson(f));
        print("Saving " + id);
    }

    void SaveBounds()
    {
    #if UNITY_EDITOR
        BodyBounds bounds = new BodyBounds()
        {
            head_POS = visor_player1.headgear.transform.parent.localPosition,
            arm_left_POS = visor_player1.arm_left.transform.parent.localPosition,
            arm_right_POS = visor_player1.arm_right.transform.parent.localPosition,
            leg_left_POS = visor_player1.leg_left.transform.parent.localPosition,
            leg_right_POS = visor_player1.leg_right.transform.parent.localPosition,
            back_POS = visor_player1.back.rectTransform.localPosition,
            special_scale = visor_player1.body.rectTransform.localScale,
            special_postion = visor_player1.body.rectTransform.localPosition
        };
        Database.Instance.GuardarBodyBounds(bounds);
    #endif
    } 

    public Visor GetPlayerVisor(int playerNumber)
    {
        Visor visor;
        if (playerNumber == 1) visor = visor_player1;
        else { visor = visor_player2; }
        return visor;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SaveBounds();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SaveArmsScale();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SaveLegsScale();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SaveHeadScale();
        }

    }
#endif



}
