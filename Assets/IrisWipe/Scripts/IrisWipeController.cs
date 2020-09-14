using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class IrisWipeController : MonoBehaviour
{
    #region VARIABLES
    public static IrisWipeController Instance;
    
    [Header("Renderer Feature")] public ForwardRendererData     frd;
    public                              bool                    postFX;

    [Header("Iris Parameters")] public  Material                mat;

    // STORED VARIABLES
    private Vector2 storedPosition;
    private float storedRadius;
    private float storedIntensity;

    public GameObject target;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (frd == null || mat == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WipeIn(0.75f, 0.6f, 0.5f, target);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MoveFromAtoB(0.75f, target, 0.35f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MoveFromAtoB(0.75f, target, .85f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            WipeOut(0.75f);
        }
    }

    private Vector2 WorldPositionToShader(GameObject _target)
    {
        if (_target == null) return Vector2.zero;
        
        Vector2 pos = Camera.main.WorldToScreenPoint(target.transform.position);
        float posX = pos.x / Screen.width  - 0.5f;
        float posY = pos.y / Screen.height - 0.5f;
        
        return new Vector2(posX, posY);
    }

    #region WIPE-IN
    public void WipeIn(float _duration = 1f, float _intensity = 0.6f, float _radius = 0.5f, GameObject _target = null)
    {
        StopAllCoroutines();
        StartCoroutine(WipeIn_Coroutine(_duration, _intensity, _radius, _target));
    }

    public IEnumerator WipeIn_Coroutine(float _duration, float _intensity, float _radius, GameObject _target)
    {
        frd.rendererFeatures[0].SetActive(true);
        
        float timer = 0;
        while (timer < 1f)
        {
            // Timer
            timer += Time.fixedDeltaTime / _duration;

            // Calculate and Evaluate
            float timerRadiusCurve = Mathf.SmoothStep(0f, 1f, timer);
            storedRadius = Mathf.Lerp(2.2f, _radius, timerRadiusCurve);
            storedIntensity = Mathf.SmoothStep(0f, 1f, timer) * _intensity;
            storedPosition = WorldPositionToShader(_target);

            // Set material properties
            mat.SetFloat("_Radius", storedRadius);
            mat.SetFloat("_Intensity", storedIntensity);
            mat.SetFloat("_PosX", storedPosition.x);
            mat.SetFloat("_PosY", storedPosition.y);

            // yield interval
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion
    
    #region WIPE-OUT
    public void WipeOut(float _duration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(WipeOut_Coroutine(_duration));
    }

    public IEnumerator WipeOut_Coroutine(float _duration)
    {
        float timer = 0;
        float startingIntensity = storedIntensity;
        float startingRadius = storedRadius;
        
        while (timer < 1f)
        {
            // Timer
            timer += Time.fixedDeltaTime / _duration;

            // Calculate and Evaluate
            float timerRadiusCurve = Mathf.SmoothStep(0, 1, timer);
            storedRadius    = Mathf.Lerp(startingRadius, 2.2f, timerRadiusCurve);
            storedIntensity = Mathf.SmoothStep(0, 1, 1 - timer) * startingIntensity;

            // Set material properties
            mat.SetFloat("_Radius",    storedRadius);
            mat.SetFloat("_Intensity", storedIntensity);

            // yield interval
            yield return new WaitForFixedUpdate();
        }
        
        frd.rendererFeatures[0].SetActive(false);
    }

    #endregion
    
    #region Move from A to B
    public void MoveFromAtoB(float _duration, GameObject _target, float _radius)
    {
        StopAllCoroutines();
        StartCoroutine(MoveFromAtoB_Coroutine(_duration, _target, _radius));
    }

    private IEnumerator MoveFromAtoB_Coroutine(float _duration, GameObject _target, float _radius)
    {
        float timer = 0;
        
        Vector2 pointA = storedPosition;
        Vector2 pointB = WorldPositionToShader(_target);
        
        float startingRadius = storedRadius;
        
        while (timer < 1f)
        {
            // yield interval
            yield return new WaitForFixedUpdate();
            
            // Timer
            timer += Time.fixedDeltaTime / _duration;

            // Calculate and Evaluate
            storedPosition = Vector2.Lerp(pointA, WorldPositionToShader(_target), Mathf.SmoothStep(0, 1, timer));
            storedRadius = Mathf.Lerp(startingRadius, _radius, Mathf.SmoothStep(0, 1, timer));
            
            // Set material properties
            mat.SetFloat("_Radius", storedRadius);
            mat.SetFloat("_PosX", storedPosition.x);
            mat.SetFloat("_PosY", storedPosition.y);
        }
    }
    #endregion
}