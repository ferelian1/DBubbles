using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the overall game loop:
/// menu -> playing -> victory/game over.
/// Also manages active bubbles and the player's breath.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        Playing,
        Victory,
        GameOver
    }

    public enum InputMode
    {
        /// <summary>
        /// Click/tap the screen and affect the bubble closest to the cursor.
        /// The wind always blows upward from below the bubble.
        /// Hold to continuously blow while consuming breath.
        /// </summary>
        NearestMouse,

        /// <summary>
        /// Flappy-Bird-like control:
        /// every active bubble gets the same impulse.
        /// </summary>
        GlobalTap
    }

    [Header("Game State")]
    [SerializeField] private GameState startingState = GameState.MainMenu;

    [Header("Bubble Prefabs")]
    [SerializeField] private Bubble largeBubblePrefab;
    [SerializeField] private Bubble mediumBubblePrefab;
    [SerializeField] private Bubble smallBubblePrefab;

    [Header("Spawn")]
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Breath")]
    [SerializeField] private float maxBreath = 100f;

    [Tooltip("Breath consumed per second while holding the blow input.")]
    [SerializeField] private float breathDrainPerSecond = 35f;

    [Tooltip("How quickly breath is restored after releasing the input.")]
    [SerializeField] private float breathRecoveryPerSecond = 80f;

    [Header("Input")]
    [SerializeField] private InputMode inputMode = InputMode.NearestMouse;

    [Header("Nearest Mouse - Puff")]
    [Tooltip("Upward impulse produced by a quick tap.")]
    [SerializeField] private float tapImpulse = 5f;
    [Tooltip("Breath consumed by one tap.")]
    [SerializeField] private float tapBreathCost = 5f;

    [Header("Nearest Mouse - Hold Blow")]
    [Tooltip("Continuous upward force applied while the button is held.")]
    [SerializeField] private float holdBlowForce = 2.5f;

    [Tooltip("Minimum vertical direction used by the blow. Currently fixed upward.")]
    [SerializeField] private Vector2 blowDirection = Vector2.up;

    [Header("Global Tap")]
    [Tooltip("Used by GlobalTap mode.")]
    [SerializeField] private Vector2 globalTapDirection = new Vector2(1f, 1f);

    [SerializeField] private float globalTapImpulse = 2f;

    private readonly HashSet<Bubble> activeBubbles = new();

    private GameUI gameUI;

    private float currentBreath;
    private GameState currentState;

    public GameState CurrentState => currentState;
    public float CurrentBreath => currentBreath;
    public float MaxBreath => maxBreath;
    public int BubbleCount => activeBubbles.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        gameUI = FindObjectOfType<GameUI>();
    }

    private void Start()
    {
        currentState = startingState;

        if (currentState == GameState.MainMenu)
        {
            gameUI.ShowMainMenu();
        }
        else
        {
            StartGame();
        }
    }

    private void Update()
    {
        if (currentState != GameState.Playing)
            return;

        UpdateBreath();
        HandleBlowVFX();
        HandleInput();

        // Lose condition.
        if (BubbleCount <= 0)
        {
            GameOver();
        }
    }

    #region Game Flow

    public void StartGame()
    {
        ResetLevel();

        currentState = GameState.Playing;
        currentBreath = maxBreath;

        SpawnInitialBubble();

        gameUI.ShowHUD();
        gameUI.UpdateBreath(currentBreath, maxBreath);
        gameUI.UpdateBubbleCount(BubbleCount);
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void ReturnToMenu()
    {
        ResetLevel();

        currentState = GameState.MainMenu;
        gameUI.ShowMainMenu();
    }

    public void Victory()
    {
        if (currentState != GameState.Playing)
            return;

        currentState = GameState.Victory;

        // Stop all bubbles.
        foreach (Bubble bubble in activeBubbles)
        {
            if (bubble != null)
            {
                bubble.DisablePhysics();
            }
        }

        gameUI.ShowVictory();
    }

    public void GameOver()
    {
        if (currentState != GameState.Playing)
            return;

        currentState = GameState.GameOver;

        gameUI.ShowGameOver();
    }

    #endregion

    #region Breath

    private void UpdateBreath()
    {
        // Breath is consumed only while actively holding the blow input.
        if (IsBlowing())
        {
            currentBreath -= breathDrainPerSecond * Time.deltaTime;
        }
        else
        {
            // Recover quickly once the player stops blowing.
            currentBreath += breathRecoveryPerSecond * Time.deltaTime;
        }

        currentBreath = Mathf.Clamp(currentBreath, 0f, maxBreath);

        if (gameUI != null)
        {
            gameUI.UpdateBreath(currentBreath, maxBreath);
        }
    }

    private bool IsBlowing()
    {
        return inputMode == InputMode.NearestMouse &&
               Input.GetMouseButton(0) &&
               currentBreath > 0f;
    }

    #endregion

    #region Input

    private void HandleInput()
    {
        switch (inputMode)
        {
            case InputMode.NearestMouse:
                HandleNearestMouseInput();
                break;

            case InputMode.GlobalTap:
                if (Input.GetMouseButtonDown(0))
                {
                    ApplyGlobalTap();
                }
                break;
        }
    }

    private void HandleNearestMouseInput()
    {
        // A quick tap gives one strong upward puff.
        if (Input.GetMouseButtonDown(0))
        {
            ApplyMousePuff();
            return;
        }

        // Holding continuously blows upward, but becomes weaker in effect
        // than the initial tap and is limited by the breath resource.
        if (Input.GetMouseButton(0) && currentBreath > 0f)
        {
            ApplyMouseBlow();
        }
    }


    private void HandleBlowVFX()
    {
        if (inputMode != InputMode.NearestMouse)
            return;

        if (GameVFX.Instance == null)
            return;

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        mouseWorldPosition.z = 0f;

        if (Input.GetMouseButtonDown(0))
        {
            GameVFX.Instance.StartBlow(mouseWorldPosition);
        }

        if (Input.GetMouseButton(0))
        {
            GameVFX.Instance.UpdateBlowPosition(mouseWorldPosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            GameVFX.Instance.StopBlow();
        }
    }
    /// <summary>
    /// Finds the bubble closest to the mouse cursor and gives it one
    /// relatively strong upward puff.
    /// </summary>
    private void ApplyMousePuff()
    {
        if (currentBreath < tapBreathCost)
            return;

        Bubble nearestBubble = GetNearestBubbleToMouse();

        if (nearestBubble == null)
            return;

        currentBreath -= tapBreathCost;

        nearestBubble.ApplyImpulse(
            blowDirection.normalized * tapImpulse);
    }

    /// <summary>
    /// Finds the bubble closest to the mouse cursor and applies a continuous
    /// upward blow force. The direction intentionally ignores mouse position.
    /// </summary>
    private void ApplyMouseBlow()
    {
        Bubble nearestBubble = GetNearestBubbleToMouse();

        if (nearestBubble == null)
            return;

        nearestBubble.ApplyImpulse(
            blowDirection.normalized * holdBlowForce);
    }

    private Bubble GetNearestBubbleToMouse()
    {
        if (activeBubbles.Count == 0)
            return null;

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return null;

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        mouseWorldPosition.z = 0f;

        Bubble nearestBubble = null;
        float nearestDistance = float.MaxValue;

        foreach (Bubble bubble in activeBubbles)
        {
            if (bubble == null)
                continue;

            float distance = Vector2.Distance(
                bubble.transform.position,
                mouseWorldPosition);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestBubble = bubble;
            }
        }

        return nearestBubble;
    }

    /// <summary>
    /// Flappy-Bird-like alternative control.
    /// Every active bubble gets the same impulse.
    /// </summary>
    private void ApplyGlobalTap()
    {
        Vector2 direction = globalTapDirection.normalized;

        foreach (Bubble bubble in activeBubbles)
        {
            if (bubble == null)
                continue;

            bubble.ApplyImpulse(direction * globalTapImpulse);
        }
    }

    #endregion

    #region Bubble Management

    private void SpawnInitialBubble()
    {
        if (largeBubblePrefab == null)
        {
            Debug.LogError("Large Bubble Prefab is not assigned.");
            return;
        }

        if (playerSpawnPoint == null)
        {
            Debug.LogError("Player Spawn Point is not assigned.");
            return;
        }

        SpawnBubble(
            largeBubblePrefab,
            playerSpawnPoint.position,
            Vector2.zero
        );
    }

    /// <summary>
    /// Registers a bubble as active.
    /// </summary>
    public void RegisterBubble(Bubble bubble)
    {
        if (bubble == null)
            return;

        activeBubbles.Add(bubble);

        if (gameUI != null)
        {
            gameUI.UpdateBubbleCount(BubbleCount);
        }
    }

    /// <summary>
    /// Removes a bubble from the active registry.
    /// Does NOT automatically trigger GameOver.
    /// This is important during splitting because the old bubble
    /// temporarily disappears before its children are created.
    /// </summary>
    public void UnregisterBubble(Bubble bubble)
    {
        if (bubble == null)
            return;

        activeBubbles.Remove(bubble);

        if (gameUI != null)
        {
            gameUI.UpdateBubbleCount(BubbleCount);
        }
    }

    private Bubble SpawnBubble(
        Bubble prefab,
        Vector2 position,
        Vector2 inheritedVelocity)
    {
        Bubble newBubble = Instantiate(
            prefab,
            position,
            Quaternion.identity
        );

        newBubble.SetInitialVelocity(inheritedVelocity);

        return newBubble;
    }

    /// <summary>
    /// Large -> 2 Medium
    /// Medium -> 2 Small
    /// Small -> Pop
    /// </summary>
    public void HandleBubbleHazardHit(Bubble bubble)
    {
        if (bubble == null || currentState != GameState.Playing)
            return;

        Bubble.BubbleSize size = bubble.Size;

        Vector2 position = bubble.transform.position;
        Vector2 inheritedVelocity = bubble.Velocity;

        // Remove old bubble first.
        UnregisterBubble(bubble);
        Destroy(bubble.gameObject);

        // Small bubbles simply pop.
        if (size == Bubble.BubbleSize.Small)
        {
            CheckLoseCondition();
            return;
        }

        Bubble nextPrefab = null;

        if (size == Bubble.BubbleSize.Large)
        {
            nextPrefab = mediumBubblePrefab;
        }
        else if (size == Bubble.BubbleSize.Medium)
        {
            nextPrefab = smallBubblePrefab;
        }

        if (nextPrefab == null)
        {
            Debug.LogError("Required split prefab is not assigned.");
            CheckLoseCondition();
            return;
        }

        SpawnSplitPair(
            nextPrefab,
            position,
            inheritedVelocity
        );

        CheckLoseCondition();
    }

    /// <summary>
    /// Creates exactly two children with a small separation and
    /// a slight random impulse so that they do not overlap perfectly.
    /// </summary>
    private void SpawnSplitPair(
        Bubble prefab,
        Vector2 center,
        Vector2 inheritedVelocity)
    {
        const float spawnOffset = 0.2f;
        const float splitImpulse = 0.5f;

        Vector2 separation =
            Random.insideUnitCircle.normalized;

        if (separation.sqrMagnitude < 0.001f)
        {
            separation = Vector2.right;
        }

        Vector2 positionA =
            center + separation * spawnOffset;

        Vector2 positionB =
            center - separation * spawnOffset;

        Bubble bubbleA =
            SpawnBubble(prefab, positionA, inheritedVelocity);

        Bubble bubbleB =
            SpawnBubble(prefab, positionB, inheritedVelocity);

        bubbleA.ApplyImpulse(separation * splitImpulse);
        bubbleB.ApplyImpulse(-separation * splitImpulse);
    }

    /// <summary>
    /// Called by a bubble when it leaves the playable area.
    /// </summary>
    public void HandleBubbleOutOfBounds(Bubble bubble)
    {
        if (bubble == null)
            return;

        if (currentState != GameState.Playing)
            return;

        UnregisterBubble(bubble);
        Destroy(bubble.gameObject);

        CheckLoseCondition();
    }

    /// <summary>
    /// Returns the active bubble that is furthest to the right.
    /// Used by the Cinemachine camera as its follow target.
    /// </summary>
    public Bubble GetLeadingBubble()
    {
        Bubble leadingBubble = null;
        float highestX = float.MinValue;

        foreach (Bubble bubble in activeBubbles)
        {
            if (bubble == null)
                continue;

            float bubbleX = bubble.transform.position.x;

            if (bubbleX > highestX)
            {
                highestX = bubbleX;
                leadingBubble = bubble;
            }
        }

        return leadingBubble;
    }

    private void CheckLoseCondition()
    {
        if (currentState != GameState.Playing)
            return;

        if (BubbleCount <= 0)
        {
            GameOver();
        }
    }

    private void ResetLevel()
    {
        Bubble[] bubbles =
            FindObjectsOfType<Bubble>();

        foreach (Bubble bubble in bubbles)
        {
            if (bubble != null)
            {
                Destroy(bubble.gameObject);
            }
        }

        activeBubbles.Clear();
    }

    #endregion
}