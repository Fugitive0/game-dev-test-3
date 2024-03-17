using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class playerActions : MonoBehaviour
{
    [Header("KeyBinds")] [Header("References")]

    public static float playerFov = 90f;
    
    
    
    public KeyCode jumpKey = KeyCode.Space;

    public KeyCode pauseKey = KeyCode.Escape;

    public KeyCode reloadKey = KeyCode.R;
    
    
    public static Action Jumping;
    public static Action Pause;
    public static Action GunReload;
    public static Action GunShoot;
    public static Action GunRecoil;
    public static Func<List<TextMeshProUGUI>> TextToDisable;
    public GameObject pauseMenu;

    public static bool _isPaused;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Jump Key
        
        JumpAction();
        
        
        // Pause Button
        
        if (Input.GetKeyDown(pauseKey))
        {
            if (!_isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
        
        // Gun reload key

        if (Input.GetKeyDown(reloadKey))
        {
            GunReload?.Invoke();
        }
        
        // Gun Shoot Key
        
        if (Input.GetMouseButton(0))
        {
            GunShoot?.Invoke();
        }

      
    }
    

    private void JumpAction()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            Jumping?.Invoke();
        }
    }



    public void PauseGame()
    {
        _isPaused = true;
        pauseMenu.SetActive(true);
        List<TextMeshProUGUI> list = TextToDisable.Invoke();
        for (int i = 0; i < list.Count; i++)
        {
            list[i].gameObject.SetActive(false);
        } 
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {   List<TextMeshProUGUI> list = TextToDisable.Invoke();
        for (int i = 0; i < list.Count; i++)
        {
            list[i].gameObject.SetActive(true);
        } 
        _isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    
}
