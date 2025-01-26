using Data;
using Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using Inventory;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region variables
        public static GameManager Instance { get; private set; }
        

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

        private bool canShoot = true;
        private float shootCooldown = 0.3f;
        private int enemyLevel = 1;
#endregion

        #region Unity Methods
        private void Awake()
        {
            InitializeSingleton();
            InitializePlayerAndEnemy();
        }

        private void Start()
        {
            InitializeUI();
            FindManagers();
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

        /// <summary>
        /// Initializes UI elements and sets up event listeners.
        /// </summary>
        private void InitializeUI()
        {
            shootButton.onClick.AddListener(Shoot);
        }

        /// <summary>
        /// Finds and assigns the InventoryManager and LootManager in the scene.
        /// </summary>
        private void FindManagers()
        {
            inventoryManager = FindFirstObjectByType<InventoryManager>();
            lootManager = FindFirstObjectByType<LootManager>();
        }

        #region Player Actions

        public void Heal(int value)
        {
            inventoryManager.ConsumeItemByName("Medkit", 1);
            player.Heal(50);
            playerHPBar.UpdateHealthBar(player.hp);
        }

        private void Shoot()
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

        /// <summary>
        /// Spawns the next enemy with increased level.
        /// </summary>
        public void SpawnNextEnemy()
        {
            enemyData.level = enemyLevel++;
            SpawnEnemy();
        }
        #endregion

        #region Inventory Management
   
 
        #endregion
    }
}