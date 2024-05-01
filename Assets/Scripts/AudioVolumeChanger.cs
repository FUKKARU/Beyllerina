using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AudioVolumeChanger : MonoBehaviour
{
    TextMeshProUGUI bgmVolumeText;
    TextMeshProUGUI seVolumeText;
    TextMeshProUGUI systemVolumeText;
    float bgmTextShowTime = 0f; // Œ¸‚Á‚Ä‚¢‚­
    float seTextShowTime = 0f; // Œ¸‚Á‚Ä‚¢‚­
    float systemTextShowTime = 0f; // Œ¸‚Á‚Ä‚¢‚­

    void Start()
    {
        bgmVolumeText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        seVolumeText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        systemVolumeText = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // BGM
        if (Input.GetKey(KeyCode.Alpha1))
        {
            string paramName = AudioSO.Entity.GetAudioParam(AudioSO.AudioType.BGM);

            AudioSO.Entity.AudioMixer.GetFloat(paramName, out float volume);
            if (Input.GetKeyDown(KeyCode.UpArrow)) volume += AudioSO.Entity.VolumeChangeStep; // 2‚Ý
            else if (Input.GetKeyDown(KeyCode.DownArrow)) volume -= AudioSO.Entity.VolumeChangeStep; // 2‚Ý
            else return; // ‚±‚±‚Åˆ—‚ðI—¹
            volume = Mathf.Clamp(volume, -80f, 20f);
            AudioSO.Entity.AudioMixer.SetFloat(paramName, volume);

            bgmTextShowTime = AudioSO.Entity.VolumeTextShowDur;
            bgmVolumeText.text = volume.ToString("F1"); // ¬”‘æˆêˆÊ‚Ü‚Å
            bgmVolumeText.enabled = true;
        }
        // SE
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            string paramName = AudioSO.Entity.GetAudioParam(AudioSO.AudioType.SE);

            AudioSO.Entity.AudioMixer.GetFloat(paramName, out float volume);
            if (Input.GetKeyDown(KeyCode.UpArrow)) volume += AudioSO.Entity.VolumeChangeStep; // 2‚Ý
            else if (Input.GetKeyDown(KeyCode.DownArrow)) volume -= AudioSO.Entity.VolumeChangeStep; // 2‚Ý
            else return; // ‚±‚±‚Åˆ—‚ðI—¹
            volume = Mathf.Clamp(volume, -80f, 20f);
            AudioSO.Entity.AudioMixer.SetFloat(paramName, volume);

            seTextShowTime = AudioSO.Entity.VolumeTextShowDur;
            seVolumeText.text = volume.ToString("F1"); // ¬”‘æˆêˆÊ‚Ü‚Å
            seVolumeText.enabled = true;
        }
        // System
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            string paramName = AudioSO.Entity.GetAudioParam(AudioSO.AudioType.SYSTEM);

            AudioSO.Entity.AudioMixer.GetFloat(paramName, out float volume);
            if (Input.GetKeyDown(KeyCode.UpArrow)) volume += AudioSO.Entity.VolumeChangeStep; // 2‚Ý
            else if (Input.GetKeyDown(KeyCode.DownArrow)) volume -= AudioSO.Entity.VolumeChangeStep; // 2‚Ý
            else return; // ‚±‚±‚Åˆ—‚ðI—¹
            volume = Mathf.Clamp(volume, -80f, 20f);
            AudioSO.Entity.AudioMixer.SetFloat(paramName, volume);

            systemTextShowTime = AudioSO.Entity.VolumeTextShowDur;
            systemVolumeText.text = volume.ToString("F1"); // ¬”‘æˆêˆÊ‚Ü‚Å
            systemVolumeText.enabled = true;
        }

        if (bgmTextShowTime > 0f)
        {
            bgmTextShowTime -= Time.deltaTime;

            if (bgmTextShowTime <= 0f)
            {
                bgmTextShowTime = 0f;
                bgmVolumeText.enabled = false;
            }
        }
        if (seTextShowTime > 0f)
        {
            seTextShowTime -= Time.deltaTime;

            if (seTextShowTime <= 0f)
            {
                seTextShowTime = 0f;
                seVolumeText.enabled = false;
            }
        }
        if (systemTextShowTime > 0f)
        {
            systemTextShowTime -= Time.deltaTime;

            if (systemTextShowTime <= 0f)
            {
                systemTextShowTime = 0f;
                systemVolumeText.enabled = false;
            }
        }
    }
}