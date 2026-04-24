<script setup lang="ts">
import { ref, watch } from 'vue';
import { Form, FormFieldInput } from '@/shared/ui/form';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/shared/ui/card';
import { Button } from '@/shared/ui/button';
import { Loader2, ArrowRight, Building2, ArrowLeft } from 'lucide-vue-next';
import { loginSchema } from './schemes';
import { registerSchema } from './schemes';
import type { LoginRequest } from './types';
import type { RegisterRequest } from './types';
import { useLogin } from './useLogin';
import { useRegister } from './useRegister';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();

const { handleLogin } = useLogin();
const { handleRegister } = useRegister();

// Определяем начальное состояние из URL
const isLogin = ref(route.query.mode !== 'register');

// Синхронизируем URL при переключении форм (чтобы можно было обновить страницу)
watch(isLogin, (newValue) => {
  router.replace({
    query: { ...route.query, mode: newValue ? 'login' : 'register' }
  });
});
</script>

<template>
  <div class="min-h-screen w-full bg-gradient-to-br from-background via-muted/30 to-background flex items-center justify-center p-4 sm:p-6 lg:p-8 relative overflow-hidden -my-6">
    <!-- Декоративные круги -->
    <div class="absolute top-0 left-0 w-full h-full overflow-hidden pointer-events-none">
      <div class="absolute -top-40 -right-40 w-96 h-96 bg-primary/5 rounded-full blur-3xl" />
      <div class="absolute top-1/2 -left-20 w-72 h-72 bg-accent/20 rounded-full blur-3xl" />
      <div class="absolute -bottom-20 right-1/4 w-80 h-80 bg-primary/5 rounded-full blur-3xl" />
    </div>

    <div class="w-full max-w-5xl grid grid-cols-1 lg:grid-cols-2 gap-8 items-center relative z-10">
      <div class="hidden lg:flex flex-col justify-center p-8 relative">
  <div class="max-w-lg space-y-6">
    <div class="space-y-2">
      <h1 class="text-4xl font-bold tracking-tight">
        Управление закупками 
        <span class="text-primary">в одном окне</span>
      </h1>
      <p class="text-muted-foreground text-lg">Личный кабинет для B2B-клиентов с историей заказов, аналитикой и быстрым повтором заказа.</p>
    </div>

    <!-- Имитация интерфейса (мокап) -->
    <div class="relative mt-8">
      <div class="absolute -inset-4 bg-gradient-to-r from-primary/20 to-accent/20 rounded-3xl blur-2xl opacity-50" />
      <div class="relative bg-card rounded-2xl border border-border shadow-2xl p-4 space-y-3 rotate-[-2deg] hover:rotate-0 transition-transform duration-500">
        <!-- Шапка мокапа -->
        <div class="flex items-center gap-2 pb-3 border-b border-border">
          <div class="w-3 h-3 rounded-full bg-red-400" />
          <div class="w-3 h-3 rounded-full bg-amber-400" />
          <div class="w-3 h-3 rounded-full bg-green-400" />
          <div class="ml-auto text-xs text-muted-foreground">golden-bread.ru</div>
        </div>
        <!-- Контент мокапа -->
        <div class="space-y-2">
          <div class="flex justify-between items-center p-2 bg-muted/50 rounded-lg">
            <div class="flex items-center gap-2">
              <div class="w-8 h-8 rounded bg-primary/20" />
              <div class="text-sm font-medium">Багет классический</div>
            </div>
            <div class="text-sm text-primary font-semibold">450 ₽</div>
          </div>
          <div class="flex justify-between items-center p-2 bg-muted/50 rounded-lg">
            <div class="flex items-center gap-2">
              <div class="w-8 h-8 rounded bg-accent/20" />
              <div class="text-sm font-medium">Круассан сливочный</div>
            </div>
            <div class="text-sm text-primary font-semibold">890 ₽</div>
          </div>
          <div class="mt-3 p-3 bg-primary/5 rounded-lg border border-primary/10">
            <div class="flex justify-between text-sm">
              <span class="text-muted-foreground">Итого:</span>
              <span class="font-bold text-primary">1 340 ₽</span>
            </div>
          </div>
        </div>
      </div>
    </div>
<div class="flex items-center justify-between pt-6 text-sm text-muted-foreground border-t border-border/50">
  <span>© 2024 Golden Bread</span>
  <div class="flex gap-4">
    <RouterLink to="/contacts" class="hover:text-primary transition-colors">Пользовательское соглашение</RouterLink>
  </div>
</div>
  </div>
