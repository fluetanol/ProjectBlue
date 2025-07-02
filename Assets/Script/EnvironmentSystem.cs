using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnvironmentSystem : MonoBehaviour
{
    public static EnvironmentSystem instance;

    [Header("Environment Settings")]
    [SerializeField] private Light _light;

    [SerializeField, Range(0, 3)] private float _lightIntensity = 1.75f;
    public float LightIntensity
    {
        get { return _lightIntensity; }
        set
        {
            _light.intensity = value;
            _lightIntensity = value;
        }
    }

    [SerializeField, Range(0,3)] private float _bloomIntensity = 0.2f;
    public float BloomIntensity
    {
        get
        {
            return _bloomIntensity;
        }
        set
        {
            _bloomIntensity = value;
            _bloomComponent.intensity.value = value;
        }
    }

    [SerializeField] private Volume _volume;

    private List<VolumeComponent> _volumeComponents;
    private Bloom _bloomComponent;

    private float _bloomInitIntensity;
    private float _lightInitIntensity;


    void Awake()
    {
        _volumeComponents = _volume.profile.components;
        _bloomComponent = _volume.profile.TryGet<Bloom>(out var bloom) ? bloom : null;

        _bloomInitIntensity = _bloomComponent != null ? _bloomComponent.intensity.value : 0f;
        _lightInitIntensity = _light != null ? _light.intensity : 1.5f;

        instance = this;
    

    }

    public void Reset()
    {
        LightIntensity = _lightInitIntensity;
        BloomIntensity = _bloomInitIntensity;
    }


}
