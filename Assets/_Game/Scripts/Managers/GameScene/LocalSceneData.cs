using UnityEngine;
using Managers;
using Inventory;
using UI;
using Game;
using UnityEngine.UI;

public class LocalSceneData : MonoBehaviour
{


    [Header("UI Elements")]
    public HPBarUpdater playerHPBar;
    public HPBarUpdater enemyHPBar;
    public Button shootButton;
    public Toggle pistolToggle;
    public Toggle rifleToggle;
    public GameObject gameOverPopup;

}