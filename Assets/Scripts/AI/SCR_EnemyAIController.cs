using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SCR_EnemyAIController : MonoBehaviour
{
    public List<GameObject> enemies;
    public List<GameObject> enemiesInRange;
    public GameObject player;

    void Update()
    {
        foreach (GameObject enemy in enemies)
        {
            SCR_EnemyAI enemyAI = enemy.GetComponent<SCR_EnemyAI>();
            if (IsPlayerInAgroRange(enemy) && !IsPlayerInMinRange(enemy))
            {
                enemyAI.stopDistance = enemyAI.defaultStopDistance;
                enemyAI.behaviour = 0;
            }
            else if (IsPlayerInAgroRange(enemy) && IsPlayerInMinRange(enemy))
            {
                enemyAI.stopDistance = enemyAI.unStopDistance;
                enemyAI.behaviour = 3;
            }
            else
            {
                if (enemiesInRange.Contains(enemy))
                    enemiesInRange.Remove(enemy);
                enemyAI.behaviour = 4;
            }
        }
    }

    //public void DecideBehaviour(GameObject enemy)
    //{
    //    if (!enemiesInRange.Contains(enemy))
    //        enemiesInRange.Add(enemy);
    //    enemy.GetComponent<SCR_EnemyAI>().behaviour = enemiesInRange.IndexOf(enemy);
    //}

    public bool IsPlayerInAgroRange(GameObject enemy)
    {
        if (Vector3.Distance(enemy.transform.position, player.transform.position) <= enemy.GetComponent<SCR_EnemyAI>().agroRange)
            return true;

        else
            return false;
    }

    bool IsPlayerInMinRange(GameObject enemy)
    {
        if (Vector3.Distance(enemy.transform.position, player.transform.position) <= enemy.GetComponent<SCR_EnemyAI>().stopDistance + 0.5)
            return true;

        else
            return false;
    }
}
