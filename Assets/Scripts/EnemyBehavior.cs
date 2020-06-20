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

        if (nav.pathStatus != NavMeshPathStatus.PathComplete)
        {
            nav.enabled = false;
            navObj.enabled = true;
        }

        Vector3 playerLookRotation = GameManager.instance.player.transform.position - transform.position;
        playerLookRotation.y = 0;

        transform.rotation = Quaternion.LookRotation(playerLookRotation);
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
            }
        }
    }
}
