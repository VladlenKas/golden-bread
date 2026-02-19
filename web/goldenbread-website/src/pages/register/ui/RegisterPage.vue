<script setup lang="ts">
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/shared/ui/form';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/shared/ui/card';
import { Button } from '@/shared/ui/button';
import { Input } from '@/shared/ui/input';
import { Loader2 } from 'lucide-vue-next';
import { registerSchema } from './registerSchema';
import { useRegister } from '../model/useRegister';
import type { RegisterRequest } from '@/shared/api';

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

            <FormField v-slot="{ componentField }" name="name">
              <FormItem v-auto-animate>
                <FormLabel>Название компании</FormLabel>
                <FormControl>
                  <Input placeholder="Пример: Golden Bread" v-bind="componentField" />
                </FormControl>
                <FormMessage />
              </FormItem>
            </FormField>

            <FormField v-slot="{ componentField }" name="inn">
              <FormItem v-auto-animate>
                <FormLabel>ИНН</FormLabel>
                <FormControl>
                  <Input placeholder="10 цифр" v-bind="componentField" />
                </FormControl>
                <FormMessage />
              </FormItem>
            </FormField>

            <FormField v-slot="{ componentField }" name="ogrn">
              <FormItem v-auto-animate>
                <FormLabel>ОГРН</FormLabel>
                <FormControl>
                  <Input placeholder="13 цифр" v-bind="componentField" />
                </FormControl>
                <FormMessage />
              </FormItem>
            </FormField>

            <FormField v-slot="{ componentField }" name="email">
              <FormItem v-auto-animate>
                <FormLabel>Email</FormLabel>
                <FormControl>
                  <Input placeholder="Пример: goldenbread@yandex.ru" v-bind="componentField" />
                </FormControl>
                <FormMessage />
              </FormItem>
            </FormField>

            <FormField v-slot="{ componentField }" name="password">
              <FormItem v-auto-animate>
                <FormLabel>Пароль</FormLabel>
                <FormControl>
                  <Input type="password" placeholder="••••••••" v-bind="componentField" />
                </FormControl>
                <FormMessage />
              </FormItem>
            </FormField>

            <FormField v-slot="{ componentField }" name="passwordConfirm">
              <FormItem v-auto-animate>
                <FormLabel>Подтверждение пароля</FormLabel>
                <FormControl>
                  <Input type="password" placeholder="••••••••" v-bind="componentField" />
                </FormControl>
                <FormMessage />
              </FormItem>
            </FormField>

            <Button type="submit" :disabled="isSubmitting" class="w-full">
              <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin"/>
            </Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  </div>
</template>
