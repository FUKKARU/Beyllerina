using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AudioVolumeChanger : MonoBehaviour
{
    TextMeshProUGUI volumeText;
    float textShowTime = 0f; // Œ¸‚Á‚Ä‚¢‚­

    void Start()
    {
        volumeText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // BGM
        if (Input.GetKey(KeyCode.Alpha1))
        {
            ChangeAudioVolume(AudioSO.AudioType.BGM);
        }
        // SE
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            ChangeAudioVolume(AudioSO.AudioType.SE);
        }
        // System
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            ChangeAudioVolume(AudioSO.AudioType.SYSTEM);
        }

        if (textShowTime > 0f)
        {
            textShowTime -= Time.deltaTime;

            if (textShowTime <= 0f)
            {
                textShowTime = 0f;
                volumeText.enabled = false;
            }
        }
    }

    void ChangeAudioVolume(AudioSO.AudioType type)
    {
        string paramName = AudioSO.Entity.GetAudioParam(type);

        AudioSO.Entity.AudioMixer.GetFloat(paramName, out float volume);

        // 2‚Ý
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            volume += AudioSO.Entity.VolumeChangeStep;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            volume -= AudioSO.Entity.VolumeChangeStep;
        }
        else
        {
            return; // ‚±‚±‚Åˆ—‚ðI—¹
        }

        volume = Mathf.Clamp(volume, -80f, 20f);

        AudioSO.Entity.AudioMixer.SetFloat(paramName, volume);

        textShowTime = AudioSO.Entity.VolumeTextShowDur;
        volumeText.text = volume.ToString("F1"); // ¬”‘æˆêˆÊ‚Ü‚Å
        volumeText.enabled = true;
    }
}