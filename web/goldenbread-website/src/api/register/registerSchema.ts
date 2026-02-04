import { MESSAGES } from '@/utils/messages';
import { toTypedSchema } from '@vee-validate/zod';
import * as z from 'zod';

export const registerSchema = toTypedSchema(
  z
    .object({
      name: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .min(2, { message: MESSAGES.VALIDATION_MIN_LENGTH(2) })
        .max(100, { message: MESSAGES.VALIDATION_MAX_LENGTH(100) }),
      inn: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .length(10, { message: MESSAGES.VALIDATION_LENGTH(10) })
        .regex(/^\d+$/, { message: MESSAGES.VALIDATION_ONLY_DIGITS }),
      ogrn: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .length(13, { message: MESSAGES.VALIDATION_LENGTH(13) })
        .regex(/^\d+$/, { message: MESSAGES.VALIDATION_ONLY_DIGITS })
        .refine((val) => val[0] === '1' || val[0] === '5', {
          message: MESSAGES.VALIDATION_OGRN_TYPE,
        })
        .refine((val) => Number(val.substring(1, 3)) >= 2, {
          message: MESSAGES.VALIDATION_OGRN_YEAR,
        })
        .refine(
          (val) => {
            const regionCode = Number(val.substring(4, 6));
            return regionCode >= 1 && regionCode <= 99;
          },
          { message: MESSAGES.VALIDATION_OGRN_REGION },
        ),
      email: z
        .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
        .email({ message: MESSAGES.VALIDATION_EMAIL })
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
