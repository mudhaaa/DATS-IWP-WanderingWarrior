using UnityEngine;

[CreateAssetMenu]
// character class with stat spread and unique aktions
public class CharacterKlass : ScriptableObject
{
    [SerializeField] private int health;
    [SerializeField] private int mana;
    [SerializeField] private int strength;
    [SerializeField] private int magic;
    [SerializeField] private int endurance;
    [SerializeField] private int speed;
    [SerializeField] private Aktion unique1;
    [SerializeField] private Aktion unique2;

    // Health
    #region Health
    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int value)
    {
        health = value;
    }
    #endregion
    // Mana
    #region Health
    public int GetMana()
    {
        return mana;
    }

    public void SetMana(int value)
    {
        health = value;
    }
    #endregion
    #region Strength
    // Strength
    public int GetStrength()
    {
        return strength;
    }

    public void SetStrength(int value)
    {
        strength = value;
    }
#endregion
    #region Magic
    // Magic
    public int GetMagic()
    {
        return magic;
    }

    public void SetMagic(int value)
    {
        magic = value;
    }
#endregion
    #region Endurance
    // Endurance
    public int GetEndurance()
    {
        return endurance;
    }

    public void SetEndurance(int value)
    {
        endurance = value;
    }
#endregion
    #region Speed
    // Speed
    public int GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(int value)
    {
        speed = value;
    }
#endregion

    // Unique1
    public Aktion GetUnique1()
    {
        return unique1;
    }

    public void SetUnique1(Aktion value)
    {
        unique1 = value;
    }

    // Unique2
    public Aktion GetUnique2()
    {
        return unique2;
    }

    public void SetUnique2(Aktion value)
    {
        unique2 = value;
    }
}