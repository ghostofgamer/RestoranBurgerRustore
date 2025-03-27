using UnityEngine;

public static class Clipboard
{
    public static void SetText(string text)
    {
        TextEditor editor = new TextEditor();
        editor.text = text;
        editor.SelectAll();
        editor.Copy();
    }
}