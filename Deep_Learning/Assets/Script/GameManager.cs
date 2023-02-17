using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

enum Constant: int
{
       Left = -1, // %size == 0 = bord
       Right = 1, // %size == size-1 = bord
       Top = 4, // +4 > size = bord
       Bottom = -4,// -4 < 0 = bord
       Stop = 0// -4 < 0 = bord
   
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

    bool PolicyTrue = false;
    
    Constant[] values = { Constant.Bottom, Constant.Left, Constant.Right, Constant.Top, Constant.Stop };

    // Start is called before the first frame update
    void Start()
    {
        //map = new GameObject[16];
        //reward = new int[16];
        
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
                if (index + 4 > 15 || map[index + 4].CompareTag("Obstacle")) return false;
                break;
            case Constant.Bottom:
                if (index - 4 < 0 || map[index - 4].CompareTag("Obstacle")) return false;
                break;
            case Constant.Left:
                if (index%4 == 0 || map[index - 1].CompareTag("Obstacle")) return false;
                break;
            case Constant.Right:
                if (index%4 == 3 || map[index + 1].CompareTag("Obstacle")) return false;
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
        try
        {
            var actionToDo = (int)action[index];
            var reward = rewards[index + actionToDo];
            return reward;
        }
        catch (Exception e)
        {
            Console.WriteLine("voici les index : " + index );
            throw;
        }
        
    }

    float CheckValueForImprovement(int index, Constant movement)
    {
        try
        {
            var actionToDo = (int)movement;
            var reward = valueInstantT[index + actionToDo];
            return reward;
        }
        catch (Exception e)
        {
            Console.WriteLine("voici les index : " + index);
            throw;
        }

    }

    void PolicyEvaluation()
    {
        float delta = 1.0f;
        while( delta > 0.0001f)
        {
            delta = .0f;
            for(int i =0;i< 16; i++)
            {
                valueT1[i] = CheckReward(i) + gamma * valueT1[i + (int)action[i]];
                delta = MathF.Max(delta,Mathf.Abs(valueInstantT[i] - valueT1[i]));
                valueInstantT = valueT1;
            }
        }
    }

    void PolicyImprovement()
    {
        var temp = new Constant[15];

        for (int i = 0; i < 15; i++)
        {
            temp[i] = action[i];
            List<Constant> allActionForCase = new List<Constant>();
            
            for (int j = 0; j < 4; j++)
            {
                Constant move = (Constant)values.GetValue(j);
                if (IsPossible(move, i)) allActionForCase.Add(move);
            }

            float[] allActionValues = new float[allActionForCase.Count];

            for (int k = 0; k < allActionForCase.Count; k++) allActionValues[k] = CheckValueForImprovement(i, allActionForCase[k]);

            int bestValueIndex = Array.IndexOf(allActionValues, allActionValues.Max());

            Constant bestAction = allActionForCase[bestValueIndex];

            action[i] = bestAction;

            Debug.Log("--------------------------");
            Debug.Log($"Case {i} : nouvelle action -> {bestAction} et ancienne action -> {temp[i]} ||| Reward de l'action : {allActionValues[bestValueIndex]}");
            Debug.Log("--------------------------");

        }

        for (int i=0; i<15; i++)
        {
            if (action[i] != temp[i])
            {
                PolicyTrue = false;
                return;
            }
        }

        PolicyTrue = true;
        return;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            while(!PolicyTrue)
            {
                PolicyEvaluation();
                PolicyImprovement();
            }
            
        }
    }
}
