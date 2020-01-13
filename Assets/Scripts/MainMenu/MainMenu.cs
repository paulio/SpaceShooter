using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void NewGameClick()
   {
        const int gameScene = 1;
        SceneManager.LoadScene(gameScene);
   }
}
