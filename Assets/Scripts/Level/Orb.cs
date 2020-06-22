using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public void Touched()
    {
        Board board = GetComponentInParent<Board>();

        //this.gameObject.SetActive(false);
        Destroy(this.gameObject);

        board.TouchOrb.Invoke();
    }

}
