using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

enum Constant
{
    Left,
    Right,
    Top,
    Bottom,
    Stop
}

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] map;    
    [SerializeField] GameObject Player; // Joueur
    [SerializeField] int playerPos; // Joueur
    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;
    [SerializeField] int victoryCase;
    [SerializeField] int[] rewards; // les recompenses
    [SerializeField] float[] valueInstantT;// valeur instant T
    [SerializeField] float[] valueT1;// instant t +1 
    [SerializeField] Constant[] action;// move foreach case
    [SerializeField] float gamma;

    private int t_size_boucle;

    bool PolicyTrue = false;

    private Dictionary<Constant, int> movements = new Dictionary<Constant, int>();

    // Start is called before the first frame update
    void Start()
    {
        rewards = new int[map.Length];
        valueInstantT = new float[map.Length];
        valueT1 = new float[map.Length];
        action = new Constant[map.Length];

        t_size_boucle = map.Length;

        movements.Add(Constant.Bottom, -mapWidth);
        movements.Add(Constant.Top, mapWidth);
        movements.Add(Constant.Right, 1);
        movements.Add(Constant.Left, -1);
        movements.Add(Constant.Stop, 0);
        
        for(int i = 0; i < t_size_boucle; i++)
        {
            rewards[i] = 0;
            
            valueT1[i] = 0;

            valueInstantT[i] = 0;

            if (map[i].CompareTag("Obstacle"))
            {
                action[i] = Constant.Stop;
                continue;
            }
            
            // 1 LEFT
            // 2 RIGHT
            // 3 TOP
            // 4 BOTTOM
            int index = Random.Range(0, 4);
            
            Constant randomMove = (Constant)index;
            
            while (!IsPossible(randomMove, i))
            {
                index = Random.Range(0, 4);
                randomMove = (Constant)index;
            }

            action[i] = randomMove;

            Debug.Log($"Index : {i} pour move {randomMove} ");
        }
        playerPos = 0;
        action[victoryCase] = Constant.Stop;
        rewards[victoryCase] = 1;
    }


    bool IsPossible(Constant random,int index)
    {
        switch (random)
        {
            case Constant.Top:
                if (index + mapWidth > t_size_boucle-1 || map[index + mapWidth].CompareTag("Obstacle")) return false;
                break;
            case Constant.Bottom:
                if (index - mapWidth < 0 || map[index - mapWidth].CompareTag("Obstacle")) return false;
                break;
            case Constant.Left:
                if (index%mapWidth == 0 || map[index - 1].CompareTag("Obstacle")) return false;
                break;
            case Constant.Right:
                if (index%mapWidth == mapWidth-1 || map[index + 1].CompareTag("Obstacle")) return false;
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
            if (map[index].CompareTag("Obstacle")) return 0;
            var actionToDo = movements[action[index]];
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
            var actionToDo = movements[movement];
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
            for(int i =0;i< t_size_boucle; i++)
            {
                valueT1[i] = CheckReward(i) + gamma * valueT1[i + movements[action[i]]];
                delta = MathF.Max(delta,Mathf.Abs(valueInstantT[i] - valueT1[i]));
                valueInstantT = valueT1;
            }
        }
    }

    void PolicyImprovement()
    {
        var temp = new Constant[t_size_boucle];

        for (int i = 0; i < t_size_boucle; i++)
        {
            temp[i] = action[i];
            
            if (i == victoryCase) continue;
            
            if (map[i].CompareTag("Obstacle"))
            {
                action[i] = Constant.Stop;
                continue;
            }
            
            List<Constant> allActionForCase = new List<Constant>();
            
            for (int j = 0; j < 4; j++)
            {
                Constant move = (Constant)j;
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

        for (int i=0; i<t_size_boucle; i++)
        {
            if (i == victoryCase) continue;
            
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
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(MovePlayer());
        }
    }

    private IEnumerator MovePlayer()
    {
        while (playerPos != victoryCase)
        {
            yield return new WaitForSeconds(1);
            var positionToGo = map[playerPos + movements[action[playerPos]]];
            Player.transform.position = positionToGo.transform.position;
            playerPos = Array.IndexOf(map, positionToGo);
        }
        Debug.Log("VICTOIRE");
    }
}
