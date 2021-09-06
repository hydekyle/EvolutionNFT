using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System.IO;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Player player;
    public UserDB userdb;
    public string testerName;


    void Awake()
    {
        if (Instance == null)
        {
            Application.runInBackground = true;
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    public IEnumerator MostrarJugador(Player playerP, int visorN, Vector3 visorPosition, bool flip)
    {
        Menu.Instance.InitializeVisor(Menu.Instance.GetPlayerVisor(visorN), visorPosition, flip);
        yield return null;
        yield return StartCoroutine(Menu.Instance.VisualizarEquipamiento(playerP.criatura.equipment, visorN));
        StartCoroutine(CanvasBase.Instance.DisolverMaterialVisor(visorN));
    }

    public Evolution GetMyEvolution()
    {
        int nivel = userdb.nivel.Substring(0, 1) == "0" ? int.Parse(userdb.nivel.Substring(1, 1)) : int.Parse(userdb.nivel.Substring(0, 2));
        if (nivel >= 30) return Evolution.Civilization;
        else if (nivel >= 20) return Evolution.Tribal;
        else return Evolution.Creature;
    }

    public string GetUserID()
    {
        if (Application.platform == RuntimePlatform.Android) return "Z_" +SystemInfo.deviceUniqueIdentifier ;
        else return testerName;
    }

    public string GetUserName()
    {
        if (Application.platform == RuntimePlatform.Android) return Social.localUser.userName;
        else return testerName;
    }

    public void PlayVsBot()
    {
        Database.Instance.invRecibida = true;

        DataTurn dataTurn = new DataTurn()
        {
            turn_number = 2, //EMPIEZA UN JUGADOR ALEATORIO
            minigame_fails = 0,
            random_seed = 0,
            used_skill = 0,
            player1 = GetUserID(),
            player2 = "IA"
        };
        Database.Instance.ReferenceDataTurn().SetRawJsonValueAsync(JsonUtility.ToJson(dataTurn)).ContinueWith(task => {
            Database.Instance.ReferenceDataTurn().ValueChanged += Database.Instance.OnDataTurnUpdate; //LISTENER
        });
    }

    public void ErrorGeneral()
    {
        Debug.LogError("Ha ocurrido un error grave");
    }

}
