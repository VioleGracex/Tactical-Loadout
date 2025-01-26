using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class HPBarUpdater : MonoBehaviour
    {
        // Reference to the Text component that displays the health value
        public TextMeshProUGUI healthText;
        // Reference to the Slider component that represents the health bar
        public Slider healthSlider;

        // Function to update both the Text and Slider components with the given health value
        public void UpdateHealthBar(float health)
        {
            // Update the slider's value
            healthSlider.value = health/100;
            // Update the text to show the health value
            healthText.text = $"{health}";
        }
    }
}