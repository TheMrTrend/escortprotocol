using UnityEngine;
using System.Collections;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;

    [SerializeField] int HP;

    Color colorOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        //used for movement, enemy looking for player, very few, everything else is calling
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed()); //automatically cleaned in c#, NOT in c++
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.10f);
        model.material.color = colorOrig;
    }
}
