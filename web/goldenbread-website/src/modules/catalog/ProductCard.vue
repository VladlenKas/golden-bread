<script setup lang="ts">
import { Card, CardContent } from '@/shared/ui/card';
import { Button } from '@/shared/ui/button';
import { 
  ShoppingCart, 
  Heart, 
  Clock, 
  Package, 
  ImageOff,
  Minus,
  Plus,
  Loader2
} from 'lucide-vue-next';
import { useAuthStore } from '@/modules/auth/stores';
import { useProductCard } from './useProductCard';
import { API_DB_UPLOAD_URL } from '@/shared/constants';
import type { ProductListItem } from './types';

const props = defineProps<ProductListItem>();
const authStore = useAuthStore();
const { 
  isFavorite,
  quantityInCart,
  isLoading,
  incrementCart,
  decrementCart,
  toggleFavoriteStatus,
  goToProductDetail,
} = useProductCard(props);
</script>

<template>
  <Card 
    class="group relative overflow-hidden transition-all hover:shadow-lg cursor-pointer flex flex-col"
    @click="goToProductDetail">

    <!-- Кнопка избранного-->
    <Button
      v-if="authStore.isAuthenticated"
      variant="ghost"
      size="icon"
      class="absolute top-3 right-3 z-10 opacity-0 group-hover:opacity-100 transition-opacity bg-background/80 backdrop-blur-sm"
      @click.stop="toggleFavoriteStatus()">
      <Heart class="w-5 h-5" :class="{ 'fill-current text-red-500': isFavorite }"/>
    </Button>

    <!-- Изображение -->
    <div class="aspect-[4/3] bg-muted relative overflow-hidden shrink-0">
      <img v-if="imageUrl"
        :src="`${API_DB_UPLOAD_URL}/${imageUrl}`"
        class="w-full h-full object-cover transition-transform group-hover:scale-105"/>
      <div v-else 
        class="w-full h-full flex flex-col items-center justify-center gap-2 text-muted-foreground">
        <div class="w-16 h-16 rounded-full bg-background flex items-center justify-center">
          <ImageOff class="w-8 h-8" />
        </div>
        <span class="text-sm">Нет изображения</span>
      </div>
    </div>

    <CardContent class="p-4 flex flex-col flex-1">
      <!-- Название и описание -->
      <div class="mb-3">
        <h3 class="font-semibold text-lg leading-tight line-clamp-1 mb-1">{{ name }}</h3>
        <p class="text-sm text-muted-foreground line-clamp-2">{{ description }}</p>
      </div>

      <!-- Характеристики -->
      <div class="flex items-center justify-between text-sm text-muted-foreground mb-3 py-2 border-y">
        <div class="flex items-center gap-1.5">
          <Clock class="w-4 h-4 shrink-0" />
          <span>{{ productionTimeMinutes }} мин</span>
        </div>
        <div class="flex items-center gap-1.5">
          <Package class="w-4 h-4 shrink-0" />
          <span>{{ quantityPerBatch }} шт/партия</span>
        </div>
      </div>

      <!-- Цена и кнопки -->
      <div class="mt-auto flex items-center justify-between gap-3">
        <div class="min-w-0">
          <p class="text-xl sm:text-2xl font-bold truncate">{{ salePrice.toFixed(2) }} ₽</p>
          <p class="text-xs text-muted-foreground">за 1 шт. в партии</p>
        </div>

        <!-- "В корзину" -->
        <Button 
          v-if="authStore.isAuthenticated && quantityInCart === 0" 
          class="gap-2 shrink-0"
          @click.stop="incrementCart()"
          :disabled="isLoading">
          <ShoppingCart class="w-4 h-4" />
          <Loader2 v-if="isLoading" class="w-4 h-4 mr-2 animate-spin"/>
          <div v-else>
            <span class="hidden sm:inline">В корзину</span>
            <span class="sm:hidden">В корз.</span>
          </div>
        </Button>

        <div v-else-if="authStore.isAuthenticated" class="flex items-center gap-2 shrink-0">
          <!-- + -->
          <Button 
            variant="outline" 
            size="icon" 
            @click.stop="decrementCart()"
            :disabled="isLoading">
            <Loader2 v-if="isLoading" class="w-4 h-4 mr-2 animate-spin"/>
            <Minus v-else class="w-4 h-4 shrink-0" />
          </Button>
          <span class="w-8 text-center font-medium">{{ quantityInCart }}</span>
          <!-- - -->
          <Button 
            variant="outline" 
            size="icon" 
            @click.stop="incrementCart()"
            :disabled="isLoading">
            <Loader2 v-if="isLoading" class="w-4 h-4 mr-2 animate-spin"/>
            <Plus v-else class="w-4 h-4 shrink-0" />
          </Button>
        </div>
      </div>
    </CardContent>
  </Card>
</template>