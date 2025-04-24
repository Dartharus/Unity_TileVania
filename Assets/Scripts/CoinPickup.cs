using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int coinValue = 100;
    bool hasBeenPickedUp = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && !hasBeenPickedUp)
        {
            hasBeenPickedUp = true;
            FindObjectOfType<GameSession>().AddToScore(coinValue);
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            Destroy(gameObject);
        }
    }
}
