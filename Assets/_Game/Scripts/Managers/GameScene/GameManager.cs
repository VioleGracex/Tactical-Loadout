using Data;
using Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using Inventory;
using SaveSystem;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region Variables
        public static GameManager Instance { get; private set; }
        public int currentSaveSlot = -1;

        [Header("Player and Enemy")]
        public Player player;
        public Enemy enemy;
        private PlayerDataSO playerData; // Loaded from Resources
        private EnemyDataSO enemyData; // Loaded from Resources

        [Header("Managers")]
        private InventoryManager inventoryManager;
        private LootManager lootManager;
        private EquipmentManager equipmentManager;

        [Header("UI Elements")]
        private HPBarUpdater playerHPBar;
        private HPBarUpdater enemyHPBar;
        private Button shootButton;
        private Toggle pistolToggle;
        private Toggle rifleToggle;
        private GameObject gameOverPopup;

        private bool canShoot = true;
        private float shootCooldown = 0.3f;
        private int enemyLevel = 1;
        #endregion

        #region Unity Methods
        private void Awake()
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
            InitializeSingleton();
            LoadScriptableObjects();
            currentSaveSlot = DataCarrier.Instance.CurrentSaveSlotId;
        }

        private void Start()
        {
            InitializeSceneData();
        }
        #endregion

        private void InitializeSingleton()
        {
            
        }

        private void LoadScriptableObjects()
        {
            // Load PlayerDataSO and EnemyDataSO from Resources
            playerData = Resources.Load<PlayerDataSO>("SO/NewPlayerData");
            enemyData = Resources.Load<EnemyDataSO>("SO/NewEnemyData");

            if (playerData == null || enemyData == null)
            {
                Debug.LogError("Failed to load ScriptableObjects from Resources.");
            }
        }

        private void InitializeSceneData()
        {
            LocalSceneData sceneData = FindFirstObjectByType<LocalSceneData>();
            if (sceneData == null)
            {
                Debug.LogError("LocalSceneData not found in the scene.");
                return;
            }

            // Assign references from LocalSceneData
            inventoryManager = FindFirstObjectByType<InventoryManager>();
            lootManager = FindFirstObjectByType<LootManager>();
            equipmentManager = FindFirstObjectByType<EquipmentManager>();
            playerHPBar = sceneData.playerHPBar;
            enemyHPBar = sceneData.enemyHPBar;
            shootButton = sceneData.shootButton;
            pistolToggle = sceneData.pistolToggle;
            rifleToggle = sceneData.rifleToggle;
            gameOverPopup = sceneData.gameOverPopup;

            // Initialize player and enemy
            InitializePlayerAndEnemy();
        }

        private void InitializePlayerAndEnemy()
        {
            player = new Player();
            enemy = new Enemy();
            InitializePlayer();
            SpawnEnemy();
            shootButton.onClick.AddListener(Shoot);

            if (currentSaveSlot != -1)
            {
                LoadGame();
                Debug.Log("Loaded Game from Save Slot: " + currentSaveSlot);
            }
            else
            {
                FindFirstObjectByType<InitialPlayerItems>().AddInitialItems(inventoryManager);
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
                Debug.Log("Found Save Data");
                // Load Player Data
                player.hp = saveData.playerHP;
                player.level = saveData.playerLevel;

                // Load Enemy Data
                enemy.hp = saveData.enemyHP;
                enemy.level = saveData.enemyLevel;

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
            SaveLoadManager.SaveGame(currentSaveSlot, this, inventoryManager, equipmentManager);
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

        private IEnumerator ShootCooldown()
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
            gameOverPopup.SetActive(true);
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