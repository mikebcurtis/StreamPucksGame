using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    public Sprite[] LevelSelectImages;
    public GameObject SelectedLevel;
    public MenuScript menuScript;
    private int SelectedLevelId = 1;

	public void SelectLevel(int levelNum)
    {
       
        if (LevelSelectImages != null && LevelSelectImages.Length > 0 && levelNum - 1 < LevelSelectImages.Length)
        {
            SpriteRenderer spriteRenderer = SelectedLevel.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = LevelSelectImages[levelNum - 1];
            SelectedLevelId = levelNum;
        }
    }

    public void PlayButtonClick()
    {
        if (TwitchUserManager.instance.GetComponent<TwitchUserManager>().userInfo != null)
        {
            SceneManager.LoadScene(Constants.GetEndlessSceneName(SelectedLevelId));
        }
        else
        {
            menuScript.SignInOutClick();
        }
    }

    public void RandomButtonClick()
    {
        int randLvl = SelectedLevelId;
        while (randLvl == SelectedLevelId)
        {
            randLvl = Random.Range(1, LevelSelectImages.Length + 1);
        }
        SelectLevel(randLvl);
    }
}
