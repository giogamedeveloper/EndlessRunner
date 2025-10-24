using UnityEngine;

public interface ISpawneable
{
    public bool IsActive { get; }

    public void Activate();
    public void Deactivate();

}

