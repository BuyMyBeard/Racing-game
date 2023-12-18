using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Crash : MonoBehaviour
{
    [SerializeField] float forceTreshold = 100;
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        float force = collision.impulse.magnitude / Time.fixedDeltaTime;
        Debug.Log(force);
        if (force > forceTreshold)
        {
            audioSource.Play();
        }
    }
}
