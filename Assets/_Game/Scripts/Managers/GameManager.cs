using Data;
using Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Player and Enemy")]
        public Player player;
        public Enemy enemy;
        [SerializeField] PlayerDataSO playerData;
        [SerializeField] HPBarUpdater playerHPBar;
        [SerializeField] EnemyDataSO enemyData;
        [SerializeField] HPBarUpdater enemyHPBar;

        [Header("UI Elements")]
        [SerializeField] GameObject gameOverText;
        [SerializeField] Button shootButton;
        [SerializeField] Toggle pistolToggle;
        [SerializeField] Toggle rifleToggle;

        [Header("Images")]
        [SerializeField] Image pistolSprite;
        [SerializeField] Image rifleSprite;

        [Header("Managers")]
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private LootManager lootManager;

        private bool canShoot = true;
        private float shootCooldown = 1.0f;

        void Awake()
        {
            player = new Player();
            enemy = new Enemy();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            InitializePlayer();
            SpawnEnemy();
        }

        void Start()
        {
            shootButton.onClick.AddListener(Shoot);

            inventoryManager = FindFirstObjectByType<InventoryManager>();
            lootManager = FindFirstObjectByType<LootManager>();
        }

        void InitializePlayer()
        {
            player.Initialize(playerData);
            playerHPBar.UpdateHealthBar(player.hp);
            //playerSprite = playerData.image;
        }

        void SpawnEnemy()
        {
            enemy.Initialize(enemyData);
            enemyHPBar.UpdateHealthBar(enemy.hp);
            //enemySprite = enemyData.image;
        }
        public void Heal()
        {
            inventoryManager.ConsumeItemByName("Medkit", 1);    
            player.Heal(50);
        }
        void Shoot()
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

        System.Collections.IEnumerator ShootCooldown()
        {
            canShoot = false;
            yield return new WaitForSeconds(shootCooldown);
            canShoot = true;
        }

        public void ShowGameOver()
        {
            Time.timeScale = 0.0f;
            gameOverText.gameObject.SetActive(true);
        }

        public void GenerateLoot(int enemyLevel)
        {
            lootManager.GenerateLoot(enemyLevel);
        }

        void SpawnNextEnemy()
        {
            enemyData = Instantiate(enemyData); // Clone the enemyData to reset values if necessary
            SpawnEnemy();
        }

        void SaveInventory()
        {
            if (inventoryManager != null)
            {
                inventoryManager.SaveInventory();
            }
            else
            {
                Debug.LogError("InventoryManager not found in the scene.");
            }
        }
    }
}