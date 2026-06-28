<script setup lang="ts">
import { Loader2, Lock, Package } from 'lucide-vue-next';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger
} from '@/shared/ui/alert-dialog';
import {
  Form,
  FormFieldInput
} from '@/shared/ui/form';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter,
  DialogClose,
} from '@/shared/ui/dialog';
import { Label } from '@/shared/ui/label';
import { onMounted, ref, watch } from 'vue';
import {
  User,
  ShoppingCart,
  Heart,
  Building2,
  LogOut,
  Search,
  Filter,
  MoreHorizontal,
  ChevronRight,
  Mail,
  Phone,
  Download,
  FileText
} from 'lucide-vue-next';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/shared/ui/card';
import { Button } from '@/shared/ui/button';
import { Badge } from '@/shared/ui/badge';
import { Input } from '@/shared/ui/input';
import { Separator } from '@/shared/ui/separator';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/shared/ui/table';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/shared/ui/dropdown-menu';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/ui/select';
import ProductCard from '../catalog/ProductCard.vue'
import type {
  ChangeEmailRequest,
  ChangePasswordRequest,
  UpdateContactsRequest,
  UpdateRequisitesRequest
} from './types';
import {
  requisitesSchema,
  contactsSchema,
  changeEmailSchema,
  changePasswordSchema
} from './schemes';
import { useProfile } from './useProfile';
import { useOrders } from './useOrders';
import {
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
} from '@/shared/ui/dropdown-menu';
import { useFavorites } from './useFavorites';
import type { ProductListItem } from '../catalog/types';
import { useDocuments } from './useDocuments';
import { logout } from '../auth/api';

const activeTab = ref<'profile' | 'orders' | 'favorites'>('orders');
const props = defineProps<ProductListItem>();

const {
  favorites,
  isLoading: isFavoritesLoading,
  sortBy: sortByFavorites,
  sortedFavorites,
  fetchFavorites,
  goToProductDetail,
} = useFavorites(props);


const {
  orders,
  isLoading: isOrdersLoading,
  filteredOrders,
  searchQuery,
  statusFilter,
  sortBy,
  statusOptions,
  fetchOrders,
  getStatusLabel,
  getStatusClasses,
  formatDate,
  formatPrice
} = useOrders();

watch(activeTab, (newTab) => {
  if (newTab === 'orders') {
    fetchOrders();
  }
});

watch(activeTab, (newTab) => {
  if (newTab === 'favorites') {
    fetchFavorites();
  }
});

onMounted(() => {
  if (activeTab.value === 'orders') {
    fetchOrders();
  }
});

const {
  isLoading,
  profile,
  loadProfile,
  handleUpdateRequisites,
  handleUpdateContacts,
  handleChangePassword,
  handleChangeEmail,
} = useProfile();

const {
  isDownloadingInvoice,
  isDownloadingAgreement,
  downloadDeliveryInvoice,
  downloadCooperationAgreement,
} = useDocuments();

const isPasswordDialogOpen = ref(false);
const isEmailDialogOpen = ref(false);

const isAlertOpen = ref(false);
const pendingValues = ref<UpdateRequisitesRequest | null>(null);
const pendingSetErrors = ref<((errors: Record<string, string>) => void) | null>(null);

function onSubmitClick(values: UpdateRequisitesRequest, setErrors: (errors: Record<string, string>) => void) {
  pendingValues.value = values;
  pendingSetErrors.value = setErrors;
  isAlertOpen.value = true;
}

async function onConfirm() {
  if (pendingValues.value && pendingSetErrors.value) {
    await handleUpdateRequisites(pendingValues.value, pendingSetErrors.value);
    isAlertOpen.value = false;
    pendingValues.value = null;
    pendingSetErrors.value = null;
  }
}

async function onLogoutConfirm() {
  await logout();
  window.location.reload();
}
</script>

