using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SingleInputTutorialCondition", menuName = "Scriptable Objects/SingleInputTutorialCondition")]
public class SingleInputTutorialCondition : TutorialCondition
{
    [SerializeField] private InputActionReference input;

    private bool completed;

    public override void StartListening()
    {
        completed = false;
        input.action.performed += OnPerformed;
        input.action.Enable();
    }

    private void OnPerformed(InputAction.CallbackContext context)
    {
        completed = true;
    }

    public override bool IsComplete()
    {
        return completed;
    }

    public override void StopListening()
    {
        input.action.performed -= OnPerformed;
    }
}

