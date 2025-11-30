using UnityEngine;

public class SlimeOrb : MonoBehaviour
{
    private SlimeAtk_Orb owner;


    public void Init(SlimeAtk_Orb slime, int dmg)
    {
        owner = slime;
    }
}
