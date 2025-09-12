using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SectionManager : MonoBehaviour
{
    //List of sections available to generate.
    public Section[] sectionPrefabs;
    //Transform that will contain the generated sections.
    public Transform sectionContainer;
    //Last section generated.
    public Section currentSection;
    //Platforms that are initially generated.
    public int initialLoad = 4;
    private List<Section> sectionPool;
    [Header("Tutorial Support")]
    public bool isTutorialMode = false;
    private Vector3 initialSectionPosition;
    private Section initialSection;
    private static SectionManager _instance;
    public static SectionManager Instance => _instance;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (_instance == null)
        {
            sectionPool = new List<Section>();
            _instance = this;
        }
        else Destroy(this);

    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        //If a section container is not assigned, we initialize it with the manager's own transform.
        if (!sectionContainer) sectionContainer = transform;
        InitializePool();
        initialSectionPosition = transform.position;

        for (int i = 0; i < initialLoad; i++)
        {
            SpawnSectionFromPool();
        }
        if (currentSection != null)
        {
            initialSection = currentSection;
        }
    }
    /// <summary>
    ///Method that initializes the pool with the number of Prefabs sections added.
    /// </summary>
    public void InitializePool()
    {
        for (int i = 0; i < sectionPrefabs.Length; i++)
        {
            Section tmpSection = Instantiate(sectionPrefabs[i]);
            AddSectionToPool(tmpSection);
        }
    }
    /// <summary>
    ///Adds a section to the Pool.
    /// </summary>
    /// <param name="section"></param>
    public void AddSectionToPool(Section section)
    {
        section.gameObject.SetActive(false);
        section.transform.SetParent(transform);
        sectionPool.Add(section);
    }
    /// <summary>
    /// GetSectionFromPool -> They get a random selection within the pool generated previously.
    /// </summary>
    /// <returns></returns>
    private Section GetSectionFromPool()
    {
        int index = Random.Range(0, sectionPool.Count);
        Section section = sectionPool[index];
        sectionPool.RemoveAt(index);
        section.gameObject.SetActive(true);
        section.transform.SetParent(sectionContainer);

        return section;
    }

    /// <summary>
    /// SpawnSectionFromPool-> Method that creates a new level section taken from a pool and positions it after the last one generated.
    /// </summary>
    public void SpawnSectionFromPool()
    {
        //We obtain the prefabs corresponding to the index obtained from a value between 0 and the total number of sections available.
        Section nextSection = GetSectionFromPool();

        Vector3 nextPositionOffset = Vector3.zero;
        nextPositionOffset.x = currentSection.halfWidth + nextSection.halfWidth;

        nextSection.transform.position = currentSection.transform.position + nextPositionOffset;
        currentSection = nextSection;
    }

    public void ActivePowerUp()
    {
        for (int i = 0; i < sectionPrefabs.Length; i++)
        {
            sectionPrefabs[i].ActivePowerUp();
        }
    }
    public void CheckEnemies()
    {
        currentSection.DestroyEnemies();
    }
    
}
