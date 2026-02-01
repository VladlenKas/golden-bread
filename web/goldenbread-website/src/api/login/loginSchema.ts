import { MESSAGES } from '@/utils/messages';
import { toTypedSchema } from '@vee-validate/zod';
import * as z from 'zod';

export const loginSchema = toTypedSchema(
  z.object({
    email: z
      .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
      .min(1, { message: MESSAGES.VALIDATION_REQUIRED })
      .max(50, { message: MESSAGES.VALIDATION_MAX_LENGTH(50) }),
    password: z
      .string({ required_error: MESSAGES.VALIDATION_REQUIRED })
      .min(1, { message: MESSAGES.VALIDATION_REQUIRED })
      .max(50, { message: MESSAGES.VALIDATION_MAX_LENGTH(50) }),
  }),
);
