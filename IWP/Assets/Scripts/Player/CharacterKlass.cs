using UnityEngine;

[CreateAssetMenu]
// character class with stat spread and unique aktions
public class CharacterKlass : ScriptableObject
{
    [SerializeField] private int health;
    [SerializeField] private int aktionPoints;
    [SerializeField] private int strength;
    [SerializeField] private int magic;
    [SerializeField] private int endurance;
    [SerializeField] private int speed;
    [SerializeField] private Aktion unique1;
    [SerializeField] private Aktion unique2;
    [SerializeField] private Aktion basicStrength;
    [SerializeField] private Aktion basicMagic;

    [SerializeField] private GameObject characterPrefab;

    #region Health
    // Health
    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int value)
    {
        health = value;
    }
    #endregion
    #region Aktion Points
    public int GetAP()
    {
        return aktionPoints;
    }
    public void SetAP(int i)
    {
        aktionPoints = i;
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

    #region Aktions
    public Aktion GetBasic1()
    {
        return basicStrength;
    }
    public Aktion GetBasic2()
    {
        return basicMagic;
    }

    public Aktion GetUnique1()
    {
        return unique1;
    }

    public void SetUnique1(Aktion value)
    {
        unique1 = value;
    }

    public Aktion GetUnique2()
    {
        return unique2;
    }

    public void SetUnique2(Aktion value)
    {
        unique2 = value;
    }
    #endregion
}