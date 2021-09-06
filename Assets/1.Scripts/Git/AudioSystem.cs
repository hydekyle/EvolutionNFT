using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance;

    AudioSource music_source, sound_source;

    public AudioClip BattleMusic, ClickMenu, ClickEquipItem1, ClickEquipItem2, ClickEquipItem3, ClickEquipList;

    public void Awake()
    {
        Instance = this;
        music_source = transform.Find("Music").GetComponent<AudioSource>();
        sound_source = transform.Find("Sound").GetComponent<AudioSource>();
    }


    public void Sound_ClickMenu1()
    {
        sound_source.PlayOneShot(ClickMenu);
    }

    public void Sound_ClickEquipItem()
    {
        switch (Random.Range(1, 4))
        {
            case 1: sound_source.PlayOneShot(ClickEquipItem1); break;
            case 2: sound_source.PlayOneShot(ClickEquipItem2); break;
            case 3: sound_source.PlayOneShot(ClickEquipItem3); break;
        }
    }

    public void Sound_ClickEquipList()
    {
        sound_source.PlayOneShot(ClickEquipList);
    }

    public void Music_Battle()
    {
        music_source.clip = BattleMusic;
        music_source.Play();
    }
}
