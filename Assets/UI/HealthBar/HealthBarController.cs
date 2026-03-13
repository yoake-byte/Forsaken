using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Attach this to any GameObject (does NOT need to live on the same object
/// as the UIDocument or the IDamageable target).
///
/// Setup options
///   Inspector : drag any MonoBehaviour that implements IDamageable into
///               'Damage Target', set Max Health, done.
///   Runtime   : call  Bind(IDamageable target, int maxHealth)  at any point.
///
/// The controller polls the bound target's Health every frame and drives the
/// bar automatically — you never need to push values into it manually.
///
/// Visual behaviour
///   BarFill   (red)  → follows Health immediately on damage.
///   BehindFill(gold) → lingers at the previous value, then slowly drains.
/// </summary>
public class HealthBarController : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────────────────

    [Header("UI Reference")]
    [Tooltip("UIDocument that contains HealthBar.uxml")]
    public UIDocument uiDocument;

    [Header("Target (must implement IDamageable)")]
    [Tooltip("Drag any MonoBehaviour that implements IDamageable here.")]
    [SerializeField] private MonoBehaviour damageableComponent;
    private IDamageable damageableTarget => damageableComponent as IDamageable;

    [Header("Health Settings")]
    [Tooltip("Maximum possible health — used to calculate bar fill %.")]
    [SerializeField] private int maxHealth = 100;

    [Header("Behind-Bar Settings")]
    [Tooltip("Seconds to wait after damage before the gold bar starts draining.")]
    public float damageLingerTime = 1.0f;

    [Tooltip("Health units per second at which the gold bar drains toward the red bar.")]
    public float drainSpeed = 40f;

    // ── UI elements ────────────────────────────────────────────────────────

    private VisualElement _barMask;
    private VisualElement _behindMask;
    private Label         _label;
    private float         _barFullWidth;  // resolved pixel width = 100 % health

    // ── Binding & animation state ──────────────────────────────────────────

    private IDamageable _bound;           // the live target being tracked
    private int         _lastHealth;      // last observed Health value
    private float       _behindHealth;    // current position of the gold bar
    private Coroutine   _drainCoroutine;

    // ──────────────────────────────────────────────────────────────────────
    #region Unity Messages

    private void OnEnable()
    {
        var root    = uiDocument.rootVisualElement;
        var barFill = root.Q<VisualElement>("BarFill");

        _barMask    = root.Q<VisualElement>("BarMask");
        _behindMask = root.Q<VisualElement>("BehindMask");
        _label      = root.Q<Label>("HealthLabel");
        // Defer the first paint until Unity's layout engine resolves element sizes.
        barFill.RegisterCallback<GeometryChangedEvent>(OnLayoutReady);

        // If a target was assigned in the Inspector, bind it straight away.
        if (damageableTarget != null)
            Bind(damageableTarget as IDamageable, maxHealth);
    }

    private void OnDisable()
    {
        _bound = null;

        if (_drainCoroutine != null)
        {
            StopCoroutine(_drainCoroutine);
            _drainCoroutine = null;
        }
    }

    private void Update()
    {
        // Bail out if nothing is bound or the layout isn't ready yet.
        if (_bound == null || _barFullWidth <= 0f) return;

        int health = _bound.Health;

        if (health == _lastHealth) return;   // nothing changed this frame

        if (health < _lastHealth)
            HandleDamage(health);
        else
            HandleHeal(health);

        _lastHealth = health;
    }

    #endregion

    // ──────────────────────────────────────────────────────────────────────
    #region Public API

    /// <summary>
    /// Bind the health bar to any IDamageable at runtime.
    /// Safe to call multiple times; rebinds to the new target immediately.
    /// </summary>
    /// <param name="target">Object implementing IDamageable whose Health to track.</param>
    /// <param name="max">The maximum health value (= full bar).</param>
    public void Bind(IDamageable target, int max)
    {
        if (target == null)
        {
            Debug.LogWarning("[HealthBarController] Bind() called with a null target.");
            return;
        }

        if (_drainCoroutine != null)
        {
            StopCoroutine(_drainCoroutine);
            _drainCoroutine = null;
        }

        _bound        = target;
        maxHealth     = max;
        _lastHealth   = target.Health;
        _behindHealth = target.Health;

        // Snap to current state (no-op if layout width isn't resolved yet;
        // OnLayoutReady will handle the initial paint in that case).
        ApplyWidths(_lastHealth, _behindHealth);
        UpdateLabel(_lastHealth);
    }

    #endregion

    // ──────────────────────────────────────────────────────────────────────
    #region Health change handlers

    private void HandleDamage(int newHealth)
    {
        // Red bar snaps to the new value; gold bar stays, then drains after a delay.
        ApplyWidths(newHealth, _behindHealth);
        UpdateLabel(newHealth);

        if (_drainCoroutine != null)
            StopCoroutine(_drainCoroutine);

        _drainCoroutine = StartCoroutine(LingerThenDrain(newHealth));
    }

    private void HandleHeal(int newHealth)
    {
        // Both bars jump up together — heals don't show a ghost.
        _behindHealth = newHealth;

        if (_drainCoroutine != null)
        {
            StopCoroutine(_drainCoroutine);
            _drainCoroutine = null;
        }

        ApplyWidths(newHealth, _behindHealth);
        UpdateLabel(newHealth);
    }

    #endregion

    // ──────────────────────────────────────────────────────────────────────
    #region Coroutine

    private IEnumerator LingerThenDrain(int targetHealth)
    {
        yield return new WaitForSeconds(damageLingerTime);

        while (_behindHealth > targetHealth + 0.1f)
        {
            _behindHealth -= drainSpeed * Time.deltaTime;
            _behindHealth  = Mathf.Max(_behindHealth, targetHealth);
            ApplyWidths(targetHealth, _behindHealth);
            yield return null;
        }

        _behindHealth   = targetHealth;
        _drainCoroutine = null;
    }

    #endregion

    // ──────────────────────────────────────────────────────────────────────
    #region UI helpers

    private void OnLayoutReady(GeometryChangedEvent evt)
    {
        var barFill = uiDocument.rootVisualElement.Q<VisualElement>("BarFill");
        barFill.UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);

        _barFullWidth = barFill.resolvedStyle.width;

        if (_bound != null)
        {
            ApplyWidths(_lastHealth, _behindHealth);
            UpdateLabel(_lastHealth);
        }
    }

    /// <summary>
    /// Clips BarMask and BehindMask widths so their fills appear at the correct percentage.
    /// overflow:hidden on the masks does the actual clipping of the fill images inside them.
    /// </summary>
    private void ApplyWidths(float health, float behindHealth)
    {
        if (_barFullWidth <= 0f) return;

        _barMask.style.width    = _barFullWidth * Mathf.Clamp01(health       / maxHealth);
        _behindMask.style.width = _barFullWidth * Mathf.Clamp01(behindHealth / maxHealth);
    }

    private void UpdateLabel(float health)
    {
        if (_label != null)
            _label.text = Mathf.CeilToInt(health).ToString();
    }

    #endregion

    // ──────────────────────────────────────────────────────────────────────
    #region Editor safety

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (damageableTarget != null && damageableTarget is not IDamageable)
            Debug.LogError(
                $"[HealthBarController] '{damageableComponent.name}' does not implement IDamageable!",
                this);
    }
#endif

    #endregion
}