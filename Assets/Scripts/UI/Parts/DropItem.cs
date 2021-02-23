using System.Collections;
using System.Collections.Generic;
using GameBase;
using UnityEngine;
using UnityEngine.UI;

[ResourcePath("UI/Parts/Parts-DropItem")]
public class DropItem : MonoBehaviour
{
    [SerializeField] protected List<Sprite> _dropSpriteList;
    [SerializeField] protected Image _dropImage;
    [SerializeField] protected Image _grayOutImage;
    [SerializeField] protected GameObject _grayOutPanel;

    public void SetInfo()
    {
        var index = UnityEngine.Random.Range(0, _dropSpriteList.Count);
        _dropImage.sprite = _dropSpriteList[index];
        _grayOutImage.sprite = _dropSpriteList[index];
    }

    public void ShowGrayOutPanel(bool isShow)
    {
        _grayOutPanel.SetActive(isShow);
    }
}
