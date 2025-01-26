using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Microsoft.Unity.VisualStudio.Editor;

public class StatInfo:MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text;

    public void SetStat(string value)
    {
        Text.text = value;  
        this.gameObject.SetActive(true);    
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}