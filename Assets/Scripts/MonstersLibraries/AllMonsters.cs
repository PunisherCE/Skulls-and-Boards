public enum ColorType
{
    Black, White, Gray, Brown,
    Blue, Red, Green, Yellow,
    Orange, Purple
}
public enum AbilityType
{
    Fire, Ice, Poison, Arcane,
    Lava, Electric, Sand, Shadow,
    Light, Metal, trap, Heal,
    Buff, Debuff, Wind, Earth,
    Water, Dark, zombie, holy,
    neutral, legendary, mythical, defense,
    teleportation, summon, transformation
}

public struct Ability
{
    public string name;
    public int manaCost;
    public AbilityType type;
    public int value;
    public string description;
    public ColorType color;
}

public struct Monster
{
    public string name;
    public int health;
    public int mana;
    public int damage;
    public ColorType colorType;

    public Ability ability1;
    public Ability ability2;
    public Ability ability3;

    public Monster(string name, int health, int mana, int damage, ColorType colorType,
                   Ability ability1, Ability ability2, Ability ability3)
    {
        this.name = name;
        this.health = health;
        this.mana = mana;
        this.damage = damage;
        this.colorType = colorType;
        this.ability1 = ability1;
        this.ability2 = ability2;
        this.ability3 = ability3;
    }

};

public static class AllMonsters
{

    public static Monster[] monsters =
    {
        
    };
}