</div>
      <!-- Правая часть - Переключаемые формы -->
      <div class="flex justify-center lg:justify-end perspective-1000">
        <Transition 
          mode="out-in"
          enter-active-class="transition-all duration-300 ease-out"
          enter-from-class="opacity-0 translate-x-8 rotate-y-12"
          enter-to-class="opacity-100 translate-x-0 rotate-y-0"
          leave-active-class="transition-all duration-200 ease-in"
          leave-from-class="opacity-100 translate-x-0"
          leave-to-class="opacity-0 -translate-x-8"
        >
          
          <!-- ФОРМА ВХОДА -->
          <Card v-if="isLogin" key="login" class="w-full max-w-md bg-card/95 backdrop-blur-xl border-border shadow-2xl">
            <CardHeader class="space-y-1 p-6">
              <CardTitle class="text-2xl font-bold tracking-tight">Вход в профиль</CardTitle>
              <CardDescription>
                Рады вас видеть снова! Пожалуйста, заполните поля ниже для получения доступа к профилю и заказам.
              </CardDescription>
            </CardHeader>
            
            <CardContent class="p-6 pt-0">
              <Form 
                v-slot="{ handleSubmit, isSubmitting }" 
                as=""
                keep-values 
                :validation-schema="loginSchema"
              >
                <form 
                  class="space-y-4" 
                  @submit="handleSubmit($event, values => handleLogin(values as LoginRequest))"
                >
                  <FormFieldInput 
                    name="email" 
                    label="Email" 
                    placeholder="goldenbread@yandex.ru"
                  />
                  
                  <FormFieldInput 
                    name="password" 
                    label="Пароль" 
                    type="password" 
                    placeholder="••••••••"
                  />

                  <Button 
                    type="submit" 
                    :disabled="isSubmitting" 
                    class="w-full gap-2 mt-2"
                  >
                    <Loader2 v-if="isSubmitting" class="w-4 h-4 animate-spin"/>
                    {{ isSubmitting ? "Входим..." : "Войти" }}
                    <ArrowRight v-if="!isSubmitting" class="w-4 h-4" />
                  </Button>
                </form>
              </Form>

              <div class="mt-6 text-center">
                <p class="text-sm text-muted-foreground">
                  Еще не зарегистрированы? 
                  <button 
                    @click="isLogin = false" 
                    class="text-primary hover:text-primary/80 font-medium transition-colors underline-offset-4 hover:underline ml-1"
                  >
                    Зарегистрироваться
                  </button>
                </p>
              </div>
            </CardContent>
          </Card>

          <!-- ФОРМА РЕГИСТРАЦИИ -->
          <Card v-else key="register" class="w-full max-w-md bg-card/95 backdrop-blur-xl border-border shadow-2xl">
            <CardHeader class="space-y-1 p-6">
              <div class="flex items-center gap-2 mb-2">
                <button 
                  @click="isLogin = true" 
                  class="text-muted-foreground hover:text-foreground transition-colors"
                >
                  <ArrowLeft class="h-4 w-4" />
                </button>
              </div>
              <CardTitle class="text-2xl font-bold tracking-tight">Регистрация профиля</CardTitle>
              <CardDescription>
                Создайте аккаунт для доступа к профилю и оформлению заказов
              </CardDescription>
            </CardHeader>

            <CardContent class="p-6 pt-0">
              <Form
                v-slot="{ handleSubmit, isSubmitting, setErrors }"
                as=""
                keep-values
                :validation-schema="registerSchema"
              >
                <form
                  class="space-y-4"
                  @submit="handleSubmit($event, values => handleRegister(values as RegisterRequest, setErrors))"
                >
                  <FormFieldInput name="name" label="Название компании" placeholder="Пример: Golden Bread"/>
                  <FormFieldInput name="inn" label="ИНН" placeholder="10 цифр"/>
                  <FormFieldInput name="ogrn" label="ОГРН" placeholder="13 цифр"/>
                  <FormFieldInput name="email" label="Email" placeholder="Пример: goldenbread@yandex.ru"/>
                  <FormFieldInput name="password" label="Пароль" type="password" placeholder="••••••••" />
                  <FormFieldInput name="passwordConfirm" label="Подтвердите пароль" type="password" placeholder="••••••••" />

                  <Button 
                    type="submit" 
                    :disabled="isSubmitting" 
                    class="w-full gap-2 mt-2"
                  >
                    <Loader2 v-if="isSubmitting" class="w-4 h-4 animate-spin"/>
                    {{ isSubmitting ? "Регистрируемся..." : "Зарегистрироваться" }}
                  </Button>
                </form>
              </Form>

              <div class="mt-6 text-center">
                <p class="text-sm text-muted-foreground">
                  Уже есть аккаунт? 
                  <button 
                    @click="isLogin = true" 
                    class="text-primary hover:text-primary/80 font-medium transition-colors underline-offset-4 hover:underline ml-1"
                  >
                    Войти
                  </button>
                </p>
              </div>
            </CardContent>
          </Card>

        </Transition>
      </div>
    </div>
  </div>
</template>

<style scoped>
.perspective-1000 {
  perspective: 1000px;
}
</style>