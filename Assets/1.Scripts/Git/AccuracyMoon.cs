using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyMoon : MonoBehaviour {

    RectTransform t_pointer;
    public int difficulty = 1;
    bool active = true;
    bool dir;

    void OnEnable()
    {
        t_pointer = transform.Find("Pointer").GetComponent<RectTransform>();
        t_pointer.localPosition = new Vector3(0, 309, 0);
        active = true;
        dir = false;
        difficulty = BattleSystem.Instance.difficult + 1;
    }

    void Update()
    {
        if (active)
        {
            if (dir)
            {
                t_pointer.localPosition = Vector3.Lerp(t_pointer.localPosition, new Vector3(0, -400, 0), Time.deltaTime * difficulty);
            }
            else
            {
                t_pointer.localPosition = Vector3.Lerp(t_pointer.localPosition, new Vector3(0, 400, 0), Time.deltaTime * difficulty);
            }
            if (t_pointer.localPosition.y > 310) dir = !dir;
            if (t_pointer.localPosition.y < -300) dir = !dir;
        }
    }

    public void Clicked()
    {
        active = !active;
        transform.parent.gameObject.SetActive(false);
        BattleSystem.Instance.minigameFails = Fails();
        BattleSystem.Instance.EndMinigame();
    }

    int Fails()
    {
        if      (Mathf.Abs(t_pointer.localPosition.y) <= 30) return 0;
        else if (Mathf.Abs(t_pointer.localPosition.y) <= 80) return 1;
        else if (Mathf.Abs(t_pointer.localPosition.y) <= 120) return 2;
        else if (Mathf.Abs(t_pointer.localPosition.y) <= 200) return 3;
        return 4;
    }
}
