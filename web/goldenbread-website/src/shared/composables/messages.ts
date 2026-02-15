export const validationMessages = {
  required: 'Поле должно быть заполнено',
  email: 'Поле должно содержать корректный формат email',
  onlyDigits: 'Поле должно содержать только цифры',
  passwordsMismatch: 'Пароли не совпадают',

  length: (num: number) => `Поле должно содержать ${num} символов`,
  minLength: (min: number) => `Поле должно содержать не менее ${min} символов`,
  maxLength: (max: number) => `Поле должно содержать не более ${max} символов`,
} as const;

export const ogrnMessages = {
  year: 'Год регистрации должен быть не ранее 2002',
  region: 'Код региона должен быть от 01 до 99',
  type: 'ОГРН должен начинаться с 1 (основной) или 5 (ГРН)',
} as const;
