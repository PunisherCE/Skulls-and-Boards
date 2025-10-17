public enum ColorType
{
    Black, White, Gray, Brown,
    Blue, Red, Green, Yellow,
    Orange, Purple
}

public struct Ability
{
    public string name;
    public int manaCost;
    public int damage;
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
        new Monster("Red-Dragon", 300, 200, 50, ColorType.Red,
            new Ability { name = "Fire Breath", manaCost = 50, damage = 100, description = "Unleashes a fiery breath that scorches enemies in front of the dragon.", color = ColorType.Red },
            new Ability { name = "Wing Buffet", manaCost = 30, damage = 40, description = "Creates a powerful gust of wind with its wings to knock back enemies.", color = ColorType.Gray },
            new Ability { name = "Tail Swipe", manaCost = 20, damage = 30, description = "Swings its tail in a wide arc to hit multiple enemies.", color = ColorType.Brown }
        ),
        new Monster("Blue-Dragon", 250, 250, 40, ColorType.Blue,
            new Ability { name = "Ice Breath", manaCost = 50, damage = 90, description = "Breathes a chilling frost that slows and damages enemies.", color = ColorType.Blue },
            new Ability { name = "Water Jet", manaCost = 30, damage = 35, description = "Shoots a high-pressure stream of water to knock back and damage foes.", color = ColorType.Blue },
            new Ability { name = "Tail Whip", manaCost = 20, damage = 25, description = "Uses its tail to strike enemies in close range.", color = ColorType.Brown }
        ),
        new Monster("Green-Dragon", 280, 220, 45, ColorType.Green,
            new Ability { name = "Poison Breath", manaCost = 50, damage = 80, description = "Exhales a toxic cloud that poisons and damages enemies over time.", color = ColorType.Green },
            new Ability { name = "Vine Whip", manaCost = 30, damage = 30, description = "Summons vines to lash out at enemies, dealing damage and entangling them.", color = ColorType.Green },
            new Ability { name = "Tail Slam", manaCost = 20, damage = 35, description = "Slams its tail into the ground to create a shockwave that damages nearby foes.", color = ColorType.Brown }
        ),      
        new Monster("Purple-Dragon", 270, 230, 38, ColorType.Purple,
            new Ability { name = "Arcane Breath", manaCost = 50, damage = 85, description = "Unleashes a blast of arcane energy that disrupts and damages enemies.", color = ColorType.Purple },
            new Ability { name = "Mystic Shield", manaCost = 30, damage = 0, description = "Creates a magical barrier that absorbs damage for a short duration.", color = ColorType.Blue },
            new Ability { name = "Tail Spin", manaCost = 20, damage = 30, description = "Spins rapidly to strike multiple enemies with its tail.", color = ColorType.Brown }
        ),
        new Monster("Orange-Dragon", 290, 210, 42, ColorType.Orange,
            new Ability { name = "Lava Breath", manaCost = 50, damage = 95, description = "Breathes molten lava that burns and damages enemies over time.", color = ColorType.Orange },
            new Ability { name = "Earthquake Stomp", manaCost = 30, damage = 50, description = "Stomps the ground to create a localized earthquake that damages and knocks down foes.", color = ColorType.Brown },
            new Ability { name = "Tail Whirl", manaCost = 20, damage = 35, description = "Whirls its tail in a wide arc to hit multiple enemies.", color = ColorType.Brown }
        ),
        new Monster("Yellow-Dragon", 280, 220, 40, ColorType.Yellow,
            new Ability { name = "Electric Breath", manaCost = 50, damage = 85, description = "Unleashes a bolt of lightning that jumps between enemies, dealing damage.", color = ColorType.Yellow },
            new Ability { name = "Thunder Clap", manaCost = 30, damage = 40, description = "Claps its wings together to create a shockwave that stuns and damages foes.", color = ColorType.Gray },
            new Ability { name = "Tail Flick", manaCost = 20, damage = 20, description = "Flicks its tail to strike enemies at a distance.", color = ColorType.Brown }
        ),
        new Monster("Brown-Dragon", 310, 190, 48, ColorType.Brown,
            new Ability { name = "Sand Breath", manaCost = 50, damage = 80, description = "Exhales a cloud of sand that blinds and damages enemies.", color = ColorType.Brown },
            new Ability { name = "Rock Throw", manaCost = 30, damage = 40, description = "Hurls large rocks at enemies to deal damage from a distance.", color = ColorType.Gray },
            new Ability { name = "Tail Bash", manaCost = 20, damage = 30, description = "Bashes its tail into enemies to knock them back and deal damage.", color = ColorType.Brown }
        ),
        new Monster("Black-Dragon", 320, 180, 55, ColorType.Black,
            new Ability { name = "Shadow Breath", manaCost = 50, damage = 110, description = "Releases a dark mist that damages and blinds enemies.", color = ColorType.Black },
            new Ability { name = "Nightmare Howl", manaCost = 30, damage = 45, description = "Emits a terrifying roar that frightens and disorients foes.", color = ColorType.Gray },
            new Ability { name = "Tail Crush", manaCost = 20, damage = 40, description = "Uses its powerful tail to crush enemies in close proximity.", color = ColorType.Brown }
        ),
        new Monster("White-Dragon", 260, 240, 35, ColorType.White,
            new Ability { name = "Light Breath", manaCost = 50, damage = 70, description = "Breathes a radiant light that heals allies and damages undead enemies.", color = ColorType.White },
            new Ability { name = "Solar Flare", manaCost = 30, damage = 40, description = "Emits a burst of sunlight that blinds and burns enemies.", color = ColorType.Yellow },
            new Ability { name = "Tail Flick", manaCost = 20, damage = 20, description = "Flicks its tail to strike enemies at a distance.", color = ColorType.Brown }
        ),
        new Monster("Gray-Dragon", 300, 200, 45, ColorType.Gray,
            new Ability { name = "Metal Breath", manaCost = 50, damage = 90, description = "Breathes a stream of molten metal that damages and slows enemies.", color = ColorType.Gray },
            new Ability { name = "Steel Claw", manaCost = 30, damage = 35, description = "Slashes enemies with razor-sharp claws made of steel.", color = ColorType.Gray },
            new Ability { name = "Tail Strike", manaCost = 20, damage = 25, description = "Strikes enemies with its tail to deal damage and knock them back.", color = ColorType.Brown }
        ),
    };
}
