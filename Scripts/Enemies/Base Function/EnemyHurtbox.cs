using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    public void Health(int damage)
    {
        GetComponentInParent<Enemy>().Health(damage);
    }
    public void Coroutine(string param)
    {
        GetComponentInParent<Enemy>().StartCoroutine(param);
    }
    public void Coroutine2Param(string param, int param2)
    {
        GetComponentInParent<Enemy>().StartCoroutine(param, param2);
    }
}
