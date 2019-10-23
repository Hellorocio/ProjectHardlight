using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class BuffBar : MonoBehaviour
{
    public GameObject buffIconPrefab;


    private Dictionary<BuffInstance, GameObject> buffToIcon;

    void Start()
    {
        buffToIcon = new Dictionary<BuffInstance, GameObject>();
    }
    
    // Called from Fighter that owns this BuffBar
    public void AddBuffInstance(BuffInstance buffInstance)
    {
        // Set the icon sprite and attach to buff bar
        GameObject buffIconInstance = Instantiate(buffIconPrefab);
        buffIconInstance.GetComponent<Image>().sprite = buffInstance.buff.buffIcon;
        buffIconInstance.transform.SetParent(gameObject.transform, false);
        buffIconInstance.transform.localScale = Vector3.one;

        buffToIcon.Add(buffInstance, buffIconInstance);
    }

    // Called from Fighter that owns this BuffBar
    public void RemoveBuffInstance(BuffInstance buffInstance)
    {
        // Destroy the icon associated with the buff and remove from records
        Destroy(buffToIcon[buffInstance]);
        buffToIcon.Remove(buffInstance);
    }
}
