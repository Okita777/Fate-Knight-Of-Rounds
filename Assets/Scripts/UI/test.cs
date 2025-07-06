using ResourceSystem.Core;
using UnityEngine;
using UnityEngine.UI;

public class SimpleButtonLoader : MonoBehaviour
{
    public Button button;
    public string imagePath = "sword_area";

    async void Start()
    {
        // 🎯 使用我们的资源管理系统加载图片
        try
        {
            Debug.Log($"🔍 使用 ResourceManager 加载图片: {imagePath}");

            var sprite = await ResourceManager.Instance.LoadAssetAsync<Sprite>(imagePath);

            if (sprite != null && button != null && button.image != null)
            {
                button.image.sprite = sprite;
                Debug.Log("✅ 图片设置成功！");
            }
            else
            {
                Debug.LogError("❌ 图片加载失败或按钮组件为空");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ ResourceManager 加载失败: {e.Message}");
        }
    }

    // 🎯 可选：在对象销毁时释放引用
    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(imagePath))
        {
            ResourceManager.Instance.RemoveReference(imagePath);
        }
    }
}