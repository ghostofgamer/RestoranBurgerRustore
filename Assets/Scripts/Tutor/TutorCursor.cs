using System.Collections.Generic;
using UnityEngine;

public class TutorCursor : MonoBehaviour
{
    [SerializeField] private Transform fingerTran;
    //[SerializeField] private Animator anim;

    private readonly Dictionary<CursorViewType, CursorTransformData> cursorViewDatas = new()
    {
        { CursorViewType.Default, new(new(0, 0, 0), new(1, 1, 1)) },

        { CursorViewType.UpEdge, new(new(0, 0, 0), new(1, 1, 1)) },
        { CursorViewType.BottomEnge, new(new(0, 0, 0), new(1, -1, 1)) },
        { CursorViewType.LeftEdge, new(new(0, 0, 90), new(-1, 1, 1)) },
        { CursorViewType.RightEdge, new(new(0, 0, -90), new(1, 1, 1)) },
        
        { CursorViewType.ToUpToRight, new(new(0, 0, 0), new(1, 1, 1)) },
        { CursorViewType.ToUpToLeft, new(new(0, 0, 0), new(-1, 1, 1)) },
        { CursorViewType.ToBottomToRight, new(new(0, 0, 0), new(1, -1, 1)) },
        { CursorViewType.ToBottomToLeft, new(new(0, 0, 0), new(-1, -1, 1)) },
    };

    public void SetTransformData(CursorViewType viewType)
    {
        var data = cursorViewDatas[viewType];
        fingerTran.rotation = Quaternion.Euler(data.Rotation);
        fingerTran.localScale = data.Scale;

        //bool toUp = viewType == CursorViewType.ToUpToLeft || viewType == CursorViewType.ToUpToRight;

        //anim.SetBool("up", toUp);
    }
}