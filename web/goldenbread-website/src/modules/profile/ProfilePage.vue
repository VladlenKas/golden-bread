<script setup lang="ts">
import { ref } from 'vue';
import { Loader2, Mail, Building2, Phone, Lock } from 'lucide-vue-next';

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger
} from '@/shared/ui/alert-dialog';
import {
  Form,
  FormFieldInput
} from '@/shared/ui/form';
import { 
  Card, 
  CardContent, 
  CardDescription, 
  CardHeader, 
  CardTitle, 
} from '@/shared/ui/card';
import { 
  Dialog, 
  DialogContent, 
  DialogDescription, 
  DialogHeader, 
  DialogTitle, 
  DialogTrigger, 
  DialogFooter, 
  DialogClose, 
} from '@/shared/ui/dialog';
import { Label } from '@/shared/ui/label';
import { Button } from '@/shared/ui/button';
import { Input } from '@/shared/ui/input';

import type { 
  ChangeEmailRequest, 
  ChangePasswordRequest, 
  UpdateContactsRequest, 
  UpdateRequisitesRequest 
} from './types';
import { 
  requisitesSchema, 
  contactsSchema, 
  changeEmailSchema, 
  changePasswordSchema 
} from './schemes';

import { useProfile } from './useProfile';

const {
  isLoading,
  profile,
  handleUpdateRequisites,
  handleUpdateContacts,
  handleChangePassword,
  handleChangeEmail,
} = useProfile();

const isPasswordDialogOpen = ref(false);
const isEmailDialogOpen = ref(false);

const isAlertOpen = ref(false);
const pendingValues = ref<UpdateRequisitesRequest | null>(null);
const pendingSetErrors = ref<((errors: Record<string, string>) => void) | null>(null);

function onSubmitClick(values: UpdateRequisitesRequest, setErrors: (errors: Record<string, string>) => void) {
  pendingValues.value = values;
  pendingSetErrors.value = setErrors;
  isAlertOpen.value = true;
}

