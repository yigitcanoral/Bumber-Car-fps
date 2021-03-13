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
    public float explosionpower;
    bool canmove = false;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("delaystarting",3f);
    }
    void delaystarting() 
    {
        canmove = true;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (canmove==false)
        {
            return;
        }
        Vector3 rot = transform.localRotation.eulerAngles;
        rot.z = Mathf.Clamp(rot.z,min,max);
        wheelobj.transform.Rotate(0,0, Mathf.Clamp(  -Input.GetAxis("Horizontal") * rotatespeed/2,min,max));

        //rotatevalue += -Input.GetAxis("Horizontal") * wheelrotatespeed;
        //rotatevalue = Mathf.Clamp(rotatevalue,min,max);
        //wheelobj.transform.localRotation = Quaternion.Euler(34, 0, rotatevalue );
        
        car.AddRelativeForce(0,0, Input.GetAxis("Vertical") * speed ,fmodemove);
        car.AddRelativeTorque(0, Input.GetAxis("Horizontal") * rotatespeed, 0,fmoderotate);
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
            lasthitcarindex = collision.gameObject.GetComponent<Aicontrol>().carindex;
            hitsound.PlayOneShot(hitsound.clip);
            Vector3 direction = collision.transform.position - this.transform.position;

            car.AddExplosionForce(explosionpower, collision.transform.position, 10f, 0, ForceMode.Impulse);

            collision.gameObject.GetComponent<Rigidbody>().
            AddExplosionForce(explosionpower, this.transform.position, 10f, 0, ForceMode.Impulse);

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

        //Camera.main.transform.LookAt(direction);
    }

    /*
     * Make a cube. Put a low friction material on it. Add a force in the forwards direction. To turn add a force in the left/right direction at the front end of the cube. Instant car.

Gears are all about the ratio of force to speed. I would create an animation curves for each car and simply read off the values. At low gears you can provide a high force at low speeds, but the force drops off as the speed increases. At high gears you get the opposite behaviour.

Its by no means a physically accurate representation of a car. But it will do pretty well as a simple model.
     * 
     */

}
