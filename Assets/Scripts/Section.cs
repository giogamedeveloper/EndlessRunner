using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Section : MonoBehaviour
{
    [Range(0, 100)]
    public int columns;
    [Range(0, 100)]
    public int row;

    public Grid grid;
    [SerializeField]
    private RandomObjectSpawnPoint[] powerUp;
    [SerializeField]

    private RandomObjectSpawnPoint[] enemies;


    public Transform cameraTransform;
    // Read-only property that will give us half the number of columns multiplied by the grid size to determine the size occupied by the half section in units.

    public float halfWidth
    {
        get
        {
            return ((columns / 2) * grid.cellSize.x);
        }
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        //If there is no reference to the gruid.
        if (!grid)
        {
            //We try to get it from the hierarchy.
            grid = GetComponentInChildren<Grid>();
            //If it does not exist, we cut the drawing to avoid errors.
            if (!grid) return;
        }
        //If the values ​​are even, we draw green, otherwise we draw red.
        if (columns % 2 == 0 && row % 2 == 0)
        { Gizmos.color = Color.green; }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireCube(transform.position, new Vector3(columns * grid.cellSize.x, row * grid.cellSize.y, 0));
    }

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        //1- We calculate the x position of the left side of the screen in the world.
        //2- The orthographiSize is half the height of the camera.
        //3- Screen.width is the width of the screen in pixels.
        //4- Screen.height is the height of the screen in pixels.

        float leftSideOfScreen = cameraTransform.position.x - Camera.main.orthographicSize * Screen.width / Screen.height;
        //If the current position in X is displayed on the left side of the screen.
        if (transform.position.x < (leftSideOfScreen - halfWidth))
        {
            //We send to create a new section and destroy this one.
            DestroySection();
        }
    }
    /// <summary>
    /// Manages the section destruction process.
    /// </summary>
    private void DestroySection()
    {
        //Using our manager with the Singleton tactic (Section Manager), we are going to tell it to generate a new section.
        SectionManager.Instance.SpawnSectionFromPool();
        //We add the section to the pool and destroy the section.
        SectionManager.Instance.AddSectionToPool(this);
    }

    public void DestroyEnemies()
    {
        foreach (RandomObjectSpawnPoint spawn in enemies)
        {
            if (spawn.Spawneable.IsActive)
                spawn.Spawneable.Deactivate();
        }
    }

    public void ActivePowerUp()
    {
        if (powerUp.Length > 0)
            for (int i = 0; i < powerUp.Length; i++)
            {
                powerUp[i].spawnRatio = 1;
            }
    }
}
