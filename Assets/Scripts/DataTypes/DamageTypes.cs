public class DamageTypes {
    public DamageType damageType;
    public enum DamageType {
        Physical,
        Magical
    }
    public DamageSchool school; 
    public enum DamageSchool {
        Acid,
        Bludgeoning,
        Cold,
        Fire,
        Force,
        Healing,
        Lightning,
        Necrotic,
        Piercing,
        Poison,
        Psychic,
        Radiant,
        Slashing,
        Thunder
    }
}
