using UnityEngine;
using UnityEngine.InputSystem;

namespace FactoryColony
{
    public static class PlayerInputReader
    {
        public static bool IsKeyPressed(Key key)
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard[key].isPressed;
        }

        public static bool WasKeyPressedThisFrame(Key key)
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard[key].wasPressedThisFrame;
        }

        public static bool WasLeftMousePressedThisFrame()
        {
            Mouse mouse = Mouse.current;
            return mouse != null && mouse.leftButton.wasPressedThisFrame;
        }

        public static bool WasRightMousePressedThisFrame()
        {
            Mouse mouse = Mouse.current;
            return mouse != null && mouse.rightButton.wasPressedThisFrame;
        }

        public static Vector2 GetMousePosition()
        {
            Mouse mouse = Mouse.current;
            return mouse != null ? mouse.position.ReadValue() : Vector2.zero;
        }

        public static float GetMouseScrollY()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return 0f;
            }

            float scrollY = mouse.scroll.ReadValue().y;
            return Mathf.Abs(scrollY) > 1f ? scrollY / 120f : scrollY;
        }

        public static Vector2 GetMoveInput()
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (IsKeyPressed(Key.A) || IsKeyPressed(Key.LeftArrow))
            {
                horizontal -= 1f;
            }

            if (IsKeyPressed(Key.D) || IsKeyPressed(Key.RightArrow))
            {
                horizontal += 1f;
            }

            if (IsKeyPressed(Key.S) || IsKeyPressed(Key.DownArrow))
            {
                vertical -= 1f;
            }

            if (IsKeyPressed(Key.W) || IsKeyPressed(Key.UpArrow))
            {
                vertical += 1f;
            }

            Vector2 input = new Vector2(horizontal, vertical);
            return input.sqrMagnitude > 1f ? input.normalized : input;
        }
    }
}
