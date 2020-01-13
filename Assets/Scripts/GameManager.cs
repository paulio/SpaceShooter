using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public bool IsGameOver { get; set; }

    private void Update()
    {
        if (IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            print("ESC pressed");
            Application.Quit();
        }
    }
}
