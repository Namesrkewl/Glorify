using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Healthy", menuName = "Passive/Priest/Healthy")]
public class Healthy : Spell {
    public float healthIncreasePercentage = 0.1f; // 10% increase

    public override IEnumerator Cast(PlayerBehaviour playerBehaviour, EntityBehaviour targetBehaviour) {
        int healthIncrease = (int)(targetBehaviour.characterData.maxHealth * healthIncreasePercentage);
        targetBehaviour.characterData.maxHealth += healthIncrease;
        targetBehaviour.characterData.currentHealth += healthIncrease;
        yield return null;
    }

    public override void RemoveEffect(Character character) {
        //Player player = character as Player;
        Player player = new Player();
        int healthDecrease = (int)(player.maxHealth / healthIncreasePercentage);
        player.maxHealth -= healthDecrease;
        player.currentHealth -= healthDecrease;
    }
}
