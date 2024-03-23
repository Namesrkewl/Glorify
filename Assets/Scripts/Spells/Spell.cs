public class Spell : PlayerAction, Identifiable {
    public SpellType type;
    public enum SpellType {
        Damaging,
        Healing,
        Movement,
        Passive,
        Support,
    }
    public TargetType targetType;
    public enum TargetType {
        Ally,
        Any,
        Area,
        Aura,
        Enemy,
        Self
    }
    public School school;
    public enum School {
        Magic,
        Physical,
        Ranged
    }
    public float cooldown;
    public float duration;
    public float castTime;
    public int manaCost;
    public int levelRequired;
    public int damage;
    public int range;
    public bool needsLineOfSight;
    public bool isGlobalCooldown;
    public DamageType damageType;
    public DamageSchool damageSchool;
    public Key key;
    // Other spell properties like damage, duration, etc., can be added here.

    public override void ExecuteAction(PlayerBehaviour playerBehaviour) {
        //playerBehaviour.HandleSpell(this);
    }

    public virtual void Cast(Character caster, Character target) {
        
    }

    public virtual void RemoveEffect(Character character) {

    }

    public virtual void UpdateDescription() {

    }

    public Spell() {
        key = new Key();
    }

    public Key GetKey() {
        return key;
    }

    public void SetKey(Key _key) {
        key = _key;
    }
}
