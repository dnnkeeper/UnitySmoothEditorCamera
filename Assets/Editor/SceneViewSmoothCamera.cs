using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorCameraSettings : EditorWindow
{
    public float maxSpeed = 100f;
    public float accelerationRate = 10f;
    public float decelerationRate = 10f;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Editor Camera Settings")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        EditorCameraSettings window = (EditorCameraSettings)EditorWindow.GetWindow(typeof(EditorCameraSettings));
        window.Show();
    }

    void OnGUI()
    {
        maxSpeed = EditorGUILayout.FloatField("MaxSpeed", maxSpeed);
        accelerationRate = EditorGUILayout.FloatField("accelerationRate", accelerationRate);
        decelerationRate = EditorGUILayout.FloatField("decelerationRate", decelerationRate);
        SceneViewSmoothCamera.targetSpeed = EditorGUILayout.Slider("targetSpeed", SceneViewSmoothCamera.targetSpeed, 0f, maxSpeed);

        SceneViewSmoothCamera.maxSpeed = maxSpeed;
        SceneViewSmoothCamera.accelerationRate = accelerationRate;
        SceneViewSmoothCamera.decelerationRate = decelerationRate;
    }
}


[InitializeOnLoad]
public static class SceneViewSmoothCamera
{
    // Silly helper to get proper deltatime inside events
    internal struct TimeHelper
    {
        public float deltaTime;
        long lastTime;

        public void Begin()
        {
            lastTime = System.DateTime.Now.Ticks;
        }

        public float Update()
        {
            deltaTime = (System.DateTime.Now.Ticks - lastTime) / 10000000.0f;
            lastTime = System.DateTime.Now.Ticks;
            return deltaTime;
        }
    }

    public static float maxSpeed = 100f;

    public static float accelerationRate = 10f;

    public static float decelerationRate = 10f;

    public static float targetSpeed = 1f;

    public static float currentSpeed;

    static TimeHelper s_TimeHelper = new TimeHelper();

    public static Vector3 pivotLocalPos, moveDirection;

    static bool inited, rightMouseDown;

    static bool w, s, a, d, q, e;

    static SceneViewSmoothCamera()
    {
        s_TimeHelper.Begin();

        Debug.Log("SceneViewSmoothCamera created");

        SceneView.onSceneGUIDelegate += view =>
        {
            if (!inited) {
                inited = true;
                
            }
            pivotLocalPos = view.camera.transform.InverseTransformPoint(view.pivot);
            s_TimeHelper.Update();

            float deltaTime = s_TimeHelper.deltaTime;

            var eventCurrent = Event.current;


            //Vector3.Lerp(view.pivot, view.camera.transform.TransformPoint(pivotLocalPos), deltaTime * 10f);


            if (Event.current.button == 1)
            {
                if (eventCurrent.type == EventType.MouseDown)
                {
                    rightMouseDown = true;
                    //Debug.Log("true");
                }
                else if (eventCurrent.type == EventType.MouseUp)
                {
                    rightMouseDown = false;
                    //Debug.Log("false");
                }
            }

            if (rightMouseDown)
            {
                if (eventCurrent.keyCode == KeyCode.W)
                {
                    if (eventCurrent.type == EventType.KeyDown)
                    {
                        w = true;
                        eventCurrent.Use();
                    }
                    else if (eventCurrent.type == EventType.KeyUp)
                    {
                        w = false;
                        eventCurrent.Use();
                    }
                }
                if (eventCurrent.keyCode == KeyCode.S)
                {
                    if (eventCurrent.type == EventType.KeyDown)
                    {
                        s = true;
                        eventCurrent.Use();
                    }
                    else if (eventCurrent.type == EventType.KeyUp)
                    {
                        s = false;
                        eventCurrent.Use();
                    }
                }
                if (eventCurrent.keyCode == KeyCode.A)
                {
                    if (eventCurrent.type == EventType.KeyDown)
                    {
                        a = true;
                        eventCurrent.Use();
                    }
                    else if (eventCurrent.type == EventType.KeyUp)
                    {
                        a = false;
                        eventCurrent.Use();
                    }
                }
                if (eventCurrent.keyCode == KeyCode.D)
                {
                    if (eventCurrent.type == EventType.KeyDown)
                    {
                        d = true;
                        eventCurrent.Use();
                    }
                    else if (eventCurrent.type == EventType.KeyUp)
                    {
                        d = false;
                        eventCurrent.Use();
                    }
                }
                if (eventCurrent.keyCode == KeyCode.Q)
                {
                    if (eventCurrent.type == EventType.KeyDown)
                    {
                        q = true;
                        eventCurrent.Use();
                    }
                    else if (eventCurrent.type == EventType.KeyUp)
                    {
                        q = false;
                        eventCurrent.Use();
                    }
                }
                if (eventCurrent.keyCode == KeyCode.E)
                {
                    if (eventCurrent.type == EventType.KeyDown)
                    {
                        e = true;
                        eventCurrent.Use();
                    }
                    else if (eventCurrent.type == EventType.KeyUp)
                    {
                        e = false;
                        eventCurrent.Use();
                    }
                }


                if (eventCurrent.type == EventType.ScrollWheel)
                {
                    targetSpeed = Mathf.Clamp(targetSpeed * (1f-eventCurrent.delta.y*0.05f), 0f, maxSpeed);
                    //Debug.Log("targetSpeed " + targetSpeed);
                    eventCurrent.Use();
                }

                if (w || a || s || d || q || e)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accelerationRate * deltaTime);

                    moveDirection = Vector3.zero;

                    if (w)
                    {
                        moveDirection += Vector3.forward;
                    }
                    if (s)
                    {
                        moveDirection -= Vector3.forward;
                    }
                    if (a)
                    {
                        moveDirection -= Vector3.right;
                    }
                    if (d)
                    {
                        moveDirection += Vector3.right;
                    }
                    if (q)
                    {
                        moveDirection += -Vector3.up;
                    }
                    if (e)
                    {
                        moveDirection += Vector3.up;
                    }
                }
                else
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, 0f, decelerationRate * deltaTime);
                }

                pivotLocalPos += moveDirection * deltaTime * currentSpeed;

                view.pivot = view.camera.transform.TransformPoint(pivotLocalPos);
            }
            else
            {
                currentSpeed = 0f;
            }
        };
    }
}

