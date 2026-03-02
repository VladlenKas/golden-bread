<script setup lang="ts">
import { onMounted } from 'vue';
import {
  Card,
  CardContent,
} from '@/shared/ui/card';
import {
  Badge
} from '@/shared/ui/badge';
import {
  Button
} from '@/shared/ui/button';
import {
  Separator
} from '@/shared/ui/separator';
import {
  ArrowLeft,
  Heart,
  Clock,
  Package,
  Scale,
  ChefHat,
  ShoppingCart,
  Minus,
  Plus,
  Loader2,
  ChevronLeft,
  ChevronRight,
  ImageOff,
  Check,
  AlertCircle,
  Sparkles,
  ChevronDown,
  ChevronUp,
  Calendar,
  Thermometer,
} from 'lucide-vue-next';
import { TooltipBase } from '@/shared/ui/tooltip'
import { useProductDetail } from './useProductDetail';
import { useAuthStore } from '@/modules/auth/stores';
import { API_DB_UPLOAD_URL } from '@/shared/constants';
import { ref } from 'vue';

const authStore = useAuthStore();
const showAllIngredients = ref(false);

const {
  // State
    product,
    isLoading,
    isUpdating,
    currentImageIndex,
    
    // Computed
    currentBatch,
    quantity,
    totalCost,
    hasInCart,
    hasMultipleImages,
    
    // Methods
    loadProduct,
    changeImage,
    setQuantity,
    increment,
    decrement,
    selectBatch,
    toggleFavorite,
    goBack,
} = useProductDetail();

onMounted(() => {
  loadProduct();
});

function formatUnit(unit: string): string {
  const units: Record<string, string> = {
    'Kg': 'кг',
    'Pcs': 'шт',
    'L': 'л',
    'G': 'г',
    'Ml': 'мл'
  };
  return units[unit] || unit;
}
</script>

