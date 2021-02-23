using System.Collections;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

public class BoardItem : MonoBehaviour
{
    [SerializeField] RectTransform _boardRT;

    private float boardHeight;
    private float boardWidth;
    private float boardMargin;
    private float dropSpace;
    private int maxRowNum;
    private int columnNum;

    private int totalDropNum;
    private float dropBaseHeight;
    private float dropBaseWidth;
    private float dropHeight;
    private float dropWidth;

    private List<DropItem> dropList = new List<DropItem>();
    private List<Vector3> dropLocalPositionList = new List<Vector3>();

    public void Initialize(float boardMargin,float dropSpace,int maxRowNum, int columnNum)
    {
        // ボードパラメータの取得・設定
        boardHeight = _boardRT.rect.height;
        boardWidth = _boardRT.rect.width;
        this.boardMargin = boardMargin;
        this.dropSpace = dropSpace;
        this.maxRowNum = maxRowNum;
        this.columnNum = columnNum;

        // ドロップパラメータの計算・設定
        dropBaseHeight = CalculateDropBaseHeight();
        dropBaseWidth = CalculateDropBaseWidth();
        dropHeight = CalculateDropHeight();
        dropWidth = CalculateDropWidth();
        totalDropNum = GetTotalDropNum();

        // ドロップの生成
        dropList.Clear();
        dropLocalPositionList.Clear();
        for (var i = 0; i < totalDropNum; i++)
        {
            var drop = UIManager.Instance.CreateContent<DropItem>(_boardRT);
            var position = GetDropPosition(i);

            drop.GetComponent<RectTransform>().sizeDelta = new Vector2(dropWidth, dropHeight);
            drop.transform.localPosition = position;
            drop.SetInfo();
            dropList.Add(drop);
            dropLocalPositionList.Add(position);
        }
    }

    #region Setting
    private float CalculateDropBaseHeight()
    {
        return (boardHeight - (2 * boardMargin)) / maxRowNum;
    }

    private float CalculateDropBaseWidth()
    {
        return (2 * (boardWidth - (2 * boardMargin))) / (columnNum + 1);
    }

    private float CalculateDropHeight()
    {
        return dropBaseHeight - (2 * (dropSpace / 2));
    }

    private float CalculateDropWidth()
    {
        return dropHeight;
    }

    private int GetTotalDropNum()
    {
        if (columnNum % 2 == 0)
        {
            return (columnNum / 2) * (maxRowNum + maxRowNum - 1);
        }
        else
        {
            return (Mathf.CeilToInt((float)columnNum / 2) * maxRowNum) + (Mathf.FloorToInt((float)columnNum / 2) * (maxRowNum - 1));
        }
    }

    // 盤面の左下から上方向にindexが進む
    // 盤面のPivotは左下
    private Vector2 GetDropPosition(int index)
    {
        var rIndex = GetRowIndex(index); // 下から数えたインデックス
        var cIndex = GetColumnIndex(index); // 左から数えたインデックス

        var x = boardMargin + (dropBaseWidth / 2) + ((dropBaseWidth / 2) * cIndex);
        var y = cIndex % 2 == 0 ? boardMargin + (dropBaseHeight / 2) + dropBaseHeight * rIndex : boardMargin + dropBaseHeight + dropBaseHeight * rIndex;
        return new Vector2(x, y);
    }

    private int GetRowIndex(int index)
    {
        var row = maxRowNum;
        while (index >= row)
        {
            index -= row;
            row = row == maxRowNum ? maxRowNum - 1 : maxRowNum;
        }
        return index;
    }

    private int GetColumnIndex(int index)
    {
        var cIndex = 0;
        var row = maxRowNum;
        while (index >= row)
        {
            cIndex++;
            index -= row;
            row = row == maxRowNum ? maxRowNum - 1 : maxRowNum;
        }
        return cIndex;
    }
    #endregion

    #region Game
    public DropItem GetNearestDrop()
    {
        var minDistance = float.MaxValue;
        DropItem nearestDrop = null;

        // 入力値と盤面のピース位置との距離を計算し、一番距離が短いピースを探す
        dropList.ForEach(drop =>
        {
            var inputWorldPosition = TapPositionManager.Instance.GetWorldPositionFromInput();
            var distance = Vector3.Distance(inputWorldPosition, drop.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestDrop = drop;
            }
        });

        return nearestDrop;
    }
    #endregion
}
