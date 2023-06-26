using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
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

        if (!isattack)
        {
            bossmonster.StartCoroutine(AttackRoutin(0.4f));

        }
        Debug.Log("Attack1 Update");
        foreach (Transform t in bossmonster.Attackpoints)
        {
            t.Rotate(-Vector3.back * bossmonster.rotatespeed * Time.deltaTime);
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
      

    }
    IEnumerator Attack2Routin()
    {
      
        GameObject leftobj = bossmonster.handobj[0];
       
        if (Mathf.Abs(leftobj.transform.position.y - bossmonster.player.position.y) > 0.1f && !isAttack2left)
        {

          leftobj.transform.Translate(new Vector3(0, bossmonster.player.position.y,0) * 4 * Time.deltaTime);
          
        }
        else if(isAttack2left)
            {
                bossmonster.handobj[0].transform.Translate(0, 0, 0);
            }

        if (Mathf.Abs(leftobj.transform.position.y - bossmonster.player.position.y) < 0.1f && !isAttack2left)
        {
            bossmonster.handobj[0].transform.Translate(0,0,0);
            isAttack2left = true;
            leftobj.GetComponent<Animator>().SetBool("Attack", true);
           yield return new WaitForSeconds(1.5f);
          leftobj.GetComponent<Animator>().SetBool("Attack", false);
           
        }
 
        yield return new WaitForSeconds(2f);

        GameObject rightobj = bossmonster.handobj[1];
        if (Mathf.Abs(rightobj.transform.position.y - bossmonster.player.position.y) > 0.1f && !isAttack2right)
        {

            rightobj.transform.Translate(new Vector3(0, bossmonster.player.position.y, 0) * 4 * Time.deltaTime);

        }
        else if (isAttack2right)
        {
            bossmonster.handobj[1].transform.Translate(0, 0, 0);
        }


        if (Mathf.Abs(rightobj.transform.position.y - bossmonster.player.position.y) < 0.1f && !isAttack2right)
        {
            bossmonster.handobj[1].transform.Translate(0, 0, 0);
            isAttack2right = true;
            rightobj.GetComponent<Animator>().SetBool("righthand", true);
           yield return new WaitForSeconds(1.5f);
            rightobj.GetComponent<Animator>().SetBool("righthand", false);
        
        }
        yield return new WaitForSeconds(0.5f);
      

        if (Mathf.Abs(leftobj.transform.position.y - bossmonster.player.position.y) > 0.1f && !isAttack2last)
        {

            leftobj.transform.Translate(new Vector3(0, bossmonster.player.position.y, 0) * 4 * Time.deltaTime);

        }
        else if (isAttack2last)
        {
            leftobj.transform.Translate(new Vector3(0, bossmonster.player.position.y, 0) * 4 * Time.deltaTime);
        }

        if (Mathf.Abs(leftobj.transform.position.y - bossmonster.player.position.y) < 0.1f&& !!isAttack2last)
        {
            bossmonster.handobj[0].transform.Translate(0, 0, 0);
            isAttack2last = true;
            leftobj.GetComponent<Animator>().SetBool("Attack", true);
            yield return new WaitForSeconds(1.5f);
            leftobj.GetComponent<Animator>().SetBool("Attack", false);
            
        }
        yield return new WaitForSeconds(5.5f);
        isAttack2end = true;
        bossmonster.StopAllCoroutines();
        bossmonster.ChangeState(SkellBossState.State.Attack1);
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



