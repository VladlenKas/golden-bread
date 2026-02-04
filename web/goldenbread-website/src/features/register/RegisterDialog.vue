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
import { useRegisterDialog } from '@/composables/useRegisterDialog';
import { registerSchema } from '@/api/register/registerSchema';
import { vAutoAnimate } from '@formkit/auto-animate/vue';
import { Loader2 } from 'lucide-vue-next';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

const { isOpen, onSubmit } = useRegisterDialog();
</script>

<template>
    <Form 
      v-slot="{ handleSubmit, isSubmitting, setErrors, }" 
      as="" 
      keep-values
      :validation-schema="registerSchema">
    <Dialog v-model:open="isOpen">
      <DialogTrigger as-child>
        <Button variant="outline">
          Зарегистрироваться
        </Button>
      </DialogTrigger>
      <DialogContent class="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Регистрация профиля</DialogTitle>
          <DialogDescription>
            Оформляйте заказы, сохраняйте
            понравившуюся продукцию и следите за ростом Вашего бизнеса!
          </DialogDescription>
        </DialogHeader>

        <form 
          id="registerForm" 
          @submit="handleSubmit($event, values => onSubmit(values, setErrors))" 
          class="space-y-4 w-full">
          <!-- Название компании -->
          <FormField v-slot="{ componentField }" name="name">
            <FormItem v-auto-animate>
              <FormLabel>Название компании</FormLabel>
              <FormControl>
                <Input type="text" placeholder="Golden Bread" v-bind="componentField" />
              </FormControl>
              <FormMessage />
            </FormItem>
          </FormField>

          <!-- ИНН -->
          <FormField v-slot="{ componentField }" name="inn">
            <FormItem v-auto-animate>
              <FormLabel>ИНН</FormLabel>
              <FormControl>
                <Input type="text" placeholder="00 00 000000 00" v-bind="componentField" />
              </FormControl>
              <FormMessage />
            </FormItem>
          </FormField>

          <!-- ОГРН -->
          <FormField v-slot="{ componentField }" name="ogrn">
            <FormItem v-auto-animate>
              <FormLabel>ОГРН</FormLabel>
              <FormControl>
                <Input type="text" placeholder="0 00 00 00000 0" v-bind="componentField" />
              </FormControl>
              <FormMessage />
            </FormItem>
          </FormField>

          <!-- Email -->
          <FormField v-slot="{ componentField }" name="email">
            <FormItem v-auto-animate>
              <FormLabel>Электронная почта</FormLabel>
              <FormControl>
                <Input type="email" placeholder="goldenbread@yandex.ru" v-bind="componentField" />
              </FormControl>
              <FormMessage />
            </FormItem>
          </FormField>

          <!-- Password -->
          <FormField v-slot="{ componentField }" name="password">
            <FormItem v-auto-animate>
              <FormLabel>Пароль</FormLabel>
              <FormControl>
                <Input type="password" placeholder="••••••••" v-bind="componentField" />
              </FormControl>
              <FormMessage />
            </FormItem>
          </FormField>

          <!-- Password Confirm -->
          <FormField v-slot="{ componentField }" name="passwordConfirm">
            <FormItem v-auto-animate>
              <FormLabel>Подтверждение пароля</FormLabel>
              <FormControl>
                <Input type="password" placeholder="••••••••" v-bind="componentField" />
              </FormControl>
              <FormMessage />
            </FormItem>
          </FormField>
        </form>

        <DialogFooter class="w-full pt-2">
          <Button 
            type="submit" 
            form="registerForm" 
            :disabled="isSubmitting" 
            class="w-full">
              <Loader2 
                v-if="isSubmitting" 
                class="w-4 h-4 mr-2 animate-spin" />
              {{ isSubmitting ? "Регистрируемся..." : "Зарегистрироваться" }}
            </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  </Form>
</template>