using UnityEngine;
using UnityEngine.AI;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float baseSpawnInterval = 2f;
    [SerializeField] private float spawnIntervalRandomness = 1f;
    [SerializeField] private float intervalLevelMultiplier = 0.9f;
    [SerializeField] private GameObject[] enemyPresets;
    private float timeUntilSpawn = 0f;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.yellow;
        Handles.matrix = transform.localToWorldMatrix;
        Handles.DrawWireDisc(Vector3.zero, Vector3.up, 1f);
    }
#endif

    private void FixedUpdate()
    {
        timeUntilSpawn -= Time.fixedDeltaTime;
        if (timeUntilSpawn > 0f) return;
        
        var enemyPreset = enemyPresets[Random.Range(0, enemyPresets.Length)];
        Vector2 unitCircle = Random.onUnitCircle;
        Vector3 pos = transform.TransformPoint(unitCircle.x, 0f, unitCircle.y);
        if (!NavMesh.SamplePosition(pos, out var hit, 5f, NavMesh.AllAreas)) return;
        pos = hit.position;
        pos.y += 0.75f;
        Instantiate(enemyPreset, pos, Quaternion.identity);
        timeUntilSpawn = baseSpawnInterval;
        timeUntilSpawn += Random.Range(-spawnIntervalRandomness, spawnIntervalRandomness);
        timeUntilSpawn *= Mathf.Pow(intervalLevelMultiplier, Player.Instance.playerXP.Level);
    }
}
