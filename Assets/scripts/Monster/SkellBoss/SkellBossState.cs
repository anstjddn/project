using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SkellBossState : MonoBehaviour
{
    public enum State { Idle, Attack1, Attack2, Attack3, size }
 
    public Transform player;
    private BaseState[] states;
    private State curstate;
    public int Handspeed;
    public Rigidbody2D monsterRb;
    public Animator monsteranim;
    public Collider2D monsterCollider;
    [SerializeField] public GameObject[] handobj;
    [SerializeField] public GameObject Attack1;
    [SerializeField] public Transform[] Attackpoints;
    [SerializeField] public float rotatespeed;




    private void Awake()
    {
        states = new BaseState[(int)State.size];
        states[(int)State.Idle] = new BossIdleState(this);
        states[(int)State.Attack1] = new BossAttack1State(this);
        states[(int)State.Attack2] = new BossAttack2State(this);
        states[(int)State.Attack3] = new BossAttack3State(this);

        monsterRb = GetComponent<Rigidbody2D>();
        monsteranim = GetComponent<Animator>();
        monsterCollider = GetComponent<Collider2D>();
       

    }
   /* private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, );
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }*/

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        curstate = State.Idle;
        states[(int)curstate].Enter();
    }
    private void Update()
    {
        states[(int)curstate].Update();

    }

    public void ChangeState(State state)
    {
        states[(int)curstate].Exit();
        curstate = state;
        states[(int)curstate].Enter();
    }
   
  
  
}
public class BossIdleState : BaseState           //가만히 있는거
{
    public SkellBossState bossmonster;

    public BossIdleState(SkellBossState bossmonster)
    {
        this.bossmonster = bossmonster;
    }


    public override void Enter()
    {
        Debug.Log("Idle Enter");
    }


    public override void Exit()
    {
        Debug.Log("Idle Exit");

    }

    public override void Update()
    {
        bossmonster.ChangeState(SkellBossState.State.Attack2);


    }
}
public class BossAttack1State : BaseState             //입에서 회전
{
    public SkellBossState bossmonster;
    public bool isattack;
    public int curTime = 0;
    public BossAttack1State(SkellBossState bossmonster)
    {
        this.bossmonster = bossmonster;
    }

    public override void Enter()
    {
       
        bossmonster.monsteranim.SetBool("Attack1", true);
        Debug.Log("Attack1 Enter");
        isattack = false;
    }

    public override void Exit()
    {
        Debug.Log("Attack1 Exit");
        bossmonster.StopAllCoroutines();
    }

    public override void Update()
    {
        float curTime = 0;
        curTime += Time.deltaTime;
       
        if (!isattack)
        {
            bossmonster.StartCoroutine(AttackRoutin(0.4f));

        }
        Debug.Log("Attack1 Update");
        foreach (Transform t in bossmonster.Attackpoints)
        {
            t.Rotate(-Vector3.back * bossmonster.rotatespeed * Time.deltaTime);
           
        }
        if (curTime > 8f)
        {

            bossmonster.StopAllCoroutines();
            bossmonster.ChangeState(SkellBossState.State.Attack2);

        }


        /* if (!isattack)
         {
             bossmonster.StartCoroutine(AttackRoutin(0.4f));

         }*/


    }
    IEnumerator AttackRoutin(float dalay)
    {
       
        isattack = true;
        if (isattack)
        {
            foreach (Transform t in bossmonster.Attackpoints)
            {
                SkellBossState.Instantiate(bossmonster.Attack1, t.position, t.rotation);
            }
            
        }
        yield return new WaitForSeconds(dalay);
      
        isattack = false;
      

    }
}

public class BossAttack2State : BaseState               //손따라가서 레이저
{
    public SkellBossState bossmonster;
    private bool isAttack2left;
    private bool isAttack2right;
    private bool isAttack2end;
    private bool isAttack2last;
 
    public BossAttack2State(SkellBossState bossmonster)
    {
        this.bossmonster = bossmonster;
       

    }

