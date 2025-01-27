using Data;
using Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using Inventory;
using SaveSystem;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region variables
        public static GameManager Instance { get; private set; }
        public int currentSaveSlot = -1;

        [Header("Player and Enemy")]
        public Player player;
        public Enemy enemy;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private HPBarUpdater playerHPBar;
        [SerializeField] private EnemyDataSO enemyData;
        [SerializeField] private HPBarUpdater enemyHPBar;

        [Header("UI Elements")]
        [SerializeField] private GameObject gameOverText;
        [SerializeField] private Button shootButton;
        [SerializeField] private Toggle pistolToggle;
        [SerializeField] private Toggle rifleToggle;


        [Header("Images")]
        [SerializeField] private Image pistolSprite;
        [SerializeField] private Image rifleSprite;

        [Header("Managers")]
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private LootManager lootManager;
        [SerializeField] private EquipmentManager equipmentManager;

        private bool canShoot = true;
        private float shootCooldown = 0.3f;
        private int enemyLevel = 1;
#endregion

        #region Unity Methods
        private void Awake()
        {
            InitializeSingleton();
            InitializePlayerAndEnemy();
            currentSaveSlot = DataCarrier.Instance.CurrentSaveSlotId;
        }

        #endregion

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePlayerAndEnemy()
        {
            player = new Player();
            enemy = new Enemy();
            InitializePlayer();
            SpawnEnemy();
            shootButton.onClick.AddListener(Shoot);
            if(currentSaveSlot != -1)
            {
                LoadGame();
                Debug.Log("New Game -1");
            }       
        }

        private void InitializePlayer()
        {
            player.Initialize(playerData);
            playerHPBar.UpdateHealthBar(player.hp);
        }
        private void SpawnEnemy()
        {
            enemy.Initialize(enemyData);
            enemyHPBar.UpdateHealthBar(enemy.hp);
            canShoot = true;
            shootButton.interactable = true;
        }

        private void LoadGame()
        {
            SaveData saveData = SaveLoadManager.LoadGame(currentSaveSlot);

            if (saveData != null)
            {
                Debug.Log("Found Save DAta");
                // Load Player Data
                player.hp = saveData.playerHP;
                player.level = saveData.playerLevel;

                // Load Enemy Data
                enemy.hp = saveData.enemyHP;
                enemy.level = saveData.enemyLevel;
                Debug.Log("items "+ saveData.inventorySlots);
                // Load Inventory Data
                inventoryManager.LoadInventory(saveData.inventorySlots);
                

                // Re-equip items using EquipmentManager
                equipmentManager.LoadEquipment(saveData);

                Debug.Log("Game loaded successfully.");
            }
            else
            {
                Debug.Log("No save data found. Starting new game.");
            }
        }

        public void SaveGame(int slotId)
        {
            currentSaveSlot = slotId;
            SaveLoadManager.SaveGame( currentSaveSlot,this, inventoryManager, equipmentManager);
        }

        #region Player Actions

        public void Heal(int value)
        {
            inventoryManager.ConsumeItemByName("Medkit", 1);
            player.Heal(50);
            playerHPBar.UpdateHealthBar(player.hp);
        }

        public void Shoot()
        {
            Debug.Log("shooting");
            if (!canShoot)
            {
                Debug.Log("Shoot is on cooldown.");
                return;
            }

            int damage = 0;

            if ((pistolToggle.isOn && inventoryManager.pistolAmmoCount >= 1) || (rifleToggle.isOn && inventoryManager.rifleAmmoCount >= 3))
            {
                if (pistolToggle.isOn)
                {
                    inventoryManager.ConsumeItemByName("9x18mm Ammo", 1);
                    damage = player.pistol.damageModifier;
                }
                else
                {
                    inventoryManager.ConsumeItemByName("5.45х39mm Ammo", 3);
                    damage = player.rifle.damageModifier;
                }

                enemy.TakeDamage(damage);
                player.TakeDamageFromEnemy();
            }
            else
            {
                Debug.Log("Not enough ammo.");
            }

            inventoryManager.UpdateAmmoAndWeight();
            StartCoroutine(ShootCooldown());
            playerHPBar.UpdateHealthBar(player.hp);
            enemyHPBar.UpdateHealthBar(enemy.hp);
        }

        private System.Collections.IEnumerator ShootCooldown()
        {
            canShoot = false;
            shootButton.interactable = false;
            yield return new WaitForSeconds(shootCooldown);
            canShoot = true;
            shootButton.interactable = true;
        }
        #endregion

        #region Game State Management

        public void ShowGameOver()
        {
            Time.timeScale = 0.0f;
            gameOverText.gameObject.SetActive(true);
        }

        public void GenerateLoot(int enemyLevel)
        {
            canShoot = false;
            shootButton.interactable = false;
            lootManager.GenerateLoot(enemyLevel);
        }

        public void SpawnNextEnemy()
        {
            enemyData.level = enemyLevel++;
            SpawnEnemy();
        }
        #endregion
    }
}