using UnityEngine;

/// <summary>
/// 连招管理器，专门管理攻击连段索引、输入缓冲、Combo窗口
/// 不执行攻击动作，只负责管理 Combo 状态和触发时机
/// </summary>
public class PlayerComboManager : MonoBehaviour
{
    private PlayerStateManager state;
    private PlayerAnimatorManager anim;

    // 当前 Combo 索引
    public int ComboIndex { get; private set; }

    // 连招超时重置时间
    public float comboResetTime = 1.0f;
    private float comboTimer;

    // Combo窗口开启标记
    public bool ComboWindowOpen { get; private set; }

    void Awake()
    {
        state = GetComponent<PlayerStateManager>();
        anim = GetComponent<PlayerAnimatorManager>();
    }

    void Update()
    {
        UpdateComboTimer();
    }

    /// <summary>
    /// 连招超时检测
    /// </summary>
    void UpdateComboTimer()
    {
        if (ComboIndex > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    /// <summary>
    /// 开启 Combo窗口（由动画事件触发）
    /// </summary>
    public void OpenComboWindow()
    {
        ComboWindowOpen = true;
    }

    /// <summary>
    /// 关闭 Combo窗口
    /// </summary>
    public void CloseComboWindow()
    {
        ComboWindowOpen = false;
    }

    /// <summary>
    /// 尝试触发下一段连击
    /// </summary>
    public void TryCombo()
    {
        if (ComboWindowOpen)
        {
            ComboIndex++;
            comboTimer = comboResetTime;

            // TODO: 播放下一段攻击动画
            // TODO: anim.SetComboIndex(ComboIndex)
        }
    }

    /// <summary>
    /// 强制重置连招
    /// </summary>
    public void ResetCombo()
    {
        ComboIndex = 0;
        comboTimer = 0;
        CloseComboWindow();
        anim.SetComboIndex(0);
    }
}
