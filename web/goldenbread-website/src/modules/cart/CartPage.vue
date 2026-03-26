<!-- modules/cart/pages/CartPage.vue -->
<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/ui/card';
import { Button } from '@/shared/ui/button';
import { Checkbox } from '@/shared/ui/checkbox';
import { Badge } from '@/shared/ui/badge';
import { Separator } from '@/shared/ui/separator';
import {
  ShoppingCart,
  ArrowLeft,
  Package,
  Clock,
  Trash2,
  ShoppingBag,
  AlertCircle,
  ChevronRight,
  Sparkles,
  Layers,
  Truck,
  Banknote,
  Layers2,
  Percent,
  Info
} from 'lucide-vue-next';
import { useCart } from './useCart';
import CartItemCard from './CartItemCard.vue';
import { computed, ref, type Ref } from 'vue';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/ui/select';
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
import type { DateValue } from '@internationalized/date'

const {
  items,
  summary,
  toggleSelection,
  removeItem,
  selectAll,
  incrementBatch,
  decrementBatch,
} = useCart();

const selectedTariff = ref('');
const selectedDate = ref(today(getLocalTimeZone())) as Ref<DateValue>
const calendarOpen = ref(false);

const allSelected = computed(() => items.value.length > 0 && items.value.every(i => i.isSelected));
const someSelected = computed(() => items.value.some(i => i.isSelected) && !allSelected.value);

function formatUnit(unit: number): string {
  const lastDigit = unit % 10;
  const lastTwoDigits = unit % 100;

  if (lastTwoDigits >= 11 && lastTwoDigits <= 19) return `${unit} штук`;
  if (lastDigit === 1) return `${unit} штука`;
  if (lastDigit >= 2 && lastDigit <= 4) return `${unit} штуки`;
  return `${unit} штук`;
}
</script>

<template>
  <div class="min-h-screen bg-background">
    <div class="mx-auto max-w-screen-2xl px-4 sm:px-6 lg:px-8 py-6">
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
              @click="items.filter(i => i.isSelected).forEach(i => removeItem(i.cartItemId))">
              <Trash2 class="h-4 w-4" />
              Удалить выбранные
            </Button>
          </div>

          <!-- Карточки товаров -->
          <TransitionGroup name="list" tag="div" class="space-y-4">
            <CartItemCard v-for="item in items" :key="item.cartItemId" :item="item" @toggle-selection="toggleSelection"
              @increment="incrementBatch" @decrement="decrementBatch" @remove="removeItem" />
          </TransitionGroup>
        </div>

        <!-- Правая панель: Итоги -->
        <div class="space-y-6 lg:sticky lg:top-6 h-fit">
          <Card class="border-2 border-primary/10 bg-gradient-to-br from-primary/5 to-transparent shadow-lg">
            <CardContent class="p-6 space-y-5">
              <!-- Выбор тарифа -->
              <div class="space-y-2">
                <Label class="text-sm font-medium">Тариф заказа</Label>
                <Select v-model="selectedTariff">
                  <SelectTrigger class="w-full">
                    <SelectValue placeholder="Выберите тариф" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="standard">
                      <div class="flex items-center justify-between w-full gap-2">
                        <span>Стандарт</span>
                      </div>
                    </SelectItem>
                    <SelectItem value="express">
                      <div class="flex items-center justify-between w-full gap-2">
                        <span class="flex items-center gap-2">
                          Экспресс
                        </span>
                      </div>
                    </SelectItem>
                    <SelectItem value="night">
                      <div class="flex items-center justify-between w-full gap-2">
                        <span class="flex items-center gap-2">
                          Ночной
                        </span>
                      </div>
                    </SelectItem>
                  </SelectContent>
                </Select>
                <p class="flex items-center text-xs text-muted-foreground gap-1">
                  <Info class="w-3 h-3" />
                  Узнать подробнее
                  <span class="text-amber-600 cursor-pointer">здесь</span>
                </p>
              </div>

              <Separator class="bg-primary/10" />

              <!-- Календарь -->
              <div class="space-y-2">
                <Label class="text-sm font-medium">Дата поставки</Label>
                <Popover v-model:open="calendarOpen">
                  <PopoverTrigger as-child>
                    <Button variant="outline" class="w-full justify-between text-left font-normal"
                      :class="{ 'text-muted-foreground': !selectedDate }">
                      {{ selectedDate ? format(selectedDate.toDate(getLocalTimeZone()), 'PPP', { locale: ru }) :
                      'Выберите дату' }}
                      <CalendarIcon class="h-4 w-4 text-muted-foreground opacity-60" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent class="w-auto p-0" align="start">
                    <Calendar v-model="selectedDate" mode="single" :min-value="today(getLocalTimeZone())"
                      initial-focus />
                  </PopoverContent>
                </Popover>
                <p v-if="selectedDate" class="text-xs text-muted-foreground flex items-center gap-1">
                  <Clock class="w-3 h-3" />
                  Доставка с 9:00 до 18:00
                </p>
              </div>

              <Separator class="bg-primary/10" />

              <!-- Детали заказа -->
              <div class="space-y-3">
                <div class="flex items-center justify-between text-sm">
                  <span class=" flex items-center gap-2">
                    <Layers2 class="w-4 h-4" />
                    {{ summary.selectedItems }} позиций, {{ formatUnit(summary.totalUnits) }}
                  </span>
                  <span class="font-semibold">{{ summary.totalPrice.toFixed(2) }} ₽</span>
                </div>

                <div class="flex items-center justify-between text-sm">
                  <span class=" flex items-center gap-2">
                    <Percent class="w-4 h-4" />
                    Наценка за тариф
                  </span>
                  <span class="font-semibold">1 000 ₽</span>
                </div>
              </div>

              <Separator class="bg-primary/10" />

              <!-- Итоговая сумма -->
              <div class="flex items-end justify-between">
                <span class="text-base font-medium">К оплате:</span>
                <div class="flex items-baseline gap-2">
                  <span class="text-4xl font-bold tracking-tight text-foreground">
                    {{ (summary.totalPrice + 1000).toFixed(2) }}
                  </span>
                  <span class="text-xl text-muted-foreground">₽</span>
                </div>
              </div>

              <!-- Кнопка оформления -->
              <Button class="w-full gap-2 h-12 text-base font-semibold shadow-md hover:shadow-lg transition-shadow"
                size="lg" :disabled="summary.selectedItems === 0 || !selectedTariff || !selectedDate">
                <ShoppingBag class="w-5 h-5" />
                Оформить заказ
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