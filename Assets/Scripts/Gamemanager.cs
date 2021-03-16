using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
                  
public class Gamemanager : MonoBehaviour
{
    public GameObject playercar;
    public int carcount;
    public bool StartWithRandomCarCount;
    public GameObject enemycarobj;
    public GameObject[] cars;
    public bool[] alivecars;

    public float carspeed;
    public float carsrotatespeed;
    public float explosionpower;



    public TMP_Text fpstext;
    public TMP_Text feed;
    public TMP_Text countdown;
    float currentvalue;
    void Start()
    {
        if (StartWithRandomCarCount==true)
        {
            carcount = Random.Range(1,8);
            feed.text = "Random enemy count enabled, spawned" + carcount + "enemy car. \n";
        }
        cars = new GameObject[carcount+1];
        alivecars = new bool[carcount+1];
        InvokeRepeating("checkfps", 0.2f,0.2f);
        float angletospawn = 0;
        GameObject spawnmanager= new GameObject();
        for (int i = 0; i < carcount; i++)
        {
            GameObject enemycar = Instantiate(enemycarobj,spawnmanager.transform.forward*12f,Quaternion.identity);
            enemycar.gameObject.name = "enemy_Car"+i;
            enemycar.transform.LookAt(Vector3.zero);

            enemycar.GetComponent<Aicontrol>().gm = this;
            enemycar.GetComponent<Aicontrol>().carindex = i;
            enemycar.GetComponent<Aicontrol>().speed = carspeed;
            enemycar.GetComponent<Aicontrol>().rotatespeed = carsrotatespeed;

            cars[i] = enemycar;
            alivecars[i] = true;
            angletospawn += 360f / cars.Length;
            spawnmanager.transform.rotation=Quaternion.Euler(0,angletospawn,0);
        }
        cars[carcount] = playercar;
        playercar.GetComponent<Carcontrol>().carindex = carcount;
        playercar.GetComponent<Carcontrol>().speed = carspeed;
        playercar.GetComponent<Carcontrol>().rotatespeed = carsrotatespeed;

        alivecars[carcount] = true;
        playercar.transform.position = spawnmanager.transform.forward * 12f;
        playercar.transform.LookAt(Vector3.zero);
        StartCoroutine(delay(3f));
    }

    IEnumerator delay(float t) 
    {
        currentvalue = t;
        while (currentvalue >= 0)
        {
        countdown.text = currentvalue.ToString();
            yield return new WaitForSeconds(1f);
            currentvalue--;
        }
        countdown.text = "";
        playercar.GetComponent<Carcontrol>().canmove = true;
        for (int i = 0; i < cars.Length-1; i++)
        {
            cars[i].GetComponent<Aicontrol>().canmove = true;
        }
        yield return new WaitForSeconds(3);
    }
    
    void checkfps() 
    {
        int fps = (int)(1f / Time.unscaledDeltaTime);
        fpstext.text = fps.ToString();
    }


    public GameObject getnewtarget(int carindex)
    {
        List<GameObject> alives = new List<GameObject>();
        for (int i = 0; i < cars.Length; i++)
        {
            if (alivecars[i] == true && carindex != i)
            {
                alives.Add(cars[i]);
            }
        }
        int newtargetindex = 0;
        newtargetindex = Random.Range(0, alives.Count);
        return alives[newtargetindex];
    }
        
    
    public void carkilled(int killedcarindex,int whokilled) 
    {
        alivecars[killedcarindex] = false;

        if (killedcarindex == carcount)
        {
            print("player killed");
            feed.text += "" + whokilled + ". car killed you    \n";
        }
        else if (whokilled!=carcount)
        {
            feed.text += "" + whokilled + ". car killed " + killedcarindex + ". car \n";

        }
        else if(whokilled==carcount)
        {
        feed.text += "You killed " + killedcarindex+". car \n";
        }
        


        int destroyedcarcount = 0;
        int alivecarindex = 0;
        for (int i = 0; i < cars.Length; i++)
        {
            if (alivecars[i] == false)
            {
                destroyedcarcount++;
            }
            else { alivecarindex = i; }
        }
        if (destroyedcarcount==carcount)
        {
            if (alivecarindex!=carcount)
            {
                feed.text += "Game Ended, winner car:" + alivecarindex;

                cars[alivecarindex].GetComponent<Aicontrol>().enabled = false;
            }
            else
            {
                feed.text += "Game Ended, You Win!";
                cars[alivecarindex].GetComponent<Carcontrol>().enabled = false;
            }
            this.enabled = false;
        }

    }

    public void collisioncalculate(float speeddifference,GameObject collisionobj,Rigidbody r) 
    {
        float multiplier = 0;
        if (speeddifference > 0)
        {
            multiplier = 1; //0
        }
        else if (speeddifference == 0)
        {
            multiplier = Random.Range(0.6f, 1.2f);  //2
        }
        else
        {
            multiplier = 0.2f;//1
        }
        //push back the car if multiplier is not 0(mean this car have lower speed than other one)
        //r.AddExplosionForce(explosionpower / 2 * multiplier, collisionobj.transform.position, 10f, 0, ForceMode.Impulse);

        if (collisionobj!=null)
        {

            
         collisionobj.GetComponent<Rigidbody>().AddForceAtPosition((collisionobj.transform.position - r.transform.position)
          * multiplier * explosionpower, r.transform.position, ForceMode.Impulse);
              /* 
            multiplier = Mathf.Clamp(r.velocity.magnitude,0,8f);
            collisionobj.GetComponent<Rigidbody>().AddForceAtPosition((collisionobj.transform.position - r.transform.position)
    * multiplier , r.transform.position, ForceMode.Impulse);

            */
        }
        

        //collisionobj.GetComponent<Rigidbody>().AddForceAtPosition((collisionobj.transform.position-r.transform.position)
        //    * explosionpower,r.transform.position,ForceMode.Impulse);


        /*
        collisionobj.gameObject.GetComponent<Rigidbody>().
        AddExplosionForce(explosionpower, r.transform.position, 10f, 0, ForceMode.Impulse);
        */
    }


    public void restartscene() 
    {
        SceneManager.LoadScene("SampleScene");
    }



  

}
