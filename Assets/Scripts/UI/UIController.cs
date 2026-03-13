using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

/// <summary>
/// UIController is a singleton manager that handles UI initialization and data binding for the HUD.
/// It loads the UIDocument and automatically binds UI bars (health and cooldown) to an IDamageable data source.
/// The controller queries CustomBar elements from the UIDocument and establishes DataBindings that connect them
/// to the Health and Cooldown properties of the assigned damageable component.
/// 
/// Setup:
/// - Attach this script to a GameObject with a UIDocument component.
/// - Assign a MonoBehaviour that implements IDamageable to the damageableComponent field.
/// - The controller will persist across scene loads via DontDestroyOnLoad.
/// 
/// Usage:
/// - Bars are automatically bound on initialization.
/// - Call SetDataSource() to change the root data source for all UI elements.
/// </summary>
public class UIController : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the UIController. Persists across scene loads.
    /// </summary>
    public static UIController Instance { get; private set; }

    /// <summary>
    /// Reference to the UIDocument component that contains the UI visual tree.
    /// </summary>
    [SerializeField] private UIDocument uiDocument;

    /// <summary>
    /// Optional reference to a MonoBehaviour that implements IDamageable.
    /// Can be assigned in the Inspector to bind UI bars to a specific damageable component.
    /// </summary>
    [SerializeField] private MonoBehaviour damageableComponent;

    /// <summary>
    /// The IDamageable interface implementation that holds health and cooldown data for UI binding.
    /// </summary>
    private IDamageable damageableData;

    /// <summary>
    /// Reference to the health bar CustomBar element for direct manipulation if needed.
    /// </summary>
    private CustomBar healthBar;

    /// <summary>
    /// Reference to the cooldown bar CustomBar element for direct manipulation if needed.
    /// </summary>
    private CustomBar cooldownBar;

    /// <summary>
    /// Root visual element of the UI document.
    /// </summary>
    private VisualElement rootElement;

    /// <summary>
    /// The panel visual element that contains the health and cooldown bars from CustomBars.uxml.
    /// </summary>
    private VisualElement barPanel;

    /// <summary>
    /// Initializes the singleton instance and sets up the UI document.
    /// Called when the GameObject is first instantiated.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        InitializeUIDocument();
    }

    /// <summary>
    /// Initializes the UIDocument and binds the damageable data to the UI bars.
    /// Retrieves the root visual element, finds the bar panel, and sets up data bindings.
    /// </summary>
    private void InitializeUIDocument()
    {
        if (damageableComponent != null)
        {
            damageableData = damageableComponent as IDamageable;
            if (damageableData == null)
            {
                Debug.LogError("Assigned damageableComponent does not implement IDamageable!");
            }
        }

        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component not found on UIController GameObject!");
            return;
        }

        rootElement = uiDocument.rootVisualElement;
        if (rootElement == null)
        {
            Debug.LogError("Root visual element not found!");
            return;
        }

        // Find the bar panel from CustomBars UXML
        barPanel = rootElement.Query<VisualElement>("Panel").First();
        if (barPanel != null)
        {
            BindDamageableBars();
        }
    }

    /// <summary>
    /// Binds the IDamageable data to the health and cooldown bars.
    /// Sets up DataBinding for barValue property to Health and Cooldown using PropertyPath.
    /// </summary>
    private void BindDamageableBars()
    {
        // Set the data source
        barPanel.dataSource = damageableData;

        // Get all CustomBar elements
        this.healthBar = barPanel.Query<CustomBar>().Where(el => el.ClassListContains("healthBar")).First();
        this.cooldownBar = barPanel.Query<CustomBar>().Where(el => el.ClassListContains("cdBar")).First();
    
        // Bind health bar
        if (healthBar != null)
        {
            healthBar.SetBinding("barValue", new DataBinding()
            {
                dataSource = damageableData,
                dataSourcePath = new PropertyPath(nameof(damageableData.Health)),
                bindingMode = BindingMode.ToTarget
            });
        }

        // Bind cooldown bar
        if (cooldownBar != null)
        {
            cooldownBar.SetBinding("barValue", new DataBinding()
            {
                dataSource = damageableData,
                dataSourcePath = new PropertyPath(nameof(damageableData.Cooldown)),
                bindingMode = BindingMode.ToTarget
            });
        }
        Debug.Log("DamageableData bindings initialized");
    }

    /// <summary>
    /// Sets the data source for the entire root visual element.
    /// </summary>
    /// <param name="dataSource">The object to use as the data source for UI bindings.</param>
    public void SetDataSource(object dataSource)
    {
        if (rootElement != null)
        {
            rootElement.dataSource = dataSource;
        }
    }

    /// <summary>
    /// Gets the root visual element of the UIDocument.
    /// </summary>
    /// <returns>The root VisualElement, or null if the UIDocument is not initialized.</returns>
    public VisualElement GetRootElement()
    {
        return rootElement;
    }

    /// <summary>
    /// Gets the UIDocument component reference.
    /// </summary>
    /// <returns>The UIDocument component, or null if not initialized.</returns>
    public UIDocument GetUIDocument()
    {
        return uiDocument;
    }
}
