using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera camera;
    [SerializeField] private Image fillImage;

    public void ShieldDestroyed() {
        fillImage.color = Color.green;
    }
    public void updateHealthBar(float currentValue, float maxValue) {
        slider.value = currentValue/maxValue;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag == "Enemy") transform.rotation = camera.transform.rotation;
    }

    public void pintarBarraBoss() {
        gameObject.SetActive(true);
    }
}