async function onConfirm() {
  if (pendingValues.value && pendingSetErrors.value) {
    await handleUpdateRequisites(pendingValues.value, pendingSetErrors.value);
    isAlertOpen.value = false;
    pendingValues.value = null;
    pendingSetErrors.value = null;
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center">

    <div v-if="isLoading">
      <Loader2 class="w-8 h-8 animate-spin text-muted-foreground" />
    </div>

    <div v-else-if="profile" class="w-full max-w-[600px] space-y-6">

      <!-- Карточка: Основные данные -->
      <Card>
        <CardHeader>
          <div class="flex items-center gap-2">
            <Building2 class="w-5 h-5 text-muted-foreground" />
            <CardTitle class="text-xl">Основные данные</CardTitle>
          </div>
          <CardDescription>
            Юридическая информация о вашей компании и доступ к аккаунту
          </CardDescription>
        </CardHeader>

        <CardContent>
          <Form 
            v-slot="{ handleSubmit, isSubmitting, setErrors }" 
            as="" 
            keep-values 
            :initial-values="profile" 
            :validation-schema="requisitesSchema">
            <form class="space-y-4"
              @submit="handleSubmit($event, values => onSubmitClick(values as UpdateRequisitesRequest, setErrors))">
              <div class="flex items-end gap-2">
                
                <div class="flex-1 space-y-2">
                  <Label class="text-sm font-medium leading-none">Email</Label>
                  <Input disabled :model-value="profile.email"/>
                </div>

                <!-- Диалог: смена email -->
                <Dialog v-model:open="isEmailDialogOpen">
                  <DialogTrigger as-child>
                    <Button type="button" variant="outline" size="icon">
                      <Mail/>
                    </Button>
                  </DialogTrigger>
                  <DialogContent class="sm:max-w-[425px]">
                    <DialogHeader>
                      <DialogTitle>Изменить email</DialogTitle>
                      <DialogDescription>
                        Введите новый адрес электронной почты. 
                        После этой операции необходимо будет повторно войти в систему
                      </DialogDescription>
                    </DialogHeader>

                    <Form 
                      v-slot="{ handleSubmit, isSubmitting, setErrors }" 
                      as="" 
                      keep-values 
                      :validation-schema="changeEmailSchema">
                      <form class="space-y-4 py-4"
                        @submit="handleSubmit($event, values => handleChangeEmail(values as ChangeEmailRequest, setErrors))">

                        <FormFieldInput name="newEmail" label="Email" placeholder="Пример: newemail@example.com"/>
                        <FormFieldInput name="password" label="Текущий пароль (для подтверждения)" type="password" placeholder="••••••••" />

                        <DialogFooter>
                          <DialogClose as-child>
                            <Button type="button" variant="outline">Отмена</Button>
                          </DialogClose>
                          <Button type="submit" :disabled="isSubmitting">
                            <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
                            Сменить email
                          </Button>
                        </DialogFooter>
                      </form>
                    </Form>
                  </DialogContent>
                </Dialog>

                <!-- Диалог: смена пароля -->
                <Dialog v-model:open="isPasswordDialogOpen">
                  <DialogTrigger as-child>
                    <Button type="button" variant="outline" size="icon" class="shrink-0">
                      <Lock/>
                    </Button>
                  </DialogTrigger>
                  <DialogContent class="sm:max-w-[425px]">
                    <DialogHeader>
                      <DialogTitle>Сменить пароль</DialogTitle>
                      <DialogDescription>
                        Для безопасности введите текущий пароль, затем укажите новый.
                      </DialogDescription>
                    </DialogHeader>

                    <Form 
                      v-slot="{ handleSubmit, isSubmitting, setErrors }" 
                      as="" 
                      keep-values 
                      :validation-schema="changePasswordSchema">
                      <form class="space-y-4 py-4" 
                        @submit="handleSubmit($event, values => handleChangePassword(values as ChangePasswordRequest, setErrors))">
                        
                        <FormFieldInput name="oldPassword" label="Текущий пароль" type="password" placeholder="••••••••"/>
                        <FormFieldInput name="newPassword" label="Новый пароль" type="password" placeholder="••••••••"/>
                        <FormFieldInput name="confirmNewPassword" label="Подтвердите новый пароль" type="password" placeholder="••••••••"/>

                        <DialogFooter>
                          <DialogClose as-child>
                            <Button type="button" variant="outline">Отмена</Button>
                          </DialogClose>
                          <Button type="submit" :disabled="isSubmitting">
                            <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
                            Обновить пароль
                          </Button>
                        </DialogFooter>
                      </form>
                    </Form>
                  </DialogContent>
                </Dialog>
              </div>

              <!-- Карточка: основные данные -->
              <FormFieldInput name="name" label="Название компании" placeholder="Golden Bread"/>
              <div class="grid grid-cols-2 gap-4">
                <FormFieldInput name="inn" label="ИНН" placeholder="10 цифр"/>
                <FormFieldInput name="ogrn" label="ОГРН" placeholder="13 цифр"/>
              </div>

              <div class="pt-2">
                <AlertDialog v-model:open="isAlertOpen">
                  <AlertDialogTrigger as-child>
                    <Button type="submit" :disabled="isSubmitting" class="w-full sm:w-auto">
                      <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
                      Обновить данные
                    </Button>
                  </AlertDialogTrigger>
                  <AlertDialogContent>
                    <AlertDialogHeader>
                      <AlertDialogTitle>Подтвердите изменение реквизитов</AlertDialogTitle>
                      <AlertDialogDescription>
                        После сохранения изменений ваш аккаунт будет отправлен на повторную проверку.
                        Доступ к профилю и оформлению заказов будет временно ограничен
                      </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                      <AlertDialogCancel @click="pendingValues = null; pendingSetErrors = null">
                        Отмена
                      </AlertDialogCancel>
                      <AlertDialogAction @click="onConfirm">Подтвердить</AlertDialogAction>
                    </AlertDialogFooter>
                  </AlertDialogContent>
                </AlertDialog>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>

      <!-- Карточка: Контактные данные -->
      <Card>
        <CardHeader>
          <div class="flex items-center gap-2">
            <Phone class="w-5 h-5 text-muted-foreground" />
            <CardTitle class="text-xl">Контактные данные</CardTitle>
          </div>
          <CardDescription>
            Способы связи с вашей компанией
          </CardDescription>
        </CardHeader>

        <CardContent>
          <Form 
            v-slot="{ handleSubmit, isSubmitting, setErrors }" 
            as="" 
            keep-values 
            :initial-values="profile" 
            :validation-schema="contactsSchema">
            <form class="space-y-4"
              @submit="handleSubmit($event, values => handleUpdateContacts(values as UpdateContactsRequest, setErrors))">
              
              <FormFieldInput name="phone" label="Номер телефона" type="phone" placeholder="Пример: +7 (999) 000-00-00"/>
              <FormFieldInput name="address" label="Адрес" placeholder="Пример: г. Москва, ул. Примерная, д. 1"/>

              <div class="pt-2">
                <Button type="submit" :disabled="isSubmitting" class="w-full sm:w-auto">
                  <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
                  Сохранить контакты
                </Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>

    </div>
  </div>
</template>