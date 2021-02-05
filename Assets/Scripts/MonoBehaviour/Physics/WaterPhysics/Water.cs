using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Water : MonoBehaviour 
{
    [SerializeField] private PostProcessProfile profile;
    [SerializeField, Range(0.0f, 100.0f)] private float viscosity;
    public float WaterLevelY {get; private set;}

    public PostProcessProfile GetProfile => profile;
    public float GetViscosity => viscosity;

    private void Start() => WaterLevelY = transform.Find("WaterLevel").position.y;

}