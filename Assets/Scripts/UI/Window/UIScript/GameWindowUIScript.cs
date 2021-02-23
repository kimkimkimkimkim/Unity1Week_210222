using System.Collections.Generic;
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

    [SerializeField] protected BoardItem _board;

    private List<DropItem> selectedDropList = new List<DropItem>();

    public override void Init(WindowInfo info)
    {
        _board.Initialize(BOARD_MARGIN,DROP_SPACE,MAX_ROW_NUM,COLUMN_NUM);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var drop = _board.GetNearestDrop();
            if(drop != null && !selectedDropList.Contains(drop))
            {
                selectedDropList.Add(drop);
                drop.ShowGrayOutPanel(true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectedDropList.ForEach(drop => drop.ShowGrayOutPanel(false));
            selectedDropList.Clear();
        }
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