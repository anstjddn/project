using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.UI;

public class Banshee : MonoBehaviour, IHitable
{
    [SerializeField] public int maxhp;
    [SerializeField] GameObject textprefabs;
    [SerializeField] GameObject coinprefabs;
    [SerializeField] private int coinmoney;
    [SerializeField] GameObject dieeffect;
    [SerializeField] public int curhp;
    [SerializeField] Slider hpbar;
    [SerializeField] Canvas hpbackground;

    private SpriteRenderer hit;
    private void Awake()
    {
        coinmoney = Random.Range(5, 11);
        hit = GetComponent<SpriteRenderer>();
        curhp = maxhp;
        hpbar.maxValue = maxhp;
        hpbar.value = curhp;
        hpbackground.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        hpbar.value = curhp;
    /*    if (curhp > 0)
        {
         
        }
        else
        {
            Destroy(gameObject);
            dieeffect = Instantiate(dieeffect, transform.position, Quaternion.identity);
            Destroy(dieeffect, 3f);
            StartCoroutine(CoinRoutin());
        }*/
    }

    public void TakeHit(int dagame)
    {
        SoundManager.Instance.PlaySFX("MonsterHit");
        hpbackground.gameObject.SetActive(true);
        Instantiate(textprefabs, transform.position, Quaternion.identity);
        curhp -= dagame;
        hit.color = new Color(255, 0, 0, 255);
       StartCoroutine(damageRoutin());
        //  Invoke("prihit", 0.1f);
        if (curhp <= 0)
        {
            SoundManager.Instance.PlaySFX("MonsterDie");
            GameManager.Pool.Release(gameObject);
           GameObject monsterdie = GameManager.Pool.Get(dieeffect, transform.position, Quaternion.identity);
            StartCoroutine(DieRoutine(monsterdie, 3f));
          //  Destroy(dieeffect, 3f);
            StartCoroutine(CoinRoutin());
        }
    }

    IEnumerator CoinRoutin()
    {
        while (coinmoney > 0)
        {
            GameObject coin= GameManager.Pool.Get(coinprefabs, transform.position, Quaternion.identity);
            coinmoney--;
        }
        yield return null;
    }

    IEnumerator damageRoutin()
    {
        hit.color = new Color(255, 255, 255, 255);
        yield return new WaitForSeconds(0.3f);
    }
   private void prihit()
    {
        hit.color = new Color(255, 255, 255, 255);
    }

    IEnumerator DieRoutine(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        GameManager.Pool.Release(obj);
    }

} 

   
   