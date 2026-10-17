using UnityEngine;
using UnityEditor;

public class PrefabStamp : EditorWindow
{
    private GameObject prefab;
    private float spacing = 1f;
    private float placementZ = 0f;
    private float minRotation = 0f;
    private float maxRotation = 0f;
    private bool flipXScale = false;

    private bool isPainting;
    private Vector2 lastMousePosition;
    private Vector3 lastPlacement;

    [MenuItem("Tools/Prefab Stamp")]
    private static void OpenWindow()
    {
        GetWindow<PrefabStamp>("Prefab Stamp");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        isPainting = false;
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Stamp", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "Assign a prefab, then click or drag in the Scene view to paint.",
            MessageType.Info
        );

        prefab = (GameObject)EditorGUILayout.ObjectField(
            "Decoration Prefab",
            prefab,
            typeof(GameObject),
            false
        );

        spacing = EditorGUILayout.Slider(
            "Brush Spacing",
            spacing,
            0.1f,
            10f
        );

        placementZ = EditorGUILayout.FloatField(
            "Placement Z",
            placementZ
        );

        minRotation = EditorGUILayout.FloatField(
            "Min Rotation",
            minRotation);

        maxRotation = EditorGUILayout.FloatField(
            "Max Rotation",
            maxRotation);

        flipXScale = EditorGUILayout.Toggle(
            "Allow flipping the X Scale",
            flipXScale);

        if (GUILayout.Button("Stop Painting"))
        {
            isPainting = false;
        }

        EditorGUILayout.HelpBox(
            "Paint on the Z plane specified above. " +
            "Hold Alt to navigate the Scene view normally.",
            MessageType.None
        );
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (prefab == null)
        {
            isPainting = false;
            return;
        }

        Event e = Event.current;

        if (e.alt)
        {
            isPainting = false;
            return;
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane plane = new Plane(
            Vector3.forward,
            new Vector3(0f, 0f, placementZ)
        );

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 worldPosition = ray.GetPoint(distance);
        worldPosition.z = placementZ;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            isPainting = true;
            lastMousePosition = e.mousePosition;
            PlacePrefab(worldPosition);
            e.Use();
        }
        else if (e.type == EventType.MouseDrag &&
                 e.button == 0 && isPainting)
        {
            if (Vector2.Distance(e.mousePosition, lastMousePosition) > 0f)
            {
                if (Vector3.Distance(worldPosition, lastPlacement) >= spacing)
                {
                    PlacePrefab(worldPosition);
                }

                lastMousePosition = e.mousePosition;
            }

            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            isPainting = false;
        }

        sceneView.Repaint();
    }

    private void PlacePrefab(Vector3 position)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(
            prefab,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
        );

        if (instance == null)
            return;

        Undo.RegisterCreatedObjectUndo(instance, "Paint Decoration");

        instance.transform.SetParent(Selection.activeGameObject.transform, true);

        instance.transform.position = position;

        instance.transform.rotation = Quaternion.Euler(0, 0, Random.Range(minRotation, maxRotation));

        if (flipXScale)
        {
            instance.transform.localScale = new Vector3(Random.value > 0.5f ? 1f : -1f, 1f, 1f);
        }

        lastPlacement = position;
    }
}