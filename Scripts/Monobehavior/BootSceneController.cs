using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneController : MonoBehaviour
{
    public static BootSceneController bootInstance;
    public string keyLevel = "level";
    public string keyPLevel = "pLevel";

    public int mainLevel;

    private void Awake()
    {
        bootInstance = this;
        bootInstance.mainLevel = PlayerPrefs.GetInt(keyLevel, 1);
        Debug.Log("main level started from " + bootInstance.mainLevel);
        PlayerPrefs.SetInt(keyLevel, bootInstance.mainLevel);

        if (mainLevel < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(bootInstance.mainLevel);
        }
        else
        {
            Debug.Log(PlayerPrefs.GetInt(bootInstance.keyPLevel));
            SceneManager.LoadScene(PlayerPrefs.GetInt(bootInstance.keyPLevel));
        }

        //TinySauce.OnGameStarted("" + PlayerPrefs.GetInt(BootSceneController.bootInstance.keyLevel));

        DontDestroyOnLoad(gameObject);
    }
}
