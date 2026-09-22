using UnityEngine;
using UnityEngine.Tilemaps;

// 실제 프로젝트에서 유저 맵 에디터의 오브젝트와 타일 관리만 발췌한 코드입니다.
// 입력, 오브젝트/타일 프리뷰, 특수 오브젝트/타일 처리 코드는 생략했습니다. 

public class UserEditor : MonoBehaviour
{
    enum TileRemoveType
    {
        All,
        CantPlaceWithObj
    }
    enum SelectType
    {
        None,
        Tile,
        Object
    }

    private SelectType selectType = SelectType.Tile;
    private bool onMap = false;
    private int selectCode = 0;
    private Vector3Int mousePos;

    private void SetEntity()
    {
        Vector3Int pos = mousePos;

        if (selectType == SelectType.Object)
            SetObject(pos);
        else if (selectType == SelectType.Tile)
            SetTile(pos);
    }

    private void RemoveEntity()
    {            
        Vector3Int pos = mousePos;

        if (RemoveObject(pos))
        {
            EditorFileIO.isChangeMap = true;
            return;
        }
        
        if (RemoveTile(pos)) {
            EditorFileIO.isChangeMap = true;
            return;
        }
    }

    private bool RemoveObject(Vector3Int pos)
    {
        if (!onMap) return false;

        for (int i = MapManager.instance.objectList.Count - 1; i >= 0; i--)
        {
            MyObject myObject = MapManager.instance.objectList[i];

            if (myObject.pos == pos)
            {
                Destroy(myObject.gameObject);
                MapManager.instance.objectList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    private bool RemoveTile(Vector3Int pos, TileRemoveType type = TileRemoveType.All)
    {
        if (!onMap) return false;

        for (int i = MapManager.instance.tileList.Count - 1; i >= 0; i--)
        {
            MyTile tile = MapManager.instance.tileList[i];

            if (tile.pos == pos)
            {
                if (type == TileRemoveType.CantPlaceWithObj)
                {
                    if (!CantPlaceWithObj(tile.code)) continue;
                }

                MapManager.instance.tileList.RemoveAt(i);
                ObjectManager.SetTile(pos, null);
                ObjectManager.SetMarker(pos, null);
                return true;
            }
        }

        return false;
    }

    private void SetObject(Vector3Int pos, int code = -1)
    {
        if (!onMap) return;
        if (EditorPause.instance.IsOpenPanel()) return;

        EditorFileIO.isChangeMap = true;
        if (code < 0) code = selectCode;
        RemoveObject(pos);

        if (code == ObjectCode.Wall)
        {
            RemoveTile(pos);
        }
        else
        {
            RemoveTile(pos, TileRemoveType.CantPlaceWithObj);
        }

        if (code == ObjectCode.Player)
        {
            for (int i = MapManager.instance.objectList.Count - 1; i >= 0; i--)
            {
                MyObject myObject = MapManager.instance.objectList[i];

                if (myObject.code == ObjectCode.Player)
                {
                    Destroy(myObject.gameObject);
                    MapManager.instance.objectList.RemoveAt(i);
                }
            }
        }

        MyObject newObject = new MyObject(pos);

        newObject.code = code;
        MapManager.instance.objectList.Add(newObject);

        GameObject myGameObject = ObjectManager.CreateGameObject(code);
        newObject.SetGameobject(myGameObject);
    }

    private void SetTile(Vector3Int pos, int code = -1)
    {
        if (!onMap) return;
        if (EditorPause.instance.IsOpenPanel()) return;

        EditorFileIO.isChangeMap = true;
        if (code < 0) code = selectCode;
        
        if (CantPlaceWithObj(code))
        {
            RemoveObject(pos);
        }
        
        RemoveTile(pos);

        MyTile newTile;
        newTile = new MyTile(pos);

        newTile.code = code;
        MapManager.instance.tileList.Add(newTile);

        ObjectManager.SetTile(pos, ObjectManager.TileArrays[code][0]);
    }

    private bool CantPlaceWithObj(int code)
    {
        return code == TileCode.Bug || code == TileCode.OuterVoid;
    }
}
