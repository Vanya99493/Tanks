using UnityEngine;
using UnityEngine.UI;

namespace TankModule
{
    public class TankCanvas : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Slider aimSlider;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Image fillImage;
        
        [Header("Color settings")]
        [SerializeField] private Color fullHealthColor = Color.green;
        [SerializeField] private Color zeroHealthColor = Color.red;
        
        public void SetHealth(int maxHealth, int currentHealth)
        {
            healthSlider.value = currentHealth;
            fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / maxHealth);
        }

        public void SetAimSlider(float newValue)
        {
            aimSlider.value = newValue;
        }
    }
}