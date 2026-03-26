using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInputHandler : MonoBehaviour
{
    public TestAction Input { get; private set; }

    [Tooltip("일시정지 팝업")]
    [SerializeField] private GameObject _pausePopup;

    private bool _isPausePopupOpened => _pausePopup.activeSelf;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (Input == null)
        {
            Input = new TestAction();
        }
    }

    private void OnEnable()
    {
        Input.Enable();

        Input.UI.Esc.performed += OnEsc;
    }

    private void OnDisable()
    {
        Input.UI.Esc.performed -= OnEsc;

        Input.Disable();
    }

    public void OnEsc(InputAction.CallbackContext context)
    {
        TogglePausePopup();
    }

    public void TogglePausePopup()
    {
        _pausePopup.transform.SetAsLastSibling();
        _pausePopup.SetActive(!_isPausePopupOpened);
        TimeScaleController.Instance.SetPausePopup(!_isPausePopupOpened);
    }
}
