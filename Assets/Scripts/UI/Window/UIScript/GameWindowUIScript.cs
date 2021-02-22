using GameBase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[ResourcePath("UI/Window/Window-Game")]
public class GameWindowUIScript : WindowBase
{
    private const float BOARD_MARGIN = 10.0f;
    private const float DROP_SPACE = 10f; // ドロップ間の距離
    private const int MAX_ROW_NUM = 6;
    private const int COLUMN_NUM = 7;

    [SerializeField] protected RectTransform _boardRT;

    private int totalDropNum;
    private float boardHeight;
    private float boardWidth;
    private float dropBaseHeight;
    private float dropBaseWidth;
    private float dropHeight;
    private float dropWidth;

    public override void Init(WindowInfo info)
    {
        boardHeight = _boardRT.rect.height;
        boardWidth = _boardRT.rect.width;

        InitialSetUp();

        for(var i = 0; i < totalDropNum; i++)
        {
            var drop = UIManager.Instance.CreateContent<DropItem>(_boardRT);
            drop.GetComponent<RectTransform>().sizeDelta = new Vector2(dropWidth, dropHeight);
            drop.transform.localPosition = GetDropPosition(i);
        }
    }

    private void InitialSetUp()
    {
        dropBaseHeight = CalculateDropBaseHeight();
        dropBaseWidth = CalculateDropBaseWidth();
        dropHeight = CalculateDropHeight();
        dropWidth = CalculateDropWidth();
        totalDropNum = GetTotalDropNum();
    }

    private float CalculateDropBaseHeight()
    {
       return (boardHeight - (2 * BOARD_MARGIN)) / MAX_ROW_NUM;
    }

    private float CalculateDropBaseWidth()
    {
        return (2 * (boardWidth - (2 * BOARD_MARGIN))) / (COLUMN_NUM + 1);
    }

    private float CalculateDropHeight()
    {
        return dropBaseHeight - (2 * (DROP_SPACE / 2));
    }

    private float CalculateDropWidth()
    {
        return dropHeight;
    }

    private int GetTotalDropNum()
    {
        if (COLUMN_NUM % 2 == 0)
        {
            return (COLUMN_NUM / 2) * (MAX_ROW_NUM + MAX_ROW_NUM - 1);
        }
        else
        {
            return (Mathf.CeilToInt((float)COLUMN_NUM / 2) * MAX_ROW_NUM) + (Mathf.FloorToInt((float)COLUMN_NUM / 2) * (MAX_ROW_NUM - 1));
        }
    }

    // 盤面の左下から上方向にindexが進む
    // 盤面のPivotは左下
    private Vector2 GetDropPosition(int index)
    {
        var rIndex = GetRowIndex(index); // 下から数えたインデックス
        var cIndex = GetColumnIndex(index); // 左から数えたインデックス

        var x = BOARD_MARGIN + (dropBaseWidth / 2) + ((dropBaseWidth / 2) * cIndex);
        var y = cIndex % 2 == 0 ? BOARD_MARGIN + (dropBaseHeight / 2) + dropBaseHeight * rIndex : BOARD_MARGIN + dropBaseHeight + dropBaseHeight * rIndex;
        return new Vector2(x, y);
    }

    private int GetRowIndex(int index)
    {
        var row = MAX_ROW_NUM;
        while (index >= row)
        {
            index -= row;
            row = row == MAX_ROW_NUM ? MAX_ROW_NUM - 1 : MAX_ROW_NUM;
        }
        return index;
    }

    private int GetColumnIndex(int index)
    {
        var cIndex = 0;
        var row = MAX_ROW_NUM;
        while (index >= row)
        {
            cIndex++;
            index -= row;
            row = row == MAX_ROW_NUM ? MAX_ROW_NUM - 1 : MAX_ROW_NUM;
        }
        return cIndex;
    }

    public override void Open(WindowInfo info)
    {
    }

    public override void Back(WindowInfo info)
    {
    }

    public override void Close(WindowInfo info)
    {
    }
}