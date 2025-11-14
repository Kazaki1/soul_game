using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;
    public int stackSize = 99;

    public bool isGold;

    [Header("Stats")]
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int damage;

    [Header("For Temporary Items")]
    public float duration;
}
