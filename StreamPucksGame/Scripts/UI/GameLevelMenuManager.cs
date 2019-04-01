using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevelMenuManager : MonoBehaviour
{
	public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
