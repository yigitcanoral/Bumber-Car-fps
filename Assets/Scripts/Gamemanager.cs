using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
                  
public class Gamemanager : MonoBehaviour
{
    public GameObject playercar;
    public int carcount;
    public GameObject enemycarobj;
    public GameObject[] cars;
    public bool[] alivecars;

    public TMP_Text fpstext;
    public TMP_Text feed;
    public TMP_Text countdown;
    float currentvalue;
    void Start()
    {
        cars = new GameObject[carcount+1];
        alivecars = new bool[carcount+1];
        InvokeRepeating("checkfps", 0.2f,0.2f);
        float angletospawn = 0;
        GameObject spawnmanager= new GameObject();
        for (int i = 0; i < carcount; i++)
        {
            //GameObject enemycar = Instantiate(enemycarobj,new Vector3(Random.Range(-12,12),0.1f, Random.Range(-12, 12)),Quaternion.identity);
            GameObject enemycar = Instantiate(enemycarobj,spawnmanager.transform.forward*12f,Quaternion.identity);
            enemycar.transform.LookAt(Vector3.zero);

            enemycar.GetComponent<Aicontrol>().gm = this;
            enemycar.GetComponent<Aicontrol>().carindex = i;
            cars[i] = enemycar;
            alivecars[i] = true;
            angletospawn += 360 / cars.Length+1;
            spawnmanager.transform.rotation=Quaternion.Euler(0,angletospawn,0);
        }
        cars[carcount] = playercar;
        playercar.GetComponent<Carcontrol>().carindex = carcount;
        alivecars[carcount] = true;
        playercar.transform.position = spawnmanager.transform.forward * 12f;
        playercar.transform.LookAt(Vector3.zero);
        StartCoroutine(delay(3f));
    }

    void Update()
    {
        
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
          /*
        int whileender = 0;

        while (newtargetindex == carindex || alivecars[newtargetindex] == false&&whileender>)
        {
        newtargetindex = Random.Range(0, alives.Count + 1);
            whileender++;
        }
           */
        return alives[newtargetindex];
    }
        
    
    public void carkilled(int killedcarindex,int whokilled) 
    {
        alivecars[killedcarindex] = false;
        //print(cars[whokilled].gameObject.name+" killed"+cars[killedcarindex].gameObject.name);
        feed.text += "" + whokilled + ". car killed " + killedcarindex+". car \n";
        if (whokilled!=carcount)
        {
            //cars[whokilled].GetComponent<Aicontrol>().target = getnewtarget(whokilled);
        }
         
        //chek if its the last car
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
            feed.text += "Game Ended, winner car:"+alivecarindex;
            cars[alivecarindex].GetComponent<Aicontrol>().enabled = false;
            this.enabled = false;
        }

    }

}
