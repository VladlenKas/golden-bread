<script setup lang="ts">
import { NavigationMenuList, NavigationMenuItem, NavigationMenu } from '@/shared/ui/navigation-menu';
import { useUserStore } from '@/entities/user';
import { Button } from '@/shared/ui/button';

const userStore = useUserStore();
</script>

<template>
  <div class="h-20 border-b">
    <div class="mx-auto max-w-[1200px] h-full px-8 flex items-center justify-between">

      <div class="flex items-center gap-3">
        <div class="h-9 w-9 rounded-md bg-muted" />
        <span class="font-semibold">Golden Bread</span>
      </div>

      <NavigationMenu>
        <NavigationMenuList class="flex items-center gap-8 text-sm text-muted-foreground">
          <NavigationMenuItem as-child>
            <RouterLink class="hover:text-foreground" to="/home">Главная</RouterLink>
          </NavigationMenuItem>
          <NavigationMenuItem><a class="hover:text-foreground" href="#">Каталог</a></NavigationMenuItem>
          <NavigationMenuItem><a class="hover:text-foreground" href="#">Корзина</a></NavigationMenuItem>
          <NavigationMenuItem><a class="hover:text-foreground" href="#">О нас</a></NavigationMenuItem>
          <NavigationMenuItem><a class="hover:text-foreground" href="#">Контакты</a></NavigationMenuItem>
        </NavigationMenuList>
      </NavigationMenu>

      <div class="flex gap-4" v-if="!userStore.isAuthenticated">
        <Button as-child>
          <RouterLink to="/login">Войти</RouterLink>
        </Button>
        <Button as-child>
          <RouterLink to="/register">Зарегистрироваться</RouterLink>
        </Button>
      </div>

      <div v-else>
        <Button as-child>
          <RouterLink to="/profile">Профиль</RouterLink>
        </Button>
      </div>
    </div>
  </div>
</template>