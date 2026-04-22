<script setup lang="ts">
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle
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
  Building2,
  Package,
  Truck,
  Clock,
  ShieldCheck,
  Users,
  MapPin,
  Phone,
  Mail,
  ChevronRight,
  ArrowRight,
  ChefHat,
  Warehouse,
  BarChart3
} from 'lucide-vue-next';
</script>

<template>
  <div class="min-h-screen bg-background -my-6">
    <!-- Hero Section -->
    <section class="relative overflow-hidden bg-gradient-to-br from-background via-muted/30 to-background border-b">
      <div class="absolute inset-0 pointer-events-none">
        <div class="absolute -top-40 -right-40 w-96 h-96 bg-primary/5 rounded-full blur-3xl" />
        <div class="absolute top-1/2 -left-20 w-72 h-72 bg-accent/20 rounded-full blur-3xl" />
      </div>

      <div class="mx-auto max-w-screen-2xl px-4 sm:px-6 lg:px-8 py-16 lg:py-24 relative z-10">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <div class="space-y-6">
            <Badge variant="secondary"
              class="rounded-full px-4 py-1.5 text-sm font-medium bg-primary/10 text-primary border-0">
              B2B-портал для пекарен и HoReCa
            </Badge>
            <h1 class="text-4xl lg:text-5xl xl:text-6xl font-bold tracking-tight leading-tight">
              Свежий хлеб для вашего бизнеса —
              <span class="text-primary">каждое утро</span>
            </h1>
            <p class="text-lg lg:text-xl text-muted-foreground leading-relaxed max-w-lg">
              Оптовые поставки выпечки ручного производства.
              Автоматизация заказов, точное планирование и гарантия свежести.
            </p>
            <div class="flex flex-col sm:flex-row gap-4 pt-2">
              <Button size="lg" class=" gap-2 h-12 px-8" asChild>
                <RouterLink to="/catalog">
                  Перейти в каталог
                  <ArrowRight class="w-5 h-5" />
                </RouterLink>
              </Button>
              <Button size="lg" variant="outline" class="h-12 px-8" asChild>
                <RouterLink to="/auth">Стать клиентом</RouterLink>
              </Button>
            </div>
          </div>

          <!-- Stats Grid -->
          <div class="grid grid-cols-2 gap-4">
            <Card class="bg-card/80 backdrop-blur-sm border-border/50">
              <CardContent class="p-6">
                <div class="text-3xl lg:text-4xl font-bold text-primary">1100+</div>
                <div class="text-sm text-muted-foreground mt-1">поставок за 2026 год</div>
              </CardContent>
            </Card>
            <Card class="bg-card/80 backdrop-blur-sm border-border/50">
              <CardContent class="p-6">
                <div class="text-3xl lg:text-4xl font-bold text-primary">>4 лет</div>
                <div class="text-sm text-muted-foreground mt-1">работы на рынке</div>
              </CardContent>
            </Card>
            <Card class="bg-card/80 backdrop-blur-sm border-border/50">
              <CardContent class="p-6">
                <div class="text-3xl lg:text-4xl font-bold text-primary">50+</div>
                <div class="text-sm text-muted-foreground mt-1">организаций по всей Уфе</div>
              </CardContent>
            </Card>
            <Card class="bg-card/80 backdrop-blur-sm border-border/50">
              <CardContent class="p-6">
                <div class="text-3xl lg:text-4xl font-bold text-primary">с 6:00 утра</div>
                <div class="text-sm text-muted-foreground mt-1">бесплатная доставка</div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </section>

    <!-- Features -->
    <section class="py-16 lg:py-20">
      <div class="mx-auto max-w-screen-2xl px-4 sm:px-6 lg:px-8">
        <div class="text-center max-w-2xl mx-auto mb-12">
          <h2 class="text-3xl font-bold tracking-tight mb-4">Почему работают с нами</h2>
          <p class="text-muted-foreground text-lg">
            Упрощаем закупки для кафе, ресторанов и магазинов.
            Экономим время и снижаем потери продукции.
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <Card class="border-border/50 hover:border-primary/20 transition-colors">
            <CardContent class="p-6 space-y-4">
              <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center">
                <ChefHat class="w-6 h-6 text-primary" />
              </div>
              <h3 class="font-semibold text-lg">Ручное производство</h3>
              <p class="text-sm text-muted-foreground leading-relaxed">
                Выпечка свежей продукции самыми лучшими пекарями. От замеса теста до выпечки
                изедлия — работа наших мастеров.
              </p>
            </CardContent>
          </Card>

          <Card class="border-border/50 hover:border-primary/20 transition-colors">
            <CardContent class="p-6 space-y-4">
              <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center">
                <Clock class="w-6 h-6 text-primary" />
              </div>
              <h3 class="font-semibold text-lg">Точное планирование</h3>
              <p class="text-sm text-muted-foreground leading-relaxed">
                Заказывайте заранее, получайте свежую выпечку к открытию.
                Никаких простоев и списаний благодаря умной системе.
              </p>
            </CardContent>
          </Card>

          <Card class="border-border/50 hover:border-primary/20 transition-colors">
            <CardContent class="p-6 space-y-4">
              <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center">
                <Warehouse class="w-6 h-6 text-primary" />
              </div>
              <h3 class="font-semibold text-lg">Контроль качества</h3>
              <p class="text-sm text-muted-foreground leading-relaxed">
                Отслеживаем сроки годности, температуру хранения и партийность продукции
                с помощью новейших технологий.
              </p>
            </CardContent>
          </Card>

          <Card class="border-border/50 hover:border-primary/20 transition-colors">
            <CardContent class="p-6 space-y-4">
              <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center">
                <BarChart3 class="w-6 h-6 text-primary" />
              </div>
              <h3 class="font-semibold text-lg">Прозрачная аналитика</h3>
              <p class="text-sm text-muted-foreground leading-relaxed">
                История заказов, статистика расходов и рекомендации по закупкам в личном кабинете.
              </p>
            </CardContent>
          </Card>
        </div>
      </div>
    </section>

    <!-- About Section -->
    <section id="about" class="py-16 lg:py-20 bg-muted/30">
      <div class="mx-auto max-w-screen-2xl px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <div class="space-y-6">
            <h2 class="text-3xl font-bold tracking-tight">О проекте</h2>
            <div class="space-y-4 text-muted-foreground leading-relaxed">
              <p>
                Golden Bread — информационная система, созданная для автоматизации закупок
                в сфере хлебопечения. Мы объединяем небольшие пекарни, которые ценят качество
                и свежесть, с предприятиями HoReCa, которым нужна стабильная поставка.
              </p>
              <p>
                Платформа решает типичные проблемы ручного учёта: перерасход ингредиентов,
                потери продукции из-за неточного планирования, сложности с контролем сроков годности.
                Сквозная автоматизация от заказа до доставки экономит время и снижает издержки.
              </p>
              <p>
                Для пекарен мы предлагаем прямой канал сбыта без посредников.
                Для закупщиков — закрытые оптовые цены и гарантированное утреннее поступление товара.
              </p>
            </div>

            <div class="flex flex-wrap gap-3 pt-2">
              <Badge variant="outline" class="rounded-full px-3 py-1">FIFO-учёт</Badge>
              <Badge variant="outline" class="rounded-full px-3 py-1">Партийное производство</Badge>
              <Badge variant="outline" class="rounded-full px-3 py-1">Интеграция с 1С</Badge>
            </div>
          </div>

          <Card class="bg-card border-border/50 shadow-lg">
            <CardContent class="p-8 space-y-6">
              <h3 class="font-semibold text-lg">Что получают клиенты</h3>
              <ul class="space-y-4">
                <li class="flex items-start gap-3">
                  <ShieldCheck class="w-5 h-5 text-primary shrink-0 mt-0.5" />
                  <span class="text-sm text-muted-foreground">
                    <strong class="text-foreground">Верификация поставщиков.</strong>
                    Работаем только с лицензированными производствами, прошедшими проверку ингредиентов.
                  </span>
                </li>
                <li class="flex items-start gap-3">
                  <Truck class="w-5 h-5 text-primary shrink-0 mt-0.5" />
                  <span class="text-sm text-muted-foreground">
                    <strong class="text-foreground">Гибкая логистика.</strong>
                    Доставка по Уфе и пригороду. Возможен самовывоз с производства.
                  </span>
                </li>
                <li class="flex items-start gap-3">
                  <Users class="w-5 h-5 text-primary shrink-0 mt-0.5" />
                  <span class="text-sm text-muted-foreground">
                    <strong class="text-foreground">Персональный менеджер.</strong>
                    Помощь с первым заказом, консультации по ассортименту и условиям.
                  </span>
                </li>
                <li class="flex items-start gap-3">
                  <Package class="w-5 h-5 text-primary shrink-0 mt-0.5" />
                  <span class="text-sm text-muted-foreground">
                    <strong class="text-foreground">Минимальные партии.</strong>
                    Начните с пробной закупки. Нет жёстких обязательств по объёмам.
                  </span>
                </li>
              </ul>
            </CardContent>
          </Card>
        </div>
      </div>
    </section>

    <!-- Contacts Section -->
    <section id="contacts" class="py-16 lg:py-20">
      <div class="mx-auto max-w-screen-2xl px-4 sm:px-6 lg:px-8">
        <div class="text-center max-w-2xl mx-auto mb-12">
          <h2 class="text-3xl font-bold tracking-tight mb-4">Контакты</h2>
          <p class="text-muted-foreground text-lg">
            Остались вопросы? Свяжитесь с нами — расскажем о системе и поможем с регистрацией.
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 max-w-4xl mx-auto">
          <Card class="border-border/50 hover:border-primary/20 transition-colors">
            <CardContent class="p-6 text-center space-y-4">
              <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center mx-auto">
                <Phone class="w-6 h-6 text-primary" />
              </div>
              <div>
                <h3 class="font-semibold mb-1">Телефон</h3>
                <a href="tel:+79961064304" class="text-muted-foreground hover:text-primary transition-colors">
                  +7 (996) 106-43-04
                </a>
              </div>
            </CardContent>
          </Card>

          <Card class="border-border/50 hover:border-primary/20 transition-colors">
            <CardContent class="p-6 text-center space-y-4">
              <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center mx-auto">
                <Mail class="w-6 h-6 text-primary" />
              </div>
              <div>
                <h3 class="font-semibold mb-1">Email</h3>
                <a href="mailto:vladlenkas06@gmail.com"
                  class="text-muted-foreground hover:text-primary transition-colors">
                  vladlenkas06@gmail.com
                </a>
              </div>
            </CardContent>
          </Card>

          <Card class="border-border/50 hover:border-primary/20 transition-colors">
            <CardContent class="p-6 text-center space-y-4">
              <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center mx-auto">
                <MapPin class="w-6 h-6 text-primary" />
              </div>
              <div>
                <h3 class="font-semibold mb-1">Адрес</h3>
                <p class="text-muted-foreground text-sm">
                  г. Уфа, ул. Кирова 65/2<br />УКСИВТ
                </p>
              </div>
            </CardContent>
          </Card>
        </div>

      </div>
    </section>

    <Separator />
  </div>
</template>