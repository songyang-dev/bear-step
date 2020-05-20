using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbMessageUI : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Text>().text = "";
    }

    public void Display(string message)
    {
        GetComponent<Graphic>().CrossFadeAlpha(1f, 0f, false);

        var text = GetComponent<Text>().text = message;

        GetComponent<Graphic>().CrossFadeAlpha(0f, 5f, false);

    }
}
