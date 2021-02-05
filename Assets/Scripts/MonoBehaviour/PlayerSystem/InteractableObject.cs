using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public virtual void OnInteract()
    { }
    public virtual void OnInteract(GameObject player)
    { }
    public virtual void OnInteract(GameObject player, float speed)
    { }
}
