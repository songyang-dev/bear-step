using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbMessageUI : MonoBehaviour
{
    public GameObject gameManager;

    private Animator animationController;

    public Queue<(string, float)> messageQueue = new Queue<(string, float)>();

    private void Awake()
    {
        animationController = gameManager.GetComponent<GameManager>().orbMessageUIAnimator;
    }

    /// <summary>
    /// Queues the message for the UI to display after the current display is over
    /// </summary>
    /// <param name="message">Message for the UI</param>
    /// <param name="duration">Duration of the message</param>
    public void Display(string message, float duration)
    {
        messageQueue.Enqueue((message, duration));
        animationController.SetTrigger("Appear");
    }

    /// <summary>
    /// Initiates a coroutine that will trigger the disappear trigger after some time
    /// </summary>
    /// <param name="duration">Time before triggering the disappear trigger</param>
    public void FadeAfter(float duration)
    {
        IEnumerator Fade()
        {
            yield return new WaitForSeconds(duration);
            animationController.SetTrigger("Disappear");
        }
        StartCoroutine(Fade());
    }
}
