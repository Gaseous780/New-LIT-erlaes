using UnityEngine;

public interface ICanBeGrabbed
{
    public void Grab(Vector3 position) { }
    public void Drop() { }
}
