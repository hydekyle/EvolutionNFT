using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System;
using Enums;

public class CanvasBase : MonoBehaviour {

	public string userID;

    public class CameraPos
    {
        public Vector3 leftPos = new Vector3(-100, -90, -500);
        public Vector3 midPos = new Vector3(0, 0, -500);
        public Vector3 rightPos = new Vector3(224, -125, -500);
        public Vector3 upPos = new Vector3(185, 142, -500);
    }

    public Material dissolve_material;

    StatsWindow stats_window = new StatsWindow();

    Transform start_menu;
    Transform equipment;
    Transform battleIA;
    Transform treasures;

    Text goldText;
    Text gold_VIP;
    EquipMenu equip_menu;

    CameraPos camPosition = new CameraPos();
    Camera camara;
    bool camMovementEnabled = true;
    public float cam_velocity = 10f;
    public float cam_size = 300;
    public Vector3 camVectorPoint;

    GameObject sub_menu_play;
    
    public static CanvasBase Instance;

    void Awake()
    {
        Instance = this;
        camara = Camera.main;
        camVectorPoint = camPosition.midPos;
        SetCamSize(300);
    }

    private IEnumerator TryReconnect(float timeAwait)
    {
        if (!BattleSystem.Instance.battleON)
        {
            yield return new WaitForSeconds(timeAwait);
            if (Application.platform == RuntimePlatform.Android)
            {
                ConectarseGooglePlay();
            }
            else
            {
                LogFirebaseFirstTime();
            }
        }
        
    }

    void Start()
    {
        Inicialize();
        LogFirebaseFirstTime();
    }

    void Inicialize()
    {
        //sub_menu_play = transform.Find("Start_Menu").Find("SubPanel_Versus").gameObject;
        CheckPlayerPrefs();
        start_menu = transform.Find("Start_Menu");
        equipment = transform.Find("Equipamiento");
        battleIA = transform.Find("BattleIA");
        treasures = transform.Find("Treasures");
        goldText = treasures.Find("Money").Find("Text").GetComponent<Text>();
        gold_VIP = treasures.Find("Money_VIP").Find("Text").GetComponent<Text>();
        equip_menu = equipment.GetComponent<EquipMenu>();

        Transform statsT = transform.Find("Equipamiento").Find("STATS");
        stats_window.alpha = statsT.Find("Value_Alpha").GetComponent<Text>();
        stats_window.assassin = statsT.Find("Value_Assassin").GetComponent<Text>();
        stats_window.charming = statsT.Find("Value_Charming").GetComponent<Text>();
        stats_window.pacifist = statsT.Find("Value_Pacifist").GetComponent<Text>();

        stats_window.health = statsT.Find("Value_Life").GetComponent<Text>();
        stats_window.strenght = statsT.Find("Value_Attack").GetComponent<Text>();
        stats_window.skill = statsT.Find("Value_Skill").GetComponent<Text>();
        stats_window.luck = statsT.Find("Value_Luck").GetComponent<Text>();
    }

    public void StatsRefresh()
    {
        BaseStats totalStats = Items.Instance.CalcularTotalBasePoints(GameManager.Instance.player.criatura.equipment);
        stats_window.assassin.text = totalStats.assassin.ToString();
        stats_window.alpha.text = totalStats.alpha.ToString();
        stats_window.charming.text = totalStats.charming.ToString();
        stats_window.pacifist.text = totalStats.pacifist.ToString();

        stats_window.strenght.text = totalStats.strenght.ToString();
        stats_window.health.text = totalStats.health.ToString();
        stats_window.skill.text = totalStats.skill.ToString();
        stats_window.luck.text = totalStats.luck.ToString();
    }

    void ConectarseGooglePlay()
    {
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //    .Build();
        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.Activate();

        LogFirebaseFirstTime();

        //Social.Active.Authenticate(Social.localUser, (bool success) => {
        //    if (success) {
        //        Message.Instance.NewMessage("Hola " + Social.localUser.userName);
                
        //    }else
        //    {
        //        Message.Instance.NewMessage("GPlay error, intentando reconectar");
        //        StartCoroutine(TryReconnect(0.1f));
        //    }
        //});
        
    }

    public void UpdateGoldView()
    {
        goldText.text = GameManager.Instance.userdb.gold.ToString();
        gold_VIP.text = GameManager.Instance.userdb.gold_VIP.ToString();
    }

    public void UpdateGoldViewNoDB(string value)
    {
        goldText.text = value;
    }

    public void ShowItemInfo(string id)
    {
        if (id != null) StartCoroutine(equip_menu.ViewItemInfo(id));
    }

