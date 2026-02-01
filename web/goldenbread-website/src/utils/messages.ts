export const MESSAGES = {
  // HTTP status messages
  UNAUTHORIZED: 'Сессия истекла',
  FORBIDDEN: 'Недостаточно прав',
  NOT_FOUND: 'Страница не найдена',
  SERVER_ERROR: 'Ошибка сервера',
  NETWORK_ERROR: 'Не удалось подключиться к серверу',

  // Validation messages
  VALIDATION_REQUIRED: 'Это обязательное поле',
  VALIDATION_LENGTH: (num: number) => `Поле должно содержать ${num} символов`,
  VALIDATION_MIN_LENGTH: (min: number) => `Минимальная длина: ${min} символов`,
  VALIDATION_MAX_LENGTH: (max: number) => `Максимальная длина: ${max} символов`,
  VALIDATION_EMAIL: 'Некорректный формат email',
  VALIDATION_PASSWORDS_MISMATCH: 'Пароли не совпадают',
  
  // Toast default titles
  TOAST_SUCCESS: 'Успех!',
  TOAST_WARNING: 'Внимание!',
  TOAST_ERROR: 'Ошибка!',
  TOAST_API_ERROR: 'Ой, что-то пошло не так...',
  
  // Toast optional titles
  LOGIN_SUCCESS: 'Вы успешно вошли в систему!',
  LOGIN_FAILED: 'Аккаунт не найден...',
  REGISTER_SUCCESS: 'Вы успешно зарегистрировались!',
  REGISTER_FAILED: 'Реквизиты уже заняты...',
  
  // Toast descriptions
  LOGIN_SUCCESS_DESC: 'Вход в систему выполнен успешно. Новая сессия зарегистрирована.',
  LOGIN_FAILED_DESC: 'Пользователь с введенными данными не найден или не существует.',
  REGISTER_SUCCESS_DESC: 'Регистрация аккаунта прошла успешно. Теперь вы можете войти в систему.',
  REGISTER_FAILED_DESC: 'Некоторые данные уже зарегистрированы в системе. Измените их или попробуйте войти.',
} as const;