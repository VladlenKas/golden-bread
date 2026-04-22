<script setup lang="ts">
import { Loader2 } from 'lucide-vue-next';

import { parseDate, type DateValue, type CalendarDate } from '@internationalized/date';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/ui/card';
import { Button } from '@/shared/ui/button';
import { Checkbox } from '@/shared/ui/checkbox';
import { Badge } from '@/shared/ui/badge';
import { Separator } from '@/shared/ui/separator';
import {
  ShoppingCart,
  Clock,
  Trash2,
  ShoppingBag,
  AlertCircle,
  Layers2,
} from 'lucide-vue-next';
import { useCart } from './useCart';
import CartItemCard from './CartItemCard.vue';
import { computed, ref, type Ref, onMounted, watch  } from 'vue';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/shared/ui/popover';
import { Calendar } from '@/shared/ui/calendar';
import { Label } from '@/shared/ui/label';
import { Calendar as CalendarIcon, Zap, Moon } from 'lucide-vue-next';
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';
import { getLocalTimeZone, today } from '@internationalized/date'

const {
  items,
  isLoading,
  summary, 
  minimalDeliveryDate,
  maximalDeliveryDate,
  loadCart,
  toggleSelection,
  removeItem,
  selectAll,
  incrementQuantity, 
  decrementQuantity, 
  toggleFavorite,
  isCreatingOrder,
  submitOrder
} = useCart();

onMounted(() => {
  loadCart();
});

const allSelected = computed(() => items.value.length > 0 && items.value.every(i => i.isSelected));
const someSelected = computed(() => items.value.some(i => i.isSelected) && !allSelected.value);

const minDate = computed<DateValue | undefined>(() => {
  if (!minimalDeliveryDate.value) return undefined;
  return parseDate(minimalDeliveryDate.value) as DateValue; 
});

const maxDate = computed<DateValue | undefined>(() => {
  if (!maximalDeliveryDate.value) return undefined;
  return parseDate(maximalDeliveryDate.value) as DateValue; 
});

const formattedDate = computed(() => {
  if (!selectedDate.value) return 'Выберите дату';
  return format(selectedDate.value.toDate(getLocalTimeZone()), 'PPP', { locale: ru });
});

const isCalendarDisabled = computed(() => !minDate.value || !maxDate.value);
const selectedDate = ref<DateValue | undefined>(undefined);

watch([minDate, maxDate], ([newMin, newMax]) => {
  if (newMin && newMax && selectedDate.value) {
    if (selectedDate.value.compare(newMin) < 0 || selectedDate.value.compare(newMax) > 0) {
      selectedDate.value = undefined;
    }
  }
});

watch(isCalendarDisabled, (disabled) => {
  if (disabled && selectedDate.value) {
    selectedDate.value = undefined;
  }
});
</script>

