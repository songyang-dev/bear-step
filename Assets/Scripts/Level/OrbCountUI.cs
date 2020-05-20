using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbCountUI : MonoBehaviour
{
    private int totalCount;

    private int currentCount;

    public void InitiateCount(int count)
    {
        this.totalCount = count;
        this.currentCount = 0;
        ChangeDisplay();
    }

    public void Increment()
    {
        this.currentCount++;
        ChangeDisplay();
    }

    private void ChangeDisplay()
    {
        GetComponent<Text>().text = $"Orbs: {currentCount}/{totalCount}";
    }
}
