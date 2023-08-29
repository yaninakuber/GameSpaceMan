using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager SharedInstance;

    public List<LevelBlock> AllTheLevelBlock = new List<LevelBlock>();
    public List<LevelBlock> CurrentLevelBlock = new List<LevelBlock>();

    public Transform LevelStartPosition;

    private int _blockCount = 0;
    private int _quantityTotalBlocks = 8;
    LevelBlock _block;
    Vector3 _spawnPosition = Vector3.zero;

    void Awake()
    {
        if(SharedInstance == null)
        {
            SharedInstance = this;
        }
    }


    void Start()
    {
        GenerateInitialBlock();
        AddNextBlock();
    }

    public void GenerateInitialBlock()
    {
        _block = Instantiate(AllTheLevelBlock[0]);
        _spawnPosition = LevelStartPosition.position;
        AddCurrentLevelBlock();
    }

    private void AddCurrentLevelBlock()
    {
        CurrentLevelBlock.Add(_block);
    }

    public void AddNextBlock()
    {
        for (int i = 0; i < 2; i++)
        {
            AddRandomLevelBlock();
            _blockCount++;
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
        int randomIdx = Random.Range(0, AllTheLevelBlock.Count - 1);
        _block = Instantiate(AllTheLevelBlock[randomIdx]);
        _spawnPosition = CurrentLevelBlock[CurrentLevelBlock.Count - 1].EndPoint.position;
    }

    private void CorrectionPosition()
    {
        _block.transform.SetParent(this.transform, false);

        Vector3 correction = new Vector3(_spawnPosition.x - _block.StartPoin.position.x, _spawnPosition.y - _block.StartPoin.position.y, 0);
        _block.transform.position = correction;
    }


    public void GenerateFinalBlock() //privados todos los que llamo aca
    {
        _block = Instantiate(AllTheLevelBlock[6]); // 6 cambiar por last _block
        _spawnPosition = CurrentLevelBlock[CurrentLevelBlock.Count - 1].EndPoint.position;
    }

    public void AddFinalBlock()
    {
        GenerateFinalBlock();
        CorrectionPosition();
    }


    public void GenerateBlocks()
    {
        _blockCount = 0; //la inicializo en 0 para que cuando muera se vuelvan a generar los bloques

        while (_blockCount < _quantityTotalBlocks)
        {
            AddNextBlock();
        }

        if (_blockCount == _quantityTotalBlocks)
        {
            AddFinalBlock();
            _blockCount++;
        }
    }


    public void RemoveAllLevelBlock()  // game Manager
    {
        while (CurrentLevelBlock.Count > 0)
        {
            RemoveLevelBlock();
        }
    }

    public void RemoveLevelBlock() 
    {
        LevelBlock oldBlock = CurrentLevelBlock[0];
        CurrentLevelBlock.Remove(oldBlock);
        Destroy(oldBlock.gameObject);
    }
}
