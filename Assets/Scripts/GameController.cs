using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public Canvas canvasobj;
    private bool Paused = false;
    private bool Muted = false;

    public Sprite[] spriteArray;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)){
            if (Paused) { Paused = false; Time.timeScale = 1; }
            else { Paused = true; Time.timeScale = 0; }
            canvasobj.enabled = Paused;
        }
        
    }

    public void PlayApp()
    {
        canvasobj.enabled = false;
        Paused = false;
        Time.timeScale = 1;
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Closing Application.");
    }

    public void MuteButton(Button button)
    {
        if (Muted) Muted = false;
        else Muted = true;

        var State = button.spriteState;

        if (Muted)
        {
            button.image.sprite = spriteArray[0];
            State.pressedSprite = spriteArray[1];
           // source.Pause();
        }
        else
        {
            button.image.sprite = spriteArray[2];
            State.pressedSprite = spriteArray[3];
           // source.Play();
        }
        button.spriteState = State;
    }
}
