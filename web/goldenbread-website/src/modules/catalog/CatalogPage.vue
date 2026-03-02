<script setup lang="ts">
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from '@/shared/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/ui/select';
import {
  Slider
} from '@/shared/ui/slider';
import {
  Badge
} from '@/shared/ui/badge';
import {
  Button
} from '@/shared/ui/button';
import {
  Checkbox
} from '@/shared/ui/checkbox';
import {
  Label
} from '@/shared/ui/label';
import {
  Input
} from '@/shared/ui/input';
import {
  Separator
} from '@/shared/ui/separator';
import {
  Search,
  Filter,
  X,
  Clock,
  Package,
  ChevronRight,
  Loader2
} from 'lucide-vue-next';
import { useCatalog } from './useCatalog';
import ProductCard from './ProductCard.vue'

const {
  isLoading,
  timeOptions,
  products,
  categories,
  groupedProducts,
  filteredProducts,
  selectedCategory,
  searchQuery,
  priceRange,
  selectedProductionTime,
  sortBy,
  activeFiltersCount,
  resetFilters,
  toggleTimeFilter,
} = useCatalog();


</script>

<template>
  <div class="min-h-screen bg-background">
    <div class="mx-auto max-w-screen-2xl px-4 sm:px-6 lg:px-8 py-6">

      <!-- Заголовок и поиск -->
      <div class="mb-6 space-y-4">
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <h1 class="text-3xl font-bold tracking-tight">Каталог продукции</h1>
            <p class="text-muted-foreground mt-1">
              {{ filteredProducts.length }} товаров доступно
            </p>
          </div>

          <!-- Поиск и сортировка -->
          <div class="flex flex-col sm:flex-row gap-3">
            <div class="relative">
              <Search class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input v-model="searchQuery" placeholder="Поиск по названию..." class="pl-9 w-full sm:w-[280px]" />
            </div>

            <Select v-model="sortBy">
              <SelectTrigger class="w-full sm:w-[180px]">
                <SelectValue placeholder="Сортировка" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="name">По названию</SelectItem>
                <SelectItem value="price-asc">Цена: по возрастанию</SelectItem>
                <SelectItem value="price-desc">Цена: по убыванию</SelectItem>
                <SelectItem value="time">Время приготовления</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>
      </div>

      <!-- Сетка страницы -->
      <div class="grid grid-cols-1 lg:grid-cols-[240px_1fr_280px] gap-6">

        <!-- Левая панель: Навигация по категориям -->
        <aside class="space-y-6 lg:block">
          <Card>
            <CardHeader class="pb-3">
              <CardTitle class="text-base flex items-center gap-2">
                <Package class="h-4 w-4" />
                Категории
              </CardTitle>
            </CardHeader>
            <CardContent class="pt-0">
              <div class="space-y-1">
                <Button variant="ghost" class="w-full justify-start gap-2 font-normal"
                  :class="{ 'bg-muted': selectedCategory === null }" @click="selectedCategory = null">
                  <span class="flex-1 text-left">Все категории</span>
                  <Badge variant="secondary">{{ products.length }}</Badge>
                </Button>

                <Button v-for="category in categories" :key="category.productCategoryId" variant="ghost"
                  class="w-full justify-start gap-2 font-normal"
                  :class="{ 'bg-muted': selectedCategory === category.productCategoryId }"
                  @click="selectedCategory = category.productCategoryId">
                  <span class="w-3 h-3 rounded-full" :style="{ backgroundColor: '#' + category.color }" />
                  <span class="flex-1 text-left">{{ category.name }}</span>
                  <Badge variant="secondary">{{ category.productCount }}</Badge>
                </Button>
              </div>
            </CardContent>
          </Card>

          <!-- Быстрые ссылки -->
          <Card>
            <CardHeader class="pb-3">
              <CardTitle class="text-base">Быстрый доступ</CardTitle>
            </CardHeader>
            <CardContent class="pt-0">
              <div class="space-y-2 text-sm">
                <RouterLink to="/cart"
                  class="flex items-center gap-2 text-muted-foreground hover:text-foreground transition-colors">
                  <ChevronRight class="h-4 w-4" />
                  Перейти в корзину
                </RouterLink>
                <RouterLink to="/about"
                  class="flex items-center gap-2 text-muted-foreground hover:text-foreground transition-colors">
                  <ChevronRight class="h-4 w-4" />
                  О компании
                </RouterLink>
              </div>
            </CardContent>
          </Card>
        </aside>

        <!-- Центральная зона: Контент -->

         <!-- Загрузка -->
        <div v-if="isLoading" class="flex min-h-[400px] items-center justify-center">
          <Loader2 class="w-8 h-8 animate-spin text-muted-foreground" />
        </div>

        <!-- Продукция -->
        <main v-else-if="products" class="min-w-0">
          <!-- Активные фильтры -->
          <div v-if="activeFiltersCount > 0" class="mb-4 flex flex-wrap items-center gap-2">
            <span class="text-sm text-muted-foreground">Активные фильтры:</span>
            <Badge v-if="selectedCategory" variant="secondary" class="gap-1 cursor-pointer hover:bg-muted-foreground/20"
              @click="selectedCategory = null">
              {{categories.find(c => c.productCategoryId === selectedCategory)?.name}}
              <X class="h-3 w-3" />
            </Badge>
            <Badge v-if="searchQuery" variant="secondary" class="gap-1 cursor-pointer hover:bg-muted-foreground/20"
              @click="searchQuery = ''">
              Поиск: {{ searchQuery }}
              <X class="h-3 w-3" />
            </Badge>
            <Button variant="ghost" size="sm" class="h-6 px-2 text-xs" @click="resetFilters">
              Сбросить все
            </Button>
          </div>

          <!-- Группировка по категориям -->
          <div v-if="groupedProducts.length > 0" class="space-y-8">
            <section v-for="group in groupedProducts" :key="group.category.id" class="space-y-4">
              <!-- Заголовок категории -->
              <div class="flex items-center gap-3 pb-2 border-b">
                <span class="w-4 h-4 rounded-full" :style="{ backgroundColor: '#' + group.category.color }" />
                <h2 class="text-xl font-semibold">{{ group.category.name }}</h2>
                <Badge variant="outline">{{ group.items.length }}</Badge>
              </div>

              <!-- Отображение товаров -->
              <div class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4">
                <ProductCard v-for="product in group.items" v-bind="product" :key="product.productId" />
              </div>
            </section>
          </div>

          <!-- "Ничего не найдено" -->
          <div v-else class="text-center py-12">
            <div class="w-16 h-16 rounded-full bg-muted flex items-center justify-center mx-auto mb-4">
              <Search class="h-8 w-8 text-muted-foreground" />
            </div>
            <h3 class="text-lg font-medium mb-2">Товары не найдены</h3>
            <p class="text-muted-foreground mb-4">Попробуйте изменить параметры фильтрации</p>
            <Button @click="resetFilters">Сбросить фильтры</Button>
          </div>
        </main>

        <!-- Правая панель: Фильтры -->
        <aside class="space-y-6 lg:block">
          <Card>
            <CardHeader class="pb-3">
              <div class="flex items-center justify-between">
                <CardTitle class="text-base flex items-center gap-2">
                  <Filter class="h-4 w-4" />
                  Фильтры
                </CardTitle>
                <Button v-if="activeFiltersCount > 0" variant="ghost" size="sm" @click="resetFilters">
                  Сбросить
                </Button>
              </div>
            </CardHeader>
            <CardContent class="pt-0 space-y-6">

              <!-- Фильтр по цене -->
              <div class="space-y-3">
                <Label class="text-sm font-medium">Цена, ₽</Label>
                <div class="px-2">
                  <Slider v-model="priceRange" :min="0" :max="1000" :step="10" class="w-full" />
                </div>
                <div class="flex items-center justify-between text-sm text-muted-foreground">
                  <span>{{ priceRange[0] }} ₽</span>
                  <span>{{ priceRange[1] }} ₽</span>
                </div>
              </div>

              <Separator />

              <!-- Фильтр по времени приготовления -->
              <div class="space-y-3">
                <Label class="text-sm font-medium flex items-center gap-2">
                  <Clock class="h-4 w-4" />
                  Время приготовления
                </Label>
                <div class="space-y-2">
                  <div v-for="option in timeOptions" :key="option.value" class="flex items-center space-x-2">
                    <Checkbox :id="option.value" :checked="selectedProductionTime.includes(option.value)"
                      @update:checked="(checked) => toggleTimeFilter(option.value, checked)" />
                    <Label :for="option.value" class="text-sm font-normal cursor-pointer">
                      {{ option.label }}
                    </Label>
                  </div>
                </div>
              </div>

              <Separator />

              <!-- Дополнительные опции -->
              <div class="space-y-3">
                <Label class="text-sm font-medium">Дополнительно</Label>
                <div class="space-y-2">
                  <div class="flex items-center space-x-2">
                    <Checkbox id="in-stock" />
                    <Label for="in-stock" class="text-sm font-normal cursor-pointer">
                      В наличии
                    </Label>
                  </div>
                  <div class="flex items-center space-x-2">
                    <Checkbox id="new" />
                    <Label for="new" class="text-sm font-normal cursor-pointer">
                      Новинки
                    </Label>
                  </div>
                </div>
              </div>

            </CardContent>
          </Card>

          <!-- Помощь -->
          <Card class="bg-muted/50">
            <CardContent class="p-4">
              <p class="text-sm text-muted-foreground">
                Нужна помощь с выбором?
                <RouterLink to="/contacts" class="text-primary hover:underline">
                  Свяжитесь с нами
                </RouterLink>
              </p>
            </CardContent>
          </Card>
        </aside>

      </div>
    </div>
  </div>
</template>