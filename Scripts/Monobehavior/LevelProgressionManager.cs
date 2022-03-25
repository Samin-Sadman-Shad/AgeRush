using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressionManager : MonoBehaviour
{

    public static LevelProgressionManager levelInstance;

    [SerializeField] string keyCurrent;
    [SerializeField] string keyPrevious;
    [SerializeField] int currLevel;
    [SerializeField] int prevLevel;
    [SerializeField] int repeatingFromLevel;
    [SerializeField] int nonRepeatingLevel;

    void Start()
    {
        levelInstance = this;
        if (BootSceneController.bootInstance != null)
        {
            keyCurrent = BootSceneController.bootInstance.keyLevel;
            keyPrevious = BootSceneController.bootInstance.keyPLevel;
            levelInstance.currLevel = BootSceneController.bootInstance.mainLevel;
        }
    }

    public void LoadNextLevel()
    {
        int tScenes = SceneManager.sceneCountInBuildSettings;
        if (tScenes == 0)
        {
            throw new System.Exception("no scenes in built settings");
        }

        int score = BootSceneController.bootInstance.mainLevel * 2;

        //TinySauce.OnGameFinished(true, score, "" + PlayerPrefs.GetInt(BootSceneController.bootInstance.keyLevel));

        SetNewLevel();
        Debug.Log("current level is set to "+levelInstance.currLevel);
        if (levelInstance.currLevel < tScenes)
        {
            SceneManager.LoadScene(levelInstance.currLevel);
            levelInstance.prevLevel = levelInstance.currLevel;
            PlayerPrefs.SetInt(BootSceneController.bootInstance.keyPLevel, levelInstance.prevLevel);
        }
        else
        {
            var remainder = levelInstance.currLevel % (tScenes - 1 - nonRepeatingLevel);
            if (remainder != 0)
            {
                SceneManager.LoadScene(remainder);
                levelInstance.prevLevel = remainder;
                PlayerPrefs.SetInt(BootSceneController.bootInstance.keyPLevel, levelInstance.prevLevel);
            }
            else
            {
                SceneManager.LoadScene(tScenes - 1- nonRepeatingLevel);
                levelInstance.prevLevel = tScenes - 1- nonRepeatingLevel;
                PlayerPrefs.SetInt(BootSceneController.bootInstance.keyPLevel, levelInstance.prevLevel);
            }
        }

        //TinySauce.OnGameStarted("" + PlayerPrefs.GetInt(BootSceneController.bootInstance.keyLevel));
        /*
        levelInstance.prevLevel = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt(BootSceneController.bootInstance.keyPLevel, levelInstance.prevLevel);
        Debug.Log("previous level is set to " + levelInstance.prevLevel);
        */
    }

    void SetNewLevel()
    {
        BootSceneController.bootInstance.mainLevel++;
        levelInstance.currLevel = BootSceneController.bootInstance.mainLevel;
        PlayerPrefs.SetInt(BootSceneController.bootInstance.keyLevel, levelInstance.currLevel);
        Debug.Log("level number is " + BootSceneController.bootInstance.mainLevel);
    }

    public void ReplayAction()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadPreviousLevel()
    {
        int tScenes = SceneManager.sceneCountInBuildSettings;
        if (tScenes == 0)
        {
            throw new System.Exception("no scenes in built settings");
        }

        if (levelInstance.currLevel < tScenes)
        {
            SceneManager.LoadScene(levelInstance.currLevel);
        }
        else
        {
            var remainder = levelInstance.currLevel % (tScenes - 1 - nonRepeatingLevel);
            if (remainder != 0)
            {
                SceneManager.LoadScene(remainder);
            }
            else
            {
                SceneManager.LoadScene(tScenes - 1 - nonRepeatingLevel);
            }
        }
    }
}
