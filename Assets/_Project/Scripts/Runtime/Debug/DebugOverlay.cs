using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections.Generic;
using System.Text;

using CarTrickRush.Definitions;

namespace CarTrickRush.Debugging
{
    /// =========================================================================================
    /// <summary>
    /// デバッグ用の簡易HUDオーバーレイ.
    /// </summary>
    /// =========================================================================================
    public sealed class DebugOverlay : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        private static DebugOverlay _instance;

        [SerializeField] private Vector2 _position = new(32f, 32f);
        [SerializeField] private Vector2 _boxSize = new(1120f, 360f);
        [SerializeField] private float _displayDuration = 4.0f;
        [SerializeField] private int _maxLogLines = 8;
        [SerializeField] private int _fontSize = 40;
        [SerializeField] private Color _windowColor = new(0f, 0f, 0f, 0.65f);
        [SerializeField] private float _toggleInputWindow = 1.0f;

        private readonly List<LogEntry> _logs = new();
        private bool _isVisible = true;
        private int _sequenceIndex;
        private float _sequenceExpireAt;
        private GUIStyle _labelStyle;
        private readonly char[] _toggleSequence = { 'd', 'b', 'g' };

        private struct LogEntry
        {
            public string Message;
            public float HideAt;
        }

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void Update()
        {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            return;
#endif
            UpdateToggleByDbgSequence();
            PruneExpiredLogs();
        }

        private void OnGUI()
        {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            return;
#endif
            if (!_isVisible || _logs.Count == 0)
            {
                return;
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.fontSize = _fontSize;
                _labelStyle.alignment = TextAnchor.UpperLeft;
                _labelStyle.wordWrap = true;
                _labelStyle.normal.textColor = Color.white;
                _labelStyle.hover.textColor = Color.white;
                _labelStyle.active.textColor = Color.white;
                _labelStyle.focused.textColor = Color.white;
            }

            var area = new Rect(_position.x, _position.y, _boxSize.x, _boxSize.y);
            GUI.color = _windowColor;
            GUI.Box(area, GUIContent.none);
            GUI.color = Color.white;

            var stringBuilder = new StringBuilder();
            for (var index = _logs.Count - 1; index >= 0; index--)
            {
                stringBuilder.AppendLine(_logs[index].Message);
            }

            var padding = 20f;
            var textRect = new Rect(area.x + padding, area.y + padding, area.width - padding * 2f, area.height - padding * 2f);
            GUI.Label(textRect, stringBuilder.ToString(), _labelStyle);
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 回転ログをHUDに表示する.
        /// DebugOverlayが存在しない場合は何もしない.
        /// </summary>
        /// <param name="message">表示メッセージ.</param>
        public static void ShowRotationLog(string message)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _instance.PushLog(message);
        }

        /// <summary>
        /// ボーナス発動内容とキュー内容をHUDに表示する.
        /// </summary>
        public static void ShowBonusLog(string bonusName, int score, IReadOnlyList<TrickInputType> queueSnapshot)
        {
            if (_instance == null)
            {
                return;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"[Bonus] {bonusName} (+{score})");

            if (queueSnapshot == null || queueSnapshot.Count == 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("Queue: (empty)");
            }
            else
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("Queue:");

                for (var index = 0; index < queueSnapshot.Count; index++)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append($"{index + 1}. {ToShortLabel(queueSnapshot[index])}");
                }
            }

            _instance.PushLog(stringBuilder.ToString());
        }

        private void PushLog(string message)
        {
            _logs.Add(new LogEntry
            {
                Message = message,
                HideAt = Time.unscaledTime + _displayDuration
            });

            if (_logs.Count > _maxLogLines)
            {
                _logs.RemoveAt(0);
            }
        }

        private void PruneExpiredLogs()
        {
            float now = Time.unscaledTime;
            _logs.RemoveAll(log => log.HideAt <= now);
        }

        private void UpdateToggleByDbgSequence()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (Time.unscaledTime > _sequenceExpireAt)
            {
                _sequenceIndex = 0;
            }

            if (keyboard.dKey.wasPressedThisFrame) { ProcessToggleSequence('d'); }
            if (keyboard.bKey.wasPressedThisFrame) { ProcessToggleSequence('b'); }
            if (keyboard.gKey.wasPressedThisFrame) { ProcessToggleSequence('g'); }
        }

        private void ProcessToggleSequence(char input)
        {
            if (input == _toggleSequence[_sequenceIndex])
            {
                _sequenceIndex++;
            }
            else
            {
                _sequenceIndex = input == _toggleSequence[0] ? 1 : 0;
            }

            _sequenceExpireAt = Time.unscaledTime + _toggleInputWindow;

            if (_sequenceIndex >= _toggleSequence.Length)
            {
                _isVisible = !_isVisible;
                _sequenceIndex = 0;
            }
        }

        private static string ToShortLabel(TrickInputType input)
        {
            return input switch
            {
                TrickInputType.RotateUp => "Up",
                TrickInputType.RotateDown => "Down",
                TrickInputType.RotateLeft => "Left",
                TrickInputType.RotateRight => "Right",
                _ => "None"
            };
        }

        #endregion
    }
}
