<script setup lang="ts">
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage
} from '@/components/ui/form';
import { useLoginDialog } from '@/composables/useLoginDialog';
import { loginSchema } from '@/api/login/loginSchema';
import { Loader2 } from 'lucide-vue-next';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

const { isOpen, onSubmit } = useLoginDialog();
</script>

<template>
  <Form 
    v-slot="{ handleSubmit, isSubmitting }" 
    as="" 
    keep-values 
    :validation-schema="loginSchema">
    <Dialog v-model:open="isOpen">
      <DialogTrigger as-child>
        <Button>
          Войти
        </Button>
      </DialogTrigger>
      <DialogContent class="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Вход в профиль</DialogTitle>
          <DialogDescription>
            Рады вас видеть снова! Пожалуйста, заполните поля ниже
            для получения доступа к профилю и заказам.
          </DialogDescription>
        </DialogHeader>

        <form 
          id="loginForm" 
          @submit="handleSubmit($event,onSubmit)" 
          class="space-y-4 w-full">
          <!-- Email -->
          <FormField v-slot="{ componentField }" name="email">
            <FormItem v-auto-animate>
              <FormLabel>Электронная почта</FormLabel>
              <FormControl>
                <Input 
                  type="text" 
                  placeholder="goldenbread@yandex.ru" 
                  v-bind="componentField" />
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
                  type="password" 
                  placeholder="••••••••" 
                  v-bind="componentField" />
              </FormControl>
              <FormMessage />
            </FormItem>
          </FormField>
        </form>

        <DialogFooter class="w-full pt-2">
          <Button 
            type="submit" 
            form="loginForm" 
            :disabled="isSubmitting" 
            class="w-full">
            <Loader2 
              v-if="isSubmitting" 
              class="w-4 h-4 mr-2 animate-spin" />
            {{ isSubmitting ? "Входим..." : "Войти" }}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  </Form>
</template>