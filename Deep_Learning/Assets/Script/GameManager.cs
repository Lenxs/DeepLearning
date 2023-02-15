using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Constant: int
{
       Left = -1, // %size == 0 = bord
       Right = 1, // %size == size-1 = bord
       Top = 4, // +4 > size = bord
       Bottom = -4 // -4 < 0 = bord
}


public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] map;
    [SerializeField] int playerPos; //position du joueur
    [SerializeField] int[] rewards; // les recompenses
    [SerializeField] int[] instantT;// valeur instant T
    [SerializeField] int[] T1;// instant t +1 
    [SerializeField] Constant[] action;// move foreach case
    [SerializeField] float gamma;

    // Start is called before the first frame update
    void Start()
    {
        //map = new GameObject[16];
        //reward = new int[16];
        for(int i = 0; i < 16; i++)
        {
            rewards[i] = 0;
            if (i == 15)
            {
                rewards[i] = 1;
            }
            T1[i] = 0;
            instantT[i] = 0;
            int randomMove = (int)Random.Range(0, 4);
            if(randomMove + i > 16)
            {

            }
        }
        playerPos = 0;
    }


    void ValueIteration()
    {
        //Choose move random(0,4)

    }
    void Policy()
    {

    }

    


    // Update is called once per frame
    void Update()
    {
        
    }
}
