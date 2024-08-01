using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move; // Направление движения персонажа
        public Vector2 look; // Направление взгляда персонажа
        public bool sprint; // Состояние спринта

        [Header("Movement Settings")]
        public bool analogMovement; // Использовать ли аналоговое движение

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true; // Заблокировать ли курсор
        public bool cursorInputForLook = true; // Использовать ли курсор для поворота камеры

#if ENABLE_INPUT_SYSTEM
        // Метод вызывается при изменении ввода движения (новый Input System)
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        // Метод вызывается при изменении ввода поворота (новый Input System)
        public void OnLook(InputValue value)
        {
            if(cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        // Метод вызывается при изменении состояния спринта (новый Input System)
        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
#endif

        // Метод для обновления ввода движения
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        } 

        // Метод для обновления ввода поворота
        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        // Метод для обновления состояния спринта
        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        // Метод вызывается при изменении фокуса приложения
        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        // Метод для установки состояния курсора
        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}