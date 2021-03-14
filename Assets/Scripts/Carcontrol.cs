using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carcontrol : MonoBehaviour
{
    public Gamemanager gm;
    public float speed;
    public ForceMode fmodemove;
    public float rotatespeed;
    public ForceMode fmoderotate;


    public int carindex;
    public int lasthitcarindex;
    public GameObject carobject;
    public Rigidbody car;
    public GameObject wheelobj;

    public float wheelrotatespeed;
    public float min;
    public float max;
   public AudioSource hitsound;

    public float rotatevalue;
   public bool canmove = false;


    void FixedUpdate()
    {
        if (canmove==false)
        {
            return;
        }
        Vector3 rot = transform.localRotation.eulerAngles;
        rot.z = Mathf.Clamp(rot.z,min,max);
        wheelobj.transform.Rotate(0,0, Mathf.Clamp(  -Input.GetAxis("Horizontal") * rotatespeed/2,min,max));


        //Input.GetAxis("Vertical")
        car.AddRelativeForce(0,0, speed ,fmodemove);
        car.AddRelativeTorque(0, (int)Input.GetAxis("Horizontal") * rotatespeed, 0,fmoderotate);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            car.gameObject.transform.position = new Vector3(0,0,0);
            car.velocity = new Vector3(0,0,0);
            car.gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer==LayerMask.NameToLayer("carcollision"))
        {
            float speeddifference = speed-collision.gameObject.GetComponent<Aicontrol>().currentspeed;
            lasthitcarindex = collision.gameObject.GetComponent<Aicontrol>().carindex;
            hitsound.PlayOneShot(hitsound.clip);

            Vector3 direction = collision.transform.position - this.transform.position;

           
            StartCoroutine(callcollisionfunc(speeddifference,collision.gameObject));
            //gm.collisioncalculate(speeddifference,collision.gameObject,car);
            speed -= 5;

        }
        else if (collision.gameObject.layer==LayerMask.NameToLayer("outside"))
        {
            print("player killed");
            gm.carkilled(carindex,lasthitcarindex);
            Camera.main.transform.parent = null;
            Camera.main.transform.position = new Vector3(-20,15,0);
            Camera.main.transform.rotation = Quaternion.Euler(45,85,0);
            Destroy(this.transform.gameObject);
            this.enabled = false;
        }

    }

    IEnumerator callcollisionfunc( float speeddifference,GameObject collision) 
    {
        yield return new WaitForSeconds(Random.Range(0,0.22f));
        gm.collisioncalculate(speeddifference, collision.gameObject, car);


    }


}
