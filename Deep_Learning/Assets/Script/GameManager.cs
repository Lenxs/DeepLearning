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
       Bottom = -4,// -4 < 0 = bord
       Stop = 99// -4 < 0 = bord
   
}


public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] map;
    [SerializeField] int playerPos; //position du joueur
    [SerializeField] int[] rewards; // les recompenses
    [SerializeField] float[] valueInstantT;// valeur instant T
    [SerializeField] float[] valueT1;// instant t +1 
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
            

            valueT1[i] = 0;

            valueInstantT[i] = 0;

            // 1 LEFT
            // 2 RIGHT
            // 3 TOP
            // 4 BOTTOM
            int index = Random.Range(0, 4);
            Constant randomMove = (Constant)values.GetValue(index);
            
            while (!IsPossible(randomMove, i))
            {
                 index = Random.Range(0, 4);
                randomMove = (Constant)values.GetValue(index);
            }

            action[i] = randomMove;
            if (i == 15)
            {
                rewards[i] = 1;
            }
            Debug.Log($"Index : {i} pour move {randomMove} ");
        }
        playerPos = 0;
        action[15] = Constant.Stop;

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

    int CheckReward(int index)
    {
        return rewards[index + (int)action[index]];
    }

    void PolicyEvaluation()
    {
        Debug.Log("Play");
        float delta = 0;
        while( delta > 0.001)
        {
            for(int i =0;i< 16; i++)
            {
                
                valueT1[i] = CheckReward(i) + gamma * valueT1[i + (int)action[i]];
                delta = MathF.Max(delta,Mathf.Abs(valueInstantT[i] - valueT1[i]));
                valueInstantT = valueT1;

            }
        }
    }

    


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PolicyEvaluation();
        }
    }
}
