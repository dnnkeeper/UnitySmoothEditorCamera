
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
        SceneViewSmoothCamera.Init();
        window.maxSpeed = SceneViewSmoothCamera.MaxSpeed;
        window.accelerationRate = SceneViewSmoothCamera.AccelerationRate;
        window.decelerationRate = SceneViewSmoothCamera.DecelerationRate;
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        SceneViewSmoothCamera.enabled = EditorGUILayout.Toggle("Enabled", SceneViewSmoothCamera.enabled);

        EditorPrefs.SetBool("smoothCamera", SceneViewSmoothCamera.enabled);

        maxSpeed = EditorGUILayout.FloatField("MaxSpeed", maxSpeed);
        accelerationRate = EditorGUILayout.FloatField("accelerationRate", accelerationRate);
        decelerationRate = EditorGUILayout.FloatField("decelerationRate", decelerationRate);
        SceneViewSmoothCamera.TargetSpeed = EditorGUILayout.Slider("targetSpeed", SceneViewSmoothCamera.TargetSpeed, 0f, maxSpeed);

        SceneViewSmoothCamera.MaxSpeed = maxSpeed;
        SceneViewSmoothCamera.AccelerationRate = accelerationRate;
        SceneViewSmoothCamera.DecelerationRate = decelerationRate;
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

    public static bool enabled = true;

    private static float maxSpeed = 100f;

    private static float accelerationRate = 10f;

    private static float decelerationRate = 10f;

    private static float targetSpeed = 1f;

    public static float currentSpeed;

    static TimeHelper s_TimeHelper = new TimeHelper();

    public static Vector3 pivotLocalPos, moveDirection;

    static bool inited, rightMouseDown;

    static bool w, s, a, d, q, e;

    public static float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }

        set
        {
            maxSpeed = value;
            EditorPrefs.SetFloat("maxCameraSpeed", maxSpeed);
        }
    }

    public static float AccelerationRate
    {
        get
        {
            return accelerationRate;
        }

        set
        {
            accelerationRate = value;
            EditorPrefs.SetFloat("accelerationRate", accelerationRate);
        }
    }

    public static float DecelerationRate
    {
        get
        {
            return decelerationRate;
        }

        set
        {
            decelerationRate = value;
            EditorPrefs.SetFloat("decelerationRate", decelerationRate);
        }
    }

    public static float TargetSpeed
    {
        get
        {
            return targetSpeed;
        }

        set
        {
            targetSpeed = value;
            EditorPrefs.SetFloat("targetCameraSpeed", targetSpeed);
        }
    }

    static float speedMod = 1f;

    public static void Init()
    {
        Debug.Log("Smooth camera init");

        inited = true;

        if (EditorPrefs.HasKey("smoothCamera"))
            enabled = EditorPrefs.GetBool("smoothCamera");
        if (EditorPrefs.HasKey("maxCameraSpeed"))
            MaxSpeed = EditorPrefs.GetFloat("maxCameraSpeed");
        if (EditorPrefs.HasKey("targetCameraSpeed"))
            TargetSpeed = EditorPrefs.GetFloat("targetCameraSpeed");
        if (EditorPrefs.HasKey("accelerationRate"))
            AccelerationRate = EditorPrefs.GetFloat("accelerationRate");
        if (EditorPrefs.HasKey("decelerationRate"))
            DecelerationRate = EditorPrefs.GetFloat("decelerationRate");
    }

    static SceneViewSmoothCamera()
    {
        s_TimeHelper.Begin();

        Debug.Log("SceneViewSmoothCamera created");
        
        SceneView.onSceneGUIDelegate += view =>
        {
            if (!enabled)
                return;

            if ( EditorWindow.focusedWindow != view || EditorWindow.mouseOverWindow == null)
            {
                Reset();
                return;
            }

            if (!inited) {
                Init();
            }
            pivotLocalPos = view.camera.transform.InverseTransformPoint(view.pivot);
            s_TimeHelper.Update();

            float deltaTime = s_TimeHelper.deltaTime;

            var eventCurrent = Event.current;

            if (Event.current.button == 1)
            {
                if (eventCurrent.type == EventType.MouseDown)
                {
                    Reset();

                    rightMouseDown = true;
                }
                else if (eventCurrent.type == EventType.MouseUp)
                {
                    rightMouseDown = false;
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

                if (eventCurrent.shift)
                {
                    speedMod = 5f;
                }
                else
                    speedMod = 1f;

                if (eventCurrent.type == EventType.ScrollWheel)
                {
                    TargetSpeed = Mathf.Clamp(TargetSpeed * (1f-eventCurrent.delta.y*0.05f), 0f, MaxSpeed);

                    //Debug.Log("targetSpeed " + targetSpeed);

                    eventCurrent.Use();
                }

                if (w || a || s || d || q || e)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, TargetSpeed * speedMod, AccelerationRate * deltaTime);

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
                    currentSpeed = Mathf.Lerp(currentSpeed, 0f, DecelerationRate * deltaTime);
                }

                pivotLocalPos += moveDirection * deltaTime * currentSpeed;

                view.pivot = view.camera.transform.TransformPoint(pivotLocalPos);
            }
            else
            {
                moveDirection = Vector3.zero;

                currentSpeed = 0f;
            }
        };
    }

    static void Reset()
    {
        rightMouseDown = false;

        moveDirection = Vector3.zero;

        w = s = a = d = q = e = false;

        speedMod = 1f;
    }
}

