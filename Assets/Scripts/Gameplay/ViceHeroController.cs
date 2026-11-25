using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class ViceHeroController : MonoBehaviour
{
    [Header("Movement & Abilities")]
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float grappleBoostMultiplier = 2.1f;
    [SerializeField] private float jumpHeight = 1.8f;
    [SerializeField] private float gravity = -20f;

    [Header("HUD References")]
    [SerializeField] private Image ropeFillBar;
    [SerializeField] private TMP_Text ropePercentText;
    [SerializeField] private TMP_Text missionScoreText;
    [SerializeField] private GameObject laserBeamVisual;

    [Header("Mission Tracking")]
    [SerializeField] private int totalCoinsInArena = 4;

    private CharacterController controller;
    private Vector3 velocity;
    private float ropeEnergy = 1f;
    private float boostTimer;
    private float elapsedMissionTime;
    private int patrolScore;
    private int coinsCollected;
    private bool missionEnded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        MusicPlayer.Instance?.SetMusicState(MusicPlayer.MusicState.Patrol);
        UpdateHudBars();
    }

    private void Update()
    {
        if (UIManager.IsPaused || missionEnded) return;

        elapsedMissionTime += Time.deltaTime;
        HandleMovementAndJump();
        HandleAbilities();
        UpdateHudBars();
    }

    private void HandleMovementAndJump()
    {
        var kb = Keyboard.current;
        Vector2 input = Vector2.zero;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1f;
        }

        Vector3 move = new Vector3(input.x, 0f, input.y).normalized;
        float speed = moveSpeed * (boostTimer > 0f ? grappleBoostMultiplier : 1f);
        if (boostTimer > 0f) boostTimer -= Time.deltaTime;

        controller.Move(move * speed * Time.deltaTime);
        if (move.sqrMagnitude > 0.01f)
            transform.forward = Vector3.Slerp(transform.forward, move, 14f * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        if (kb != null && kb.spaceKey.wasPressedThisFrame && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            SfxPlayer.Instance?.Play(SfxPlayer.Instance.grappleBoostClip, 0.75f);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        ropeEnergy = Mathf.Clamp01(ropeEnergy + 0.18f * Time.deltaTime);
    }

    private void HandleAbilities()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if ((kb.leftShiftKey.wasPressedThisFrame || kb.eKey.wasPressedThisFrame) && ropeEnergy >= 0.25f)
            TriggerGrappleBoost();

        if (kb.fKey.wasPressedThisFrame)
            TriggerLaserAttack();
    }

    public void TriggerGrappleBoost()
    {
        if (UIManager.IsPaused || missionEnded || ropeEnergy < 0.25f) return;
        ropeEnergy = Mathf.Max(0f, ropeEnergy - 0.30f);
        boostTimer = 0.55f;
        SfxPlayer.Instance?.Play(SfxPlayer.Instance.grappleBoostClip, 1f);
        UpdateHudBars();
    }

    public void TriggerLaserAttack()
    {
        if (UIManager.IsPaused || missionEnded) return;
        SfxPlayer.Instance?.Play(SfxPlayer.Instance.laserAttackClip, 0.95f);
        if (laserBeamVisual != null)
            StartCoroutine(FlashLaserRoutine());
    }

    private IEnumerator FlashLaserRoutine()
    {
        laserBeamVisual.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        laserBeamVisual.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (missionEnded) return;

        if (other.CompareTag("Pickup"))
        {
            coinsCollected++;
            patrolScore += 100;
            if (ViceGameData.Instance != null)
                ViceGameData.Instance.AddRewards(100);

            SfxPlayer.Instance?.Play(SfxPlayer.Instance.pickupClip, 1f);
            UpdateHudBars();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Goal"))
        {
            CompleteMissionWin();
        }
    }

    public void CompleteMissionWin()
    {
        if (missionEnded) return;
        missionEnded = true;
        SfxPlayer.Instance?.Play(SfxPlayer.Instance.winClip, 1f, varyPitch: false);
        MusicPlayer.Instance?.SetMusicState(MusicPlayer.MusicState.Results);
        UIManager.Instance?.NavigateTo("Results");
    }

    public void CompleteMissionLose()
    {
        if (missionEnded) return;
        missionEnded = true;
        SfxPlayer.Instance?.Play(SfxPlayer.Instance.loseClip, 1f, varyPitch: false);
        MusicPlayer.Instance?.SetMusicState(MusicPlayer.MusicState.Results);
        UIManager.Instance?.NavigateTo("Results");
    }

    public void ReturnToMainMenuScene()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        MusicPlayer.Instance?.SetMusicState(MusicPlayer.MusicState.Menu);
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartGameplayScene()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        MusicPlayer.Instance?.SetMusicState(MusicPlayer.MusicState.Patrol);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateHudBars()
    {
        if (ropeFillBar != null)
            ropeFillBar.fillAmount = ropeEnergy;

        if (ropePercentText != null)
            ropePercentText.text = $"ROPE CHARGE: {Mathf.RoundToInt(ropeEnergy * 100f)}%";

        if (missionScoreText != null)
        {
            int mins = Mathf.FloorToInt(elapsedMissionTime / 60f);
            int secs = Mathf.FloorToInt(elapsedMissionTime % 60f);
            missionScoreText.text = $"TIME: {mins:00}:{secs:00}  -  SCORE: {patrolScore:N0}  |  COINS: {coinsCollected}/{totalCoinsInArena}";
        }
    }
}