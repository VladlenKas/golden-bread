import { validationMessages } from '@/shared/composables/messages';
import { toTypedSchema } from '@vee-validate/zod';
import * as z from 'zod';

export const loginSchema = toTypedSchema(
  z.object({
    email: z
      .string({ required_error: validationMessages.required })
      .min(1, { message: validationMessages.required })
      .max(50, { message: validationMessages.minLength(50) }),
    password: z
      .string({ required_error: validationMessages.required })
      .min(1, { message: validationMessages.required })
      .max(50, { message: validationMessages.maxLength(50) }),
  }),
);
