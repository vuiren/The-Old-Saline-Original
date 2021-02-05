using System;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Interactor
    {
        [Header("Interact Settings")]
        [SerializeField, Range(0.0f, 10.0f)] private float InteractDistance = 1.0f;
        Transform rayStartPoint;
        LayerMask interactionLayers;

        //public Action<GameObject> OnHit { get; set; }

        public Interactor(float interactDistance, Transform rayStartPoint, LayerMask interactionLayers)
        {
            InteractDistance = interactDistance;
            this.rayStartPoint = rayStartPoint;
            this.interactionLayers = interactionLayers;
        }

        public void CharacterInteract()
        {
            bool isHit = Physics.Raycast(rayStartPoint.position, rayStartPoint.forward,
             out RaycastHit objectHit, InteractDistance, interactionLayers);
            if (isHit && objectHit.collider.CompareTag("Interactable"))
            {
                objectHit.collider.GetComponent<InteractableObject>().OnInteract();
            }
        }
    }
}