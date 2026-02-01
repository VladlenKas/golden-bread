import { MESSAGES } from '@/utils/messages';
import { toTypedSchema } from '@vee-validate/zod';
import * as z from 'zod';

export const registerSchema = toTypedSchema(
  z
    .object({
      name: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .min(3, { message: MESSAGES.VALIDATION_MIN_LENGTH(3) })
        .max(100, { message: MESSAGES.VALIDATION_MAX_LENGTH(100) }),
      inn: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .length(12, { message: MESSAGES.VALIDATION_LENGTH(12) }),
      ogrn: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .length(13, { message: MESSAGES.VALIDATION_LENGTH(13) }),
      email: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .email({ message: MESSAGES.VALIDATION_MIN_LENGTH(8) })
        .min(1, { message: MESSAGES.VALIDATION_REQUIRED })
        .max(50, { message: MESSAGES.VALIDATION_MAX_LENGTH(50) }),
      password: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .min(8, { message: MESSAGES.VALIDATION_MIN_LENGTH(8) })
        .max(50, { message: MESSAGES.VALIDATION_MAX_LENGTH(50) }),
      passwordConfirm: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .min(8, { message: MESSAGES.VALIDATION_MIN_LENGTH(8) })
        .max(50, { message: MESSAGES.VALIDATION_MAX_LENGTH(50) }),
    })
    .refine((data) => data.password === data.passwordConfirm, {
      message: MESSAGES.VALIDATION_PASSWORDS_MISMATCH,
      path: ['passwordConfirm'],
    }),
);
