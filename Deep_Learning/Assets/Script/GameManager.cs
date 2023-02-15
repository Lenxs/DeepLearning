using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

enum Constant: int
{
       Left = -1, // %size == 0 = bord
       Right = 1, // %size == size-1 = bord
       Top = 4, // +4 > size = bord
       Bottom = -4// -4 < 0 = bord
   
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
        Array values = Enum.GetValues(typeof(Constant));
        for(int i = 0; i < 16; i++)
        {
            rewards[i] = 0;
            if (i == 15)
            {
                rewards[i] = 1;
            }

            T1[i] = 0;

            instantT[i] = 0;

            // 1 LEFT
            // 2 RIGHT
            // 3 TOP
            // 4 BOTTOM
            int index = Random.Range(0, values.Length);
            Constant randomMove = (Constant)values.GetValue(index);
            
            while (!IsPossible(randomMove, i))
            {
                 index = Random.Range(0, values.Length);
                randomMove = (Constant)values.GetValue(index);
            }
            action[i] = randomMove;
            Debug.Log($"Index : {i} pour move {randomMove} ");
        }
        playerPos = 0;

    }


    bool IsPossible(Constant random,int index)
    {
        switch (random)
        {
            case Constant.Top:
                if (index + 4 > 15) return false;
                break;
            case Constant.Bottom:
                if (index - 4 < 0) return false;
                break;
            case Constant.Left:
                if (index%4==0) return false;
                break;
            case Constant.Right:
                if (index % 4 == 3) return false;
                break;
            default:
                break;
            
        }
        return true;
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