<template>
    <div class="mx-auto max-w-screen-2xl px-4 sm:px-6 lg:px-8">
      <!-- Заголовок -->
      <div class="mb-6 space-y-4">
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <h1 class="text-3xl font-bold tracking-tight">Корзина</h1>
            <p class="text-muted-foreground mt-1">
              {{ items.length }} товаров в корзине
              <span v-if="summary.selectedItems > 0" class="text-foreground font-medium">
                • {{ summary.selectedItems }} выбрано
              </span>
            </p>
          </div>
        </div>
      </div>

      <!-- Основная сетка -->
      <div v-if="items.length > 0" class="grid grid-cols-1 lg:grid-cols-[1fr_330px] gap-6">
        <!-- Список товаров -->
        <div class="space-y-4">
          <!-- Выбрать все -->
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <Checkbox :checked="allSelected" :indeterminate="someSelected" @update:checked="selectAll(!allSelected)"
                id="select-all" />
              <Label for="select-all" class="text-sm font-medium cursor-pointer">
                Выбрать все
              </Label>
            </div>

            <Button v-if="summary.selectedItems > 0" variant="ghost" size="sm"
              class="h-8 gap-1.5 text-destructive hover:text-destructive hover:bg-destructive/10"
              @click="items.filter(i => i.isSelected).forEach(i => removeItem(i.productBatchId, i.productId))">
              <Trash2 class="h-4 w-4" />
              Удалить выбранные
            </Button>
          </div>

          <!-- Карточки товаров -->
          <TransitionGroup name="list" tag="div" class="space-y-4">
            <CartItemCard 
              v-for="item in items" 
              :key="item.productBatchId" 
              :item="item" 
              @toggle-selection="toggleSelection"
              @increment="incrementQuantity" 
              @decrement="decrementQuantity" 
              @remove="removeItem"
              @toggle-favorite="toggleFavorite"
            />
          </TransitionGroup>
        </div>

        <!-- Правая панель: Итоги -->
        <div class="space-y-6 lg:sticky lg:top-6 h-fit">
          <Card class="border-2 border-primary/10 bg-gradient-to-br from-primary/5 to-transparent shadow-lg">
            <CardContent class="p-6 space-y-5">
              <!-- Календарь -->
              <div class="space-y-2">
                <Label class="text-sm font-medium">Дата поставки</Label>
                <Popover>
                  <PopoverTrigger as-child>
                    <Button 
                      variant="outline" 
                      class="w-full justify-between text-left font-normal"
                      :class="{ 'text-muted-foreground': !selectedDate }"
                      :disabled="isCalendarDisabled">
                      {{ formattedDate }}
                      <CalendarIcon class="h-4 w-4 text-muted-foreground opacity-60" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent class="w-auto p-0" align="start">
                     <Calendar 
                        v-model="selectedDate as DateValue"
                        mode="single" 
                        :min-value="minDate"
                        :max-value="maxDate"
                        :disabled="isCalendarDisabled"
                        initial-focus />
                  </PopoverContent>
                </Popover>
                <p v-if="isCalendarDisabled" class="text-xs text-destructive flex items-center gap-1">
                  <AlertCircle class="w-3 h-3" />
                  Слишком большой заказ
                </p>
                <p v-else-if="selectedDate" class="text-xs text-muted-foreground flex items-center gap-1">
                  <Clock class="w-3 h-3" />
                  Доставка с 6:00 до 18:00
                </p>
              </div>

              <Separator class="bg-primary/10" />

              <!-- Детали заказа -->
              <div class="space-y-3">
                <div class="flex items-center justify-between text-sm">
                  <span class="flex items-center gap-2">
                    <Layers2 class="w-4 h-4" />
                    {{ summary.selectedItems }} позиций, {{ summary.totalUnits }} единиц
                  </span>
                  <span class="font-semibold">{{ summary.totalPrice.toFixed(2) }} ₽</span>
                </div>
              </div>

              <Separator class="bg-primary/10" />

              <!-- Итоговая сумма -->
              <div class="flex items-end justify-between">
                <span class="text-base font-medium">К оплате:</span>
                <div class="flex items-baseline gap-2">
                  <span class="text-4xl font-bold tracking-tight text-foreground">
                    {{ (summary.totalPrice).toFixed(2) }}
                  </span>
                  <span class="text-xl text-muted-foreground">₽</span>
                </div>
              </div>

              <!-- Кнопка оформления -->
              <Button 
                class="w-full gap-2 h-12 text-base font-semibold shadow-md hover:shadow-lg transition-shadow"
                size="lg" 
                @click="selectedDate && submitOrder(selectedDate as CalendarDate)"
                :disabled="summary.selectedItems === 0 || !selectedDate || isCreatingOrder">
                <Loader2 v-if="isCreatingOrder" class="w-5 h-5 animate-spin" />
                <ShoppingBag class="w-5 h-5" />
                {{ isCreatingOrder ? 'Производим оплату...' : 'Оформить заказ'}} 
              </Button>
            </CardContent>
          </Card>

          <!-- Помощь -->
          <Card class="bg-muted/50">
            <CardContent class="p-4">
              <p class="text-sm text-muted-foreground">
                Нужна помощь с заказом?
                <RouterLink to="/contacts" class="text-primary hover:underline">
                  Свяжитесь с нами
                </RouterLink>
              </p>
            </CardContent>
          </Card>
        </div>
      </div>

      <!-- Пустая корзина -->
      <div v-else class="text-center py-16">
        <div class="w-24 h-24 rounded-full bg-muted flex items-center justify-center mx-auto mb-6">
          <ShoppingCart class="h-12 w-12 text-muted-foreground" />
        </div>
        <h3 class="text-xl font-semibold mb-2">Корзина пуста</h3>
        <p class="text-muted-foreground mb-6 max-w-md mx-auto">
          Добавьте товары из каталога, чтобы оформить заказ
        </p>
        <Button size="lg" @click="$router.push('/catalog')">
          Перейти в каталог
        </Button>
      </div>
    </div>
</template>

<style scoped>
.list-move,
.list-enter-active,
.list-leave-active {
  transition: all 0.3s ease;
}

.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateX(-30px);
}

.list-leave-active {
  position: absolute;
}
</style>