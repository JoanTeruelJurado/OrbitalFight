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

    public Image _image;
    [SerializeField] private TextMeshProUGUI _timer;

    private int GunSelected = 2;

    private bool Paused = false;
    private bool Muted = false;

    public Sprite[] spriteArray;
    public Sprite[] GunspriteArray;

    private bool bosskilled = false;
    private bool playerkilled = false;

    public TextMeshProUGUI _title;
    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P) && !(bosskilled || playerkilled)){
            if (Paused) { Paused = false; Time.timeScale = 1; }
            else { Paused = true; Time.timeScale = 0; }
            canvasobj.enabled = Paused;
        }
        switch (GunSelected) {
            case 1: _image.enabled = true;   //rifle
                _image.sprite = GunspriteArray[0];
                break;
            case 2: _image.enabled = true;   //laser
                _image.sprite = GunspriteArray[1];
                break;
            default: break;
        }
    }

    public void SetGunSelected(int n) { GunSelected = n; }

    public void SetTimer(float TIMELEFT) {
        int minutes, seconds, cents;
        minutes = (int)(TIMELEFT / 60f);
        seconds = (int)(TIMELEFT - minutes*60f);
        cents = (int)((TIMELEFT - (int)TIMELEFT) * 100f);

        _timer.SetText("{0:00}:{1:00}:{2:00}", minutes, seconds, cents);
    }

    public void Setplayerkilled() {
        playerkilled = true;
        print("You lost!");
        canvasobj.enabled = false;
        endgame.enabled = true;
        Time.timeScale = 0;
        _title.SetText("You lost!");
    }

    public void Setbosskilled() {
        bosskilled = true;
        print("You won!");
        canvasobj.enabled = false;
        endgame.enabled = true;
        Time.timeScale = 0;
        _title.SetText("You won!");
    }

    public void ReturnApp() {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
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
