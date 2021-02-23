﻿using System.Collections;
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
        InitializeList();
        for (var c = 0; c < columnNum; c++)
        {
            var row = GetRowNum(c);
            for(var r = 0; r < row; r++) CreateDrop(new DropIndex(c, r));
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

    // 盤面のドロップの個数に応じて要素defaultのリストを作成
    private void InitializeList()
    {
        dropList.Clear();

        for(var c = 0; c < columnNum; c++)
        {
            dropList.Add(new List<DropItem>());

            var row = GetRowNum(c);
            for(var r = 0; r < row; r++)
            {
                dropList[c].Add(default(DropItem));
            }
        }

    }

    private void CreateDrop(DropIndex index)
    {
        var drop = UIManager.Instance.CreateContent<DropItem>(_boardRT);
        var position = GetDropPosition(index.column, index.row);

        drop.GetComponent<RectTransform>().sizeDelta = new Vector2(dropWidth, dropHeight);
        drop.transform.localPosition = position;
        drop.SetInfo(index);

        dropList[index.column][index.row] = drop;
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
        selectedDropList.ForEach(d =>
        {
            var index = d.GetIndex();
            var drop = dropList[index.column][index.row];

            Destroy(drop.gameObject);
            dropList[index.column][index.row] = null;
        });
    }

    // 削除したドロップを埋めるように新たにドロップを作成
    public void FillDrop()
    {
        var dropMoveList = new List<DropMoveInfo>();
        var deletedDropNumList = new List<int>();

        // 詰める
        dropList.ForEach((list, c) =>
        {
            var deletedDropNum = 0;
            list.ForEach((drop, r) =>
            {
                if(drop == null)
                {
                    deletedDropNum++;
                }
                else if (deletedDropNum != 0)
                {
                    // どの位置に移動させるかの情報をリストに追加
                    dropMoveList.Add(new DropMoveInfo(new DropIndex(c, r), new DropIndex(c, r - deletedDropNum)));
                }
            });
            deletedDropNumList.Add(deletedDropNum);
        });
        dropMoveList.ForEach(dropMove =>
        {
            // ドロップの移動
            var drop = dropList[dropMove.beforeIndex.column][dropMove.beforeIndex.row];
            var position = GetDropPosition(dropMove.afterIndex.column, dropMove.afterIndex.row);
            drop.transform.localPosition = position;
            drop.RefreshIndex(dropMove.afterIndex);
            dropList[dropMove.afterIndex.column][dropMove.afterIndex.row] = drop;

            // 元々いた場所をnullにする
            dropList[dropMove.beforeIndex.column][dropMove.beforeIndex.row] = null;
        });

        // 埋める
        deletedDropNumList.ForEach((n,c) =>
        {
            if (n > 0)
            {
                var rowNum = GetRowNum(c);
                for (var r = n - 1; r >= 0; r--)
                {
                    CreateDrop(new DropIndex(c, rowNum - r - 1));
                }
            }
        });
    }

    private struct DropMoveInfo
    {
        public DropIndex beforeIndex;
        public DropIndex afterIndex;

        public DropMoveInfo(DropIndex beforeIndex,DropIndex afterIndex)
        {
            this.beforeIndex = beforeIndex;
            this.afterIndex = afterIndex;
        }
    }


    #endregion
}
