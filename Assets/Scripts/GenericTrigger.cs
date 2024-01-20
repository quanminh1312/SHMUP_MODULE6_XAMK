using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTrigger : MonoBehaviour
{
    public UnityEvent eventToTrigger;
    public AudioManager.Track musicToTrigger = AudioManager.Track.None;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        eventToTrigger.Invoke();
        if (musicToTrigger != AudioManager.Track.None)
        {
            AudioManager.instance.PlayMusic(musicToTrigger,true,0.55f);
        }
    }
    private void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, col.size);
        }
    }

}
