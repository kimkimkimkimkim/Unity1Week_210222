using System.Collections.Generic;
using System.Linq;
using GameBase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[ResourcePath("UI/Window/Window-Game")]
public class GameWindowUIScript : WindowBase
{
    private const float BOARD_MARGIN = 0.0f;
    private const float DROP_SPACE = 4f; // ドロップ間の距離
    private const int MAX_ROW_NUM = 6;
    private const int COLUMN_NUM = 7;

    [SerializeField] protected BoardItem _board;

    private bool canTap = true;
    private List<DropItem> selectedDropList = new List<DropItem>();

    public override void Init(WindowInfo info)
    {
        canTap = false;
        _board.Initialize(BOARD_MARGIN,DROP_SPACE,MAX_ROW_NUM,COLUMN_NUM).Do(_ => canTap = true).Subscribe();
    }

    private void Update()
    {
        if (canTap)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                SelectDrop();
            }

            if (Input.GetMouseButtonUp(0))
            {
                canTap = false;
                OnPointerUp();
            }
        }
    }

    // 必要に応じてドロップの選択状態を更新
    private void SelectDrop()
    {
        var drop = _board.GetNearestDrop();
        if (drop == null || drop.GetDropType() == DropType.Disturb) return;

        if (!selectedDropList.Contains(drop))
        {
            // 距離などにより選択不可の場合ははじく
            if (selectedDropList.Any() && !selectedDropList.Last().CanSelect(drop)) return;

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

    private void OnPointerUp() {
        _board.DeleteDropObservable(selectedDropList)
            .SelectMany(_ => _board.FillDropObservable())
            .Do(_ => canTap = true)
            .Subscribe();

        selectedDropList.Clear();
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