    public override void Enter()
    {
        
        Debug.Log("Attack2 Enter");
        isAttack2left = false;
        isAttack2right = false;
        isAttack2end = false;

    }

    public override void Exit()
    {
        Debug.Log("Attack2 Exit");
    }

    public override void Update()
    {
       
            bossmonster.StartCoroutine(Attack2Routin());
 
        if (isAttack2last)
        {
            bossmonster.StopAllCoroutines();
            bossmonster.ChangeState(SkellBossState.State.Attack1);
        }
      

    }
    IEnumerator Attack2Routin()
    {
      GameObject leftobj = bossmonster.handobj[0];
        GameObject rightobj = bossmonster.handobj[1];

        if (Mathf.Abs(bossmonster.player.position.y- leftobj.transform.position.y) > 0.1f && !isAttack2left)
        {
            Vector2 playerdirleft = (bossmonster.player.position - leftobj.transform.position);
            playerdirleft = new Vector2(0, playerdirleft.y).normalized;
            leftobj.transform.Translate(playerdirleft*4*Time.deltaTime);

        }
   
        if (Mathf.Abs(leftobj.transform.position.y - bossmonster.player.position.y) < 0.1f && !isAttack2left)
        {
            leftobj.transform.Translate(Vector3.zero);
            isAttack2left = true;
            leftobj.GetComponent<Animator>().SetBool("Attack", true);
            yield return new WaitForSeconds(1.5f);                              //지속할떄까지 스탑
            leftobj.GetComponent<Animator>().SetBool("Attack", false);
        }

        yield return new WaitForSeconds(1f);   //왼손쓰고 1초후 오른손

        if (Mathf.Abs(rightobj.transform.position.y - bossmonster.player.position.y) > 0.1f && !isAttack2right)
        {
            Vector2 playerdirRight = (bossmonster.player.position - rightobj.transform.position);
            playerdirRight = new Vector2(0, playerdirRight.y).normalized;
            rightobj.transform.Translate(playerdirRight * 4 * Time.deltaTime);

        }


       if (Mathf.Abs(rightobj.transform.position.y - bossmonster.player.position.y) < 0.1f && !isAttack2right)
        {
            rightobj.transform.Translate(Vector3.zero);
            isAttack2right = true;
            rightobj.GetComponent<Animator>().SetBool("righthand", true);
           yield return new WaitForSeconds(1.5f);
            rightobj.GetComponent<Animator>().SetBool("righthand", false);
        
        }

        yield return new WaitForSeconds(0.5f);

       if (Mathf.Abs(leftobj.transform.position.y - bossmonster.player.position.y) > 0.1f && !isAttack2last)
        {
            Vector2 playerdirleft = (bossmonster.player.position - leftobj.transform.position);
            playerdirleft = new Vector2(0, playerdirleft.y).normalized;
            leftobj.transform.Translate(playerdirleft * 4 * Time.deltaTime);

        }

        if (Mathf.Abs(leftobj.transform.position.y - bossmonster.player.position.y) < 0.1f&& !isAttack2last)
        {
            leftobj.transform.Translate(Vector3.zero);
             
            leftobj.GetComponent<Animator>().SetBool("Attack", true);
            yield return new WaitForSeconds(1.5f);
            leftobj.GetComponent<Animator>().SetBool("Attack", false);
            isAttack2last = true;

        }
        yield return new WaitForSeconds(7f);
        
    }
}
public class BossAttack3State : BaseState                           //칼 꼿히는거
{
    public SkellBossState bossmonster;

    public BossAttack3State(SkellBossState bossmonster)
    {
        this.bossmonster = bossmonster;
    }

    public override void Enter()
    {
        Debug.Log("Attack3 Enter");
    }

    public override void Exit()
    {
        Debug.Log("Attack3 Exit");

    }

    public override void Update()
    {
        Debug.Log("Attack3 Update");
    }
}



