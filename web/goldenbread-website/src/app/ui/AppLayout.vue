<script setup lang="ts">
import { RouterView } from 'vue-router';
import { default as Header } from './AppHeader.vue';
import { default as Footer } from './AppFooter.vue';
</script>

<template>
  <Header/>
  <div class="flex-1 min-h-screen">
    <RouterView />
  </div>
  <Footer/>
</template>