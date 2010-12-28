using UnityEngine;
using UnityEditor;
using System.Collections;

class BoardEditor: MonoBehaviour 
{
    
    [MenuItem("zz/Add Board InChild")]
    static void addBoardInAllChild()
    {
        Board.addBoardInAllChild(Selection.activeTransform.gameObject);
    }

    [MenuItem("zz/Add Board InChild", true)]
    static bool ValidateAddBoardInAllChild()
    {
        return Selection.activeTransform != null;
    }

    [MenuItem("zz/Remove Board InChild")]
    static void removeBoardInAllChild()
    {
        Board.removeBoardInAllChild(Selection.activeTransform.gameObject);
    }

    [MenuItem("zz/Remove Board InChild", true)]
    static bool ValidateRemoveBoardInAllChild()
    {
        return Selection.activeTransform != null;
    }
}