<script setup lang="ts">
import { Card, CardContent } from '@/shared/ui/card';
import { Button } from '@/shared/ui/button';
import { Checkbox } from '@/shared/ui/checkbox';
import { Badge } from '@/shared/ui/badge';
import { Separator } from '@/shared/ui/separator';
import {
  Package,
  Clock,
  Minus,
  Plus,
  Trash2,
  ImageOff,
  Heart 
} from 'lucide-vue-next';
import { TooltipBase } from '@/shared/ui/tooltip';
import { API_DB_UPLOAD_URL } from '@/shared/constants';
import type { CartItem } from './types';

const props = defineProps<{
  item: CartItem;
  isTogglingFavorite?: boolean;
}>();

const emit = defineEmits<{
  (e: 'toggle-selection', productBatchId: number): void;
  (e: 'increment', productId: number, productBatchId: number): void;
  (e: 'decrement', productId: number, productBatchId: number): void;
  (e: 'remove', productId: number, productBatchId: number): void;
  (e: 'toggle-favorite', productId: number): void;
}>();
</script>

<template>
  <Card 
    class="group relative overflow-hidden transition-all duration-200 hover:shadow-md"
    :class="{ 'opacity-60': !item.isSelected }"
  >
    <CardContent class="p-4">
      <div class="flex gap-4">
        <!-- Чекбокс -->
        <div class="pt-1">
          <Checkbox 
            :checked="item.isSelected" 
            @update:checked="emit('toggle-selection', item.productBatchId)"
            class="h-4 w-4" 
          />
        </div>

        <!-- Изображение (клик по картинке тогглит выбор) -->
        <div
          class="w-24 h-24 rounded-lg overflow-hidden bg-muted border cursor-pointer hover:opacity-90 transition-opacity shrink-0"
          @click="emit('toggle-selection', item.productBatchId)"
        >
          <img 
            v-if="item.imageUrl" 
            :src="`${API_DB_UPLOAD_URL}/${item.imageUrl}`" 
            :alt="item.name"
            class="w-full h-full object-cover" 
          />
          <div v-else class="w-full h-full flex items-center justify-center text-muted-foreground">
            <ImageOff class="w-5 h-5" />
          </div>
        </div>

        <!-- Контент -->
        <div class="flex-1 min-w-0 flex flex-col justify-between">
          <!-- Верх: Название + удаление -->
          <div class="flex items-start justify-between gap-2">
            <h3 class="font-medium text-base leading-tight line-clamp-2">
              {{ item.name }} <!-- Изменено: productName -> name -->
            </h3>

            <div class="flex items-center gap-1">
              <Button 
                variant="ghost" 
                size="icon"
                class="h-7 w-7 text-muted-foreground group-hover:opacity-100 transition-opacity backdrop-blur-sm"
                @click="emit('toggle-favorite', item.productId)"
              >
                <Heart class="h-4 w-4" :class="{ 'fill-current text-red-500': item.isFavorite }" />
              </Button>

              <Button 
                variant="ghost" 
                size="icon"
                class="h-7 w-7 text-muted-foreground hover:text-destructive hover:bg-destructive/10"
                @click="emit('remove', item.productId, item.productBatchId)"
              >
                <Trash2 class="h-3.5 w-3.5" />
              </Button>
            </div>
          </div>

          <!-- Мета-информация -->
          <div class="flex items-center gap-2 text-muted-foreground text-xs">
            <TooltipBase>
              <template #icon>
                <Package class="w-3.5 h-3.5" />
              </template>
              <template #trigger>{{ item.quantityPerBatch }} шт</template>
              <template #content>Количество единиц в партии</template>
            </TooltipBase>

            <span class="text-muted-foreground/30">|</span>

            <TooltipBase>
              <template #icon>
                <Clock class="w-3.5 h-3.5" />
              </template>
              <template #trigger>{{ item.productionTimeMinutes }} мин</template>
              <template #content>Производство</template>
            </TooltipBase>
          </div>

          <!-- Низ: цена и управление количеством -->
          <div class="flex justify-between items-center">
            <!-- Цена -->
            <div class="flex items-baseline gap-1">
              <span class="text-xl font-bold">{{ item.totalCostInCart.toFixed(2) }}</span> <!-- Изменено: totalPrice -> totalCostInCart -->
              <span class="text-sm text-muted-foreground">₽</span>
            </div>

            <!-- Управление количеством -->
            <div class="flex items-center gap-1 bg-muted rounded-md p-0.5">
              <Button 
                variant="ghost" 
                size="icon" 
                class="h-7 w-7 rounded-sm"
                @click="emit('decrement', item.productId, item.productBatchId)" 
                :disabled="item.quantityInCart <= 1" 
              >
                <Minus class="h-3.5 w-3.5" />
              </Button>

              <span class="w-10 text-center font-medium text-sm">{{ item.quantityInCart }}</span> <!-- Изменено: batchCount -> quantityInCart -->

              <Button 
                variant="ghost" 
                size="icon" 
                class="h-7 w-7 rounded-sm"
                @click="emit('increment', item.productId, item.productBatchId)"
              >
                <Plus class="h-3.5 w-3.5" />
              </Button>
            </div>
          </div>
        </div>
      </div>
    </CardContent>
  </Card>
</template>