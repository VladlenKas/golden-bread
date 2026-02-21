<script setup lang="ts">
import { Form, FormFieldInput } from '@/shared/ui/form';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/shared/ui/card';
import { Loader2 } from 'lucide-vue-next';
import { Button } from '@/shared/ui/button';

import { loginSchema } from './schemes';
import type { LoginRequest } from './types';
import { useLogin } from './useLogin';

const { handleLogin } = useLogin();
</script>

<template>
  <div class="flex min-h-screen items-center justify-center p-4">
    <Card class="w-full max-w-[425px]">
      <CardHeader class="space-y-1">
        <CardTitle class="text-2xl">Вход в профиль</CardTitle>
        <CardDescription>
          Рады вас видеть снова! Пожалуйста, заполните поля ниже
          для получения доступа к профилю и заказам.
        </CardDescription>
      </CardHeader>
      
      <CardContent>
        <Form 
          v-slot="{ handleSubmit, isSubmitting }" 
          as=""
          keep-values 
          :validation-schema="loginSchema">

          <form 
            class="space-y-4" 
            @submit="handleSubmit($event, values => handleLogin(values as LoginRequest))">

            <FormFieldInput name="email" label="Email" placeholder="goldenbread@yandex.ru" />
            <FormFieldInput name="password" label="Пароль" type="password" placeholder="••••••••" />

            <Button 
              type="submit" 
              :disabled="isSubmitting" 
              class="w-full">

              <Loader2 
                v-if="isSubmitting" 
                class="w-4 h-4 mr-2 animate-spin"/>
              {{ isSubmitting ? "Входим..." : "Войти" }}
            </Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  </div>
</template>