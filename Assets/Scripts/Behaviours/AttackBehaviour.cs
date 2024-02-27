using FishNet.Demo.AdditiveScenes;
using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour
{
    /*

    #region Auto Attack Logic

    //HandleAutoAttack(player);

    [Server(Logging = LoggingType.Off)]
    private void HandleAutoAttack(Player player) {
        if (playerTargeting.IsAutoAttacking && IsTargetInRangeAndVisible(player)) {
            if (player.autoAttackTimer <= 0f) {
                PerformAutoAttack(player);
                player.autoAttackTimer = CalculateAutoAttackCooldown(player);
            } else {
                player.autoAttackTimer -= Time.deltaTime;
            }
        } else {
            player.autoAttackTimer -= Time.deltaTime;
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void PerformAutoAttack(Player player) {
        GameObject targetObject = playerTargeting.GetCurrentTargetGameObject();
        ServerPerformAutoAttack(player, targetObject);
    }

    [Server(Logging = LoggingType.Off)]
    private bool IsTargetInRangeAndVisible(Player player) {
        GameObject currentTarget = playerTargeting.GetCurrentTargetGameObject();
        if (currentTarget == null) return false;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget > player.autoAttackRange) return false;

        // Calculate direction to target
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;

        // Check if target is in front of the player
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > 45) return false; // Assuming 90 degree field of view (45 degrees on either side of forward direction)

        // Perform raycast to check line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, player.autoAttackRange)) {
            return hit.collider.gameObject == currentTarget;
        }

        return false;
    }

    [Server(Logging = LoggingType.Off)]
    private float CalculateAutoAttackCooldown(Player player) {
        float hasteEffect = 1.0f + (player.haste / 100.0f);
        return Mathf.Max(player.autoAttackCooldown / hasteEffect, 0.5f);
    }
    #endregion
    */
}
