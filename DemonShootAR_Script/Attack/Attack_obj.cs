using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_obj : MonoBehaviour
{
    Rigidbody rigidbody;
    [SerializeField] GameObject Eff_hit;
    [SerializeField] float shotSpeed = 3;
    public bool hitAny_flag = false;
    public bool hitTarget_flag = false;
    public bool killTarget_flag = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity = transform.forward * shotSpeed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "target")
        {
            CPU_move _CPU_move = collision.gameObject.GetComponent<CPU_move>();
            if (_CPU_move.damageCoolTime <= 0)
            {
                ContactPoint hitPos = collision.contacts[0];
                Instantiate(Eff_hit, hitPos.point, Quaternion.identity);
            }
            killTarget_flag = _CPU_move.TapDamage();
            hitTarget_flag = true;
            
        }
        hitAny_flag = true;
    }
}
