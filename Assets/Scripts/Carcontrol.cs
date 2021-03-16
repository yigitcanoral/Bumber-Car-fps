using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Carcontrol : MonoBehaviour
{
    public Gamemanager gm;
    public float speed;
    public float currentspeed;
    public ForceMode fmodemove;
    public float rotatespeed;
    public ForceMode fmoderotate;


    public int carindex;
    public int lasthitcarindex;
    public GameObject carobject;
    public Rigidbody car;
    public GameObject wheelobj;

    public float wheelrotatespeed;
    public float maxwheelangle;
   public AudioSource hitsound;

    public float rotatevalue;
   public bool canmove = false;
   public float axis;
    Vector2 startpos;
    Vector2 endpos;

    public float veloci;
    void Update()
    {
        veloci = car.velocity.magnitude;
        if (Input.touchCount>0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase==TouchPhase.Began)
            {
                startpos = new Vector2(t.position.x/(float)Screen.width,t.position.y/(float)Screen.width);
            }
            if (t.phase==TouchPhase.Moved)
            {
                endpos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
                Vector2 a = endpos - startpos;
                axis = a.x*5;
                axis = Mathf.Clamp(axis,-1.0f,1.0f);
            }
        }
        else
        {
            axis = 0;
        }
    }
    void FixedUpdate()
    {
        if (canmove==false)
        {
            return;
        }
        currentspeed += Time.deltaTime * 3;
        currentspeed = Mathf.Clamp(currentspeed,12,speed);

        wheelobj.transform.localRotation=Quaternion.Euler(0,0, Mathf.Clamp(  -axis * wheelrotatespeed*10, -maxwheelangle, maxwheelangle));
        // wheelobj.transform.Rotate(0, 0, Mathf.Clamp(-Input.GetAxis("Horizontal") * rotatespeed, min, max));

        //Camera.main.fieldOfView = Mathf.Clamp(55+currentspeed,55,70);
        Camera.main.fieldOfView = 55 + currentspeed;


        car.AddRelativeForce(0,0, currentspeed ,fmodemove);
        car.transform.RotateAround(this.transform.position,Vector3.up, Input.GetAxis("Horizontal") * rotatespeed);
        car.transform.RotateAround(this.transform.position, Vector3.up, axis * rotatespeed);

        //car.AddRelativeTorque(0, (int)Input.GetAxis("Horizontal") * rotatespeed, 0,fmoderotate);
        //car.MoveRotation(Quaternion.Euler(0, Input.GetAxis("Horizontal") * rotatespeed, 0));

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
            speeddifference = car.velocity.magnitude - collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            lasthitcarindex = collision.gameObject.GetComponent<Aicontrol>().carindex;
            hitsound.PlayOneShot(hitsound.clip);

            Vector3 direction = collision.transform.position - Camera.main.transform.position;
            direction= (direction.normalized)/4f;
            StartCoroutine(cameraeffect(Camera.main.transform.localPosition+ direction,false,0.3f));
           
            StartCoroutine(callcollisionfunc(speeddifference,collision.gameObject));
            
        }
        else if (collision.gameObject.layer==LayerMask.NameToLayer("outside"))
        {
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
        yield return new WaitForSeconds(Random.Range(0,0.1f));
        gm.collisioncalculate(speeddifference, collision.gameObject, car);
        currentspeed -= 10;

    }
    IEnumerator cameraeffect(Vector3 dir,bool onlyonce,float timelength)
    {
        float starttime = Time.time;
        while (Time.time < starttime+ timelength)
        {
            Camera.main.transform.localPosition = Vector3.Slerp(Camera.main.transform.localPosition,dir,(Time.time-starttime)/ timelength);
            yield return null;
        }
        Camera.main.transform.localPosition = dir;
        if (onlyonce==false)
        {
            StartCoroutine(cameraeffect(new Vector3(0,1.2f,-0.7f), true,timelength*2f));
        }
    }


}
