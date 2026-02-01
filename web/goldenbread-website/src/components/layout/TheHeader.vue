<script setup lang="ts">
import { useLoginStore } from '@/features/login/loginStore';
import { Button } from '@/components/ui/button';
import RegisterDialog from '@/features/register/RegisterDialog.vue';
import LoginDialog from '@/features/login/LoginDialog.vue';

const loginStore = useLoginStore();
</script>

<template>
  <div class="h-20 border-b">
      <div class="mx-auto max-w-[1200px] h-full px-8 flex items-center justify-between">

        <div class="flex items-center gap-3">
          <div class="h-9 w-9 rounded-md bg-muted" />
          <span class="font-semibold">Golden Bread</span>
        </div>

        <NavigationMenuRoot>
          <NavigationMenuList class="flex items-center gap-8 text-sm text-muted-foreground">
            <NavigationMenuItem><a class="hover:text-foreground" href="#">Главная</a></NavigationMenuItem>
            <NavigationMenuItem><a class="hover:text-foreground" href="#">Каталог</a></NavigationMenuItem>
            <NavigationMenuItem><a class="hover:text-foreground" href="#">Корзина</a></NavigationMenuItem>
            <NavigationMenuItem><a class="hover:text-foreground" href="#">О нас</a></NavigationMenuItem>
            <NavigationMenuItem><a class="hover:text-foreground" href="#">Контакты</a></NavigationMenuItem>
          </NavigationMenuList>
        </NavigationMenuRoot>

         <div class="flex gap-4" v-if="!loginStore.isAuthenticated">
           <LoginDialog/>
           <RegisterDialog/>
         </div>

         <div v-else>
          <Button>Профиль</Button>
         </div>
      </div>
    </div>
</template>