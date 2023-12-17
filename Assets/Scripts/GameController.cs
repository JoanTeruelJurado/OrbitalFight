using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public Slider _HPslider;
    public Slider _Shieldslider;

    public Canvas canvasobj;
    public Canvas endgame;


    private bool Paused = false;
    private bool Muted = false;

    public Sprite[] spriteArray;

    private bool bosskilled = false;
    private bool playerkilled = false;

    public TextMeshProUGUI _title;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)){
            if (Paused) { Paused = false; Time.timeScale = 1; }
            else { Paused = true; Time.timeScale = 0; }
            canvasobj.enabled = Paused;
        }
        if (bosskilled) {
            canvasobj.enabled = false;
            endgame.enabled = true;
            _title.text = "You won!";
        }
        else if (playerkilled) {
            canvasobj.enabled = false;
            endgame.enabled = true;
            _title.text = "You lost!";
        }
    }

    public void ReturnApp() {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetHealth(int health) { 
        _HPslider.value = health;
    }

    public void SetShield(int shield)
    {
        _Shieldslider.value = shield;
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
