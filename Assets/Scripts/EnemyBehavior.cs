using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    private NavMeshAgent nav;
    private NavMeshObstacle navObj;

    public float attackDistance = 2f;
    public float rotationSpeed = 2f;

    private bool isAttacking = false;
    private bool startMoving = false;
    private bool getCloser = false;
    private bool stopMoving = false;

    private Vector3 direction = Vector3.zero;

    void Awake()
    {
        navObj = GetComponent<NavMeshObstacle>();
        nav = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        bool activeEnemy = false;
        for (int i = 0; i < GameManager.instance.enemies.Count; i++)
        {
            if (this == GameManager.instance.enemies[i])
            {
                activeEnemy = true;
                break;
            }
        }

        if (!activeEnemy)
        {
            GameManager.instance.enemies.Add(this);
        }
    }

    void Start()
    {
        nav.SetDestination(GameManager.instance.player.transform.position);
    }

    void Update()
    {
        Vector3 playerLookDirection = GameManager.instance.player.transform.position - transform.position;
        playerLookDirection.y = 0;

        transform.rotation = Quaternion.LookRotation(playerLookDirection);

        if (startMoving && !navObj.enabled)
        {
            nav.enabled = true;
            nav.SetDestination(GameManager.instance.player.transform.position);
            startMoving = false;
        }

        if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) < attackDistance)
        {
            if (!isAttacking)
            {
                nav.enabled = false;
                navObj.enabled = true;
                isAttacking = true;
            }
        }
        
        if (getCloser)
        {
            foreach (var enemy in GameManager.instance.enemies)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < attackDistance && enemy.navObj.enabled && enemy.isAttacking)
                {
                    nav.enabled = false;
                    navObj.enabled = true;
                    getCloser = false;
                    stopMoving = true;
                }
            }
            
        }

        if (nav.pathStatus != NavMeshPathStatus.PathComplete && !getCloser && !stopMoving)
        {
            getCloser = true;
        }
    }

    void OnDestroy()
    {
        GameManager.instance.enemies.Remove(this);

        foreach (var enemy in GameManager.instance.enemies)
        {
            if (Vector3.Distance(enemy.transform.position, GameManager.instance.player.transform.position) > attackDistance)
            {
                enemy.navObj.enabled = false;
                enemy.startMoving = true;
                enemy.stopMoving = false;
            }
        }
    }
}
