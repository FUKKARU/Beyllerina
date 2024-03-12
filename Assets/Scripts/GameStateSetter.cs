using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameStateSetter
{
    [RuntimeInitializeOnLoadMethod]
    static void RuntimeInitializeOnLoadMethods()
    {
        #region�y�ݒ�z
        SetVsync(); // Vsync�̐ݒ�
        SetTargetFrameRate(); // �^�[�Q�b�g�t���[�����[�g�̐ݒ�
        #endregion

#if UNITY_EDITOR
        #region�yLogError�z
        CheckVsync(); // Vsync�̐ݒ肪���������ǂ������m�F
        CheckTargetFrameRate(); // �^�[�Q�b�g�t���[�����[�g�̐ݒ肪�����������ǂ������m�F
        #endregion

        #region�yLogWarning�z
        #endregion

        #region�yLog�z
        ShowConfirmation(); // �m�F���b�Z�[�W��\��
        #endregion
#endif
    }

    #region ���\�b�h�B�̏ڍ�
    static void SetVsync()
    {
        if (!GameStateSO.Entity.IsVsyncOn)
        {
            QualitySettings.vSyncCount = 0; // VSync��OFF�ɂ���
        }
    }

    static void SetTargetFrameRate()
    {
        Application.targetFrameRate = GameStateSO.Entity.TargetFrameRate;
    }

    static void CheckVsync()
    {
        if (!GameStateSO.Entity.IsVsyncOn && QualitySettings.vSyncCount != 0)
        {
            Debug.LogError("<color=red>Vsync���I�t�ɂȂ��Ă��܂���</color>");
        }
        else if (GameStateSO.Entity.IsVsyncOn && QualitySettings.vSyncCount == 0)
        {
            Debug.LogError("<color=red>Vsync���I���ɂȂ��Ă��܂���</color>");
        }
    }

    static void CheckTargetFrameRate()
    {
        if (Application.targetFrameRate != GameStateSO.Entity.TargetFrameRate)
        {
            Debug.LogError("<color=red>�^�[�Q�b�g�t���[�����[�g���ݒ�ł��Ă��܂���</color>");
        }
    }

    static void ShowConfirmation()
    {
        Debug.Log("<color=cyan>�m�F���Ă��������FBaseSystem�F�e�v���C���[��PlayerMove.cs���́Atype�����������ǂ���</color>");
        Debug.Log("<color=cyan>�m�F���Ă��������FBaseSystem�FGameManager.cs���́ABeys��Texts�̗v�f�̏��Ԃ���v���Ă��邩�ǂ���</color>");
    }
    #endregion
}

// SO�̃o�O�����m
[InitializeOnLoad]
public class AnnounceSOBug
{
    static bool errorShown = false; // �G���[���b�Z�[�W��\���������ǂ����̃t���O

    static AnnounceSOBug()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode && !errorShown)
        {
            // �I������Ă���A�Z�b�g���擾
            Object[] selectedAssets = Selection.objects;

            // �I�����ꂽ�A�Z�b�g���`�F�b�N
            foreach (Object selectedAsset in selectedAssets)
            {
                // �I������Ă���A�Z�b�g�� ScriptableObject �ł��邩���m�F
                if (selectedAsset is ScriptableObject)
                {
                    Debug.LogWarning("<color=yellow>Scriptable Object ��I��������ԂŃQ�[�������s����ƁA�G���[���o��ꍇ������܂��B\r\n����͌���Unity2022�ȍ~�Ŕ������Ă���o�O�ł���A�������m�F�������ł͎��Q�͂���܂���B</color>");
                    Debug.LogWarning("<color=yellow>�Q�l�L���Fhttps://www.create-forever.games/unity2022-3-value-cannot-be-null-_unity_self/</color>");
                    errorShown = true;
                    break;
                }
            }
        }
    }
}
