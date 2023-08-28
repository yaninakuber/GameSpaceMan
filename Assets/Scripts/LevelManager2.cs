using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager sharedInstance;

    public List<LevelBlock> allTheLevelBlock = new List<LevelBlock>();
    public List<LevelBlock> currentLevelBlock = new List<LevelBlock>();

    public Transform levelStartPosition;

    private int blockCount = 0;
    private int quantityTotalBlocks = 8;
    LevelBlock block;
    Vector3 spawnPosition = Vector3.zero;

    void Awake()
    {
        if(sharedInstance == null)
        {
            sharedInstance = this;
        }
    }


    void Start()
    {
        GenerateInitialBlock();
        AddNextBlock();
    }

    public void GenerateInitialBlock()
    {
        block = Instantiate(allTheLevelBlock[0]);
        spawnPosition = levelStartPosition.position;
        AddCurrentLevelBlock();
    }

    void AddCurrentLevelBlock()
    {
        currentLevelBlock.Add(block);
    }

    public void AddNextBlock()
    {
        for (int i = 0; i < 2; i++)
        {
            AddRandomLevelBlock();
            blockCount++;
        }
    }

    public void AddRandomLevelBlock()
    {
        CalculationRandomLevelBlock();
        CorrectionPosition();
        AddCurrentLevelBlock();
    }

    public void CalculationRandomLevelBlock()
    {
        int randomIdx = Random.Range(0, allTheLevelBlock.Count - 1);
        block = Instantiate(allTheLevelBlock[randomIdx]);
        spawnPosition = currentLevelBlock[currentLevelBlock.Count - 1].endPoint.position;
    }

    void CorrectionPosition()
    {
        block.transform.SetParent(this.transform, false);

        Vector3 correction = new Vector3(spawnPosition.x - block.startPoin.position.x, spawnPosition.y - block.startPoin.position.y, 0);
        block.transform.position = correction;
    }


    public void GenerateFinalBlock()
    {
        block = Instantiate(allTheLevelBlock[6]);
        spawnPosition = currentLevelBlock[currentLevelBlock.Count - 1].endPoint.position;
    }

    public void AddFinalBlock()
    {
        GenerateFinalBlock();
        CorrectionPosition();
    }


    public void GenerateBlocks()
    {
        blockCount = 0; //la inicializo en 0 para que cuando muera se vuelvan a generar los bloques

        while (blockCount < quantityTotalBlocks)
        {
            AddNextBlock();
        }

        if (blockCount == quantityTotalBlocks)
        {
            AddFinalBlock();
            blockCount++;
        }
    }


    public void RemoveAllLevelBlock()  // game Manager
    {
        while (currentLevelBlock.Count > 0)
        {
            RemoveLevelBlock();
        }
    }

    public void RemoveLevelBlock() 
    {
        LevelBlock oldBlock = currentLevelBlock[0];
        currentLevelBlock.Remove(oldBlock);
        Destroy(oldBlock.gameObject);
    }


}
