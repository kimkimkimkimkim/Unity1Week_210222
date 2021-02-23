using System.Collections.Generic;
using System.Linq;
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
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            SelectDrop();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DeleteDrop();
            FillDrop();
        }
    }

    // 必要に応じてドロップの選択状態を更新
    private void SelectDrop()
    {
        var drop = _board.GetNearestDrop();
        if (drop == null) return;

        if (!selectedDropList.Contains(drop))
        {
            // 距離などにより選択不可の場合ははじく
            if (selectedDropList.Any() && !selectedDropList.Last().CanSelect(drop.GetIndex())) return;

            // 未選択状態のドロップなので選択
            selectedDropList.Add(drop);
            drop.ShowGrayOutPanel(true);
        }
        else if(selectedDropList.Count >= 2 && drop == selectedDropList[selectedDropList.Count-2])
        {
            // 1つ前に選択したドロップなら直近のドロップを非選択状態にする
            var targetDrop = selectedDropList.LastOrDefault();
            targetDrop.ShowGrayOutPanel(false);
            selectedDropList.Remove(targetDrop);
        }
    }

    private void DeleteDrop()
    {
        _board.DeleteDrop(selectedDropList);
        selectedDropList.Clear();
    }

    private void FillDrop()
    {
        _board.FillDrop();
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