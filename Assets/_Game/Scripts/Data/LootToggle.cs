using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Inventory
{
    public class LootToggle : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemAmountText;
        [SerializeField] private Image itemImage;
        [SerializeField] private Toggle toggle;

        /// <summary>
        /// Sets the loot item's name, amount, and image.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="itemAmount">The amount of the item.</param>
        /// <param name="itemSprite">The sprite of the item.</param>
        public void SetLootItem(string itemName, int itemAmount, Sprite itemSprite)
        {
            itemNameText.text = itemName;
            itemAmountText.text = $"x{itemAmount}"; // Display the amount
            itemImage.sprite = itemSprite;
        }

        /// <summary>
        /// Adds a listener to the toggle's onValueChanged event.
        /// </summary>
        /// <param name="callback">The callback to invoke when the toggle value changes.</param>
        public void AddToggleListener(UnityEngine.Events.UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
        }

        /// <summary>
        /// Gets the current state of the toggle.
        /// </summary>
        public bool IsToggled()
        {
            return toggle.isOn;
        }
    }
}