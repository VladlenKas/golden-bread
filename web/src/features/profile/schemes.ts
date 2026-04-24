import { validationMessages, ogrnMessages } from '@/shared/constants/messages';
import { toTypedSchema } from '@vee-validate/zod';
import * as z from 'zod';

export const requisitesSchema = toTypedSchema(
  z.object({
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
  }),
);

export const contactsSchema = toTypedSchema(
  z.object({
    phone: z.preprocess(
      (val) => (val == null || val === '' ? undefined : val),
      z.string()
        .regex(/^\d+$/, { message: validationMessages.onlyDigits })
        .length(11, { message: validationMessages.length(11) })
        .refine((val) => val[0] === '8', {
        message: validationMessages.number,
      })
        .optional()
    ),
    address: z.preprocess(
      (val) => (val == null || val === '' ? undefined : val),
      z.string()
        .min(5, { message: validationMessages.minLength(5) })
        .max(200, { message: validationMessages.maxLength(200) })
        .optional()
    ),
  }),
);

export const changeEmailSchema = toTypedSchema(
  z.object({
    newEmail: z
      .string({ required_error: validationMessages.required })
      .email({ message: validationMessages.email })
      .min(1, { message: validationMessages.required })
      .max(50, { message: validationMessages.maxLength(50) }),
    password: z
      .string({ required_error: validationMessages.required })
      .min(1, { message: validationMessages.required })
      .max(50, { message: validationMessages.maxLength(50) }),
  }),
);

export const changePasswordSchema = toTypedSchema(
  z
    .object({
      oldPassword: z
        .string({ required_error: validationMessages.required })
        .min(1, { message: validationMessages.required })
        .max(50, { message: validationMessages.maxLength(50) }),
      newPassword: z
        .string({ required_error: validationMessages.required })
        .min(8, { message: validationMessages.minLength(8) })
        .max(50, { message: validationMessages.maxLength(50) }),
      confirmNewPassword: z
        .string({ required_error: validationMessages.required })
        .min(8, { message: validationMessages.minLength(8) })
        .max(50, { message: validationMessages.maxLength(50) }),
    })
    .refine((data) => data.newPassword === data.confirmNewPassword, {
      message: validationMessages.passwordsMismatch,
      path: ['confirmNewPassword'],
    }),
);

