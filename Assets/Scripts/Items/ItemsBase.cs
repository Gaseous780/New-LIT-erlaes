using UnityEngine;

public abstract class ItemsBase : MonoBehaviour
{
    [SerializeField] private AudioClip grabObjectSound;
    [SerializeField] private AudioClip dropObjectSound;

    protected SoundManager soundManager;

    protected virtual void Start () { soundManager = GameManager.instance._soundManager; }

    public virtual void OnGrabbed() { if (grabObjectSound != null) { soundManager.ReproduceSound(grabObjectSound); } }

    public virtual void Use() { }

    public virtual void CancelUse() { }

    public virtual void OnDrop() { if (dropObjectSound != null) { soundManager.ReproduceSound(dropObjectSound); } }
}