<template>
  <div class="min-h-screen bg-background">
    <div class="mx-auto max-w-5xl px-4 sm:px-6 lg:px-8 py-6">

      <!-- Вернуться -->
      <Button variant="ghost" class="gap-2 mb-6 -ml-2 text-muted-foreground hover:text-foreground" @click="goBack">
        <ArrowLeft class="h-4 w-4" />
        Назад к каталогу
      </Button>

      <!-- Загрузка -->
      <div v-if="isLoading" class="flex min-h-[400px] items-center justify-center">
        <Loader2 class="w-8 h-8 animate-spin text-muted-foreground" />
      </div>

      <!-- Контент -->
      <div v-else-if="product" class="space-y-8">

        <!-- Верхняя секция: Изображение + Основная информация -->
        <div class="grid grid-cols-1 lg:grid-cols-[400px_1fr] gap-8">

          <!-- Левая колонка: Галерея -->
          <div class="space-y-4">
            <div class="relative aspect-square bg-muted rounded-2xl overflow-hidden group shadow-sm border">
              <img v-if="product.imageUrls.length > 0"
                :src="`${API_DB_UPLOAD_URL}/${product.imageUrls[currentImageIndex]}`" :alt="product.name"
                class="w-full h-full object-cover" />
              <div v-else class="w-full h-full flex flex-col items-center justify-center gap-2 text-muted-foreground">
                <ImageOff class="w-12 h-12" />
                <span class="text-sm">Нет изображения</span>
              </div>

              <!-- Навигация -->
              <template v-if="hasMultipleImages">
                <Button variant="secondary" size="icon"
                  class="absolute left-3 top-1/2 -translate-y-1/2 h-9 w-9 rounded-full opacity-0 group-hover:opacity-100 transition-all shadow-md"
                  @click.stop="changeImage('prev')">
                  <ChevronLeft class="h-5 w-5" />
                </Button>
                <Button variant="secondary" size="icon"
                  class="absolute right-3 top-1/2 -translate-y-1/2 h-9 w-9 rounded-full opacity-0 group-hover:opacity-100 transition-all shadow-md"
                  @click.stop="changeImage('next')">
                  <ChevronRight class="h-5 w-5" />
                </Button>
              </template>

              <!-- Индикаторы -->
              <div v-if="hasMultipleImages" class="absolute bottom-3 left-1/2 -translate-x-1/2 flex gap-2">
                <button v-for="(_, index) in product.imageUrls" :key="index"
                  class="h-1.5 rounded-full transition-all duration-300"
                  :class="currentImageIndex === index ? 'bg-white w-6' : 'bg-white/40 w-1.5 hover:bg-white/60'"
                  @click.stop="changeImage(index)" />
              </div>
            </div>

            <!-- Миниатюры -->
            <div v-if="hasMultipleImages" class="flex gap-2 justify-center">
              <button v-for="(url, index) in product.imageUrls" :key="index"
                class="w-16 h-16 rounded-lg overflow-hidden border-2 transition-all bg-muted hover:opacity-100"
                :class="currentImageIndex === index ? 'border-primary ring-2 ring-primary/20' : 'border-transparent opacity-60'"
                @click="changeImage(index)">
                <img :src="`${API_DB_UPLOAD_URL}/${url}`" class="w-full h-full object-cover" />
              </button>
            </div>

            <!-- Под миниатюрами, в левой колонке -->
            <div class="space-y-3 pt-4 ">
              <div class="flex items-center justify-between pb-3 border-b">
                <div class="flex items-center gap-2">
                  <div class="p-1.5 rounded-lg bg-primary/10">
                    <ChefHat class="w-4 h-4 text-primary" />
                  </div>
                  <span class="text-sm font-semibold text-foreground">Состав</span>
                </div>
                <Badge variant="secondary" class="text-xs h-6 px-2">{{ product.ingredients.length }}</Badge>
              </div>

              <Card class="overflow-hidden">
                <div class="divide-y divide-border">
                  <div v-for="ingredient in product.ingredients.slice(0, 6)" :key="ingredient.ingredientId"
                    class="flex items-center justify-between py-3 px-4 hover:bg-muted/30 transition-colors">
                    <span class="text-sm font-medium text-foreground">{{ ingredient.name }}</span>
                    <span class="text-sm text-muted-foreground tabular-nums">
                      {{ ingredient.quantity }} {{ formatUnit(ingredient.unit) }}
                    </span>
                  </div>
                </div>

                <div v-show="showAllIngredients" class="divide-y divide-border border-t bg-muted/20">
                  <div v-for="ingredient in product.ingredients.slice(6)" :key="ingredient.ingredientId"
                    class="flex items-center justify-between py-3 px-4 hover:bg-muted/30 transition-colors">
                    <span class="text-sm font-medium text-foreground">{{ ingredient.name }}</span>
                    <span class="text-sm text-muted-foreground tabular-nums">
                      {{ ingredient.quantity }} {{ formatUnit(ingredient.unit) }}
                    </span>
                  </div>
                </div>

                <button v-if="product.ingredients.length > 6" @click="showAllIngredients = !showAllIngredients"
                  class="w-full py-3 px-4 text-sm font-medium text-primary hover:bg-primary/5 transition-colors border-t flex items-center justify-center gap-1">
                  <span v-if="!showAllIngredients">Показать ещё {{ product.ingredients.length - 6 }}</span>
                  <span v-else>Свернуть</span>
                  <ChevronDown v-if="!showAllIngredients" class="w-4 h-4 inline-block" />
                  <ChevronUp v-else class="w-4 h-4 inline-block" />
                </button>
              </Card>

              <p class="text-xs text-muted-foreground/70 leading-relaxed px-1">
                Точный вес может отличаться ±5% в зависимости от сезона и поставщика
              </p>
            </div>
          </div>

          <!-- Правая колонка: Информация -->
          <div class="space-y-6">

            <!-- Заголовок -->
            <div class="space-y-3">
              <div class="flex items-start justify-between gap-4">
                <Badge variant="secondary" class="gap-1.5 font-medium px-3 py-1"
                  :style="{ backgroundColor: '#' + product.categoryColor + '15', color: '#' + product.categoryColor, borderColor: '#' + product.categoryColor + '30' }">
                  <span class="w-2 h-2 rounded-full" :style="{ backgroundColor: '#' + product.categoryColor }" />
                  {{ product.categoryName }}
                </Badge>

                <Button v-if="authStore.isAuthenticated" variant="outline" size="icon" class="shrink-0"
                  :class="{ 'text-red-500 border-red-200 bg-red-50 hover:bg-red-100 hover:text-red-600': product?.isFavorite }"
                  @click="toggleFavorite" :disabled="isUpdating">
                  <Heart class="h-5 w-5" :class="{ 'fill-current': product?.isFavorite }" />
                </Button>
              </div>

              <h1 class="text-3xl font-bold tracking-tight text-foreground">{{ product.name }}</h1>
              <p class="text-muted-foreground text-base leading-relaxed">{{ product.description }}</p>

              <!-- Характеристики с подложкой -->
              <div class="flex items-center gap-x-2 pt-1 text-sm text-muted-foreground">
                <TooltipBase>
                  <template #icon><Clock class="w-4 h-4" /> </template>
                  <template #trigger>{{ product.productionTimeMinutes }} мин</template>
                  <template #content>Время производства</template>
                </TooltipBase>
                <span class="w-1 h-1 rounded-full bg-muted-foreground/40" />
                <TooltipBase>
                  <template #icon><Calendar class="w-4 h-4" /> </template>
                  <template #trigger>{{ product.shelfLifeDays }} дней</template>
                  <template #content>Срок годности при соблюдении условий хранения</template>
                </TooltipBase>
                <span class="w-1 h-1 rounded-full bg-muted-foreground/40" />
                <TooltipBase>
                  <template #icon><Thermometer class="w-4 h-4" /> </template>
                  <template #trigger>{{ `${product.storageTempMin}...${product.storageTempMax}°C` }} </template>
                  <template #content>Нормы температуры хранения</template>
                </TooltipBase>
                <span class="w-1 h-1 rounded-full bg-muted-foreground/40" />
                <TooltipBase>
                  <template #icon><Scale class="w-4 h-4" /> </template>
                  <template #trigger>{{ product.weight }} кг</template>
                  <template #content>Вес</template>
                </TooltipBase>
              </div>
            </div>
            <!-- Спецификации (компактная сетка) -->


            <Separator class="my-6" />

            <!-- Выбор партий -->
            <div class="space-y-4">
              <div class="flex items-center justify-between">
                <h3 class="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Выберите партию</h3>
                <span class="text-xs text-muted-foreground">{{ product.availableBatches.length }} вариантов</span>
              </div>

              <div class="grid grid-cols-2 sm:grid-cols-3 gap-3">
                <button v-for="batch in product.availableBatches" :key="batch.productBatchId"
                  class="relative p-4 rounded-xl border-2 text-left transition-all duration-200 hover:border-primary/50"
                  :class="currentBatch?.productBatchId === batch.productBatchId
                    ? 'border-primary bg-primary/5'
                    : 'border-muted bg-card hover:border-primary/30'" @click="selectBatch(batch)">
                  <div v-if="currentBatch?.productBatchId === batch.productBatchId"
                    class="absolute top-2 right-2 text-primary">
                    <Check class="h-4 w-4" />
                  </div>
                  <div class="space-y-1">
                    <div class="flex items-baseline gap-1">
                      <span class="text-2xl font-bold text-foreground">{{ batch.quantityPerBatch }}</span>
                      <span class="text-sm text-muted-foreground">шт</span>
                    </div>
                    <div class="text-sm font-medium text-primary">{{ batch.unitPrice.toFixed(2) }} ₽/шт</div>
                  </div>
                </button>
              </div>
            </div>

            <!-- Цена и действия -->
            <Card class="border-2 border-primary/10 bg-gradient-to-br from-primary/5 to-transparent shadow-lg">
              <CardContent class="p-6 space-y-4">
                <!-- Ценовой блок -->
                <div class="flex items-end justify-between">
                  <div class="space-y-1">
                    <div class="flex items-center gap-2 text-sm text-muted-foreground">
                      <Package class="w-4 h-4" />
                      Партия из {{ currentBatch?.quantityPerBatch }} шт
                    </div>
                    <div class="flex items-baseline gap-2">
                      <span class="text-4xl font-bold tracking-tight text-foreground">{{ currentBatch?.totalPrice.toFixed(2) }}</span>
                      <span class="text-xl text-muted-foreground">₽</span>
                    </div>
                    <div class="text-sm text-muted-foreground">
                      {{ currentBatch?.unitPrice.toFixed(2) }} ₽ за штуку
                    </div>
                  </div>

                  <div v-if="currentBatch && currentBatch.quantityPerBatch > 10" class="hidden sm:block text-right">
                    <Badge variant="secondary" class="bg-green-100 text-green-700 border-green-200">
                      <Sparkles class="w-3 h-3 mr-1" />
                      Выгодно
                    </Badge>
                  </div>
                </div>

                <Separator class="bg-primary/10" />

                <!-- Кнопки корзины -->
                <div v-if="authStore.isAuthenticated">
                  <Button v-if="!hasInCart"
                    class="w-full gap-2 h-12 text-base font-semibold shadow-md hover:shadow-lg transition-shadow"
                    size="lg" @click="increment" :disabled="isUpdating">
                    <ShoppingCart class="w-5 h-5" />
                    <Loader2 v-if="isUpdating" class="w-5 h-5 animate-spin" />
                    <span v-else>Добавить в корзину</span>
                  </Button>

                  <div v-else class="flex items-center justify-between gap-4 bg-background rounded-lg p-2 border">
                    <div class="flex items-center gap-1">
                      <Button variant="ghost" size="icon" class="h-10 w-10 rounded-md" @click="decrement"
                        :disabled="isUpdating">
                        <Loader2 v-if="isUpdating" class="w-4 h-4 animate-spin" />
                        <Minus v-else class="w-4 h-4" />
                      </Button>
                      <span class="w-12 text-center font-bold text-lg">{{ quantity }}</span>
                      <Button variant="ghost" size="icon" class="h-10 w-10 rounded-md" @click="increment"
                        :disabled="isUpdating">
                        <Loader2 v-if="isUpdating" class="w-4 h-4 animate-spin" />
                        <Plus v-else class="w-4 h-4" />
                      </Button>
                    </div>
                    <div class="pr-3 text-sm">
                      <span class="text-muted-foreground">в корзине</span>
                      <div class="font-semibold">{{ totalCost.toFixed(2) }} ₽</div>
                    </div>
                  </div>
                </div>

                <div v-else>
                  <Button variant="outline" class="w-full h-12 gap-2" disabled>
                    <AlertCircle class="w-4 h-4" />
                    Войдите, чтобы заказать
                  </Button>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>

      <!-- Пустое состояние -->
      <div v-else class="text-center py-16">
        <div class="w-16 h-16 rounded-full bg-muted flex items-center justify-center mx-auto mb-4">
          <Package class="h-8 w-8 text-muted-foreground" />
        </div>
        <h3 class="text-lg font-medium mb-2">Товар не найден</h3>
        <p class="text-muted-foreground mb-4">Возможно, он был удален или перемещен</p>
        <Button @click="goBack">Вернуться к каталогу</Button>
      </div>

    </div>
  </div>
</template>