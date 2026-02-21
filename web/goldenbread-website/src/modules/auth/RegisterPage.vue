<script setup lang="ts">
import { Form, FormFieldInput } from '@/shared/ui/form';
import { Card, CardContent, CardDescription, CardHeader, CardTitle, } from '@/shared/ui/card';
import { Button } from '@/shared/ui/button';
import { Loader2 } from 'lucide-vue-next';

import { useRegister } from './useRegister';
import { registerSchema } from './schemes';
import type { RegisterRequest } from './types';

const { handleRegister } = useRegister();
</script>

<template>
  <div class="flex min-h-screen items-center justify-center p-4">
    <Card class="w-full max-w-[425px]">
      <CardHeader class="space-y-1">
        <CardTitle class="text-2xl">Регистрация профиля</CardTitle>
        <CardDescription>
          Создайте аккаунт для доступа к профилю и оформлению заказов
        </CardDescription>
      </CardHeader>

      <CardContent>
        <Form
          v-slot="{ handleSubmit, isSubmitting, setErrors }"
          as=""
          keep-values
          :validation-schema="registerSchema">

          <form
            class="space-y-4"
            @submit="handleSubmit($event, values => handleRegister(values as RegisterRequest, setErrors))">

            <FormFieldInput name="name" label="Название компании" placeholder="Пример: Golden Bread"/>
            <FormFieldInput name="inn" label="ИНН" placeholder="10 цифр"/>
            <FormFieldInput name="ogrn" label="ОГРН" placeholder="13 цифр"/>
            <FormFieldInput name="email" label="Email" placeholder="Пример: goldenbread@yandex.ru"/>
            <FormFieldInput name="password" label="Пароль" type="password" placeholder="••••••••" />
            <FormFieldInput name="passwordConfirm" label="Подтвердите пароля" type="password" placeholder="••••••••" />

            <Button type="submit" :disabled="isSubmitting" class="w-full">
              <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin"/>
              {{ isSubmitting ? "Регистрируемся..." : "Зарегистрироваться" }}
            </Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  </div>
</template>
