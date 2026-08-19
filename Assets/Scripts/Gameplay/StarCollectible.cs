using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    private bool isCollected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isCollected)
            return;

        Bubble bubble = other.GetComponent<Bubble>();

        if (bubble != null)
        {
            isCollected = true;
            
            GameAudio.Instance.PlayStarCollect();
            
            Destroy(gameObject);
        }
    }
}
