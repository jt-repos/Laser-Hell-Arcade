using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AugmentButton : MonoBehaviour
{
    [SerializeField] int augmentIndex = 0;
    string augmentName;

    public void PassIndex()
    {
        augmentName = gameObject.GetComponent<TextMeshProUGUI>().text.ToString();
        FindObjectOfType<Level>().LoadShowAugmentMenu(augmentName, augmentIndex);
    }
}
