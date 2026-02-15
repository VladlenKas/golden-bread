import { validationMessages, ogrnMessages, } from '@/shared/composables/messages';
import { toTypedSchema } from '@vee-validate/zod';
import * as z from 'zod';

export const registerSchema = toTypedSchema(
  z
    .object({
      name: z
        .string({ required_error: validationMessages.required })
        .min(2, { message: validationMessages.minLength(2) })
        .max(100, { message: validationMessages.maxLength(100) }),
      inn: z
        .string({ required_error: validationMessages.required })
        .length(10, { message: validationMessages.length(10) })
        .regex(/^\d+$/, { message: validationMessages.onlyDigits }),
      ogrn: z
        .string({ required_error: validationMessages.required })
        .length(13, { message: validationMessages.length(13) })
        .regex(/^\d+$/, { message: validationMessages.required })
        .refine((val) => val[0] === '1' || val[0] === '5', {
          message: ogrnMessages.type,
        })
        .refine((val) => Number(val.substring(1, 3)) >= 2, {
          message: ogrnMessages.year,
        })
        .refine(
          (val) => {
            const regionCode = Number(val.substring(4, 6));
            return regionCode >= 1 && regionCode <= 99;
          },
          { message: ogrnMessages.region },
        ),
      email: z
        .string({ required_error: validationMessages.required })
        .email({ message: validationMessages.email })
        .min(1, { message: validationMessages.required })
        .max(50, { message: validationMessages.maxLength(50) }),
      password: z
        .string({ required_error: validationMessages.required })
        .min(8, { message: validationMessages.minLength(8) })
        .max(50, { message: validationMessages.maxLength(50) }),
      passwordConfirm: z
        .string({ required_error: validationMessages.required })
        .min(8, { message: validationMessages.minLength(8) })
        .max(50, { message: validationMessages.maxLength(50) }),
    })
    .refine((data) => data.password === data.passwordConfirm, {
      message: validationMessages.passwordsMismatch,
      path: ['passwordConfirm'],
    }),
);
