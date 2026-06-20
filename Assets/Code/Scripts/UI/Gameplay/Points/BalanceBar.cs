using UnityEngine;
using UnityEngine.UIElements;

public class BalanceBar : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    private VisualElement balanceDemonicBarFill;
    private VisualElement balanceAngelicBarFill;
    private Label balancePointsLabel;
    private Label demonPoints;
    private Label angelPoints;

    private void Awake()
    {
        var root = document.rootVisualElement;

        balanceDemonicBarFill = root.Q<VisualElement>("BalanceDemonicBarFill");
        balanceAngelicBarFill = root.Q<VisualElement>("BalanceAngelicBarFill");
        balancePointsLabel = root.Q<Label>("BalancePointsLabel");
        demonPoints = root.Q<Label>("DemonPoints");
        angelPoints = root.Q<Label>("AngelPoints");
    }

    private void Update()
    {
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        float scale = Mathf.Clamp(GameState.Instance.Scale, -1f, 1f);

        float angelPercent = (scale + 1f) * 0.5f;
        float demonPercent = 1f - angelPercent;

        balanceAngelicBarFill.style.width = Length.Percent(angelPercent * 100f);
        balanceDemonicBarFill.style.width = Length.Percent(demonPercent * 100f);

        balancePointsLabel.text = $"Balance: {scale:0.00}";

        demonPoints.text = $"{GameState.Instance.GlobalCombatPoints.demonPoints}";
        angelPoints.text = $"{GameState.Instance.GlobalCombatPoints.angelPoints}";
    }
}
