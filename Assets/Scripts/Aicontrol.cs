using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aicontrol : MonoBehaviour
{
    public Gamemanager gm;
    public GameObject target;
    public int carindex;
    public float angle;
    public Rigidbody r;
    public float speed;
    public float rotatespeed;
    public ForceMode fm;

    public int lasttouchcarindex;


    public AudioSource hitsound;
    public float explosionpower;



    bool canmove;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("findfirsttarget", 3f);
    }

    void findfirsttarget() 
    {
        canmove = true;

       // target = gm.getnewtarget(carindex);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canmove==false)
        {
            return;
        }
        if (target.gameObject != null)
        {
            Vector3 directiontotarget = target.transform.position - this.transform.position;

            Vector3 perp = Vector3.Cross(this.transform.forward, directiontotarget);
            float dir = Vector3.Dot(perp, Vector3.up);

            if (dir > 0f)
            {
            }
            else if (dir < 0f)
            {
            }
            else
            {
            }


            r.AddRelativeTorque(0, dir * rotatespeed, 0, fm);
            r.AddRelativeForce(0, 0, speed, fm);
            

        }
        else
        {
            //Invoke("findfirsttarget", 1f);
           // print("else working....");
           target= gm.getnewtarget(carindex); 
        }
    }






    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("carcollision"))
        {
            if (collision.gameObject.tag=="Player")
            {
                lasttouchcarindex = 7;
                print("player hit");
            }
            else
            {
                lasttouchcarindex = collision.gameObject.GetComponent<Aicontrol>().carindex;

            }
            hitsound.PlayOneShot(hitsound.clip);
            Vector3 direction = collision.transform.position - this.transform.position;

            r.AddExplosionForce(explosionpower, collision.transform.position, 10f, 0, ForceMode.Impulse);

            collision.gameObject.GetComponent<Rigidbody>().
            AddExplosionForce(explosionpower, this.transform.position, 10f, 0, ForceMode.Impulse);

            //Camera.main.transform.LookAt(direction);
        }
        else if (collision.gameObject.layer== LayerMask.NameToLayer("outside"))
        {
            print("car disqualified");
            gm.carkilled(carindex,lasttouchcarindex);
             Destroy(this.transform.gameObject);
            this.enabled = false;
        }



    }





}
