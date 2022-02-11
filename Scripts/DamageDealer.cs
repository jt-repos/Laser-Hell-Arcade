using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] GameObject hitVFX;
    [SerializeField] float hitEffectDuration;

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int damageValue)
    {
        damage = damageValue;
    }

    public void Hit()
    {
        GameObject hitEffect = Instantiate(hitVFX, transform.position, Quaternion.identity);
        Destroy(hitEffect, hitEffectDuration);
        Destroy(gameObject);
    }

    public void IncreaseDamage(int damageIncrease)
    {
        damage = damage + damageIncrease;
    } //chyba mozna wyjebac
}
