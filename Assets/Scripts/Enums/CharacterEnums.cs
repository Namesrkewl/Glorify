public enum TargetType {
    Player,
    Companion,
    Ally,
    Friendly,
    Neutral,
    Hostile,
    Object
}

public enum Races {
    Beast,
    Dragon,
    Humanoid,
    Slime,
    Undead,
    Object
}

public enum TargetStatus {
    Alive,
    Dead,
    Object
}

public enum CombatStatus {
    OutOfCombat,
    InCombat,
    Resetting
}

public enum ActionState {
    Idle,
    AutoAttacking,
    Casting,
    Restricted
}