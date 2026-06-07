using Unity.Entities;

public struct EnemySpawnPreset : IBufferElementData
{
    public Entity entity;
    public CharacterData characterData;
    public EnemyData enemyData;
}

public struct EnemySpawnConfig : IComponentData
{
    public float timeUntilSpawn;
    public float baseSpawnInterval;
    public float spawnIntervalRandomness;
    public float intervalLevelMultiplier;
    public int level;
}
