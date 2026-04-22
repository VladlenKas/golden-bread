<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { 
  NavigationMenuList, 
  NavigationMenuItem, 
  NavigationMenu,
  navigationMenuTriggerStyle
} from '@/shared/ui/navigation-menu';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from '@/shared/ui/alert-dialog'
import { Button } from '@/shared/ui/button';
import { Moon, Sun, ShoppingCart, Package, Info, Phone, Home, User, LogOut } from 'lucide-vue-next';
import { useAuthStore } from '@/modules/auth/stores';
import { useColorMode } from '@vueuse/core'
import { logout } from '@/modules/auth/api';

const authStore = useAuthStore();
const mode = useColorMode({ disableTransition: false });

const isScrolled = ref(false);

// Навигация
const navItems = [
  { name: 'Главная', path: '/home', icon: Home },
  { name: 'Каталог', path: '/catalog', icon: Package },
  { name: 'Корзина', path: '/cart', icon: ShoppingCart },
  { name: 'О нас', path: '/home#about', icon: Info },
  { name: 'Контакты', path: '/home#contacts', icon: Phone },
];

function handleScroll() {
  isScrolled.value = window.scrollY > 10;
}

onMounted(() => {
  window.addEventListener('scroll', handleScroll);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
});

async function onLogoutConfirm() {
  await logout();
  window.location.reload();
}
</script>

<template>
  <header 
    :class="[
      'fixed top-0 left-0 right-0 z-50 transition-all duration-300 ',
      isScrolled 
        ? 'bg-background/80 backdrop-blur-md border-b shadow-sm' 
        : 'bg-background border-b'
    ]" >
    <div class="mx-auto max-w-screen-2xl h-16">
      <div class="grid grid-cols-[1fr_auto_1fr] h-full items-center gap-4">
        
        <!-- Логотип -->
        <RouterLink to="/home" class="flex items-center gap-3 justify-self-start">
          <img 
            src="/src/shared/assets/logo.ico" 
            class="h-10 w-10"/>
          <div class="hidden sm:block">
            <span class="font-bold text-lg">Golden Bread</span>
            <p class="text-xs text-muted-foreground -mt-1">Оптовые поставки</p>
          </div>
        </RouterLink>

        <!-- Десктоп навигация -->
        <NavigationMenu class="hidden lg:flex justify-self-center">
          <NavigationMenuList class="flex items-center gap-1">
            <NavigationMenuItem v-for="item in navItems">
              <RouterLink 
                :to="item.path"
                :class="navigationMenuTriggerStyle()"
                class="gap-2">
                <component :is="item.icon" class="w-4 h-4" />
                {{ item.name }}
              </RouterLink>
            </NavigationMenuItem>
          </NavigationMenuList>
        </NavigationMenu>

        <!-- Правая часть -->
        <div class="flex items-center gap-2 justify-self-end">
          <!-- Переключатель темы -->
          <Button 
            variant="ghost" 
            size="icon" 
            class="rounded-full" 
            @click="mode = mode === 'dark' ? 'light' : 'dark'">
            <Sun v-if="mode === 'light'"/>
            <Moon v-else/>
          </Button>

          <!-- Неавторизован -->
          <template v-if="!authStore.isAuthenticated">
            <Button variant="ghost" as-child>
              <RouterLink to="/auth?mode=login">Вход</RouterLink>
            </Button>
            <Button as-child>
              <RouterLink to="/auth?mode=register">Регистрация</RouterLink>
            </Button>
          </template>

          <!-- Авторизован -->
          <template v-else>
            <Button variant="ghost" class="gap-2" as-child>
              <RouterLink to="/profile">
                <User/>
                <span>Личный кабинет</span>
              </RouterLink>
            </Button>
            
            <AlertDialog>
              <AlertDialogTrigger as-child>
                <Button variant="ghost" size="icon">
                  <LogOut/>
                </Button>
              </AlertDialogTrigger>
              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Вы действительно хотите выйти?</AlertDialogTitle>
                  <AlertDialogDescription>
                      После выхода ваша сессия перестанет быть действительной, 
                      а печеньки будут полностью очищены.
                      Все остальные данные сохранятся без изменений. Вы согласны?
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel>Отмена</AlertDialogCancel>
                  <AlertDialogAction @click="onLogoutConfirm">Подтвердить</AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>

          </template>
        </div>
      </div>
    </div>
  </header>

  <!-- Отступ для контента -->
  <div class="h-16" />
</template>