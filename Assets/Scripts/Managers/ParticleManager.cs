using UnityEngine;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [System.Serializable]
    public class ParticleEffect
    {
        public string effectName;
        public ParticleSystem particlePrefab;
        public int poolSize = 10;
        [HideInInspector] public List<ParticleSystem> pool = new List<ParticleSystem>();
    }

    public static ParticleSystem PFX_Jump, PFX_Run, PFX_Land, PFX_EnemyDeath, PFX_Damage;

    public ParticleEffect[] effects = new ParticleEffect[]
    {
        new ParticleEffect
            { effectName = "Jump", particlePrefab = PFX_Jump, poolSize = 5, pool = new List<ParticleSystem>() },
        new ParticleEffect
            { effectName = "Land", particlePrefab = PFX_Land, poolSize = 5, pool = new List<ParticleSystem>() },
        new ParticleEffect
            { effectName = "EnemyDeath", particlePrefab = PFX_EnemyDeath, poolSize = 10, pool = new List<ParticleSystem>() },
        new ParticleEffect
            { effectName = "Damage", particlePrefab = PFX_Damage, poolSize = 5, pool = new List<ParticleSystem>() },
        new ParticleEffect { effectName = "Run", particlePrefab = PFX_Run, poolSize = 3, pool = new List<ParticleSystem>() }
    };


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePools()
    {
        foreach (ParticleEffect effect in effects)
        {
            for (int i = 0; i < effect.poolSize; i++)
            {
                ParticleSystem ps = Instantiate(effect.particlePrefab, transform);
                ps.gameObject.SetActive(false);
                effect.pool.Add(ps);
            }
        }
    }

    public void PlayEffect(string effectName, Vector3 position, Color? color = null)
    {
        ParticleEffect effect = System.Array.Find(effects, e => e.effectName == effectName);
        if (effect == null) return;

        ParticleSystem availablePS = effect.pool.Find(ps => !ps.isPlaying);
        if (availablePS == null)
        {
            availablePS = Instantiate(effect.particlePrefab, transform);
            effect.pool.Add(availablePS);
        }

        availablePS.transform.position = position;
        if (color.HasValue)
        {
            var main = availablePS.main;
            main.startColor = color.Value;
        }
        availablePS.gameObject.SetActive(true);
        availablePS.Play();
    }
}
