using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Dungeonpos : MonoBehaviour
{
    private Animator Dungeoinanim;
    [SerializeField] Transform dungeon;

    [SerializeField] GameObject dungonimage;
    public UnityEvent Onplayed;
    [SerializeField] float pos;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        /*  if (collision.gameObject.name == "player2")
          {
              dungeon.position = new Vector2(collision.transform.position.x, collision.transform.position.y + pos);
              Onplayed?.Invoke();
              Debug.Log("플레이어 던전진입");
          }*/
        if (collision.gameObject.name == "player2")
        {
            dungonimage.SetActive(true);
            dungeon.position = new Vector2(collision.transform.position.x, collision.transform.position.y + pos);
            collision.GetComponent<PlayerInput>().enabled = false;
            StartCoroutine(dungeonRoutin());
            Debug.Log("플레이어 던전진입");
        }
    }

    IEnumerator dungeonRoutin()
    {
        yield return new WaitForSeconds(0.9f);
        Onplayed?.Invoke();
        yield return new WaitForSeconds(2f);
        GameManager.Scene.LoadSceneAsync("testest");
    }
   
}
