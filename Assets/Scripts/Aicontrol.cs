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



   public bool canmove;
     public float currentspeed;

    void FixedUpdate()
    {
        if (canmove==false)
        {
            return;
        }
        if (target != null)
        {
            currentspeed += Time.deltaTime*3;

            currentspeed = Mathf.Clamp(currentspeed,12,speed);
            Vector3 directiontotarget = target.transform.position - this.transform.position;
            Vector3 perp = Vector3.Cross(this.transform.forward, directiontotarget);
            float dir = Vector3.Dot(perp, Vector3.up);

            dir = Mathf.Clamp(dir,-1f,1f);

            r.transform.RotateAround(this.transform.position, Vector3.up, dir * rotatespeed);

            //r.AddRelativeTorque(0, dir * rotatespeed, 0, fm);

            r.AddRelativeForce(0, 0, currentspeed, fm);

        }
        else
        {
           target= gm.getnewtarget(carindex); 
        }
    }






    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("carcollision"))
        {
            float speeddifference =0;
            speeddifference = r.velocity.magnitude - collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            if (collision.gameObject.tag=="Player")
            {
                lasttouchcarindex = collision.gameObject.GetComponent<Carcontrol>().carindex;
               // speeddifference = currentspeed - collision.gameObject.GetComponent<Carcontrol>().speed;// change to current speed
            }
            else
            {
                lasttouchcarindex = collision.gameObject.GetComponent<Aicontrol>().carindex;
              //  speeddifference = currentspeed - collision.gameObject.GetComponent<Aicontrol>().currentspeed;
            }
            hitsound.PlayOneShot(hitsound.clip);
            StartCoroutine(callcollisionfunc(speeddifference, collision.gameObject));
            

        }
        else if (collision.gameObject.layer== LayerMask.NameToLayer("outside"))
        {
            print("car disqualified");
            gm.carkilled(carindex,lasttouchcarindex);
             Destroy(this.transform.gameObject);
            this.enabled = false;
        }



    }
    IEnumerator callcollisionfunc(float speeddifference, GameObject collision)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        gm.collisioncalculate(speeddifference, collision.gameObject, r);
        currentspeed -= 10;


    }




}
