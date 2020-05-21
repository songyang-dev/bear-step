using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbMessageUI : MonoBehaviour
{
    public Animator animationController;

    private void Awake()
    {
        GetComponent<Text>().text = "";
    }

    public void Display(string message)
    {
        var text = GetComponent<Text>().text = message;
        animationController.SetTrigger("Appear");
        
        // let the UI fade after some time
        IEnumerator Fade()
        {
            yield return new WaitForSeconds(3f);
            animationController.SetTrigger("Disappear");
        }
        StartCoroutine(Fade());
    }
}
