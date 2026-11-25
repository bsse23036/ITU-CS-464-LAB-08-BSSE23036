using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public static bool IsPaused { get; private set; }

    [Header("All Registered Screens")]
    [SerializeField] private List<UIScreen> screens = new List<UIScreen>();

    [Header("Initial Entry Screen ID")]
    [SerializeField] private string startingScreenId = "Title";

    private readonly Dictionary<string, UIScreen> screenLookup = new Dictionary<string, UIScreen>();
    private readonly Stack<string> navigationHistory = new Stack<string>();
    private string currentScreenId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        screenLookup.Clear();
        foreach (var screen in screens)
        {
            if (screen != null && !string.IsNullOrEmpty(screen.screenId))
            {
                screenLookup[screen.screenId] = screen;
                screen.HideImmediate();
            }
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        AudioListener.pause = false;

        if (!string.IsNullOrEmpty(startingScreenId) && screenLookup.ContainsKey(startingScreenId))
        {
            currentScreenId = startingScreenId;
            screenLookup[startingScreenId].ShowImmediate();
            SelectFirstButton(screenLookup[startingScreenId]);
        }
    }

    private void Update()
    {
        // New Input System: Escape key / Android Back button check
        bool escapePressed = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

        if (escapePressed)
        {
            if (currentScreenId == "Gameplay")
            {
                NavigateTo("Pause");
            }
            else if (currentScreenId != "Title" && currentScreenId != "MainMenu")
            {
                GoBack();
            }
        }
    }

    /// <summary>
    /// Navigates forward to a target screen and pushes the current screen onto the back stack.
    /// Wire directly to Button OnClick(string) in the Inspector.
    /// </summary>
    public void NavigateTo(string targetScreenId)
    {
        if (!screenLookup.ContainsKey(targetScreenId) || targetScreenId == currentScreenId)
            return;

        if (!string.IsNullOrEmpty(currentScreenId))
        {
            navigationHistory.Push(currentScreenId);
            screenLookup[currentScreenId].AnimateOut(isForward: true);
        }

        SwitchToScreen(targetScreenId, isForward: true);
    }

    /// <summary>
    /// Navigates backward to the previous screen in history.
    /// </summary>
    public void GoBack()
    {
        if (navigationHistory.Count == 0)
        {
            if (currentScreenId != "MainMenu" && screenLookup.ContainsKey("MainMenu"))
            {
                NavigateRoot("MainMenu");
            }
            return;
        }

        string previousId = navigationHistory.Pop();
        if (!string.IsNullOrEmpty(currentScreenId))
        {
            screenLookup[currentScreenId].AnimateOut(isForward: false);
        }

        SwitchToScreen(previousId, isForward: false);
    }

    /// <summary>
    /// Clears history and jumps to a root hub (e.g., returning to MainMenu from Pause or Results).
    /// </summary>
    public void NavigateRoot(string rootScreenId)
    {
        if (!screenLookup.ContainsKey(rootScreenId)) return;

        navigationHistory.Clear();
        if (!string.IsNullOrEmpty(currentScreenId) && currentScreenId != rootScreenId)
        {
            screenLookup[currentScreenId].AnimateOut(isForward: false);
        }

        SwitchToScreen(rootScreenId, isForward: false);
    }

    private void SwitchToScreen(string targetId, bool isForward)
    {
        string previousId = currentScreenId;
        currentScreenId = targetId;

        UIScreen targetScreen = screenLookup[targetId];
        targetScreen.AnimateIn(isForward);

        // Trigger dedicated panel / pause / resume UI audio
        if (targetId == "Pause")
        {
            UIAudioPlayer.Instance?.PlayPause();
        }
        else if (previousId == "Pause" && targetId == "Gameplay")
        {
            UIAudioPlayer.Instance?.PlayResume();
        }
        else if (targetId == "Results" || previousId == "Results")
        {
            UIAudioPlayer.Instance?.PlayPanelToggle();
        }

        // Manage pause clock & audio when entering/exiting Pause or Results
        bool shouldPause = (targetId == "Pause" || targetId == "Results");
        IsPaused = shouldPause;
        Time.timeScale = shouldPause ? 0f : 1f;
        AudioListener.pause = shouldPause;

        SelectFirstButton(targetScreen);
    }

    private void SelectFirstButton(UIScreen screen)
    {
        if (EventSystem.current == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        if (screen.firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(screen.firstSelectedButton);
        }
    }
}