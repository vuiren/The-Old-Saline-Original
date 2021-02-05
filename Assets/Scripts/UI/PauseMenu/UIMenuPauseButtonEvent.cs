using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIMenuPauseButtonEvent : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject player;
    [SerializeField] private TMP_Dropdown sceneLoad;
    public bool IsPauseMenuOpen {get; private set;}

    public void OnGameExit()
    {
        Destroy(player);
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void OnShowMenuPause()
    {
        if(!IsPauseMenuOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            //player.GetComponent<Control>().TakingInput = false;
            pauseMenu.SetActive(true);
            IsPauseMenuOpen = true;
        }
        else
            OnContinueGame();
    }

    public void OnContinueGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        IsPauseMenuOpen = false;
    }

    public void OnSceneLoad()
    {
        SceneManager.LoadScene(sceneLoad.captionText.text, LoadSceneMode.Single);
    }
}
