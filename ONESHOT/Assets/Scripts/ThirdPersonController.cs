using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))] // Требуется наличие компонента CharacterController
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))] // Если используется новый Input System, также требуется PlayerInput
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Скорость перемещения персонажа в м/с")]
        public float MoveSpeed = 2.0f; // Скорость перемещения персонажа при обычном движении

        [Tooltip("Скорость спринта персонажа в м/с")]
        public float SprintSpeed = 5.335f; // Скорость перемещения персонажа при спринте

        [Tooltip("Скорость поворота персонажа для изменения направления движения")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f; // Время для плавного поворота персонажа

        [Tooltip("Скорость изменения скорости при ускорении и замедлении")]
        public float SpeedChangeRate = 10.0f; // Параметр для изменения скорости

        public AudioClip LandingAudioClip; // Аудиоклип для приземления персонажа
        public AudioClip[] FootstepAudioClips; // Аудиоклипы для шагов персонажа
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f; // Громкость шагов

        // переменные для управления персонажем
        private float _speed; // Текущая скорость персонажа
        private float _animationBlend; // Значение для плавного перехода анимации
        private float _targetRotation = 0.0f; // Целевой угол поворота персонажа
        private float _rotationVelocity; // Скорость изменения угла поворота
        private float _verticalVelocity; // Вертикальная скорость (например, для прыжков и падений)

        // ID параметров анимации
        private int _animIDSpeed; // ID параметра анимации для скорости
        private int _animIDMotionSpeed; // ID параметра анимации для скорости движения

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput; // Ссылка на компонент PlayerInput для работы с новым Input System
#endif
        private Animator _animator; // Ссылка на компонент Animator для управления анимациями
        private CharacterController _controller; // Ссылка на компонент CharacterController для управления движением персонажа
        private StarterAssetsInputs _input; // Ссылка на класс для обработки пользовательского ввода
        private GameObject _mainCamera; // Ссылка на объект камеры

        private const float _threshold = 0.01f; // Пороговое значение для различных проверок

        private bool _hasAnimator; // Флаг, указывающий, есть ли у персонажа компонент Animator

        // Свойство для проверки, используется ли мышь как текущее устройство ввода
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                // Если активен новый Input System, проверяем, используется ли схема управления "KeyboardMouse"
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                // Если новый Input System не активен, возвращаем false
                return false;
#endif
            }
        }

        private void Awake()
        {
            // Получаем ссылку на главную камеру
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
                if (_mainCamera == null)
                {
                    Debug.LogError("Главная камера не найдена! Убедитесь, что объект с тегом 'MainCamera' существует в сцене.");
                }
            }
        }

        private void Start()
        {
            // Проверяем наличие необходимых компонентов
            if (!TryGetComponent(out _controller))
            {
                Debug.LogError("Отсутствует компонент CharacterController!");
            }

            _hasAnimator = TryGetComponent(out _animator);
            _input = GetComponent<StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput == null)
            {
                Debug.LogError("Отсутствует компонент PlayerInput, хотя активен новый Input System. Пожалуйста, установите его.");
            }
#else
            Debug.LogError("Starter Assets package использует новый Input System, но он не активен. Пожалуйста, активируйте новый Input System.");
#endif

            // Устанавливаем ID анимаций
            AssignAnimationIDs();
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            Move();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void Move()
        {
            // Определяем целевую скорость движения в зависимости от того, нажата ли кнопка спринта.
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            
            // Если игрок не вводит движение, устанавливаем целевую скорость в 0 (то есть персонаж стоит на месте).
            if (_input.move == Vector2.zero) 
            {
                targetSpeed = 0.0f;
            }

            // Вычисляем текущую горизонтальную скорость персонажа, игнорируя вертикальную составляющую.
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            
            // Настройка порога для изменения скорости, чтобы избежать колебаний из-за мелких изменений.
            float speedOffset = 0.1f;
            // Определяем значение масштаба ввода. Если используется аналогичное движение, берем величину вектора ввода, иначе используем 1.
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            
            // Проверяем, находится ли текущая горизонтальная скорость вне допустимого диапазона для изменения скорости.
            if (currentHorizontalSpeed < targetSpeed - speedOffset || 
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // Интерполяция текущей скорости к целевой скорости с учетом масштаба ввода.
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

                // Округляем скорость до 3 знаков после запятой для предотвращения дребезга.
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                // Если текущая скорость находится в пределах допустимого диапазона, устанавливаем её как целевую.
                _speed = targetSpeed;
            }

            // Интерполяция значения blend анимации к целевой скорости с учетом коэффициента изменения скорости.
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            // Если значение blend анимации очень маленькое, устанавливаем его в 0.
            if (_animationBlend < 0.01f) 
            {
                _animationBlend = 0f;
            }

            // Нормализуем направление ввода и создаем вектор направления для движения персонажа.
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // Если игрок вводит движение, вычисляем целевое направление и плавно интерполируем угол поворота.
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                          _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

                // Применяем вычисленный угол поворота к персонажу.
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            // Рассчитываем направление движения в соответствии с текущим целевым углом поворота.
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // Перемещаем персонажа в соответствии с вычисленным направлением и текущей скоростью, включая вертикальную скорость.
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                     new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // Если у персонажа есть аниматор, обновляем параметры анимации для скорости и темпа движения.
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

       private void OnFootstep(AnimationEvent animationEvent)
       {
            // Проверяем, имеет ли клип анимации достаточный вес, чтобы считать его значимым.
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                // Проверяем, есть ли аудиоклипы для шагов.
                if (FootstepAudioClips.Length > 0)
                {
                    // Выбираем случайный аудиоклип из массива FootstepAudioClips.
                    var index = Random.Range(0, FootstepAudioClips.Length);

                    // Воспроизводим выбранный аудиоклип на позиции центра контроллера.
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
       }

       private void OnLand(AnimationEvent animationEvent)
       {
            // Проверяем, имеет ли клип анимации достаточный вес, чтобы считать его значимым.
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                // Воспроизводим аудиоклип приземления на позиции центра контроллера.
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
       }
    }
}