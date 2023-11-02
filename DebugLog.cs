using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DebugUtils
{

    private void DumpComponentTree(GameObject o, int depth = 0)
    {
        if (depth > 10)
            return;

        var indent = "";
        for (var i = 0; i < depth; i++)
            indent += "    ";

        var directChildren = o.gameObject.GetComponentsInChildren<Component>()
                                         .Where(c => c.transform.parent == o.transform)
                                         .Select(c => c.gameObject)
                                         .Distinct()
                                         .ToList();

        SuperController.LogMessage(indent + o.ToString());
        foreach (var c in o.GetComponents<Component>())
            SuperController.LogMessage(indent + "    " + c.ToString());

        foreach (var child in directChildren)
            DumpComponentTree(child, depth + 1);
    }

    public static class DebugLog
    {
        private static ScrollRect _scrollRect;
        private static Text _textField;
        private static Queue<string> _customLines;

        public static bool Enabled { get; set; }
        public static int LineLimit { get; set; } = 128;
        public static int LogLevel { get; set; } = 100;

        public static void Init(Text textField)
        {
            _textField = textField;
            _textField.supportRichText = true;
            _customLines = new Queue<string>();

            _scrollRect = textField.GetComponentInParent<ScrollRect>();
        }

        public static void Info(object message) => Write(100, message);
        public static void Warn(object message) => Write(200, message);
        public static void Error(object message) => Write(300, message);
        public static void Debug(object message) => Write(400, message);
        public static void Trace(object message) => Write(500, message);

        public static void Write(int level, object message)
        {
            if (!Enabled) return;
            if (_textField == null) return;
            if (level > LogLevel) return;

            _customLines.Enqueue(FormatMessage(level, message.ToString()));
            while (_customLines.Count > LineLimit)
                _customLines.Dequeue();

            _textField.text = string.Join("\n", _customLines.ToArray());
            _scrollRect.verticalNormalizedPosition = 0;
        }

        private static string FormatMessage(int level, string message)
        {
            var time = TimeSpan.FromSeconds(Time.time);
            var timeFormat = string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);

            if (level <= 100) return $"<color=#e9e9e9>[{timeFormat}][INFO]  {message}</color>";
            if (level <= 200) return $"<color=#ffc107>[{timeFormat}][WARN]  {message}</color>";
            if (level <= 300) return $"<color=#d81b60>[{timeFormat}][ERROR] {message}</color>";
            if (level <= 400) return $"<color=#03a9f4>[{timeFormat}][DEBUG] {message}</color>";
            return $"<color=#ba68c8>[{timeFormat}][TRACE] {message}</color>";
        }
    }

    public static class DebugLogPanel
    {
        private static GameObject _panelObject;

        public static Text Init(MVRScript plugin, float width, float height, Vector2 position)
        {
            var root = GameObject.Find("HUD");

            _panelObject = new GameObject();
            _panelObject.transform.SetParent(root.transform);

            var canvas = _panelObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            SuperController.singleton.AddCanvas(canvas);

            _panelObject.AddComponent<GraphicRaycaster>();
            var canvasScaler = _panelObject.AddComponent<CanvasScaler>();
            canvasScaler.scaleFactor = 1.0f;
            canvasScaler.dynamicPixelsPerUnit = 1.0f;

            var prefab = GameObject.Instantiate<Transform>(plugin.manager.configurableTextFieldPrefab);
            prefab.SetParent(_panelObject.transform);

            var rt = prefab.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            rt.anchoredPosition = position;

            var textField = prefab.GetComponent<UIDynamicTextField>();
            textField.backgroundColor = Color.white;
            textField.height = height;
            textField.backgroundColor = new Color(1, 1, 1, 0.2f);
            textField.UItext.fontSize = 16;
            textField.UItext.font = Font.CreateDynamicFontFromOSFont("Consolas", 16);

            return textField.UItext;
        }

        public static void Dispose()
        {
            if(_panelObject != null)
            {
                SuperController.singleton.RemoveCanvas(_panelObject.GetComponent<Canvas>());
                GameObject.Destroy(_panelObject);
            }
        }
    }
}