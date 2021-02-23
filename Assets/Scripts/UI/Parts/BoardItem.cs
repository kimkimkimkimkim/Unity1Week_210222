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

    private List<List<DropItem>> dropList = new List<List<DropItem>>();
    private List<List<Vector3>> dropLocalPositionList = new List<List<Vector3>>();

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

        // ドロップの生成
        dropList.Clear();
        dropLocalPositionList.Clear();
        for (var c = 0; c < columnNum; c++)
        {
            dropList.Add(new List<DropItem>());
            dropLocalPositionList.Add(new List<Vector3>());

            var row = GetRowNum(c);
            for(var r = 0; r < row; r++)
            {
                var drop = UIManager.Instance.CreateContent<DropItem>(_boardRT);
                var position = GetDropPosition(c,r);

                drop.GetComponent<RectTransform>().sizeDelta = new Vector2(dropWidth, dropHeight);
                drop.transform.localPosition = position;
                drop.SetInfo(new DropIndex(c,r));

                dropList[c].Add(drop);
                dropLocalPositionList[c].Add(position);
            }
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

    // 盤面の左下から上方向にindexが進む
    // 盤面のPivotは左下
    private Vector2 GetDropPosition(int index)
    {
        var rIndex = GetRowIndex(index); // 下から数えたインデックス
        var cIndex = GetColumnIndex(index); // 左から数えたインデックス

        return GetDropPosition(cIndex, rIndex);
    }

    private Vector2 GetDropPosition(int cIndex,int rIndex)
    {
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

    private int GetRowNum(int columnIndex)
    {
        return columnIndex % 2 == 0 ? maxRowNum : maxRowNum - 1;
    }
    #endregion

    #region Game
    public DropItem GetNearestDrop()
    {
        var minDistance = float.MaxValue;
        DropItem nearestDrop = null;

        // 入力値と盤面のピース位置との距離を計算し、一番距離が短いピースを探す
        dropList.ForEach(list =>
        {
            list.ForEach(drop =>
            {
                var inputWorldPosition = TapPositionManager.Instance.GetWorldPositionFromInput();
                var distance = Vector3.Distance(inputWorldPosition, drop.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestDrop = drop;
                }
            });
        });

        return nearestDrop;
    }

    // ドロップオブジェクトを削除しリスト内の該当のデータをnullにする
    public void DeleteDrop(List<DropItem> selectedDropList)
    {
        selectedDropList.ForEach(drop =>
        {
            var index = drop.GetIndex();
            Destroy(drop);
            dropList[index.column][index.row] = null;
        });
    }

    public void FillDrop()
    {
        // 詰める
        dropList.ForEach((list, c) =>
        {
            var deletedDropNum = 0;
            list.ForEach((drop, r) =>
            {
                if(drop != null)
                {
                    // 今までの削除されたドロップの個数分下にずらす
                    var newRowIndex = r - deletedDropNum;
                    var position = GetDropPosition(c, newRowIndex);
                    drop.transform.localPosition = position;
                    drop.RefreshIndex(new DropIndex(c, newRowIndex));
                }
                else
                {
                    deletedDropNum++;
                }
            });
        });

        // 埋める
        dropList.ForEach((list, c) =>
        {
            var deletedDropNum = 0;
            list.ForEach((drop, r) =>
            {
                if (drop == null) deletedDropNum++;
            });
        });
    }
    #endregion
}
