<!-- pages/login/ui/LoginPage.vue -->
<script setup lang="ts">
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/shared/ui/form';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/shared/ui/card';
import { loginSchema } from './loginSchema';
import { Loader2 } from 'lucide-vue-next';
import { Button } from '@/shared/ui/button';
import { Input } from '@/shared/ui/input';
import { useLogin } from '../model/useLogin';
import type { LoginCredentials } from '@/entities/user';

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
            @submit="handleSubmit($event, values => handleLogin(<LoginCredentials> values))">

            <!-- Email -->
            <FormField v-slot="{ componentField }" name="email">
              <FormItem v-auto-animate>
                <FormLabel>Электронная почта</FormLabel>
                <FormControl>
                  <Input 
                    placeholder="goldenbread@yandex.ru" 
                    v-bind="componentField"/>
                </FormControl>
                <FormMessage />
              </FormItem>
            </FormField>

            <!-- Password -->
            <FormField v-slot="{ componentField }" name="password">
              <FormItem v-auto-animate>
                <FormLabel>Пароль</FormLabel>
                <FormControl>
                  <Input 
                    placeholder="••••••••" 
                    v-bind="componentField"/>
                </FormControl>
                <FormMessage />
              </FormItem>
            </FormField>

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