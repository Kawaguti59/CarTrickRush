using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CarTrickRush.UI.Settings
{
    /// =========================================================================================
    /// <summary>
    /// 設定用スライダー。横方向の <see cref="OnMove"/> は <see cref="VolumeSlider"/> の
    /// 押しっぱなし連続入力に任せ、二重に値が動かないようにする.
    /// </summary>
    /// =========================================================================================
    public sealed class SettingsSlider : Slider
    {
        /// <inheritdoc />
        public override void OnMove(AxisEventData eventData)
        {
            var horizontal = direction == Direction.LeftToRight || direction == Direction.RightToLeft;
            if (horizontal)
            {
                switch (eventData.moveDir)
                {
                    case MoveDirection.Left:
                    case MoveDirection.Right:
                        return;
                }
            }

            base.OnMove(eventData);
        }
    }
}
