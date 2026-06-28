<script setup lang="ts">
import { Mail, Phone, MapPin, Github, Send } from 'lucide-vue-next';
import { Separator } from '@/shared/ui/separator';

const currentYear = new Date().getFullYear();

const companyLinks = [
  { name: 'Главная', path: '/home' },
  { name: 'Каталог', path: '/catalog' },
  { name: 'О нас', path: '/home#about' },
  { name: 'Контакты', path: '/home#contacts' },
];

const accountLinks = [
  { name: 'Личный кабинет', path: '/profile' },
  { name: 'Корзина', path: '/cart' },
];

const contacts = [
  { icon: Phone, text: '+7 (996) 106-43-04', href: 'tel:+79961064304' },
  { icon: Mail, text: 'vladlenkas06@gmail.com', href: 'mailto:vladlenkas06@gmail.com' },
  { icon: MapPin, text: 'г. Уфа, ул. Кирова 65/2, УКСИВТ' },
];

const socials = [
  { icon: Github, href: 'https://github.com/VladlenKas', label: 'GitHub' },
  { icon: Send, href: 'https://t.me/vladlendev', label: 'Telegram' },
];
</script>

<template>
  <footer class="border-t bg-background">
    <div class="mx-auto max-w-[1400px] px-4 sm:px-6 lg:px-8 py-12 lg:py-16">
      
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8 lg:gap-8">
        
        <!-- Бренд -->
        <div class="sm:col-span-2 lg:col-span-1">
          <RouterLink to="/home" class="flex items-center gap-3">
            <img src="/src/shared/assets/logo.ico" class="h-10 w-10"/>
            <div>
              <span class="font-bold text-lg">Golden Bread</span>
              <p class="text-xs text-muted-foreground">Оптовые поставки</p>
            </div>
          </RouterLink>
          <p class="mt-4 text-sm text-muted-foreground leading-relaxed max-w-xs">
            Свежая выпечка и хлебобулочные изделия для кафе, ресторанов и магазинов.
            Сайт для компании разработан студентом и программистом Владленом.
          </p>
        </div>

        <!-- Компания -->
        <div class="lg:pl-4">
          <h3 class="font-semibold text-sm tracking-wide uppercase text-foreground">Компания</h3>
          <ul class="mt-4 space-y-3">
            <li v-for="link in companyLinks" :key="link.name">
              <RouterLink 
                :to="link.path"
                class="text-sm text-muted-foreground hover:text-foreground transition-colors">
                {{ link.name }}
              </RouterLink>
            </li>
          </ul>
        </div>

        <!-- Клиентам -->
        <div class="lg:pl-4">
          <h3 class="font-semibold text-sm tracking-wide uppercase text-foreground">Клиентам</h3>
          <ul class="mt-4 space-y-3">
            <li v-for="link in accountLinks" :key="link.name">
              <RouterLink 
                :to="link.path"
                class="text-sm text-muted-foreground hover:text-foreground transition-colors">
                {{ link.name }}
              </RouterLink>
            </li>
          </ul>
        </div>

        <!-- Контакты -->
        <div class="lg:pl-4">
          <h3 class="font-semibold text-sm tracking-wide uppercase text-foreground">Контакты</h3>
          <ul class="mt-4 space-y-3">
            <li v-for="contact in contacts" :key="contact.text">
              <component 
                :is="contact.href ? 'a' : 'span'"
                :href="contact.href"
                class="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
                :class="{ 'cursor-default': !contact.href }"
              >
                <component :is="contact.icon" class="w-4 h-4 shrink-0" />
                <span>{{ contact.text }}</span>
              </component>
            </li>
          </ul>
          
          <div class="mt-6 flex items-center gap-2">
            <a 
              v-for="social in socials"
              :key="social.label"
              :href="social.href"
              target="_blank"
              rel="noopener noreferrer"
              :aria-label="social.label"
              class="inline-flex items-center justify-center w-9 h-9 rounded-full bg-muted hover:bg-muted/80 transition-colors">
              <component :is="social.icon" class="w-4 h-4" />
            </a>
          </div>
        </div>
      </div>

      <Separator class="my-8" />

      <div class="flex flex-col sm:flex-row items-center justify-between gap-4">
        <p class="text-sm text-muted-foreground">
          © {{ currentYear }} Golden Bread. Все права защищены.
        </p>
        <p class="text-sm text-muted-foreground">
          Разработано с ❤️
        </p>
      </div>
    </div>
  </footer>
</template>