    void LogFirebaseFirstTime()
    {
		string username = GameManager.Instance.GetUserID();
		username = username + "@evolution.com";
		string password = "ANonSecurePassword!ñ";

        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(username, password).ContinueWith((obj) =>
        {
            if (obj.IsFaulted) //El usuario no se ha registrado.
            {
                FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).CreateUserWithEmailAndPasswordAsync(username, password).ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        CargarUsuario();
                    }

                    if (task.IsFaulted)
                    {
                        Message.Instance.NewMessage("Firebase no conecta");
                        SceneManager.LoadScene(0); //Recargar la escena y volver a intentar
                    }
                });
            }
            else
            {
                CargarUsuario();
            }
        });
    }

    void LogFirebaseReconnect()
    {
        string username = GameManager.Instance.GetUserID();
        username = username + "@evolution.com";
        string password = "ANonSecurePassword!ñ";

        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(username, password).ContinueWith((obj) =>
        {
            if (obj.IsFaulted) //El usuario no se ha registrado.
            {
                FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).CreateUserWithEmailAndPasswordAsync(username, password).ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        print("Te has reconectado");
                    }

                    if (task.IsFaulted)
                    {
                        Message.Instance.NewMessage("Firebase no conecta");
                        SceneManager.LoadScene(0); //Recargar la escena y volver a intentar
                    }
                });
            }
            else
            {
                print("Te has reconectado");
            }
        });

    }

    void CargarUsuario()
    {
        //Message.Instance.NewMessage("Cargando...");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Inventario").Child(GameManager.Instance.GetUserID()).GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                if (task.Result.Exists) //Si ya has jugado
                {
                    string json = task.Result.Child("data").GetRawJsonValue();
                    UserDB user = JsonUtility.FromJson<UserDB>(json);
                    GameManager.Instance.userdb = user;
                    LogSuccessful();
                }
                else
                {
                    start_menu.gameObject.SetActive(false);
                    transform.Find("Loading").gameObject.SetActive(false);
                    camMovementEnabled = false;
                    SceneManager.LoadScene(1); //Ir a Huevo
                }
            }
            else
            {
                Message.Instance.NewMessage("Fallo al cargar usuario");
                SceneManager.LoadScene(0);
            }
        });
    }

    bool IsUserLogged()
    {
        return FirebaseAuth.DefaultInstance.CurrentUser != null;
    }

    public void GoBattle()
    {
#if UNITY_EDITOR
        battleIA.gameObject.SetActive(!battleIA.gameObject.activeSelf);
        start_menu.gameObject.SetActive(false);
#endif

        if (Social.localUser.authenticated)
        {
            battleIA.gameObject.SetActive(!battleIA.gameObject.activeSelf);
            start_menu.gameObject.SetActive(false);
        }
        else
        {
            Message.Instance.NewMessage("No hay conexión");
        }
    }

    public void BackToMenu()
    {
        start_menu.gameObject.SetActive(true);
        camVectorPoint = camPosition.midPos;
        SetCamSize(300);
    }

    public void TryRelog(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null) StartCoroutine(TryingRelog());
    }

    IEnumerator TryingRelog()
    {
        do
        {
            Message.Instance.NewMessage("TRYING TO RECONECT DB");
            LogFirebaseReconnect();
            yield return new WaitForSeconds(1.5f);
        }
        while (FirebaseAuth.DefaultInstance.CurrentUser == null);
    }

    public void LoadYourItems()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("items");
        reference.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                string jsonData = task.Result.GetRawJsonValue();
                string[] items = jsonData.Replace('"', ' ').Replace('[', ' ').Replace(']', ' ').Replace(" ", string.Empty).Split(',');
                Items.Instance.StoreClear();
                foreach (string s in items)
                {
                    Items.Instance.StoreItem(s);
                }
            }
        });
    }

    public IEnumerator DisolverMaterialVisor(int visorN) //EFECTO DISOLVER
    {
        dissolve_material.SetFloat("_Level", 1f);
        Menu.Instance.SetMaterialVisor(Menu.Instance.GetPlayerVisor(visorN), dissolve_material);
        yield return StartCoroutine(DisolverAnim());
        Menu.Instance.SetMaterialVisor(Menu.Instance.GetPlayerVisor(visorN), null);
        dissolve_material.SetFloat("_Level", 1f);

    }

    public IEnumerator MostrarPieza(Equip_Position posi, Action<bool> ended)
    {
        dissolve_material.SetFloat("_Level", 1f);
        switch (posi)
        {
            case Equip_Position.Head: Menu.Instance.visor_player1.headgear.material = dissolve_material; break;
            case Equip_Position.Body: Menu.Instance.visor_player1.body.material = dissolve_material; break;
            case Equip_Position.Arms: Menu.Instance.visor_player1.arm_left.material = dissolve_material;
                                        Menu.Instance.visor_player1.arm_right.material = dissolve_material; break;
            case Equip_Position.Legs: Menu.Instance.visor_player1.leg_left.material = dissolve_material;
                                            Menu.Instance.visor_player1.leg_right.material = dissolve_material; break;
        }
        yield return StartCoroutine(DisolverAnim());
        switch (posi)
        {
            case Equip_Position.Head: Menu.Instance.visor_player1.headgear.material = null; break;
            case Equip_Position.Body: Menu.Instance.visor_player1.body.material = null; break;
            case Equip_Position.Arms:
                Menu.Instance.visor_player1.arm_left.material = null;
                Menu.Instance.visor_player1.arm_right.material = null; break;
            case Equip_Position.Legs:
                Menu.Instance.visor_player1.leg_left.material = null;
                Menu.Instance.visor_player1.leg_right.material = null; break;
        }
        ended(true);

    }

    IEnumerator DisolverAnim()
    {
        float t = 1.0f;
        while (t > 0.01f)
        {
            dissolve_material.SetFloat("_Level", t);
            t -= Time.deltaTime;
            t = Mathf.Clamp(t, 0f, 1f);
            yield return null;
        }
    }
    
    IEnumerator LagSpikeMenuFixer(Action<bool> onEnded)
    {
        Transform tienda = transform.Find("Tienda");
        treasures.gameObject.SetActive(true);
        equipment.gameObject.SetActive(true);
        tienda.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        treasures.gameObject.SetActive(false);
        equipment.gameObject.SetActive(false);
        tienda.gameObject.SetActive(false);
        onEnded(true);
    }

    void Update()
    {
        if (camara == null) camara = Camera.main;
        if (camMovementEnabled)
        {
            camara.transform.position = Vector3.Lerp(camara.transform.position, camVectorPoint, Time.deltaTime * cam_velocity);
            camara.orthographicSize = Mathf.Lerp(camara.orthographicSize, cam_size, Time.deltaTime * cam_velocity);
            //if (Input.GetKeyDown(KeyCode.I)) StartCoroutine(C_Inspeccionar(1));
            //if (Input.GetKeyDown(KeyCode.O)) StartCoroutine(C_Inspeccionar(2));
            //if (Input.GetKeyDown(KeyCode.K)) SetCamTarget((Posi(Equip_Position.Body, 2) + Posi(Equip_Position.Body, 1)) / 2);
            //if (Input.GetKeyDown(KeyCode.L)) SetCamTarget(Posi(Equip_Position.Legs, 2));

            //if (Input.GetKeyDown(KeyCode.N)) SetCamVelocity(Velocity.Slow);
            //if (Input.GetKeyDown(KeyCode.M)) SetCamVelocity(Velocity.Fast);
        }
    }

    public void SetCamSize(float newSize)
    {
        cam_size = newSize;
    }

    void SetCamVelocity(float newVelocity)
    {
        cam_velocity = newVelocity;
    }

    void SetCamVelocity(Velocity vel)
    {
        switch (vel)
        {
            case Velocity.Fast: cam_velocity = 8f; break;
            case Velocity.Normal: cam_velocity = 5f; break;
            case Velocity.Slow: cam_velocity = 1.0f; break;
        }
    }

    public void SetCamTarget(Vector3 targetPOS)
    {
        if (!fixer)camVectorPoint = new Vector3(targetPOS.x, targetPOS.y, -500);
    }


    bool fixer = false;
    public void SetCamTargetSpecial(Vector3 targetPOS)
    {
        StopAllCoroutines();
        fixer = true;
        SetCamSize(250);
        camVectorPoint = new Vector3(targetPOS.x, targetPOS.y - 50, -500);
    }

    public Vector3 Posi(Equip_Position equip_position, int playerN)
    {
        Vector3 v = Vector3.zero;

        switch (equip_position)
        {
            case Equip_Position.Head: v = Menu.Instance.GetPlayerVisor(playerN).headgear.transform.position; break;
            case Equip_Position.Body: v = Menu.Instance.GetPlayerVisor(playerN).body.transform.position; break;
            case Equip_Position.Arms: v = (Menu.Instance.GetPlayerVisor(playerN).arm_right.transform.position + Menu.Instance.GetPlayerVisor(playerN).arm_left.transform.position) /2; break;
            case Equip_Position.Legs: v = (Menu.Instance.GetPlayerVisor(playerN).leg_right.transform.position + Menu.Instance.GetPlayerVisor(playerN).leg_left.transform.position)/2; break;
            default: v = Menu.Instance.GetPlayerVisor(playerN).body.transform.position; break;
        }
        return v;
    }

    public IEnumerator PointReached(Action<bool> onEnded)
    {
        while (!CamIsOnTarget())
        {
            yield return new WaitForSeconds(0.1f);
        }
        onEnded(true);
    }

    bool CamIsOnTarget()
    {
        return ComparePoints((int)camara.transform.position.x, (int)camVectorPoint.x) && 
                ComparePoints((int)camara.transform.position.y, (int)camVectorPoint.y);
    }

    bool ComparePoints(int x, int y)
    {
        if(Mathf.Abs(x - y) < 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void LogSuccessful()
    {
        UpdateGoldView();
        LoadYourItems();
        
        StartCoroutine(LagSpikeMenuFixer(onEnded => {
            Message.Instance.MostrarCofresVIP();
            Database.Instance.ActiveListenersDB();
            Database.Instance.ObtenerEquipSetting();
            transform.Find("Loading").gameObject.SetActive(false);
        }));
    }

    public IEnumerator C_Inspeccionar(int playerN)
    {
        SetCamVelocity(Velocity.Slow);
        SetCamSize(300);
        SetCamTarget(Posi(Equip_Position.Body, playerN));
        while (!CamIsOnTarget()) yield return null;
        SetCamVelocity(Velocity.Slow);
        SetCamSize(120);
        SetCamTarget((Posi(Equip_Position.Body, playerN) + Posi(Equip_Position.Head, playerN)) / 2);
        while (!CamIsOnTarget()) yield return null;
        BattleSystem.Instance.cameraEnded = true;
    }

    public IEnumerator C_UsingSkill(int playerN)
    {
        SetCamVelocity(Velocity.Normal);
        SetCamSize(250);
        SetCamTarget(Posi(Equip_Position.Body, playerN));
        yield return null;
    }

    public IEnumerator C_PutOnMid()
    {
        SetCamVelocity(Velocity.Normal);
        SetCamSize(400);
        SetCamTarget(Posi(Equip_Position.Body, 1) + (Posi(Equip_Position.Body, 2) - Posi(Equip_Position.Body, 1)) / 2);
        yield return new WaitForSeconds(5.5f);
        //BattleSystem.Instance.autoCamera = true;
    }

    public void BTN_EQUIP()
    {
        AudioSystem.Instance.Sound_ClickMenu1();
        equipment.gameObject.SetActive(true);
        start_menu.gameObject.SetActive(false);
        camVectorPoint = camPosition.leftPos;
        SetCamSize(300);
    }

    public void BTN_TIENDA()
    {
        AudioSystem.Instance.Sound_ClickMenu1();
        transform.Find("Tienda").gameObject.SetActive(true);
        transform.Find("Treasures").gameObject.SetActive(false);
        camVectorPoint = camPosition.upPos;
        SetCamSize(150);
    }

    public void BTN_VERSUS()
    {
        GoBattle();
        //sub_menu_play.SetActive(!sub_menu_play.activeSelf);
    }

    public void BTN_LOGROS()
    {
        //Social.ShowAchievementsUI();

        if (Social.localUser.authenticated)
        {
            LogFirebaseFirstTime();
        }
    }

    public void BTN_COFRES()
    {
        AudioSystem.Instance.Sound_ClickMenu1();
        treasures.gameObject.SetActive(true);
        start_menu.gameObject.SetActive(false);
        camVectorPoint = camPosition.rightPos;
        SetCamSize(150);
    }

    public void BTN_COMPRAS_BACK()
    {
        AudioSystem.Instance.Sound_ClickEquipItem();
        transform.Find("Tienda").gameObject.SetActive(false);
        transform.Find("Treasures").gameObject.SetActive(true);
        camVectorPoint = camPosition.rightPos;
        SetCamSize(150);
    }

    public void BTN_OK_Equipment()
    {
        AudioSystem.Instance.Sound_ClickEquipItem();
        Database.Instance.GuardarEquipSetting(GameManager.Instance.player.criatura.equipment);
        equipment.gameObject.SetActive(false);
        start_menu.gameObject.SetActive(true);
        camVectorPoint = camPosition.midPos;
        SetCamSize(300);
    }

    private void CheckPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("victorias")) PlayerPrefs.SetInt("victorias", 0);
        if (!PlayerPrefs.HasKey("derrotas")) PlayerPrefs.SetInt("derrotas", 0);
        MatchMaking.Instance.EstadisticasIA();
    }

}