<template>
  <div class="mx-auto max-w-screen-2xl px-4 sm:px-6 lg:px-8">

    <!-- Заголовок страницы -->
    <div class="mb-8">
      <h1 class="text-3xl font-bold tracking-tight">Личный кабинет</h1>
      <p class="text-muted-foreground mt-1">Управление профилем компании и заказами</p>
    </div>

    <!-- Сетка: Sidebar + Content -->
    <div class="grid grid-cols-1 lg:grid-cols-[280px_1fr] gap-6">

      <!-- Левая панель: Навигация -->
      <aside class="space-y-6 lg:block lg:sticky lg:top-24 h-fit"">
          <!-- Карточка профиля -->
          <Card class=" shadow-sm border-border/50">
        <CardContent class="p-6">
          <div class="flex items-center gap-4">
            <div
              class="w-16 h-14 rounded-xl bg-gradient-to-br from-primary/20 to-primary/5 flex items-center justify-center border border-primary/10">
              <Building2 class="w-7 h-7 text-primary" />
            </div>
            <div class="min-w-0">
              <h2 class="font-semibold text-foreground ">{{ profile?.name }}</h2>
              <p class="text-sm text-muted-foreground"> ИНН: {{ profile?.inn }}</p>
            </div>
          </div>

          <Separator class="my-4" />

          <div class="space-y-3 text-sm">
            <div class="flex items-center gap-2 text-muted-foreground">
              <Mail class="w-4 h-4 shrink-0" />
              <span class="truncate"> {{ profile?.email }}</span>
            </div>
            <div class="flex items-center gap-2 text-muted-foreground">
              <Phone class="w-4 h-4 shrink-0" />
              <span>{{ profile?.phone ?? 'Не указан' }}</span>
            </div>
          </div>
        </CardContent>
        </Card>

        <!-- Навигация -->
        <Card class="shadow-sm border-border/50">
          <CardContent class="p-2">
            <nav class="space-y-1">
              <Button variant="ghost" class="w-full justify-start gap-3 h-11 px-3 font-normal"
                :class="{ 'bg-muted/50 hover:bg-muted/50': activeTab === 'profile' }" @click="activeTab = 'profile'">
                <div class="w-8 h-8 rounded-lg flex items-center justify-center"
                  :class="activeTab === 'profile' ? 'bg-primary/10 text-primary' : 'bg-muted text-muted-foreground'">
                  <User class="w-4 h-4" />
                </div>
                <span class="flex-1 text-left">Профиль</span>
                <ChevronRight class="w-4 h-4 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity"
                  :class="{ 'opacity-100': activeTab === 'profile' }" />
              </Button>

              <Button variant="ghost" class="w-full justify-start gap-3 h-11 px-3 font-normal group"
                :class="{ 'bg-muted/50 hover:bg-muted/50': activeTab === 'orders' }" @click="activeTab = 'orders'">
                <div class="w-8 h-8 rounded-lg flex items-center justify-center"
                  :class="activeTab === 'orders' ? 'bg-primary/10 text-primary' : 'bg-muted text-muted-foreground'">
                  <ShoppingCart class="w-4 h-4" />
                </div>
                <span class="flex-1 text-left">Заказы</span>
                <Badge variant="secondary" class="ml-auto font-normal">{{ orders.length ?? 0 }}</Badge>
              </Button>

              <Button variant="ghost" class="w-full justify-start gap-3 h-11 px-3 font-normal"
                :class="{ 'bg-muted/50 hover:bg-muted/50': activeTab === 'favorites' }"
                @click="activeTab = 'favorites'">
                <div class="w-8 h-8 rounded-lg flex items-center justify-center"
                  :class="activeTab === 'favorites' ? 'bg-primary/10 text-primary' : 'bg-muted text-muted-foreground'">
                  <Heart class="w-4 h-4" />
                </div>
                <span class="flex-1 text-left">Избранное</span>
                <ChevronRight class="w-4 h-4 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity"
                  :class="{ 'opacity-100': activeTab === 'favorites' }" />
              </Button>
            </nav>
          </CardContent>
        </Card>

        <!-- Выход -->
        <Card class="shadow-sm border-border/50 bg-muted/30">
          <CardContent class="p-2">
            <AlertDialog>
              <AlertDialogTrigger as-child>
                <Button variant="ghost"
                  class="w-full justify-start gap-3 h-11 px-3 text-destructive hover:text-destructive hover:bg-destructive/10">
                  <div class="w-8 h-8 rounded-lg bg-destructive/10 flex items-center justify-center">
                    <LogOut class="w-4 h-4" />
                  </div>

                  <span>Выйти из системы</span>
                </Button>
              </AlertDialogTrigger>

              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Вы действительно хотите выйти?</AlertDialogTitle>
                  <AlertDialogDescription>
                    После выхода ваша сессия перестанет быть действительной,
                    а печеньки будут полностью очищены.
                    Все остальные данные сохранятся без изменений.
                  </AlertDialogDescription>
                </AlertDialogHeader>

                <AlertDialogFooter>
                  <AlertDialogCancel>Отмена</AlertDialogCancel>
                  <AlertDialogAction @click="onLogoutConfirm">
                    Подтвердить
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          </CardContent>
        </Card>
      </aside>

      <!-- Основной контент -->
      <main class="min-w-0 space-y-6">

        <!-- Вкладка: Заказы -->
        <div v-if="activeTab === 'orders'" class="space-y-6">
          <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
            <div>
              <h2 class="text-2xl font-bold tracking-tight">История заказов</h2>
              <p class="text-muted-foreground mt-1">Управление вашими заказами и отслеживание статусов</p>
            </div>
          </div>

          <!-- Панель поиска и фильтров -->
          <Card class="shadow-sm border-border/50">
            <CardContent class="p-4">
              <div class="flex flex-col lg:flex-row gap-4">
                <!-- Поиск -->
                <div class="relative flex-1 max-w-md">
                  <Search class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input v-model="searchQuery" placeholder="Поиск по номеру заказа..." class="pl-9 h-11" />
                </div>

                <div class="flex flex-col sm:flex-row gap-3 lg:ml-auto">
                  <!-- Фильтр по статусу -->
                  <DropdownMenu>
                    <DropdownMenuTrigger as-child>
                      <Button variant="outline" class="h-11 gap-2 px-4">
                        <Filter class="w-4 h-4" />
                        <span>
                          {{ statusFilter === 'all' ? 'Фильтры' : getStatusLabel(statusFilter) }}
                        </span>
                        <Badge v-if="statusFilter !== 'all'" variant="secondary" class="ml-1 h-5 px-1.5">
                          1
                        </Badge>
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent class="w-56">
                      <DropdownMenuLabel>Фильтр по статусу</DropdownMenuLabel>
                      <DropdownMenuSeparator />
                      <DropdownMenuRadioGroup v-model="statusFilter">
                        <DropdownMenuRadioItem v-for="option in statusOptions" :key="option.value"
                          :value="option.value">
                          <span class="flex items-center gap-2">
                            {{ option.label }}
                          </span>
                        </DropdownMenuRadioItem>
                      </DropdownMenuRadioGroup>
                      <DropdownMenuSeparator v-if="statusFilter !== 'all'" />
                      <DropdownMenuItem v-if="statusFilter !== 'all'"
                        class="text-destructive focus:text-destructive cursor-pointer" @click="statusFilter = 'all'">
                        Сбросить фильтр
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>

                  <!-- Сортировка -->
                  <Select v-model="sortBy">
                    <SelectTrigger class="h-11 w-full sm:w-[180px]">
                      <SelectValue placeholder="Сортировка" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="newest">Сначала новые</SelectItem>
                      <SelectItem value="oldest">Сначала старые</SelectItem>
                      <SelectItem value="expensive">Дороже</SelectItem>
                      <SelectItem value="cheap">Дешевле</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
            </CardContent>
          </Card>

          <!-- Состояние загрузки -->
          <Card v-if="isOrdersLoading" class="shadow-sm border-border/50">
            <CardContent class="p-12 flex justify-center">
              <Loader2 class="w-8 h-8 animate-spin text-muted-foreground" />
            </CardContent>
          </Card>

          <!-- Таблица заказов -->
          <Card v-else-if="filteredOrders.length > 0" class="shadow-sm border-border/50 overflow-hidden">
            <Table>
              <TableHeader>
                <TableRow class="bg-muted/40 hover:bg-muted/40 border-b">
                  <TableHead class="h-12 px-6 font-semibold text-foreground">№ заказа</TableHead>
                  <TableHead class="h-12 px-6 font-semibold text-foreground">Дата создания</TableHead>
                  <TableHead class="h-12 px-6 font-semibold text-foreground">Доставка до</TableHead>
                  <TableHead class="h-12 px-6 font-semibold text-foreground">Статус</TableHead>
                  <TableHead class="h-12 px-6 font-semibold text-foreground text-right">Позиций</TableHead>
                  <TableHead class="h-12 px-6 font-semibold text-foreground text-right">Сумма</TableHead>
                  <TableHead class="h-12 w-16 px-4"></TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                <TableRow v-for="order in filteredOrders" :key="order.orderId"
                  class="border-b last:border-b-0 hover:bg-muted/20 transition-colors">
                  <TableCell class="h-16 px-6">
                    <span class="font-medium text-foreground">#{{ order.orderId }}</span>
                  </TableCell>
                  <TableCell class="h-16 px-6 text-muted-foreground">
                    {{ formatDate(order.createdAt) }}
                  </TableCell>
                  <TableCell class="h-16 px-6 text-muted-foreground">
                    {{ formatDate(order.endDate) }}
                  </TableCell>
                  <TableCell class="h-16 px-6">
                    <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium"
                      :class="getStatusClasses(order.status)">
                      {{ getStatusLabel(order.status) }}
                    </span>
                  </TableCell>
                  <TableCell class="h-16 px-6 text-right text-muted-foreground">
                    {{ order.quantityOrderItems }} шт.
                  </TableCell>
                  <TableCell class="h-16 px-6 text-right">
                    <span class="font-semibold text-foreground">{{ formatPrice(order.totalAmount) }}</span>
                  </TableCell>
                  <TableCell class="h-16 px-4">
                    <DropdownMenu>
                      <DropdownMenuTrigger as-child>
                        <Button variant="ghost" size="icon" class="h-9 w-9">
                          <MoreHorizontal class="w-5 h-5" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end" class="w-40">
                        <DropdownMenuItem class="cursor-pointer" :disabled="isDownloadingInvoice === order.orderId"
                          @click="downloadDeliveryInvoice(order.orderId)">
                          <span class="flex items-center gap-2">
                            {{ isDownloadingInvoice === order.orderId ? 'Загрузка...' : 'Скачать накладную' }}
                          </span>
                        </DropdownMenuItem>
                        <DropdownMenuItem v-if="['pending', 'processing'].includes(order.status)"
                          class="cursor-pointer text-destructive focus:text-destructive">
                          Отменить
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </Card>

          <!-- Empty State -->
          <Card v-else class="shadow-sm border-border/50">
            <CardContent class="p-12 text-center">
              <div class="w-20 h-20 rounded-full bg-muted flex items-center justify-center mx-auto mb-6">
                <Package class="w-10 h-10 text-muted-foreground" />
              </div>
              <h3 class="text-xl font-semibold text-foreground mb-2">У вас пока нет заказов</h3>
              <p class="text-muted-foreground max-w-md mx-auto mb-6">
                Сделайте свой первый заказ из каталога продукции, и он отобразится здесь
              </p>
              <Button size="default" class="h-11 px-6 gap-2">
                <Package class="w-4 h-4" />
                Перейти в каталог
              </Button>
            </CardContent>
          </Card>
        </div>

        <!-- Вкладка: Профиль -->
        <div v-else-if="activeTab === 'profile'" class="space-y-6">
          <div class="flex items-center justify-between">
            <div>
              <h2 class="text-2xl font-bold tracking-tight">Профиль компании</h2>
              <p class="text-muted-foreground mt-1">Управление реквизитами и контактными данными</p>
            </div>
          </div>

          <div class="flex items-center justify-center">

            <div v-if="isLoading">
              <Loader2 class="w-8 h-8 animate-spin text-muted-foreground" />
            </div>

            <div v-else-if="profile" class="w-full space-y-6">

              <!-- Карточка: Основные данные -->
              <Card>
                <CardHeader>
                  <div class="flex items-center gap-2">
                    <Building2 class="w-5 h-5 text-muted-foreground" />
                    <CardTitle class="text-xl">Основные данные</CardTitle>
                  </div>
                  <CardDescription>
                    Юридическая информация о вашей компании и доступ к аккаунту
                  </CardDescription>
                </CardHeader>

                <CardContent>
                  <Form v-slot="{ handleSubmit, isSubmitting, setErrors }" as="" keep-values :initial-values="profile"
                    :validation-schema="requisitesSchema">
                    <form class="space-y-4"
                      @submit="handleSubmit($event, values => onSubmitClick(values as UpdateRequisitesRequest, setErrors))">
                      <div class="flex items-end gap-2">

                        <div class="flex-1 space-y-2">
                          <Label class="text-sm font-medium leading-none">Email</Label>
                          <Input disabled :model-value="profile.email" />
                        </div>

                        <!-- Диалог: смена email -->
                        <Dialog v-model:open="isEmailDialogOpen">
                          <DialogTrigger as-child>
                            <Button type="button" variant="outline" size="icon">
                              <Mail />
                            </Button>
                          </DialogTrigger>
                          <DialogContent class="sm:max-w-[425px]">
                            <DialogHeader>
                              <DialogTitle>Изменить email</DialogTitle>
                              <DialogDescription>
                                Введите новый адрес электронной почты.
                                После этой операции необходимо будет повторно войти в систему
                              </DialogDescription>
                            </DialogHeader>

                            <Form v-slot="{ handleSubmit, isSubmitting, setErrors }" as="" keep-values
                              :validation-schema="changeEmailSchema">
                              <form class="space-y-4 py-4"
                                @submit="handleSubmit($event, values => handleChangeEmail(values as ChangeEmailRequest, setErrors))">

                                <FormFieldInput name="newEmail" label="Email"
                                  placeholder="Пример: newemail@example.com" />
                                <FormFieldInput name="password" label="Текущий пароль (для подтверждения)"
                                  type="password" placeholder="••••••••" />

                                <DialogFooter>
                                  <DialogClose as-child>
                                    <Button type="button" variant="outline">Отмена</Button>
                                  </DialogClose>
                                  <Button type="submit" :disabled="isSubmitting">
                                    <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
                                    Сменить email
                                  </Button>
                                </DialogFooter>
                              </form>
                            </Form>
                          </DialogContent>
                        </Dialog>

                        <!-- Диалог: смена пароля -->
                        <Dialog v-model:open="isPasswordDialogOpen">
                          <DialogTrigger as-child>
                            <Button type="button" variant="outline" size="icon" class="shrink-0">
                              <Lock />
                            </Button>
                          </DialogTrigger>
                          <DialogContent class="sm:max-w-[425px]">
                            <DialogHeader>
                              <DialogTitle>Сменить пароль</DialogTitle>
                              <DialogDescription>
                                Для безопасности введите текущий пароль, затем укажите новый.
                              </DialogDescription>
                            </DialogHeader>

                            <Form v-slot="{ handleSubmit, isSubmitting, setErrors }" as="" keep-values
                              :validation-schema="changePasswordSchema">
                              <form class="space-y-4 py-4"
                                @submit="handleSubmit($event, values => handleChangePassword(values as ChangePasswordRequest, setErrors))">

                                <FormFieldInput name="oldPassword" label="Текущий пароль" type="password"
                                  placeholder="••••••••" />
                                <FormFieldInput name="newPassword" label="Новый пароль" type="password"
                                  placeholder="••••••••" />
                                <FormFieldInput name="confirmNewPassword" label="Подтвердите новый пароль"
                                  type="password" placeholder="••••••••" />

                                <DialogFooter>
                                  <DialogClose as-child>
                                    <Button type="button" variant="outline">Отмена</Button>
                                  </DialogClose>
                                  <Button type="submit" :disabled="isSubmitting">
                                    <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
                                    Обновить пароль
                                  </Button>
                                </DialogFooter>
                              </form>
                            </Form>
                          </DialogContent>
                        </Dialog>
                      </div>

                      <!-- Карточка: основные данные -->
                      <FormFieldInput name="name" label="Название компании" placeholder="Golden Bread" />
                      <div class="grid grid-cols-2 gap-4">
                        <FormFieldInput name="inn" label="ИНН" placeholder="10 цифр" />
                        <FormFieldInput name="ogrn" label="ОГРН" placeholder="13 цифр" />
                      </div>

                      <div class="pt-2">
                        <AlertDialog v-model:open="isAlertOpen">
                          <AlertDialogTrigger as-child>
                            <Button type="submit" :disabled="isSubmitting" class="w-full sm:w-auto">
                              <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
                              Сохранить изменения
                            </Button>
                          </AlertDialogTrigger>
                          <AlertDialogContent>
                            <AlertDialogHeader>
                              <AlertDialogTitle>Подтвердите изменение реквизитов</AlertDialogTitle>
                              <AlertDialogDescription>
                                После сохранения изменений ваш аккаунт будет отправлен на повторную проверку.
                                Доступ к профилю и оформлению заказов будет временно ограничен
                              </AlertDialogDescription>
                            </AlertDialogHeader>
                            <AlertDialogFooter>
                              <AlertDialogCancel @click="pendingValues = null; pendingSetErrors = null">
                                Отмена
                              </AlertDialogCancel>
                              <AlertDialogAction @click="onConfirm">Подтвердить</AlertDialogAction>
                            </AlertDialogFooter>
                          </AlertDialogContent>
                        </AlertDialog>
                      </div>
                    </form>
                  </Form>
                </CardContent>
              </Card>

              <!-- Карточка: Контактные данные -->
              <Card>
                <CardHeader>
                  <div class="flex items-center gap-2">
                    <Phone class="w-5 h-5 text-muted-foreground" />
                    <CardTitle class="text-xl">Контактные данные</CardTitle>
                  </div>
                  <CardDescription>
                    Способы связи с вашей компанией
                  </CardDescription>
                </CardHeader>

                <CardContent>
                  <Form v-slot="{ handleSubmit, isSubmitting, setErrors }" as="" keep-values :initial-values="profile"
                    :validation-schema="contactsSchema">
                    <form class="space-y-4"
                      @submit="handleSubmit($event, values => handleUpdateContacts(values as UpdateContactsRequest, setErrors))">

                      <FormFieldInput name="phone" label="Номер телефона" type="phone"
                        placeholder="Пример: 89990000000" />
                      <FormFieldInput name="address" label="Адрес"
                        placeholder="Пример: г. Москва, ул. Примерная, д. 1" />

                      <div class="pt-2">
                        <Button type="submit" :disabled="isSubmitting" class="w-full sm:w-auto">
                          <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
                          Сохранить изменения
                        </Button>
                      </div>
                    </form>
                  </Form>
                </CardContent>
              </Card>

              <!-- Карточка: Договор -->
              <Card>
                <CardHeader>
                  <div class="flex items-center gap-2">
                    <FileText class="w-5 h-5 text-muted-foreground" />
                    <CardTitle class="text-xl">Документы</CardTitle>
                  </div>
                  <CardDescription>
                    Условия сотрудничества с ООО «GoldenBread»
                  </CardDescription>
                </CardHeader>

                <CardContent>
                  <Button :disabled="isDownloadingAgreement" @click="downloadCooperationAgreement">
                    <Loader2 v-if="isDownloadingAgreement" class="w-4 h-4 mr-2 animate-spin" />
                    <Download v-else class="w-4 h-4 mr-2" />
                    {{ isDownloadingAgreement ? 'Формирование...' : 'Скачать' }}
                  </Button>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>

        <!-- Вкладка: Избранное -->
        <div v-else-if="activeTab === 'favorites'" class="space-y-6">
          <div class="flex items-center justify-between">
            <div>
              <h2 class="text-2xl font-bold tracking-tight">Избранное</h2>
              <p class="text-muted-foreground mt-1">
                {{ favorites.length }} {{ 'товаров' }}
              </p>
            </div>

            <!-- Опционально: сортировка -->
            <Select v-model="sortByFavorites" v-if="favorites.length > 1">
              <SelectTrigger class="w-[180px]">
                <SelectValue placeholder="Сортировка" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="newest">Сначала новые</SelectItem>
                <SelectItem value="price-asc">Дешевле</SelectItem>
                <SelectItem value="price-desc">Дороже</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <!-- Загрузка -->
          <div v-if="isFavoritesLoading" class="flex min-h-[300px] items-center justify-center">
            <Loader2 class="w-8 h-8 animate-spin text-muted-foreground" />
          </div>

          <!-- Сетка избранного -->
          <div v-else-if="favorites.length > 0" class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4">
            <ProductCard v-for="product in sortedFavorites" v-bind="product" :key="product.productId" />
          </div>

          <!-- Empty State -->
          <Card v-else class="shadow-sm border-border/50">
            <CardContent class="p-12 text-center">
              <div class="w-16 h-16 rounded-full bg-muted flex items-center justify-center mx-auto mb-4">
                <Heart class="w-8 h-8 text-muted-foreground" />
              </div>
              <h3 class="text-lg font-medium mb-2">Список избранного пуст</h3>
              <p class="text-muted-foreground mb-6">
                Добавляйте товары в избранное, чтобы быстро находить их здесь
              </p>
              <Button @click="$router.push('/catalog')">
                Перейти в каталог
              </Button>
            </CardContent>
          </Card>
        </div>

      </main>
    </div>
  </div>
</template>