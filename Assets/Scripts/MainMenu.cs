using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioSource source;
    private bool Muted = false;

    public Sprite[] spriteArray;

    [SerializeField] private Canvas _main;
    [SerializeField] private Canvas _instructions;
    [SerializeField] private Canvas _credits;

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void MuteButton(Button button) {
       if (Muted) Muted = false;
       else Muted = true;
        
       var State = button.spriteState;
       
       if (Muted) { 
            button.image.sprite = spriteArray[0];
            State.pressedSprite = spriteArray[1];
            source.Pause(); 
       }
       else { 
            button.image.sprite = spriteArray[2];
            State.pressedSprite = spriteArray[3];
            source.Play();
       }
       button.spriteState = State;
    }

    public void QuitApp() {
        Application.Quit();
        Debug.Log("Closing Application.");
    }

    public void Instructions() {
        _instructions.enabled = true;
        _credits.enabled = false;
        _main.enabled = false;
    }

    public void Credits() {
        _instructions.enabled = false;
        _credits.enabled = true;
        _main.enabled = false;
    }

    public void Back() {
        _instructions.enabled = false;
        _credits.enabled = false;
        _main.enabled = true;
    }


}
