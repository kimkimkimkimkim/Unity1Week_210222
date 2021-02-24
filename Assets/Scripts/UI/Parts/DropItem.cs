using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameBase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[ResourcePath("UI/Parts/Parts-DropItem")]
public class DropItem : MonoBehaviour
{
    [SerializeField] protected List<Sprite> _dropSpriteList;
    [SerializeField] protected Image _dropImage;
    [SerializeField] protected Image _grayOutImage;
    [SerializeField] protected GameObject _grayOutPanel;
    [SerializeField] protected CanvasGroup _canvasGroup;

    private DropType type;
    private DropIndex index;

    public void SetInfo(DropIndex index)
    {
        var disturbDropRatio = 25.0f;
        var random = UnityEngine.Random.Range(0.0f, 100.0f);
        var type = random < disturbDropRatio ? DropType.Disturb : DropType.Normal;

        var spriteIndex = (int)type;
        _dropImage.sprite = _dropSpriteList[spriteIndex];
        _grayOutImage.sprite = _dropSpriteList[spriteIndex];

        this.type = type;
        this.index = index;

        //if (type == DropType.Disturb) transform.localScale = (1 / (float)(Math.Sqrt(2))) * Vector3.one;
    }

    public DropIndex GetIndex()
    {
        return index;
    }

    public DropType GetDropType()
    {
        return type;
    }

    public void RefreshIndex(DropIndex index)
    {
        this.index = index;
    }

    public void ShowGrayOutPanel(bool isShow)
    {
        _grayOutPanel.SetActive(isShow);
    }

    // 自身からターゲットとなるドロップの選択が可能かどうかを返す
    public bool CanSelect(DropItem drop)
    {
        // お邪魔ドロップなら選択不可
        if (drop.GetDropType() == DropType.Disturb) return false;

        var index = drop.GetIndex();
        if(index.column == this.index.column)
        {
            // 上下のドロップ
            if (this.index.row - 1 <= index.row && index.row <= this.index.row + 1) return true;
        }
        else if(index.column == this.index.column - 1 || index.column == this.index.column + 1) 
        {
            // 左右の列のドロップ
            var minRowIndex = this.index.column % 2 == 0 ? this.index.row - 1 : this.index.row;
            var maxRowIndex = this.index.column % 2 == 0 ? this.index.row : this.index.row + 1;
            if (minRowIndex <= index.row && index.row <= maxRowIndex) return true;
        }

        return false;
    }

    public IObservable<Unit> PlayDeleteAnimationObservable()
    {
        var time = 0.5f;
        var endAlpha = 0.1f;

        return _canvasGroup.DOFade(endAlpha, time).OnCompleteAsObservable().Do(_ => Destroy(gameObject)).AsUnitObservable();
    }

    public CanvasGroup GetCanvasGroup()
    {
        return _canvasGroup;
    }
}

public struct DropIndex
{
    public int column; // 左からのIndex
    public int row; // 下からのIndex

    public DropIndex(int column,int row)
    {
        this.column = column;
        this.row = row;
    }
}
